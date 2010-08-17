using System;
using System.Collections.Generic;
using System.Linq;

namespace AnjLab.FX.Devices
{
    public class BytesFilter : IBytesFilter
    {
        private readonly object _syncObj = new object();
        byte[] _buffer;

        public BytesFilter(byte packetStart, byte packetEnd)
            : this(new[] { packetStart }, new[] { packetEnd })
        {
        }

        public BytesFilter(byte[] packetStart, byte[] packetEnd)
        {
            PacketStart = packetStart;
            PacketEnd = packetEnd;
        }

        public BytesFilter(string packetStartHexString, string packetEndHexString)
            : this(packetStartHexString.ToBytes(), packetEndHexString.ToBytes())
        {
        }

        public static bool DataStartsFrom(byte[] data, byte[] source, int startIndex)
        {
            if (startIndex + data.Length > source.Length)
                return false;
            return !data.Where((t, i) => t != source[startIndex + i]).Any();
        }
        
        public byte[][] Proccess(byte[] bytes)
        {
            lock (_syncObj)
            {
                var packets = new List<byte[]>();
                int i = 0;
                if ((PacketStart.Length > 1 || PacketEnd.Length > 1) && _buffer != null)
                {
                    var oldLength = _buffer.Length;
                    Array.Resize(ref _buffer, bytes.Length + _buffer.Length);
                    Array.Copy(bytes, 0, _buffer, oldLength, bytes.Length);
                    bytes = _buffer;
                    _buffer = null;
                }
                while (i < bytes.Length)
                {
                    if (DataStartsFrom(PacketStart, bytes, i) && _buffer == null)
                    {
                        _buffer = AddBytesIntoArray(new byte[0], PacketStart);
                        i += PacketStart.Length;
                        continue;
                    }
                    if (DataStartsFrom(PacketEnd, bytes, i))
                    {
                        _buffer = AddBytesIntoArray(_buffer, PacketEnd);
                        if (_buffer != null)
                        {
                            var newPacket = new byte[_buffer.Length];
                            _buffer.CopyTo(newPacket, 0);
                            _buffer = null;
                            packets.Add(newPacket);
                        }
                        i += PacketEnd.Length;
                        continue;
                    }
                    if (_buffer != null) // append to packet
                        _buffer = AddByteIntoArray(_buffer, bytes[i]);
                    i++;
                }
                //for (int i = 0; i < bytes.Length; i++)
                //{
                //    if (bytes[i] == PacketStart && _buffer == null)
                //    {
                //        _buffer = AddByteIntoArray(new byte[0], bytes[i]);
                //        continue;
                //    }

                //    if (bytes[i] == PacketEnd)
                //    {
                //        _buffer = AddByteIntoArray(_buffer, bytes[i]);
                //        if (_buffer != null)
                //        {
                //            var newPacket = new byte[_buffer.Length];
                //            _buffer.CopyTo(newPacket, 0);
                //            _buffer = null;
                //            packets.Add(newPacket);
                //        }
                //        continue;
                //    }

                //    if (_buffer != null) // append to packet
                //        _buffer = AddByteIntoArray(_buffer, bytes[i]);
                //}
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

        public byte[] PacketStart { get; set; }

        public byte[] PacketEnd { get; set; }
    }
}
