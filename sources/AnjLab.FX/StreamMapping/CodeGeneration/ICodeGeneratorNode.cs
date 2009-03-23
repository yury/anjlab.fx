#if NET_3_5
using System;
using System.CodeDom;
using System.Reflection;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping
{
    public interface ICodeGeneratorNode
    {
        String To { get; set; }
        PropertyInfo MappedProperty { get; }
        void GenerateMappingCode(CodeGenerationContext ctx, CodeMemberMethod method);
    }
}
#endif