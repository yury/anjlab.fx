#if NET_3_5
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping
{
    public interface IOperation
    {
        CodeStatementCollection BuildOperation(CodeGenerationContext ctx, ICodeGeneratorNode element, CodeVariableReferenceExpression value);
    }
}
#endif