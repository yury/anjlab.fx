using System.CodeDom;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class MapByName : MapElement
    {
        private string _key;

        public override void BuildMapMethod(AssemblyBuilder builder, CodeMemberMethod method)
        {
            if (!string.IsNullOrEmpty(_key))
            {
                Guard.IsTrue(builder.MapInfo.NamedMappings.ContainsKey(_key),
                    "Named mapping '{0}' not found for {1}", _key, MappedType.FullName);

                IMapElement element = builder.MapInfo.NamedMappings[_key];
                element.To = To;
                element.MappedType = MappedType;
                element.BuildMapMethod(builder, method);
            }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}
