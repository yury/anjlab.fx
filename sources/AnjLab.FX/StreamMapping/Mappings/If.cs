using System;
using System.CodeDom;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class If : MapElement
    {
        private ContainerMapElement _true;
        private ContainerMapElement _false;
        private IConditionGenerator _condition;

        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            Guard.NotNull(_condition, "'Condition' property can't be null in If mapping");
            Guard.NotNull(_true, "'True' property can't be null in If mapping");

            CodeMemberMethod trueMethod = builder.GenerateElementMapMethod(_true, MappedType);
            CodeMemberMethod falseMethod = builder.GenerateElementMapMethod(_false, MappedType);
            builder.AddMethod(trueMethod);
            builder.AddMethod(falseMethod);

            method.Statements.Add(new CodeConditionStatement(_condition.GetCondition(builder, method),
                new CodeStatement[] { new CodeSnippetStatement(String.Format("this.{0}({1}, {2});",
                    trueMethod.Name, method.Parameters[0].Name, method.Parameters[1].Name)) },
                new CodeStatement[] { new CodeSnippetStatement(String.Format("this.{0}({1}, {2});", 
                    falseMethod.Name, method.Parameters[0].Name, method.Parameters[1].Name)) }));
        }

        public IConditionGenerator Condition
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

    public interface IConditionGenerator
    {
        CodeExpression GetCondition(AssemblyBuilder builder, CodeMemberMethod method);
    }

    public class MoreThanOneCounterData : IConditionGenerator
    {
        public CodeExpression GetCondition(AssemblyBuilder builder, CodeMemberMethod method)
        {
            CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            return new CodeBinaryOperatorExpression(
                new CodePropertyReferenceExpression(bitReader, "BytesAvailable"), 
                CodeBinaryOperatorType.GreaterThan, 
                new CodePrimitiveExpression(2));
        }
    }
}