using System;
using System.CodeDom;
using System.IO;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapBytes : MapElement
    {
        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            if (!String.IsNullOrEmpty(To))
                MappedProperty = TypeReflector.GetProperty(MappedType, To);

            CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            CodeArgumentReferenceExpression resultObj = new CodeArgumentReferenceExpression(method.Parameters[1].Name);

            // byte[] bytes = reader.ReadBytes(length);
            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(byte[]), "bytes",
                new CodeMethodInvokeExpression(bitReader, "ReadBytes", new CodePrimitiveExpression(Length))));

            if (MappedProperty != null)
            {
                // MemoryStream stream2 = new MemoryStream(bytes);
                CodeVariableReferenceExpression bytes = new CodeVariableReferenceExpression("bytes");
                method.Statements.Add(new CodeVariableDeclarationStatement(
                    typeof(MemoryStream), "stream2", new CodeObjectCreateExpression(typeof(MemoryStream), bytes)));
                CodeVariableReferenceExpression stream2 = new CodeVariableReferenceExpression("stream2");

                // BinaryReader newReader = new BinarryReader(stream2);
                
                method.Statements.Add(new CodeVariableDeclarationStatement(
                    typeof(BinaryReader), "reader2", new CodeObjectCreateExpression(typeof(BinaryReader), stream2)));
                CodeVariableReferenceExpression reader2 = new CodeVariableReferenceExpression("reader2");

                //Type value = ?
                method.Statements.Add(new CodeVariableDeclarationStatement(
                    MappedProperty.PropertyType, "value", GetPropertyValueExpression(MappedProperty.PropertyType, reader2)));
                CodeVariableReferenceExpression value = new CodeVariableReferenceExpression("value");

                // add operations code
                foreach (IOperation op in Operations)
                    method.Statements.AddRange(op.BuildOperation(this, value, resultObj));

                method.Statements.AddRange(GenerateSetMappedPropertyCode(resultObj, value));

                // newReader.Dispose();
                method.Statements.Add(new CodeMethodInvokeExpression(reader2, "Close"));
                // newStream.Dispose();
                method.Statements.Add(new CodeMethodInvokeExpression(stream2, "Close"));
            }
        }

        private CodeExpression GetPropertyValueExpression(Type propertyType, CodeVariableReferenceExpression binaryReader)
        {
            if (propertyType == typeof(byte))
                return new CodeMethodInvokeExpression (binaryReader, "ReadByte");

            if (propertyType == typeof(short))
                return new CodeMethodInvokeExpression(binaryReader, "ReadInt16");

            if (propertyType == typeof(ushort))
                return new CodeMethodInvokeExpression(binaryReader, "ReadUInt16");

            if (propertyType == typeof(int))
                return new CodeMethodInvokeExpression(binaryReader, "ReadInt32");
            
            if (propertyType == typeof(uint))
                return new CodeMethodInvokeExpression(binaryReader, "ReadUInt32");

            if (propertyType == typeof(float))
                return new CodeMethodInvokeExpression(binaryReader, "ReadSingle");

            if (propertyType == typeof(double))
                return new CodeMethodInvokeExpression(binaryReader, "ReadDouble");

            if (propertyType.IsEnum)
            {
                Type enumBaseType = Enum.GetUnderlyingType(propertyType);
                return new CodeCastExpression(MappedProperty.PropertyType,
                                           GetPropertyValueExpression(enumBaseType, binaryReader));
            }

            throw new NotSupportedException(String.Format("'{0}' doesn't support properties of type '{1}'", 
                GetType().FullName, propertyType.FullName));
        }
    }
}