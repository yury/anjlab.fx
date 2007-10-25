using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Ctor: Definition
    {
        private string _internalType;

        public string InternalType
        {
            get { return _internalType; }
            set { _internalType = value; }
        }

        internal override void Build(AssemblyBuilder builder)
        {
            builder.BuildFromConstructorDefinition(this);
        }
    }
}
