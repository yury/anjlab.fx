using System.CodeDom;
using System.IO;

namespace AnjLab.FX.StreamMapping
{
    public interface IMapInfoElement
    {
        CodeStatementCollection GenerateMapStatements(AssemblyBuilder builer, CodeVariableReferenceExpression binaryReader, 
            CodeVariableReferenceExpression result);
    }
}