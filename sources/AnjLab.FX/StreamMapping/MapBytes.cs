using System;
using System.CodeDom;
using System.IO;
using System.Reflection;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapBytes : MapInfoElement
    {
        public override void BuildMapElementMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            PropertyInfo pInfo = null;
            if (!String.IsNullOrEmpty(To))
                pInfo = TypeReflector.GetProperty(builder.MappedType, To);

            builder.AddReference(typeof(MemoryStream));
            builder.AddReference(typeof(BinaryReader));
            builder.AddReference(typeof(byte));

            // BinarryReader reader = new BinarryReader(dataStream);
            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BinaryReader), "reader",
                new CodeObjectCreateExpression(typeof(BinaryReader), 
                    new CodeArgumentReferenceExpression(method.Parameters[0].Name))));
            
            CodeVariableReferenceExpression reader = new CodeVariableReferenceExpression("reader");

            // byte[] bytes = reader.ReadBytes(length);
            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(byte[]), "bytes",
                new CodeMethodInvokeExpression(reader, "ReadBytes", new CodePrimitiveExpression(Length))));

            if (pInfo != null)
            {
                // MemoryStream stream2 = new MemoryStream(bytes);
                CodeVariableReferenceExpression bytes = new CodeVariableReferenceExpression("bytes");
                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(MemoryStream), "stream2", 
                    new CodeObjectCreateExpression(typeof(MemoryStream), bytes)));

                // BinaryReader newReader = new BinarryReader(stream2);
                CodeVariableReferenceExpression stream2 = new CodeVariableReferenceExpression("stream2");
                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BinaryReader), "reader2",
                    new CodeObjectCreateExpression(typeof(BinaryReader), stream2)));

                //resultObject.PropName = ?
                CodeVariableReferenceExpression reader2 = new CodeVariableReferenceExpression("reader2");
                CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(
                    new CodeArgumentReferenceExpression(method.Parameters[1].Name), pInfo.Name);
                method.Statements.Add(new CodeAssignStatement(property, GetPropertyValueExpression(pInfo.PropertyType, reader2)));

                // newReader.Dispose();
                method.Statements.Add(new CodeMethodInvokeExpression(reader2, "Close"));
                // newStream.Dispose();
                method.Statements.Add(new CodeMethodInvokeExpression(stream2, "Close"));
            }

            // reader.Dispose();
            //method.Statements.Add(new CodeMethodInvokeExpression(reader, "Close"));
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

            throw new NotSupportedException(String.Format("'{0}' doesn't support properties of type '{1}'", 
                GetType().FullName, propertyType.FullName));
        }
    }
}