using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public class NextBytesCondition : ICondition
    {
        private int _next;
        private string _bytesAre;
        private string _bytesAreNot;

        private readonly string _byteArraysAreEqualCode = @"
if (data.Length != defined.Length)
    return false;

for (int i = 0; i < data.Length;i++)
    if (data[i] != defined[i])
        return false;

return true;";

        private readonly string _byteArraysAreNotEqualCode = @"
if (data.Length != defined.Length)
    return true;

for (int i = 0; i < data.Length;i++)
    if (data[i] != defined[i])
        return true;

return false;";

        public CodeExpression GetCondition(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            Guard.IsTrue(Next > 0, "Next property of NextBytesCondition can't be 0");
            Guard.IsTrue(!String.IsNullOrEmpty(Bytes), "You must specify BytesAre or BytesAreNot property of NextBytesCondition");

            CodeMemberMethod conditionMethod = ctx.Builder.AddNewMethod(typeof(bool));
            conditionMethod.Statements.Add(new CodeSnippetStatement(String.Format(@"
                System.IO.MemoryStream stream = new System.IO.MemoryStream({0});
                System.IO.BinaryReader r = new System.IO.BinaryReader(stream);
                r.BaseStream.Position = {1}.BaseStream.Position;
                byte[] data = r.ReadBytes({2});
                r.Close();
                stream.Close();
                byte[] defined = AnjLab.FX.Devices.Convert.HexStringToBytes(""{3}"");
                {4}", ctx.DataArray.VariableName, ctx.DataReader.VariableName, Next, Bytes, CompareCode)));

            return new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), conditionMethod.Name);
        }

        public int Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public string BytesAre
        {
            get { return _bytesAre; }
            set { _bytesAre = value; }
        }

        public string BytesAreNot
        {
            get { return _bytesAreNot; }
            set { _bytesAreNot = value; }
        }

        private string Bytes
        {
            get { return !String.IsNullOrEmpty(BytesAre) ? BytesAre : BytesAreNot; }
        }

        private string CompareCode
        {
            get { return !String.IsNullOrEmpty(BytesAre) ? _byteArraysAreEqualCode : _byteArraysAreNotEqualCode; }
        }
    }
}
