using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public abstract class Definition 
    {
        private Type _type;
        private string _key;
        private List<object> _args = new List<object>();
        private Dictionary<string, object> _postInitData = new Dictionary<string, object>();

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public List<object> Args
        {
            get { return _args; }
            set { _args = value; }
        }

        public Dictionary<string, object> PostInitData
        {
            get { return _postInitData; }
            set { _postInitData = value; }
        }

        internal virtual void Build(AssemblyBuilder builder)
        {
            
        }
    }
}
