using System;
using System.CodeDom;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapBytesSegment : ContainerMapElement
    {
        private Type _segmentType;

        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            if (!String.IsNullOrEmpty(To))
                MappedProperty = TypeReflector.GetProperty(MappedType, To);

            CodeArgumentReferenceExpression bitReader = new CodeArgumentReferenceExpression(method.Parameters[0].Name);
            CodeArgumentReferenceExpression resultObj = new CodeArgumentReferenceExpression(method.Parameters[1].Name);
            
            // SegmentType segmentResult = new SegmentType();
            method.Statements.Add(new CodeVariableDeclarationStatement(
                SegmentType, "segmentResult", new CodeObjectCreateExpression(SegmentType)));
            CodeExpression segmentResult = new CodeVariableReferenceExpression("segmentResult");

            builder.GenerateElementsMapCode(Elements, method, bitReader, segmentResult, SegmentType);
         
            if (MappedProperty != null)
                method.Statements.AddRange(GenerateSetMappedPropertyCode(resultObj, segmentResult));
        }

        public Type SegmentType
        {
            get { return _segmentType; }
            set { _segmentType = value; }
        }
    }
}
