#if NET_3_5
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public class While : ContainerMapElement
    {
        private ICondition _condition;

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            Guard.NotNull(_condition, "While property can't be null in Do mapping");
            //bool conditionResult = condition;
            //while(condition)
            //{
            //    // code
            //    conditionResult = condition;
            //}
            CodeExpression condition = _condition.GetCondition(ctx, method);
            method.Statements.Add(new CodeVariableDeclarationStatement(
                typeof(bool), "conditionResult", condition));
            CodeVariableReferenceExpression conditionResult = new CodeVariableReferenceExpression("conditionResult");

            method.Statements.Add(new CodeSnippetStatement("while (conditionResult) {"));
            
            base.GenerateMappingCode(ctx, method);

            method.Statements.Add(new CodeAssignStatement(conditionResult, condition));
            method.Statements.Add(new CodeSnippetStatement("}"));
        }

        public ICondition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }
    }
}
#endif