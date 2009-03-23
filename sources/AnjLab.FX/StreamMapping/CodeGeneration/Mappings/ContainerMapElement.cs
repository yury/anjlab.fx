#if NET_3_5
using System.CodeDom;
using System.Collections.Generic;
using System.Windows.Markup;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Nodes")]
    public class ContainerMapElement : MapElement
    {
        private List<ICodeGeneratorNode> _nodes = new List<ICodeGeneratorNode>();

        public List<ICodeGeneratorNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }

        public override void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            method.Statements.AddRange(ctx.Builder.NewElementsMappingCode(ctx, Nodes));
        }
    }
}
#endif