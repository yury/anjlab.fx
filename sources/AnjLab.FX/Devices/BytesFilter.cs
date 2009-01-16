using System;
using System.Collections.Generic;

namespace AnjLab.FX.Devices
{
    public class BytesFilter
    {
        private readonly byte _packetStart;
        private readonly byte _packetEnd;

        public BytesFilter(byte packetStart, byte packetEnd)
        {
            _packetStart = packetStart;
            _packetEnd = packetEnd;
        }

        private readonly object _syncObj = new object();
        byte[] _buffer;
        public byte[][] Proccess(byte[] bytes)
        {
            lock (_syncObj)
            {
                var packets = new List<byte[]>();
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == _packetStart && _buffer == null)
                    {
                        _buffer = AddByteIntoArray(new byte[0], bytes[i]);
                        continue;
                    }

                    if (bytes[i] == _packetEnd)
                    {
                        _buffer = AddByteIntoArray(_buffer, bytes[i]);
                        if (_buffer != null)
                        {
                            var newPacket = new byte[_buffer.Length];
                            _buffer.CopyTo(newPacket, 0);
                            _buffer = null;
                            packets.Add(newPacket);
                        }
                        continue;
                    }

                    if (_buffer != null) // append to packet
                        _buffer = AddByteIntoArray(_buffer, bytes[i]);
                }
                return packets.ToArray();
            }
        }

        private static byte[] AddByteIntoArray(byte[] array, byte newByte)
        {
            return AddBytesIntoArray(array, new [] { newByte });
        }

        private static byte[] AddBytesIntoArray(byte[] array, byte[] newBytes)
        {
            if (array != null)
            {
                Array.Resize(ref array, array.Length + newBytes.Length);
                Array.Copy(newBytes, 0, array, array.Length - newBytes.Length, newBytes.Length);
            }
            return array;
        }

        public void Clear()
        {
            lock (_syncObj)
                _buffer = null;
        }

        public byte[] Buffer
        {
            get { return _buffer; }
        }
    }
}
