using System;
using System.IO.Ports;
using System.Threading;
using AnjLab.FX.System;

namespace AnjLab.FX.IO.Ports
{
    public class SerialPortDelay
    {
        private SerialPort _port;
        TimeSpan _preWrite = TimeSpan.FromMilliseconds(0);
        TimeSpan _postWrite = TimeSpan.FromMilliseconds(0);

        public SerialPortDelay(SerialPort serialPort)
        {
            _port = serialPort;
        }

        public SerialPortDelay(SerialPort port, TimeSpan preWrite, TimeSpan postWrite)
        {
            _port = port;
            _preWrite = preWrite;
            _postWrite = postWrite;
        }

        public void Write(byte[] data)
        {
            Guard.IsTrue(_port.IsOpen);

            Thread.Sleep(_preWrite);
            _port.RtsEnable = true;

            _port.Write(data, 0, data.Length);

            Thread.Sleep(_postWrite);
            _port.RtsEnable = false;
        }

        public SerialPort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public TimeSpan PreWrite
        {
            get { return _preWrite; }
            set { _preWrite = value; }
        }

        public TimeSpan PostWrite
        {
            get { return _postWrite; }
            set { _postWrite = value; }
        }
    }
}
