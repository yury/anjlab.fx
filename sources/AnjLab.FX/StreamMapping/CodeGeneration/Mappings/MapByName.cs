#if NET_3_5
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping
{
    public class MapByName : MapElement
    {
        private string _key;

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            if (!string.IsNullOrEmpty(_key))
            {
                ICodeGeneratorNode element = ctx.MapInfo.GetNamedMapping(_key);

                Guard.NotNull(element, "Named mapping '{0}' not found for {1}", _key, ctx.MappedObjectType.FullName);

                element.To = To;
                element.GenerateMappingCode(ctx, method);
            }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}
#endif
