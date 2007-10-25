using System.CodeDom;

namespace AnjLab.FX.StreamMapping
{
    public abstract class MapInfoElement : IMapInfoElement
    {
        private int _length = 0;
        private string _to = "";

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public abstract void BuildMapElementMethod(AssemblyBuilder info, CodeMemberMethod method);
    }
}