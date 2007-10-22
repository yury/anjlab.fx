using System;
using System.IO.Ports;
using System.Threading;
using AnjLab.FX.System;

namespace AnjLab.FX.IO.Ports
{
    public class SerialPortDelay
    {
        private SerialPort _Port;
        TimeSpan _PreWrite = TimeSpan.FromMilliseconds(0);
        TimeSpan _PostWrite = TimeSpan.FromMilliseconds(0);

        public SerialPortDelay(SerialPort serialPort)
        {
            _Port = serialPort;
        }

        public void Write(byte[] data)
        {
            Guard.IsTrue(_Port.IsOpen);

            Thread.Sleep(_PreWrite);
            _Port.RtsEnable = true;

            _Port.Write(data, 0, data.Length);

            Thread.Sleep(_PostWrite);
            _Port.RtsEnable = false;
        }

        public SerialPort Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        public TimeSpan PreWrite
        {
            get { return _PreWrite; }
            set { _PreWrite = value; }
        }

        public TimeSpan PostWrite
        {
            get { return _PostWrite; }
            set { _PostWrite = value; }
        }
    }
}
