using System.Collections.Generic;

namespace AnjLab.FX.Tests.StreamMapping
{
    public enum TestEnum : byte
    {
        First = 1, 
        Second = 2
    }

    public class TestObject
    {
        private byte _byteProperty = 0;
        private ushort _shortPropertyWithTwoParts = 0;
        private ushort _shortProperty = 0;
        private uint _intProperty = 0;
        private bool _boolProperty;
        private bool _boolProperty2;
        private TestEnum _enumProperty;

        List<byte> _bytes = new List<byte>();
        private byte _byteFlag = 0;

        public byte ByteProperty
        {
            get { return _byteProperty; }
            set { _byteProperty = value; }
        }

        public uint IntProperty
        {
            get { return _intProperty; }
            set { _intProperty = value; }
        }

        public ushort ShortProperty
        {
            get { return _shortProperty; }
            set { _shortProperty = value; }
        }

        public ushort ShortPropertyWithTwoParts
        {
            get { return _shortPropertyWithTwoParts; }
            set { _shortPropertyWithTwoParts = value; }
        }

        public bool BoolProperty2
        {
            get { return _boolProperty2; }
            set { _boolProperty2 = value; }
        }

        public bool BoolProperty
        {
            get { return _boolProperty; }
            set { _boolProperty = value; }
        }

        public TestEnum EnumProperty
        {
            get { return _enumProperty; }
            set { _enumProperty = value; }
        }

        public List<byte> Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        public byte ByteFlag
        {
            get { return _byteFlag; }
            set { _byteFlag = value; }
        }
    }
}