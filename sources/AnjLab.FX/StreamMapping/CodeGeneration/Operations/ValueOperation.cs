using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping.Operations
{
    public abstract class ValueOperation : IOperation
    {
        private int? _value = null;
        private string _property = null;

        public CodeStatementCollection BuildOperation(CodeGenerationContext ctx, ICodeGeneratorNode element, CodeVariableReferenceExpression value)
        {
            CodeStatementCollection statemets = new CodeStatementCollection();
            if (!string.IsNullOrEmpty(_property))
            {
                statemets.Add(new CodeAssignStatement(value, new CodeCastExpression(element.MappedProperty.PropertyType,
                    new CodeBinaryOperatorExpression(
                        value, OperationType, new CodePropertyReferenceExpression(ctx.MappedObject, _property)))));
            }

            if (Value != null)
            {
                statemets.Add(new CodeAssignStatement(value, new CodeCastExpression(element.MappedProperty.PropertyType,
                   new CodeBinaryOperatorExpression(value, OperationType, new CodePrimitiveExpression(Value)))));
            }

            return statemets;
        }

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public int? Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public abstract CodeBinaryOperatorType OperationType{ get;}
    }
}
