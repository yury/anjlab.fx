using System.CodeDom;

namespace AnjLab.FX.StreamMapping
{
    public class MapBits : MapInfoElement
    {
        public override CodeStatementCollection GenerateMapStatements(AssemblyBuilder info,
                                                                      CodeVariableReferenceExpression binaryReader,
                                                                      CodeVariableReferenceExpression result)
        {
            return new CodeStatementCollection();
        }
    }
}