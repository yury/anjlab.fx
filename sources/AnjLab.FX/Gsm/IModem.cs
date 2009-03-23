using System;

namespace AnjLab.FX.Gsm
{
    public interface IModem
    {
        void Connect();
        void Disconnect();
        ATResponse ATCommand(ATRequest request, TimeSpan timeout);
        ATResponse SendSms(Sms sms, TimeSpan timeout);
    }
}
