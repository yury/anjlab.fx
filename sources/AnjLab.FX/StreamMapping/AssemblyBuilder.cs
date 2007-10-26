using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using AnjLab.FX.IO;
using Microsoft.CSharp;

namespace AnjLab.FX.StreamMapping
{
    public class AssemblyBuilder
    {
        readonly string _namespace = "AnjLab.FX.Generated.StreamMapping";
        readonly string _assemblyFormat = "GeneratedFor{0}";
        readonly string _typeFormat = "{0}Mapper";

        private CompilerParameters _compileParameters;
        private CodeTypeDeclaration _generatedMapper;
        private MapInfo _mapInfo;
        private Type _mappedType;

        public static IBinaryMapper<TResult> BuildBinaryMapper<TResult>(MapInfo info)
        {
            return new AssemblyBuilder().InternalBuildBinaryMapper<TResult>(info);
        }

        private IBinaryMapper<TResult> InternalBuildBinaryMapper<TResult>(MapInfo info)
        {
            _mapInfo = info;
            _mappedType = typeof(TResult);

            _compileParameters = new CompilerParameters();
            _compileParameters.OutputAssembly = MapperAssemblyName + ".dll";
            _compileParameters.TempFiles.KeepFiles = true;
            
            #if DEBUG
            _compileParameters.IncludeDebugInformation = true;
            #endif

            CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromDom(_compileParameters, GenerateCompileUnit());
            if (results.Errors.HasErrors)
                throw new Exception(results.Errors[0].ToString());

            Type t = results.CompiledAssembly.GetType(_namespace + "." + MapperTypeName);
            return Activator.CreateInstance(t) as IBinaryMapper<TResult>;
        }

        public void AddReference(Type type)
        {
            _compileParameters.ReferencedAssemblies.Add(type.Assembly.Location);
        }

        private CodeCompileUnit GenerateCompileUnit()
        {
            AddReference(typeof(IBinaryMapper<>));
            AddReference(MappedType);

            GeneratedMapper = new CodeTypeDeclaration(MapperTypeName);
            GeneratedMapper.Attributes = MemberAttributes.Public;
            GeneratedMapper.BaseTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(MappedType)));
            
            GenerateMapCode();

            CodeNamespace ns = new CodeNamespace(_namespace);
            ns.Types.Add(GeneratedMapper);
            ns.Imports.Add(new CodeNamespaceImport("System = global::System"));

            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(ns);
            
            return unit;
        }

        private void GenerateMapCode()
        {
            AddReference(typeof(MemoryStream));
            AddReference(typeof(BitReader));

            CodeMemberMethod map = new CodeMemberMethod();
            map.Name = "Map";
            map.Attributes = MemberAttributes.Public;
            map.ReturnType = new CodeTypeReference(MappedType);
            map.ImplementationTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(MappedType)));
            map.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte[]), "data"));
            CodeArgumentReferenceExpression data = new CodeArgumentReferenceExpression("data");
            GeneratedMapper.Members.Add(map);

            GeneratedMapper.Members.Add(new CodeMemberField(typeof(byte[]), "_data"));
            CurrentDataCodeReference = new CodeVariableReferenceExpression("_data");

            map.Statements.Add(new CodeAssignStatement(CurrentDataCodeReference, data));
            // TypeName result = new TypeName();
            map.Statements.Add(new CodeVariableDeclarationStatement(MappedType, "result", new CodeObjectCreateExpression(MappedType)));
            CodeExpression result = new CodeVariableReferenceExpression("result");

            map.Statements.Add(new CodeVariableDeclarationStatement(
                typeof(MemoryStream), "stream", new CodeObjectCreateExpression(typeof(MemoryStream), data)));
            CodeExpression stream = new CodeVariableReferenceExpression("stream");

             map.Statements.Add(new CodeVariableDeclarationStatement(
                 typeof(BitReader), "reader", new CodeObjectCreateExpression(typeof(BitReader), stream)));
            CodeExpression reader = new CodeVariableReferenceExpression("reader");

            GenerateElementsMapCode(_mapInfo.Elements, map, reader, result, MappedType);

            map.Statements.Add(new CodeMethodInvokeExpression(reader, "Close"));
            map.Statements.Add(new CodeMethodInvokeExpression(stream, "Close"));
            map.Statements.Add(new CodeMethodReturnStatement(result));
        }

        private int _methodIndex = 0;
        public void AddMethod(CodeMemberMethod method)
        {
            method.Name += "UniqueIndex" + _methodIndex++;
            GeneratedMapper.Members.Add(method);
        }
       
        public void GenerateElementsMapCode(IList<IMapElement> elements, CodeMemberMethod rootMethod, CodeExpression reader, CodeExpression result, Type resultType)
        {
            foreach (IMapElement element in elements)
            {
                CodeMemberMethod mapElementMethod = GenerateElementMapMethod(element, resultType);
                AddMethod(mapElementMethod);

                rootMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                    mapElementMethod.Name, reader, result));
            }
        }

        public CodeMemberMethod GenerateElementMapMethod(IMapElement element, Type resultType)
        {
            CodeMemberMethod mapElement = new CodeMemberMethod();
            mapElement.Name = "MapProperty";
            mapElement.Attributes = MemberAttributes.Private;

            mapElement.Parameters.Add(new CodeParameterDeclarationExpression(typeof(BitReader), "bitReader"));
            mapElement.Parameters.Add(new CodeParameterDeclarationExpression(resultType, "mappedObject"));

            element.MappedType = resultType;
            element.BuildMapMethod(this, mapElement);
            return mapElement;
        }

        private string MapperTypeName
        {
            get { return String.Format(_typeFormat, MappedType.Name); }
        }

        private string MapperAssemblyName
        {
            get { return String.Format(_assemblyFormat, MappedType.FullName); }
        }

        public MapInfo MapInfo
        {
            get { return _mapInfo; }
        }

        private Type MappedType
        {
            get { return _mappedType; }
        }

        private CodeVariableReferenceExpression _currentData;
        public CodeVariableReferenceExpression CurrentDataCodeReference
        {
            get { return _currentData; }
            private set { _currentData = value; }
        }

        private CodeTypeDeclaration GeneratedMapper
        {
            get { return _generatedMapper; }
            set { _generatedMapper = value; }
        }
    }
}