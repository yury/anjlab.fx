using System.CodeDom;

namespace AnjLab.FX.StreamMapping.Operations
{
    public class Add : ValueOperation, IOperation
    {
        public CodeStatementCollection BuildOperation(IMapElement element, CodeVariableReferenceExpression value,
            CodeArgumentReferenceExpression resultObj)
        {
            CodeStatementCollection statemets = new CodeStatementCollection();
            statemets.Add(new CodeSnippetExpression(
                string.Format("{0} = ({2})({0} + {1})", value.VariableName, Value, element.MappedProperty.PropertyType.FullName)));
            return statemets;
        }
    }
}
