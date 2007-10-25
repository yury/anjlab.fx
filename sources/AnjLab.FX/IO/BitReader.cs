using System.IO;
using AnjLab.FX.System;

namespace AnjLab.FX.IO
{
    public class BitReader: BinaryReader 
    {
        private readonly int _bitsInByte = 8;
        private byte? _buffer = null;
        private int _bufferPosition = 0;

        public BitReader(Stream input)
            : base(input)
        {
        }

        public byte ReadBits(int count)
        {
            Guard.ArgumentBetweenInclusive("count", count, 0, 8);

            if (_buffer == null || BitsInBuffer == 0)
            {
                _buffer = ReadByte();
                _bufferPosition = 0;
            }

            if (BitsInBuffer >= count)
            {
                return GetValueFromBuffer(count);
            }
            else
            {
                byte nextByteBitsCount = (byte) (count - BitsInBuffer);
                byte value = GetValueFromBuffer(BitsInBuffer);

                byte lowBits = ReadBits(nextByteBitsCount);
                return (byte)((value << nextByteBitsCount) + lowBits);
            }
        }

        private byte GetValueFromBuffer(int count)
        {
            byte value = (byte)(_buffer.Value << _bufferPosition);
            value = (byte)(value >> (_bitsInByte - count));
            _bufferPosition += count;
            return value;
        }

        private int BitsInBuffer
        {
            get { return _bitsInByte - _bufferPosition; }
        }

        public  bool IsBuffered
        {
            get { return _buffer != null; }
        }
    }
}
