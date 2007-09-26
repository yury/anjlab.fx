namespace AnjLab.FX.System
{
    public class Pair<TA, TB>
    {
        private TA _a;
        private TB _b;

        public Pair()
        {
        }

        public Pair(TA a, TB b)
        {
            _a = a;
            _b = b;
        }

        public TA A
        {
            get { return _a; }
            set { _a = value; }
        }

        public TB B
        {
            get { return _b; }
            set { _b = value; }
        }

        public override bool Equals(object obj)
        {
            Pair<TA, TB> o = obj as Pair<TA, TB>;
            return o != null && _a.Equals(o._a) && _b.Equals(o.B);
        }

        public override int GetHashCode()
        {
            return (_a.GetHashCode().ToString() + _b.GetHashCode().ToString()).GetHashCode();
        }
    }
}
