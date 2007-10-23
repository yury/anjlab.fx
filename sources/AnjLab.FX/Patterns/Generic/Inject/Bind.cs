using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Bind
    {
        private Type _type;
        private string _to;
        private Activator _activation = Activator.PerCall;

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public Activator Activation
        {
            get { return _activation; }
            set { _activation = value; }
        }
    }
}
