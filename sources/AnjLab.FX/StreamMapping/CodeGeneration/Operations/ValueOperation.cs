#if NET_3_5
using System;
using System.CodeDom;
using System.Reflection;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping.Operations
{
    public abstract class ValueOperation : IOperation
    {
        private int? _index = null;
        private int? _value = null;
        private string _property = null;

        public virtual CodeStatementCollection BuildOperation(CodeGenerationContext ctx, ICodeGeneratorNode element, CodeVariableReferenceExpression value)
        {
            CodeStatementCollection statemets = new CodeStatementCollection();
            if (!string.IsNullOrEmpty(_property))
            {
                CodeExpression rightExpression = new CodePropertyReferenceExpression(ctx.MappedObject, _property);
                if (_index != null && element.MappedProperty.PropertyType.IsArray)
                    rightExpression = new CodeIndexerExpression(rightExpression, new CodePrimitiveExpression(_index.Value));

                statemets.Add(new CodeAssignStatement(value, new CodeCastExpression(GetValueType(element.MappedProperty),
                        new CodeBinaryOperatorExpression(value, OperationType, rightExpression))));
            }

            if (Value != null)
            {
                statemets.Add(new CodeAssignStatement(value, new CodeCastExpression(element.MappedProperty.PropertyType,
                   new CodeBinaryOperatorExpression(value, OperationType, new CodePrimitiveExpression(Value)))));
            }

            return statemets;
        }

        protected Type GetValueType(PropertyInfo mappedProperty)
        {
            if (mappedProperty.PropertyType.IsArray)
                return mappedProperty.PropertyType.GetElementType();
            return mappedProperty.PropertyType;
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

        public int? Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public abstract CodeBinaryOperatorType OperationType{ get;}
    }
}
#endif