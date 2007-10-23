using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Scope: List<Bind>
    {
        private string _name = "default";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
