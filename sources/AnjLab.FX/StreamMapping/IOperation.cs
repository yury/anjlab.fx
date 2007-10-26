using System.CodeDom;

namespace AnjLab.FX.StreamMapping
{
    public interface IOperation
    {
        CodeStatementCollection BuildOperation(IMapElement element, CodeVariableReferenceExpression value,
            CodeArgumentReferenceExpression resultObj);
    }
}