using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace AnjLab.FX.StreamMapping
{
    public class AssemblyBuilder
    {
        readonly string _namespace = "AnjLab.Fx.Generated.StreamMapping";
        readonly string _assemblyFormat = "GeneratedFor{0}";
        readonly string _typeFormat = "{0}Mapper{1}";

        readonly CompilerParameters _compileParameters = new CompilerParameters();
        private IMapInfo _mapInfo;

        public static object BuildMapper(IMapInfo info)
        {
            return new AssemblyBuilder().InternalBuildMapper(info);
        }

        private object InternalBuildMapper(IMapInfo info)
        {
            _mapInfo = info;

            CSharpCodeProvider provider = new CSharpCodeProvider();
            
            string typeName = GenerateTypeName(info);
            _compileParameters.OutputAssembly = GenerateAssemblyName(info) + ".dll";
            _compileParameters.TempFiles.KeepFiles = true;

            CompilerResults results = provider.CompileAssemblyFromDom(_compileParameters, GenerateCompileUnit(typeName));
            if (results.Errors.HasErrors)
                throw new Exception(results.Errors[0].ToString());

            Type t = results.CompiledAssembly.GetType(_namespace + "." + typeName);
            return Activator.CreateInstance(t);
        }

        public void AddReference(Type type)
        {
            _compileParameters.ReferencedAssemblies.Add(type.Assembly.Location);
        }

        private CodeCompileUnit GenerateCompileUnit(string typeName)
        {
            AddReference(_mapInfo.MapedType);
            AddReference(typeof(IMapper<>));

            CodeTypeDeclaration imapper = new CodeTypeDeclaration(typeName);
            imapper.Attributes = MemberAttributes.Public;
            imapper.BaseTypes.Add(new CodeTypeReference(typeof(IMapper<>).MakeGenericType(_mapInfo.MapedType)));
            imapper.Members.Add(GenerateMapMethod());

            CodeNamespace ns = new CodeNamespace(_namespace);
            ns.Types.Add(imapper);

            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(ns);

            return unit;
        }

        private CodeMemberMethod GenerateMapMethod()
        {
            CodeMemberMethod map = new CodeMemberMethod();
            map.Name = "Map";
            map.Attributes = MemberAttributes.Public;
            map.ReturnType = new CodeTypeReference(_mapInfo.MapedType);
            map.ImplementationTypes.Add(new CodeTypeReference(typeof(IMapper<>).MakeGenericType(_mapInfo.MapedType)));

            CodeParameterDeclarationExpression dataParameter =
                new CodeParameterDeclarationExpression(typeof(byte[]), "data");
            map.Parameters.Add(dataParameter);

            AddReference(typeof (MemoryStream));
            AddReference(typeof (BinaryReader));
            // TypeName result = new TypeName();
            map.Statements.Add(new CodeVariableDeclarationStatement(_mapInfo.MapedType, "obj",
                new CodeObjectCreateExpression(_mapInfo.MapedType)));
            // MemoryStream stream = new MemoryStream(data);
            map.Statements.Add(new CodeVariableDeclarationStatement(typeof(MemoryStream), "stream",
                new CodeObjectCreateExpression(typeof(MemoryStream), new CodeArgumentReferenceExpression("data"))));
            
            // BinarryReader reader = new BinarryReader(stream);
            CodeVariableReferenceExpression stream = new CodeVariableReferenceExpression("stream");
            map.Statements.Add(new CodeVariableDeclarationStatement(typeof(BinaryReader), "reader", 
                new CodeObjectCreateExpression(typeof(BinaryReader), stream)));

            CodeVariableReferenceExpression result = new CodeVariableReferenceExpression("obj");
            CodeVariableReferenceExpression reader = new CodeVariableReferenceExpression("reader");
            foreach (IMapInfoElement element in _mapInfo.Elements)
            {
                map.Statements.Add(new CodeCommentStatement(" ---"));
                map.Statements.AddRange(element.GenerateMapStatements(this, reader, result));
                map.Statements.Add(new CodeCommentStatement(" ---"));
            }
            // reader.Dispose();
            map.Statements.Add(new CodeMethodInvokeExpression(reader, "Close"));
            // stream.Dispose();
            map.Statements.Add(new CodeMethodInvokeExpression(stream, "Close"));
            // return result;
            map.Statements.Add(new CodeMethodReturnStatement(result));

            return map;
        }

        private string GenerateTypeName(IMapInfo info)
        {
            return String.Format(_typeFormat, info.MapedType.Name, Guid.NewGuid().ToString().Replace("-", ""));
        }

        private string GenerateAssemblyName(IMapInfo info)
        {
            return String.Format(_assemblyFormat, info.MapedType.FullName);
        }

        public IMapInfo MapInfo
        {
            get { return _mapInfo; }
        }
    }
}

