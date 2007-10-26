using System;
using System.CodeDom;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapBits : MapElement
    {
        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            if (!String.IsNullOrEmpty(To))
                MappedProperty = TypeReflector.GetProperty(MappedType, To);

            CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            CodeArgumentReferenceExpression resultObj = new CodeArgumentReferenceExpression(method.Parameters[1].Name);

            if (MappedProperty != null)
            {
                if (MapNotBoolProperty)
                {
                    // type value = bitReader.ReadBits(length);
                    method.Statements.Add(new CodeVariableDeclarationStatement(MappedProperty.PropertyType, "value",
                        new CodeMethodInvokeExpression(bitReader, "ReadBits", new CodePrimitiveExpression(Length))));
                }
                else
                {
                    method.Statements.Add(
                        new CodeVariableDeclarationStatement(MappedProperty.PropertyType, "value",
                            new CodeBinaryOperatorExpression(
                                new CodeMethodInvokeExpression(bitReader, "ReadBits", new CodePrimitiveExpression(Length)),
                                CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0))));
                }


                // add operations code
                CodeVariableReferenceExpression value = new CodeVariableReferenceExpression("value");
                foreach (IOperation op in Operations)
                    method.Statements.AddRange(op.BuildOperation(this, value, resultObj));

                method.Statements.AddRange(GenerateSetMappedPropertyCode(resultObj, value));
            }
            else
            {
                // just read
                method.Statements.Add(new CodeSnippetStatement(String.Format("{0}.ReadBits({1});", bitReader.ParameterName, Length)));
            }
        }

        private bool MapNotBoolProperty
        {
            get { return MappedProperty.PropertyType != typeof (bool); }
        }
    }
}