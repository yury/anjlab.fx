using System.IO;
using AnjLab.FX.Sys;

namespace AnjLab.FX.IO
{
    public class BitReader: BinaryReader 
    {
        private readonly int _bitsInByte = 8;
        private byte? _buffer = null;
        private int _bufferPosition = 0;
        private long _streamPositionAfterBitReading = 0;

        public BitReader(Stream input)
            : base(input)
        {
        }

        public byte ReadBits(int count)
        {
            Guard.ArgumentBetweenInclusive("count", count, 0, 8);

            if (_streamPositionAfterBitReading != this.BaseStream.Position) // there was byte reading operation
                ClearBuffer();

            if (_buffer == null || BitsInBuffer == 0)
            {
                _buffer = ReadByte();
                _bufferPosition = 0;
                _streamPositionAfterBitReading = this.BaseStream.Position;
            }

            byte result;
            if (BitsInBuffer >= count)
            {
                result = GetValueFromBuffer(count);
            }
            else
            {
                byte nextByteBitsCount = (byte) (count - BitsInBuffer);
                byte value = GetValueFromBuffer(BitsInBuffer);

                byte lowBits = ReadBits(nextByteBitsCount);
                result = (byte)((value << nextByteBitsCount) + lowBits);
            }
            return result;
        }

        private byte GetValueFromBuffer(int count)
        {
            byte value = (byte)(_buffer.Value << _bufferPosition);
            value = (byte)(value >> (_bitsInByte - count));
            _bufferPosition += count;
            return value;
        }

        private void ClearBuffer()
        {
            _buffer = null;
            _bufferPosition = 0;
        }

        public int BitsInBuffer
        {
            get { return (IsBuffered) ? _bitsInByte - _bufferPosition : 0; }
        }

        public  bool IsBuffered
        {
            get { return _buffer != null; }
        }

        public long BytesAvailable
        {
            get { return this.BaseStream.Length - this.BaseStream.Position; }
        }

        public bool CanRead
        {
            get
            {
                return BytesAvailable > 0 || BitsInBuffer > 0;
            }
        }

        public long BitsAvailable
        {
            get
            {
                return BytesAvailable * 8 + BitsInBuffer;
            }
        }
    }
}
