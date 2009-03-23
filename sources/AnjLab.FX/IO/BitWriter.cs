using System.IO;
using AnjLab.FX.Sys;

namespace AnjLab.FX.IO
{
    public class BitWriter: BinaryWriter
    {
        private readonly int _bitsInByte = 8;
        private byte? _buffer = null;
        private int _bufferPosition = 0;
        private long _bitWritingPosition = 0;

        public BitWriter(Stream input)
            : base(input)
        {
        }

        public void WriteBit(byte bit)
        {
            bit = (byte)(bit & 0x01);

            if (_bitWritingPosition != BaseStream.Position) // there was byte writing operation
            {
                FlushBits();
                _bitWritingPosition = BaseStream.Position;
            }

            if (_buffer == null)
                _buffer = 0;

            _buffer = (byte)(_buffer | (bit << 7 - _bufferPosition));
            _bufferPosition++;

            if (_bufferPosition == 8)
            {
                FlushBits();
                _bitWritingPosition++;
            }
        }

        public void FlushBits()
        {
            if (_buffer != null)
                Write(_buffer.Value);
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
            get { return BaseStream.Length - BaseStream.Position; }
        }
    }
}
