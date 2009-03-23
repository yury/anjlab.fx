using System.IO;

namespace AnjLab.FX.IO
{
    public class BasedBitReader: BinaryReader 
    {
        private readonly int _bitsCount = 8;
        private byte _buffer;
        private int _part = 0;

        public BasedBitReader(Stream input, int readBase) : base(input)
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
            switch(_bitsCount )
            {
                case 8:
                    return ReadByte();
                case 4:
                    return Read4Bits();
                case 2:
                    return Read2Bits();
                default:
                    return 0;
            }
        }

        private byte Read2Bits()
        {
            byte value;
            switch (_part)
            {
                case 0:
                    _part++;
                    _buffer = ReadByte();
                    value = _buffer;
                    return (byte)(value >> 6);
                case 1:
                    _part++;
                    value = _buffer;
                    return (byte)(0x03 & (value >> 4));
                case 2:
                    _part++;
                    value = _buffer;
                    return (byte)(0x03 & (value >> 2));
                default:
                    _part = 0;
                    value = _buffer;
                    return (byte)(0x03 & (value));
            }
        }

        private byte Read4Bits()
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

        public  bool IsBuffered
        {
            get
            {
                return _part != 0;
            }
        }
    }
}
