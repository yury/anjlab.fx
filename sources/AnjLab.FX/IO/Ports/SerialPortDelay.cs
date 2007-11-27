using System;
using System.IO.Ports;
using System.Threading;
using AnjLab.FX.System;

namespace AnjLab.FX.IO.Ports
{
    public class SerialPortDelay
    {
        private SerialPort _port;
        TimeSpan _PreWrite = TimeSpan.FromMilliseconds(0);
        TimeSpan _PostWrite = TimeSpan.FromMilliseconds(0);

        public SerialPortDelay(SerialPort serialPort)
        {
            _port = serialPort;
        }

        public void Write(byte[] data)
        {
            Guard.IsTrue(_port.IsOpen);

            Thread.Sleep(_PreWrite);
            _port.RtsEnable = true;

            _port.Write(data, 0, data.Length);

            Thread.Sleep(_PostWrite);
            _port.RtsEnable = false;
        }

        public SerialPort Port
        {
            get { return _port; }
            set { _port = value; }
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
