#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapBits : MapElement
    {
        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            if (!String.IsNullOrEmpty(To))
                MappedProperty = TypeReflector.GetProperty(ctx.MappedObjectType, To);

            if (MappedProperty != null)
            {
                if (MapNotBoolProperty)
                {
                    // type value = bitReader.ReadBits(length);
                    method.Statements.Add(new CodeVariableDeclarationStatement(GetPropertyValueType(), "value",
                        new CodeMethodInvokeExpression(ctx.DataReader, "ReadBits", new CodePrimitiveExpression(Length))));
                }
                else
                {
                    method.Statements.Add(
                        new CodeVariableDeclarationStatement(GetPropertyValueType(), "value",
                            new CodeBinaryOperatorExpression(
                                new CodeMethodInvokeExpression(ctx.DataReader, "ReadBits", new CodePrimitiveExpression(Length)),
                                CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0))));
                }

                // add operations code
                CodeVariableReferenceExpression value = new CodeVariableReferenceExpression("value");
                foreach (IOperation op in Operations)
                    method.Statements.AddRange(op.BuildOperation(ctx, this, value));

                method.Statements.AddRange(GenerateSetMappedPropertyCode(ctx.MappedObject, value));
            }
            else
            {   // just read
                method.Statements.Add(
                    new CodeMethodInvokeExpression(ctx.DataReader, "ReadBits", new CodePrimitiveExpression(Length)));
            }
        }

        private bool MapNotBoolProperty
        {
            get
            {
                if (MappedProperty.PropertyType == typeof(bool))
                    return false;

                if (MappedProperty.PropertyType.HasElementType && MappedProperty.PropertyType.GetElementType() == typeof(bool))
                    return false;

                return true;
            }
        }
    }
}
#endif