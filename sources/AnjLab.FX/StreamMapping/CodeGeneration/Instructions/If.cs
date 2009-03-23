#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public class If : MapElement
    {
        private ContainerMapElement _true;
        private ContainerMapElement _false;
        private ICondition _condition;

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            Guard.NotNull(_condition, "'Condition' property can't be null in If mapping");
            Guard.NotNull(_true, "'True' property can't be null in If mapping");

            CodeMemberMethod trueMethod = ctx.Builder.NewElementMappingMethod(ctx, _true);

            if (_false != null)
            {
                CodeMemberMethod falseMethod = ctx.Builder.NewElementMappingMethod(ctx, _false);
                method.Statements.Add(new CodeConditionStatement(_condition.GetCondition(ctx, method),
                    new CodeStatement[] { new CodeSnippetStatement(String.Format("this.{0}();", trueMethod.Name)) },
                    new CodeStatement[] { new CodeSnippetStatement(String.Format("this.{0}();", falseMethod.Name)) }));
            }
            else
            {
                method.Statements.Add(new CodeConditionStatement(_condition.GetCondition(ctx, method),
                    new CodeStatement[] { new CodeSnippetStatement(String.Format("this.{0}();", trueMethod.Name)) }));
            }

        }

        public ICondition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public ContainerMapElement True
        {
            get { return _true; }
            set { _true = value; }
        }

        public ContainerMapElement False
        {
            get { return _false; }
            set { _false = value; }
        }
    }
}
#endif