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

        private object _syncObj = new object();
        byte[] _currentPacket = null;
        public byte[][] Proccess(byte[] bytes)
        {
            lock (_syncObj)
            {
                List<byte[]> packets = new List<byte[]>();
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == _packetStart && _currentPacket == null)
                    {
                        _currentPacket = AddByteIntoArray(new byte[0], bytes[i]);
                        continue;
                    }

                    if (bytes[i] == _packetEnd)
                    {
                        _currentPacket = AddByteIntoArray(_currentPacket, bytes[i]);
                        if (_currentPacket != null)
                        {
                            byte[] newPacket = new byte[_currentPacket.Length];
                            _currentPacket.CopyTo(newPacket, 0);
                            _currentPacket = null;
                            packets.Add(newPacket);
                        }
                        continue;
                    }

                    if (_currentPacket != null) // append to packet
                        _currentPacket = AddByteIntoArray(_currentPacket, bytes[i]);
                }
                return packets.ToArray();
            }
        }

        private byte[] AddByteIntoArray(byte[] array, byte newByte)
        {
            return AddBytesIntoArray(array, new byte[] { newByte });
        }

        private byte[] AddBytesIntoArray(byte[] array, byte[] newBytes)
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
                _currentPacket = null;
        }
    }
}
