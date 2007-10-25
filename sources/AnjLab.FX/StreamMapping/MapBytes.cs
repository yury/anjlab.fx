using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AnjLab.FX.StreamMapping
{
    public class MapBytes : MapInfoElement
    {
        public override CodeStatementCollection GenerateMapStatements(AssemblyBuilder builder,
                                                              CodeVariableReferenceExpression binaryReader,
                                                              CodeVariableReferenceExpression result)
        {
            CodeStatementCollection statements = new CodeStatementCollection();
            PropertyInfo pInfo = GetPropertyToSet(builder.MapInfo);
            builder.AddReference(typeof(MemoryStream));
            builder.AddReference(typeof(byte));

            // byte[] bytes = reader.ReadBytes(length);
            statements.Add(new CodeVariableDeclarationStatement(typeof(byte[]), "bytes",
                new CodeMethodInvokeExpression(binaryReader, "ReadBytes", new CodePrimitiveExpression(Length))));

            if (pInfo != null)
            {
                // MemoryStream newStream = new MemoryStream(bytes);
                CodeVariableReferenceExpression bytes = new CodeVariableReferenceExpression("bytes");
                statements.Add(new CodeVariableDeclarationStatement(typeof(MemoryStream), GetVariabeName("stream"), 
                    new CodeObjectCreateExpression(typeof(MemoryStream), bytes)));

                // BinarryReader newReader = new BinarryReader(newStream);
                CodeVariableReferenceExpression newStream = new CodeVariableReferenceExpression(GetVariabeName("stream"));
                statements.Add(new CodeVariableDeclarationStatement(typeof(BinaryReader), GetVariabeName("reader"),
                    new CodeObjectCreateExpression(typeof(BinaryReader), newStream)));

                //result.PropName = ?
                CodeVariableReferenceExpression newReader = new CodeVariableReferenceExpression(GetVariabeName("reader"));
                CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(result, pInfo.Name);
                statements.Add(new CodeAssignStatement(property, GetPropertyValueExpression(pInfo.PropertyType, newReader)));

                // newReader.Dispose();
                statements.Add(new CodeMethodInvokeExpression(newReader, "Close"));
                // newStream.Dispose();
                statements.Add(new CodeMethodInvokeExpression(newStream, "Close"));
            }

            return statements;
        }

        private string GetVariabeName(string name)
        {
            return string.Format("{0}For{1}", name, To);
        }


        private CodeExpression GetPropertyValueExpression(Type propertyType, CodeVariableReferenceExpression binaryReader)
        {
            if (propertyType == typeof(byte))
                return new CodeMethodInvokeExpression (binaryReader, "ReadByte");

            if (propertyType == typeof(short))
                return new CodeMethodInvokeExpression(binaryReader, "ReadInt16");

            if (propertyType == typeof(int))
                return new CodeMethodInvokeExpression(binaryReader, "ReadInt32");

            if (propertyType == typeof(float))
                return new CodeMethodInvokeExpression(binaryReader, "ReadSingle");

            if (propertyType == typeof(double))
                return new CodeMethodInvokeExpression(binaryReader, "ReadDouble");

            throw new NotSupportedException(String.Format("'{0}' doesn't support properties of type '{1}'", 
                GetType().FullName, propertyType.FullName));
        }
    }
}