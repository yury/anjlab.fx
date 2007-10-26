namespace AnjLab.FX.StreamMapping.Operations
{
    public abstract class ValueOperation
    {
        private int _value = 0;


        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
