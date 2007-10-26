using System.CodeDom;
using System.Collections.Generic;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Elements")]
    public class ContainerMapElement : MapElement
    {
        private List<IMapElement> _elements = new List<IMapElement>();

        public List<IMapElement> Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }

        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            CodeArgumentReferenceExpression resultObj = new CodeArgumentReferenceExpression(method.Parameters[1].Name);

            builder.GenerateElementsMapCode(Elements, method, bitReader, resultObj, MappedType);
        }
    }
}