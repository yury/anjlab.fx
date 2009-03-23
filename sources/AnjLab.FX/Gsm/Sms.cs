using System;

namespace AnjLab.FX.Gsm
{
    public class Sms : SmsBase
    {
        protected bool _moreMessagesToSend;
        protected bool _rejectDuplicates;
        protected byte _messageReference;
        protected string _phoneNumber;
        protected byte _protocolIdentifier;
        protected byte _dataCodingScheme;
        protected byte _validityPeriod;
        protected DateTime _serviceCenterTimeStamp;
        protected string _userData;
        protected byte[] _userDataHeader;
        protected string _message;
        
        public static void Fetch(Sms sms, ref string source)
        {
            SmsBase.Fetch(sms, ref source);

            if (sms._direction == SmsDirection.Submited)
                sms._messageReference = PopByte(ref source);

            sms._phoneNumber = PopPhoneNumber(ref source);
            sms._protocolIdentifier = PopByte(ref source);
            sms._dataCodingScheme = PopByte(ref source);

            if (sms._direction == SmsDirection.Submited)
                sms._validityPeriod = PopByte(ref source);

            if (sms._direction == SmsDirection.Received)
                sms._serviceCenterTimeStamp = PopDate(ref source);

            sms._userData = source;

            if (source == string.Empty)
                return;

            int userDataLength = PopByte(ref source);

            if (userDataLength == 0)
                return;

            if (sms._userDataStartsWithHeader) 
            {
                byte userDataHeaderLength = PopByte(ref source);
                sms._userDataHeader = PopBytes(ref source, userDataHeaderLength);
                userDataLength -= userDataHeaderLength + 1;
            }

            if (userDataLength == 0)
                return;

            switch ((SmsEncoding) sms._dataCodingScheme & SmsEncoding.ReservedMask) 
            {
                case SmsEncoding.SevenBit:
                    sms._message = Decode7bit(source, userDataLength);
                    break;
                case SmsEncoding.EightBit:
                    sms._message = Decode8bit(source, userDataLength);
                    break;
                case SmsEncoding.Ucs2:
                    sms._message = DecodeUCS2(source, userDataLength);
                    break;
            }
        }

        public Sms(string phoneNumber, string message)
        {
            PhoneNumber = phoneNumber;
            Message = message;
            ValidityPeriod = new TimeSpan(4, 0, 0); // 4 hours by default
        }

        public override void ComposePDUType()
        {
            base.ComposePDUType();

            if (_moreMessagesToSend || _rejectDuplicates)
                _pduType = (byte) (_pduType | 0x04);
        }

        public virtual string Compose(SmsEncoding messageEncoding)
        {
            ComposePDUType();

            string encodedData = "00"; //Length of SMSC information. Here the length is 0, which means that the SMSC stored in the phone should be used. Note: This octet is optional. On some phones this octet should be omitted! (Using the SMSC stored in phone is thus implicit)

            encodedData += Convert.ToString(_pduType, 16).PadLeft(2, '0'); //PDU type (forst octet)
            encodedData += Convert.ToString(MessageReference, 16).PadLeft(2, '0');
            encodedData += EncodePhoneNumber(PhoneNumber);
            encodedData += "00"; //Protocol identifier (Short Message Type 0)
            encodedData += Convert.ToString((int) messageEncoding, 16).PadLeft(2, '0'); //Data coding scheme

            if (_validityPeriodFormat != ValidityPeriodFormat.FieldNotPresent)
                encodedData += Convert.ToString(_validityPeriod, 16).PadLeft(2, '0'); //Validity Period

            byte[] messageBytes;
            switch (messageEncoding) 
            {
                case SmsEncoding.Ucs2:
                    messageBytes = EncodeUCS2(_message);
                    break;
                default:
                    messageBytes = new byte[0];
                    break;
            }

            encodedData += Convert.ToString(messageBytes.Length, 16).PadLeft(2, '0'); //Length of message

            foreach (byte b in messageBytes)
                encodedData += Convert.ToString(b, 16).PadLeft(2, '0');

            return encodedData.ToUpper();
        }

        #region Properties

        public DateTime ServiceCenterTimeStamp { get { return _serviceCenterTimeStamp; } }

        public byte MessageReference
        {
            get { return _messageReference; }
            set { _messageReference = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value.Length > 70)
                    throw new ArgumentOutOfRangeException("Message.Length", value.Length, "Message length can not be greater that 70 chars.");

                _message = value;
            }
        }

        public bool RejectDuplicates
        {
            get
            {
                if (Direction == SmsDirection.Received)
                    throw new InvalidOperationException("Received message can not contains 'reject duplicates' property");

                return _rejectDuplicates;
            }
            set
            {
                if (Direction == SmsDirection.Received)
                    throw new InvalidOperationException("Received message can not contains 'reject duplicates' property");

                _rejectDuplicates = value;
            }
        }

        public bool MoreMessagesToSend
        {
            get
            {
                if (Direction == SmsDirection.Received)
                    throw new InvalidOperationException("Submited message can not contains 'more message to send' property");

                return _moreMessagesToSend;
            }
            set
            {
                if (Direction == SmsDirection.Received)
                    throw new InvalidOperationException("Submited message can not contains 'more message to send' property");

                _moreMessagesToSend = value;
            }
        }

        public TimeSpan ValidityPeriod
        {
            get
            {
                if (_validityPeriod > 196)
                    return new TimeSpan((_validityPeriod - 192) * 7, 0, 0, 0);

                if (_validityPeriod > 167)
                    return new TimeSpan((_validityPeriod - 166), 0, 0, 0);

                if (_validityPeriod > 143)
                    return new TimeSpan(12, (_validityPeriod - 143) * 30, 0);

                return new TimeSpan(0, (_validityPeriod + 1) * 5, 0);
            }
            set
            {
                if (value.Days > 441)
                    throw new ArgumentOutOfRangeException("TimeSpan.Days", value.Days, "Value must be not greater 441 days.");

                if (value.Days > 30) //Up to 441 days
                    _validityPeriod = (byte)(192 + (int)(value.Days / 7));
                else if (value.Days > 1) //Up to 30 days
                    _validityPeriod = (byte)(166 + value.Days);
                else if (value.Hours > 12) //Up to 24 hours
                    _validityPeriod = (byte)(143 + (value.Hours - 12) * 2 + value.Minutes / 30);
                else if (value.Hours > 1 || value.Minutes > 1) //Up to 12 days
                    _validityPeriod = (byte)(value.Hours * 12 + value.Minutes / 5 - 1);
                else
                {
                    _validityPeriodFormat = ValidityPeriodFormat.FieldNotPresent;

                    return;
                }

                _validityPeriodFormat = ValidityPeriodFormat.Relative;
            }
        }

        public virtual byte[] UserDataHeader { get { return _userDataHeader; } }

        public bool InParts
        {
            get
            {
                if (_userDataHeader == null || _userDataHeader.Length < 5)
                    return false;

                return (_userDataHeader[0] == 0x00 && _userDataHeader[1] == 0x03); // | 08 04 00 | 9F 02 | i have this header from siemenes in "in parts" message
            }
        }

        public int InPartsID
        {
            get
            {
                if (!InParts)
                    return 0;

                return (_userDataHeader[2] << 8) + _userDataHeader[3];
            }
        }

        public int Part
        {
            get
            {
                if (!InParts)
                    return 0;

                return _userDataHeader[4];
            }
        }

        public override SmsType Type { get { return SmsType.Sms; } }

        #endregion
    }

    public enum SmsEncoding
    {
        ReservedMask = 0x0C /*1100*/,
        SevenBit = 0,
        EightBit = 0x04     /*0100*/,
        Ucs2 = 0x08         /*1000*/
    }
}