#if NET_3_5
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.StreamMapping.Instructions;

namespace AnjLab.FX.StreamMapping.Instructions
{
    public class ValueCondition : ICondition
    {
        private string _value;
        private ValueScope _scope = ValueScope.Segment;

        public CodeExpression GetCondition(CodeGenerationContext ctx, CodeMemberMethod method)
        {
            if (Scope == ValueScope.Segment)
                return new CodePropertyReferenceExpression(ctx.MappedObject, _value);

            if (Scope == ValueScope.Global)
                return new CodePropertyReferenceExpression(ctx.ResultObject, _value);

            return new CodeSnippetExpression(Preprocess(ctx, _value));
        }

        private string Preprocess(CodeGenerationContext ctx, string value)
        {
            string preprocessed = value.Replace("$result", ctx.ResultObjectName);
            return preprocessed;
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ValueScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
    }

    public enum ValueScope
    {
        Segment,
        Global,
        None
        
    }
}

#endif