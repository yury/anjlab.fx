using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Ctor: Definition
    {
        internal override void Build(AssemblyBuilder builder)
        {
            builder.BuildFromContructorDefinition(this);
        }
    }
}
