using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace AnjLab.FX.Gsm
{
    public class GenericModem : IModem
    {
        private readonly SerialPort _port;
        
        public GenericModem(SerialPort port)
        {
            _port = port;
            _port.DataReceived += port_DataReceived;
        }

        public void Connect()
        {
            _port.Open();
        }

        public void Disconnect()
        {
            _port.Close();
        }

        #region Generic AT-Command

        private readonly object _portChannel = new object();
        public ATResponse ATCommand(ATRequest request, TimeSpan timeout)
        {
            _port.Write(request.Request);

            string responseStr = "";
            while (true)
                lock (_portChannel)
                {
                    Monitor.Wait(_portChannel, timeout); // wait for next packet
                    if (_nextPacket == null)
                        return ATResponse.Timeout;

                    responseStr += _nextPacket;
                    ATResponse response = request.ParseResponse(responseStr);
                    _nextPacket = null;
                    if (response != null)
                        return response;
                }
        }

        private string _nextPacket = String.Empty;
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_portChannel)
            {
                _nextPacket = _port.ReadExisting();
                Monitor.Pulse(_portChannel); // notify about next packet
            }
        }

        #endregion

        public ATResponse SendSms(Sms sms, TimeSpan timeout)
        {
            sms.Direction = SmsDirection.Submited;
            string pdu = sms.Compose(SmsEncoding.Ucs2);

            ATResponse response = ATCommand(
                new ATRequest(
                    String.Format("at+cmgs={0}\r", (pdu.Length / 2) - 1), 
                    new string(new char[] { (char)13, (char)10, (char)62, (char)32 })), 
                timeout);

            if (response.IsSpecial)
                response = ATCommand(new ATRequest(String.Format("{0}{1}\r", pdu, (char)26)), timeout);
            return response;
        }
    }
}
