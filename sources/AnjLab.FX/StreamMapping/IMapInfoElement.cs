using System.CodeDom;

namespace AnjLab.FX.StreamMapping
{
    public interface IMapInfoElement
    {
        void BuildMapElementMethod(AssemblyBuilder builer, CodeMemberMethod method);
    }
}