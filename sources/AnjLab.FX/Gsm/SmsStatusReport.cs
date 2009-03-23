using System;
using AnjLab.FX.Gsm;

namespace AnjLab.FX.Gsm
{
    public class SmsStatusReport : SmsBase
    {
        protected DateTime _reportTimeStamp;
        protected ReportStatus _reportStatus;
        protected byte _messageReference;
        protected string _phoneNumber;
        protected DateTime _serviceCenterTimeStamp;

        public static void Fetch(SmsStatusReport statusReport, ref string source)
        {
            SmsBase.Fetch(statusReport, ref source);

            statusReport._messageReference = PopByte(ref source);
            statusReport._phoneNumber = PopPhoneNumber(ref source);
            statusReport._serviceCenterTimeStamp = PopDate(ref source);
            statusReport._reportTimeStamp = PopDate(ref source);
            statusReport._reportStatus = (ReportStatus) PopByte(ref source);
        }

        #region Properties

        public byte MessageReference { get { return _messageReference; } }
        public string PhoneNumber { get { return _phoneNumber; } }
        public DateTime ServiceCenterTimeStamp { get { return _serviceCenterTimeStamp; } }
        public DateTime ReportTimeStamp { get { return _reportTimeStamp; } }
        public ReportStatus ReportStatus { get { return _reportStatus; } }
        public override SmsType Type { get { return SmsType.StatusReport; } }

        #endregion
    }

    public enum ReportStatus
    {
        NoResponseFromSME = 0x62,
        NotSend = 0x60,
        Success = 0
    }
}