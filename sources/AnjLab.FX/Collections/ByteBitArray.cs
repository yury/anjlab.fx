using System.Collections;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Collections
{
    public class ByteBitArray
    {
        private readonly BitArray _bitArrray;
        private readonly int _arrayBase = 8;

        public ByteBitArray() : this(0)
        {
        }

        public ByteBitArray(byte value)
        {
            _bitArrray = new BitArray(new bool[_arrayBase]);
            for (int i = 0; i < _arrayBase; i++)
            {
                byte mask = 1;
                mask = (byte)(mask << i);
                _bitArrray[i] = (value & mask) > 0;
            }
        }

        public byte GetByte()
        {
            byte result = 0;
            for (int i = 0; i < _arrayBase; i++)
            {
                byte mask = (byte)((_bitArrray[i]) ? 1 : 0);
                mask = (byte)(mask << i);
                result = (byte)(result | mask);
            }
            return result;
        }

        public bool this[int index]
        {
            get { return _bitArrray[index]; }
            set { _bitArrray[index] = value; }
        }
    }
}