#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping.Operations
{
    public class RightShift : ValueOperation
    {
        public override CodeStatementCollection BuildOperation(CodeGenerationContext ctx, ICodeGeneratorNode element, CodeVariableReferenceExpression value)
        {
            CodeStatementCollection statemets = new CodeStatementCollection();
            statemets.Add(new CodeSnippetExpression(
                string.Format("{0} = ({2})((int){0} >> {1})", value.VariableName, Value, GetValueType(element.MappedProperty).FullName)));
            return statemets;
        }

        public override CodeBinaryOperatorType OperationType
        {
            get { throw new NotSupportedException("RightShift is not supported automatically"); }
        }
    }
}
#endif