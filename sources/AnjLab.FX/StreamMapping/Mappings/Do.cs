using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class Do : ContainerMapElement
    {
        private IConditionGenerator _while;

        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            Guard.NotNull(_while, "While property can't be null in Do mapping");

            //CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            //CodeArgumentReferenceExpression resultObj = new CodeArgumentReferenceExpression(method.Parameters[1].Name);

            //builder.GenerateElementsMapCode(Elements, method, bitReader, resultObj, MappedType);
        }

        public IConditionGenerator While
        {
            get { return _while; }
            set { _while = value; }
        }
    }
}
