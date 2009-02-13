using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AnjLab.FX.Sys
{
    [DataContract]
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

        [DataMember]
        public TA A
        {
            get { return _a; }
            set { _a = value; }
        }

        [DataMember]
        public TB B
        {
            get { return _b; }
            set { _b = value; }
        }

        public override bool Equals(object obj)
        {
            Pair<TA, TB> o = obj as Pair<TA, TB>;
            return o != null && AreEqual(_a, o._a) && AreEqual(_b, o._b);
        }

        private bool AreEqual(object a1, object a2)
        {
            if (a1 == null && a2 == null)
                return true;
            return a1 != null && a2 != null && a1.Equals(a2);
        }

        public override int GetHashCode()
        {
            return (_a.GetHashCode().ToString() + _b.GetHashCode().ToString()).GetHashCode();
        }
    }

    public class Pair
    {
        public static Pair<TAA, TBB> New<TAA, TBB>(TAA a, TBB b)
        {
            return new Pair<TAA, TBB>(a, b);
        }

        public static Dictionary<TAA, TBB> ToDictionary<TAA, TBB>(Pair<TAA, TBB> [] arr)
        {
            Dictionary<TAA, TBB> dict = new Dictionary<TAA, TBB>(arr.Length);

            foreach (Pair<TAA, TBB> pair in arr)
            {
                dict.Add(pair.A, pair.B);
            }
            return dict;
        }
    }
}
