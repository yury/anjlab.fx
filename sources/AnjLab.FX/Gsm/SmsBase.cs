using System;
using System.Text;
using System.Collections.Generic;

namespace AnjLab.FX.Gsm
{
    public abstract class SmsBase
    {
        protected byte _pduType;
        protected string _serviceCenterNumber;
        //Reply path. Parameter indicating that reply path exists.
        protected bool _replyPathExists;
        //User data header indicator. This bit is set to 1 if the User Data field starts with a header. (EMS?)
        protected bool _userDataStartsWithHeader;
        //Status report indication. This bit is set to 1 if a status report is going to be returned to the SME.
        protected bool _statusReportIndication;
        //Validity Period Format
        protected ValidityPeriodFormat _validityPeriodFormat = ValidityPeriodFormat.FieldNotPresent;
        //Message type indication
        //First bit
        protected SmsDirection _direction;

        #region Public Static (Pops, Peeks, Gets)

        public static byte[] GetInvertBytes(string source)
        {
            byte[] bytes = GetBytes(source);
            Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetBytes(string source)
        {
            return GetBytes(source, 16);
        }

        public static byte[] GetBytes(string source, int fromBase)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < source.Length / 2; i++)
                bytes.Add(Convert.ToByte(source.Substring(i * 2, 2), fromBase));
            return bytes.ToArray();
        }

        public static string Decode7bit(string source, int length)
        {
            byte[] bytes = GetInvertBytes(source);
            string binary = string.Empty;
            foreach (byte b in bytes)
                binary += Convert.ToString(b, 2).PadLeft(8, '0');

            binary = binary.PadRight(length * 7, '0');
            string result = string.Empty;

            for (int i = 1; i <= length; i++)
                result += (char) Convert.ToByte(binary.Substring(binary.Length - i * 7, 7), 2);

            return result.Replace('\x0', '\x40');
        }

        public static string Decode8bit(string source, int length)
        {
            byte[] bytes = GetBytes(source.Substring(0, length * 2));
            return Encoding.UTF8.GetString(bytes); //or ASCII?
        }


        public static string DecodeUCS2(string source, int length)
        {
            byte[] bytes = GetBytes(source.Substring(0, length * 2));
            return Encoding.BigEndianUnicode.GetString(bytes);
        }

        public static byte[] EncodeUCS2(string s)
        {
            return Encoding.BigEndianUnicode.GetBytes(s);
        }

        public static string ReverseBits(string source)
        {
            return ReverseBits(source, source.Length);
        }

        public static string ReverseBits(string source, int length)
        {
            string result = string.Empty;
            for (int i = 0; i < length; i++)
                result = result.Insert(i % 2 == 0 ? i : i - 1, source[i].ToString());
            return result;
        }

        public static byte PeekByte(string source)
        {
            return PeekByte(source, 0);
        }

        public static byte PeekByte(string source, int byteIndex)
        {
            return Convert.ToByte(source.Substring(byteIndex * 2, 2), 16);
        }

        public static byte PopByte(ref string source)
        {
            byte b = Convert.ToByte(source.Substring(0, 2), 16);
            source = source.Substring(2);
            return b;
        }

        public static byte[] PopBytes(ref string source, int length)
        {
            string bytes = source.Substring(0, length * 2);
            source = source.Substring(length * 2);
            return GetBytes(bytes);
        }

        public static DateTime PopDate(ref string source)
        {
            byte[] bytes = GetBytes(ReverseBits(source.Substring(0, 12)), 10);
            source = source.Substring(14);
            return new DateTime(2000 + bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]);
        }

        public static string PopPhoneNumber(ref string source)
        {
            int numberLength = PopByte(ref source);
            if ((numberLength = numberLength + 2) == 2)
                return string.Empty;
            return PopAddress(ref source, numberLength + (numberLength % 2));
        }

        public static string PopServiceCenterAddress(ref string source)
        {
            int addressLength = PopByte(ref source);
            if ((addressLength = addressLength * 2) == 0)
                return string.Empty;
            return PopAddress(ref source, addressLength);
        }

        public static string PopAddress(ref string source, int length)
        {
            string address = source.Substring(0, length);
            source = source.Substring(address.Length);
            
            byte addressType = PopByte(ref address);
            address = ReverseBits(address).Trim('F');
            if (0x09 == addressType >> 4)
                address = "+" + address;

            return address;
        }

        public static SmsType GetSMSType(string source)
        {
            byte scaLength = PeekByte(source);
            byte pduType = PeekByte(source, scaLength + 1);
            byte smsType = (byte) ((pduType & 3) >> 1);

            if (!Enum.IsDefined(typeof(SmsType), (int) smsType))
                throw new InvalidOperationException(String.Format("Unknown sms type:{0}", smsType));

            return (SmsType) smsType;
        }

        public static void Fetch(SmsBase sms, ref string source)
        {
            sms._serviceCenterNumber = PopServiceCenterAddress(ref source);
            sms._pduType = PopByte(ref source);

            global::System.Collections.BitArray bits = new global::System.Collections.BitArray(new byte[] { sms._pduType });
            sms.ReplyPathExists = bits[7];
            sms.UserDataStartsWithHeader = bits[6];
            sms.StatusReportIndication = bits[5];
            sms.ValidityPeriodFormat = (ValidityPeriodFormat) (sms._pduType & 0x18);
            sms.Direction = (SmsDirection) (sms._pduType & 1);
        }

        #endregion

        public static string EncodePhoneNumber(string phoneNumber)
        {
            bool isInternational = phoneNumber.StartsWith("+");

            if (isInternational)
                phoneNumber = phoneNumber.Remove(0, 1);

            int header = (phoneNumber.Length << 8) + 0x81 | (isInternational ? 0x10 : 0x20);

            if (phoneNumber.Length % 2 == 1)
                phoneNumber = phoneNumber.PadRight(phoneNumber.Length + 1, 'F');

            phoneNumber = ReverseBits(phoneNumber);

            return Convert.ToString(header, 16).PadLeft(4, '0') + phoneNumber;
        }

        public virtual void ComposePDUType()
        {
            _pduType = (byte) Direction;
            _pduType = (byte) (_pduType | (int) ValidityPeriodFormat);

            if (StatusReportIndication)
                _pduType = (byte) (_pduType | 0x20);
        }

        #region Proreties

        public string ServiceCenterNumber
        {
            get { return _serviceCenterNumber; }
            set { _serviceCenterNumber = value; }
        }

        public bool ReplyPathExists
        {
            get { return _replyPathExists; }
            set { _replyPathExists = value; }
        }

        public bool UserDataStartsWithHeader
        {
            get { return _userDataStartsWithHeader; }
            set { _userDataStartsWithHeader = value; }
        }

        public bool StatusReportIndication
        {
            get { return _statusReportIndication; }
            set { _statusReportIndication = value; }
        }

        public ValidityPeriodFormat ValidityPeriodFormat
        {
            get { return _validityPeriodFormat; }
            set { _validityPeriodFormat = value; }
        }

        public SmsDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public abstract SmsType Type { get; }

        #endregion
    }

    public enum SmsDirection
    {
        Received = 0,
        Submited = 1
    }

    public enum SmsType
    {
        Sms = 0,
        StatusReport = 1
    }

    public enum ValidityPeriodFormat
    {
        FieldNotPresent = 0,
        Relative = 0x10,
        Enhanced = 0x08,
        Absolute = 0x18
    }
}