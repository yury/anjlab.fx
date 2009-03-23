#if NET_3_5
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public class Do : ContainerMapElement
    {
        private ICondition _while;

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            Guard.NotNull(_while, "While property can't be null in Do mapping");
            //bool conditionResult = true;
            //do
            //{
            //    // code
            //    conditionResult = (1 > 2);
            //} while (condition);
            method.Statements.Add(new CodeSnippetStatement("bool conditionResult = true;"));
            CodeVariableReferenceExpression conditionResult = new CodeVariableReferenceExpression("conditionResult");
            method.Statements.Add(new CodeSnippetStatement("do {"));

            base.GenerateMappingCode(ctx, method);

            method.Statements.Add(new CodeAssignStatement(conditionResult, _while.GetCondition(ctx, method)));
            method.Statements.Add(new CodeSnippetStatement("} while (conditionResult);"));
        }

        public ICondition While
        {
            get { return _while; }
            set { _while = value; }
        }
    }
}
#endif