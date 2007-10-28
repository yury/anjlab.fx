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
        private AssemblyBuilder _builder;

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

        public AssemblyBuilder Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        public CodeGenerationContext Clone()
        {
            CodeGenerationContext clone = new CodeGenerationContext();
            clone.Builder = Builder;
            clone.DataArray = DataArray;
            clone.DataReader = DataReader;
            clone.MapInfo = MapInfo;
            clone.MappedObject = MappedObject;
            clone.MappedObjectType = MappedObjectType;
            return clone;
        }
    }
}