using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Operations")]
    public abstract class MapElement : IMapElement
    {
        private int _length = 0;
        private string _to = "";
        List<IOperation> _operations = new List<IOperation>();
        private Type _mappedType;
        private PropertyInfo _mappedProperty;


        protected CodeStatementCollection GenerateSetMappedPropertyCode(CodeExpression targetObj, CodeExpression value)
        {
            CodeStatementCollection statements = new CodeStatementCollection();
             
            CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(targetObj, MappedProperty.Name);

            if (IsCollection(_mappedProperty.PropertyType))
            {
                CodeBinaryOperatorExpression isNull = 
                    new CodeBinaryOperatorExpression(property, CodeBinaryOperatorType.ValueEquality,  new CodeSnippetExpression("null"));
                CodeAssignStatement create = new CodeAssignStatement(property, new CodeObjectCreateExpression(_mappedProperty.PropertyType));

                statements.Add(new CodeConditionStatement(isNull, create));
                statements.Add(new CodeMethodInvokeExpression(property, "Add", value));
            }
            else
                statements.Add(new CodeAssignStatement(property, value));

            return statements;
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

        public Type MappedType
        {
            get { return _mappedType; }
            set { _mappedType = value; }
        }

        public PropertyInfo MappedProperty
        {
            get { return _mappedProperty; }
            protected set { _mappedProperty = value; }
        }

        public abstract void BuildMapMethod(AssemblyBuilder info, CodeMemberMethod method);
    }
}