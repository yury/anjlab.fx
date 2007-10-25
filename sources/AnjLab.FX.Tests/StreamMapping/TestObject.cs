namespace AnjLab.FX.Tests.StreamMapping
{
    public class TestObject
    {
        private byte _byteProperty = 0;
        private ushort _shortProperty = 0;
        private uint _intProperty = 0;

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
    }
}