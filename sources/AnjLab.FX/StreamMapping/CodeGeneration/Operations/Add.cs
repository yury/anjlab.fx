#if NET_3_5
using System.CodeDom;

namespace AnjLab.FX.StreamMapping.Operations
{
    public class Add : ValueOperation
    {
        public override CodeBinaryOperatorType OperationType
        {
            get { return CodeBinaryOperatorType.Add; }
        }
    }
}
#endif
