using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;

namespace AnjLab.FX.StreamMapping
{
    public class AssemblyBuilder
    {
        readonly string _namespace = "AnjLab.FX.Generated.StreamMapping";
        readonly string _assemblyFormat = "GeneratedFor{0}";
        readonly string _typeFormat = "{0}Mapper";

        private CompilerParameters _compileParameters;
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

            CodeTypeDeclaration imapper = new CodeTypeDeclaration(MapperTypeName);
            imapper.Attributes = MemberAttributes.Public;
            imapper.BaseTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(MappedType)));
            GenerateMapCode(imapper);

            CodeNamespace ns = new CodeNamespace(_namespace);
            ns.Types.Add(imapper);
            ns.Imports.Add(new CodeNamespaceImport("System = global::System"));

            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(ns);
            
            return unit;
        }

        private void GenerateMapCode(CodeTypeDeclaration imapper)
        {
            AddReference(typeof(Stream));
            AddReference(typeof(Debugger));

            CodeMemberMethod map = new CodeMemberMethod();
            map.Name = "Map";
            map.Attributes = MemberAttributes.Public;
            map.ReturnType = new CodeTypeReference(MappedType);
            map.ImplementationTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(MappedType)));
            map.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte[]), "data"));

            // TypeName result = new TypeName();
            map.Statements.Add(new CodeVariableDeclarationStatement(MappedType, "result",
                new CodeObjectCreateExpression(MappedType)));
            // MemoryStream stream = new MemoryStream(data);
            map.Statements.Add(new CodeVariableDeclarationStatement(typeof(MemoryStream), "stream",
                new CodeObjectCreateExpression(typeof(MemoryStream), new CodeArgumentReferenceExpression("data"))));
            
            CodeExpression stream = new CodeVariableReferenceExpression("stream");
            CodeExpression result = new CodeVariableReferenceExpression("result");
            foreach (IMapInfoElement element in _mapInfo.Elements)
            {
                // this.MapPropName(stream, result);
                CodeMemberMethod mapElementMethod = CreateMapElementMethod(element);
                map.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                    mapElementMethod.Name, stream, result));
                imapper.Members.Add(mapElementMethod);
            }

            // stream.Dispose();
            map.Statements.Add(new CodeMethodInvokeExpression(stream, "Close"));
            // return result;
            map.Statements.Add(new CodeMethodReturnStatement(result));

            imapper.Members.Add(map);
        }

        private int _methodIndex = 0;
        private CodeMemberMethod CreateMapElementMethod(IMapInfoElement element)
        {
            CodeMemberMethod mapElement = new CodeMemberMethod();
            mapElement.Name = "MapProperty" + _methodIndex++;
            mapElement.Attributes = MemberAttributes.Private;

            mapElement.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Stream), "dataStream") );
            mapElement.Parameters.Add(new CodeParameterDeclarationExpression(MappedType, "mappedObject"));

            element.BuildMapElementMethod(this, mapElement);
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

        public Type MappedType
        {
            get { return _mappedType; }
        }
    }
}