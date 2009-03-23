#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

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
                    method.Statements.Add(new CodeVariableDeclarationStatement(MappedValueType, "value",
                        new CodeMethodInvokeExpression(ctx.DataReader, "ReadBits", new CodePrimitiveExpression(Length))));
                }
                else
                {
                    method.Statements.Add(
                        new CodeVariableDeclarationStatement(MappedValueType, "value",
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
                if (MappedValueType == typeof(bool))
                    return false;

                return true;
            }
        }
    }
}
#endif