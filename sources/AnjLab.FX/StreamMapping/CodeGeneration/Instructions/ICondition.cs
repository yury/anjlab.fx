using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public interface ICondition
    {
        CodeExpression GetCondition(CodeGenerationContext ctx, CodeMemberMethod method);
    }
}