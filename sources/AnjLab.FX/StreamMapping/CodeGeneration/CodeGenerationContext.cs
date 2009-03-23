#if NET_3_5
using System;
using System.CodeDom;
using AnjLab.FX.StreamMapping.CodeGeneration;

namespace AnjLab.FX.StreamMapping.CodeGeneration
{
    public class CodeGenerationContext
    {
        private MapInfo _mapInfo;
        private Type _mappedType;
        private CodeVariableReferenceExpression _dataArray;
        private CodeVariableReferenceExpression _reader;
        private CodeExpression _mappedObject;
        private CodeExpression _resultObject;
        private StreamMapperBuilder _builder;
        private string _resultObjectName = "";

        public string ResultObjectName
        {
            get { return _resultObjectName; }
            set { _resultObjectName = value; }
        }

        public MapInfo MapInfo
        {
            get { return _mapInfo; }
            set { _mapInfo = value; }
        }

        public Type MappedObjectType
        {
            get { return _mappedType; }
            set { _mappedType = value; }
        }

        public CodeVariableReferenceExpression DataArray
        {
            get { return _dataArray; }
            set { _dataArray = value; }
        }

        public CodeVariableReferenceExpression DataReader
        {
            get { return _reader; }
            set { _reader = value; }
        }

        public CodeExpression MappedObject
        {
            get { return _mappedObject; }
            set { _mappedObject = value; }
        }

        public StreamMapperBuilder Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        public CodeExpression ResultObject
        {
            get { return _resultObject; }
            set { _resultObject = value; }
        }

        public CodeGenerationContext Clone()
        {
            return (CodeGenerationContext)MemberwiseClone();
        }
    }
}
#endif