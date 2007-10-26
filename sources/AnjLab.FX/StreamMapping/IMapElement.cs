using System;
using System.CodeDom;
using System.Reflection;

namespace AnjLab.FX.StreamMapping
{
    public interface IMapElement
    {
        Type MappedType{ get; set;}
        String To { get; set; }
        PropertyInfo MappedProperty { get; }
        void BuildMapMethod(AssemblyBuilder builer, CodeMemberMethod method);
    }
}