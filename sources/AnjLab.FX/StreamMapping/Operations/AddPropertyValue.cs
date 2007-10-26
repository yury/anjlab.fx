using System.CodeDom;

namespace AnjLab.FX.StreamMapping.Operations
{
    public class AddPropertyValue : IOperation
    {
        public CodeStatementCollection BuildOperation(IMapElement element, CodeVariableReferenceExpression value,
            CodeArgumentReferenceExpression resultObj)
        {
            CodeStatementCollection statemets = new CodeStatementCollection();
            statemets.Add(new CodeSnippetExpression(
                string.Format("{0} = ({1})({0} + {2}.{3})", 
                value.VariableName, 
                element.MappedProperty.PropertyType.FullName,
                resultObj.ParameterName, 
                element.MappedProperty.Name)));
            return statemets;
        }
    }
}
