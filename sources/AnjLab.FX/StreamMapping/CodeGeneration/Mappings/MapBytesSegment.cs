#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping
{
    public class MapBytesSegment : ContainerMapElement
    {
        private Type _segmentType;

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            if (!String.IsNullOrEmpty(To))
                MappedProperty = TypeReflector.GetProperty(ctx.MappedObjectType, To);

            CodeExpression segmentObj;
            if (MappedProperty != null)
            {
                // mappedObj.PropName = new SegmentType();
                method.Statements.AddRange(GenerateSetMappedPropertyCode(ctx.MappedObject, new CodeObjectCreateExpression(_segmentType)));
                segmentObj = GetMappedProperty(ctx.MappedObject);
            }
            else
            {
                // this._segmentField = new SegmentType();
                segmentObj = ctx.Builder.AddNewField(SegmentType);
                method.Statements.Add(new CodeAssignStatement(segmentObj, new CodeObjectCreateExpression(_segmentType)));
            }

            CodeGenerationContext segmentCtx = ctx.Clone();
            segmentCtx.MappedObject = segmentObj;
            segmentCtx.MappedObjectType = _segmentType;
            method.Statements.AddRange(ctx.Builder.NewElementsMappingCode(segmentCtx, Nodes));
        }

        public Type SegmentType
        {
            get { return _segmentType; }
            set { _segmentType = value; }
        }
    }
}
#endif