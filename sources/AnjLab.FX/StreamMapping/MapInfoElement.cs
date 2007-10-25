using System;
using System.CodeDom;
using System.Reflection;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public abstract class MapInfoElement : IMapInfoElement
    {
        private int _length = 0;
        private string _to = "";

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public abstract CodeStatementCollection GenerateMapStatements(AssemblyBuilder info, CodeVariableReferenceExpression binaryReader,
            CodeVariableReferenceExpression result);

        protected PropertyInfo GetPropertyToSet(IMapInfo info)
        {
            if (String.IsNullOrEmpty(To))
                return null;

            PropertyInfo pInfo = info.MapedType.GetProperty(To);
            Guard.NotNull(pInfo, "Property '{0}' not found in '{1}'", To, info.MapedType.FullName);
            return pInfo;
        }
    }
}