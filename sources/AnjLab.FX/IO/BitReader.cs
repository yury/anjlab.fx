using System.IO;

namespace AnjLab.FX.IO
{
    public class BitReader: BinaryReader 
    {
        private readonly int _bitsCount = 8;
        private byte _buffer;
        private int _part = 0;

        public BitReader(Stream input, int readBase) : base(input)
        {
            if (readBase <= 16)
                _bitsCount = 4;
            if (readBase <= 4)
                _bitsCount = 2;
        }

        public int ReadReduced()
        {
            return Read7BitEncodedInt();
        }

        public byte ReadBits()
        {
            if (_bitsCount == 8)
                return ReadByte();
            else if (_bitsCount == 4)
            {
                if (_part == 0)
                {
                    _part++;
                    _buffer = ReadByte();
                    byte value = _buffer;
                    return (byte)(value >> 4);
                } 
                else
                {
                    _part = 0;
                    return (byte)(0x0f & _buffer);
                }
            } 
            else if (_bitsCount == 2)
            {
                if (_part == 0)
                {
                    _part++;
                    _buffer = ReadByte();
                    byte value = _buffer;
                    return (byte) (value >> 6);
                }
                else if (_part == 1)
                {
                    _part++;
                    byte value = _buffer;
                    return (byte) (0x03 & (value >> 4));
                } 
                else if (_part == 2)
                {
                    _part++;
                    byte value = _buffer;
                    return (byte) (0x03 & (value >> 2));
                } 
                else
                {
                    _part = 0;
                    byte value = _buffer;
                    return (byte) (0x03 & (value));
                }
            }
            return 0;
        }

        public  bool IsBuffered
        {
            get
            {
                return _part != 0;
            }
        }
    }
}
