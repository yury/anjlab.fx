using System;
using System.IO;

namespace AnjLab.FX.IO
{
    public class BasedBitWriter: BinaryWriter 
    {
        private readonly int _bitsCount = 8;
        private byte _buffer;
        private int _part = 0;

        public BasedBitWriter(Stream output, int writeBase) :base (output)
        {
            if (writeBase <= 16)
                _bitsCount = 4;
            if (writeBase <= 4)
                _bitsCount = 2;
        }

        public void WriteReduced(int value)
        {
            Write7BitEncodedInt(value);
        }

        public void WriteBits(byte value)
        {
            if (_bitsCount == 8)
                Write(value);
            else if (_bitsCount == 4)
            {
                if (_part == 0)
                {
                    _part++;
                    _buffer = (byte)(value << 4);
                }
                else
                {
                    _part = 0;
                    _buffer = (byte)(_buffer | value);
                    Write(_buffer);
                    _buffer = 0;
                }
            }
            else if (_bitsCount == 2)
            {
                if (_part == 0)
                {
                    _part++;
                    _buffer = (byte)(value << 6);
                }
                else if (_part == 1)
                {
                    _part++;
                    _buffer = (byte)(_buffer | (value << 4));
                }
                else if (_part == 2)
                {
                    _part++;
                    _buffer = (byte)(_buffer | (value << 2));
                }
                else
                {
                    _part = 0;
                    _buffer = (byte)(_buffer | value);
                    Write(_buffer);
                    _buffer = 0;
                }
            }
        }

        public override void Flush()
        {
            if (_part > 0)
                Write(_buffer);
            base.Flush();
        }
    }
}
