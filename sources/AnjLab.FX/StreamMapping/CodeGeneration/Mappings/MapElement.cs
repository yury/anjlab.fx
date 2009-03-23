#if NET_3_5
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Markup;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Operations")]
    public abstract class MapElement : ICodeGeneratorNode
    {
        private int _length = 0;
        private int _index = 0;
        private string _to = "";
        List<IOperation> _operations = new List<IOperation>();
        private PropertyInfo _mappedProperty;

        protected CodeStatementCollection GenerateSetMappedPropertyCode(CodeExpression targetObj, CodeExpression value)
        {
            CodeStatementCollection statements = new CodeStatementCollection();

            CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(targetObj, MappedProperty.Name);

            if (_mappedProperty.PropertyType.IsArray)
            {
                statements.Add(new CodeAssignStatement(
                    new CodeIndexerExpression(property, new CodePrimitiveExpression(_index)), value));
                return statements;
            }

            if (IsCollection(_mappedProperty.PropertyType))
            {
                CodeBinaryOperatorExpression isNull =
                    new CodeBinaryOperatorExpression(property, CodeBinaryOperatorType.ValueEquality, new CodeSnippetExpression("null"));
                CodeAssignStatement create = new CodeAssignStatement(property, new CodeObjectCreateExpression(_mappedProperty.PropertyType));

                statements.Add(new CodeConditionStatement(isNull, create));
                statements.Add(new CodeMethodInvokeExpression(property, "Add", value));
                return statements;
            }

            statements.Add(new CodeAssignStatement(property, value));
            return statements;
        }

        protected CodeExpression GetMappedProperty(CodeExpression targetObj)
        {
            CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(targetObj, MappedProperty.Name);
            if (IsCollection(_mappedProperty.PropertyType))
            {
                return new CodeIndexerExpression(property,
                    new CodeBinaryOperatorExpression(
                        new CodePropertyReferenceExpression(property, "Count"), 
                        CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)));

            }

            if (_mappedProperty.PropertyType.IsArray)
                return new CodeIndexerExpression(property, new CodePrimitiveExpression(_index));

            return property;
        }

        private bool IsCollection(Type type)
        {
            foreach (Type t in type.GetInterfaces())
            {
                if (t == typeof(IList<>) || t == typeof(IList) || t == typeof(ICollection<>) || t == typeof(ICollection))
                    return true;
            }
            return false;
        }

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public List<IOperation> Operations
        {
            get { return _operations; }
            set { _operations = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public PropertyInfo MappedProperty
        {
            get { return _mappedProperty; }
            protected set { _mappedProperty = value; }
        }

        protected Type MappedValueType
        {
            get
            {
                if (_mappedProperty.PropertyType.HasElementType)
                    return _mappedProperty.PropertyType.GetElementType();

                if (_mappedProperty.PropertyType.IsArray)
                    return _mappedProperty.PropertyType.GetElementType();

                if (_mappedProperty.PropertyType.IsGenericType)
                    return _mappedProperty.PropertyType.GetGenericArguments()[0];

                return _mappedProperty.PropertyType;
            }
        }

        public abstract void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method);
    }
}
#endif