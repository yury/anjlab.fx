#if NET_3_5
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using AnjLab.FX.IO;
using Microsoft.CSharp;

namespace AnjLab.FX.StreamMapping.CodeGeneration
{
    public class StreamMapperBuilder
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
            return new StreamMapperBuilder().InternalBuildBinaryMapper<TResult>(info);
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

        private CodeCompileUnit GenerateCompileUnit()
        {
            AddReference(typeof(IBinaryMapper<>));
            AddInheritedReferences(_mappedType);

            _generatedMapper = new CodeTypeDeclaration(MapperTypeName);
            _generatedMapper.Attributes = MemberAttributes.Public;
            _generatedMapper.BaseTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(_mappedType)));
            _generatedMapper.BaseTypes.Add(typeof(ICloneable));
            
            GenerateMapCode();
            GenerateCloneCode();

            CodeNamespace ns = new CodeNamespace(_namespace);
            ns.Types.Add(_generatedMapper);
            ns.Imports.Add(new CodeNamespaceImport("System = global::System"));
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(ns);
            
            return unit;
        }

        private void GenerateCloneCode()
        {
            CodeMemberMethod clone = new CodeMemberMethod();
            clone.Name = "Clone";
            clone.Attributes = MemberAttributes.Public;
            clone.ReturnType = new CodeTypeReference(typeof(object));
            clone.ImplementationTypes.Add(typeof(ICloneable));

            clone.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(_namespace + "." + MapperTypeName)));

            _generatedMapper.Members.Add(clone);
        }

        private void GenerateMapCode()
        {
            AddReference(typeof(MemoryStream));
            AddReference(typeof(BitReader));

            CodeMemberMethod map = new CodeMemberMethod();
            map.Name = "Map";
            map.Attributes = MemberAttributes.Public;
            map.ReturnType = new CodeTypeReference(_mappedType);
            map.ImplementationTypes.Add(new CodeTypeReference(typeof(IBinaryMapper<>).MakeGenericType(_mappedType)));
            map.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte[]), "data"));
            
            _generatedMapper.Members.Add(map);

            _generatedMapper.Members.Add(new CodeMemberField(typeof(byte[]), "_data"));
            _generatedMapper.Members.Add(new CodeMemberField(typeof(BitReader), "_reader"));
            _generatedMapper.Members.Add(new CodeMemberField(_mappedType, "_result"));

            CodeVariableReferenceExpression dataArray = new CodeVariableReferenceExpression("_data");
            CodeVariableReferenceExpression reader = new CodeVariableReferenceExpression("_reader");
            CodeVariableReferenceExpression result = new CodeVariableReferenceExpression("_result");

            // this._result = new TypeName();
            // this._data = data;
            // MemoryStream stream = new MemoryStream(this._data);
            // this._reader = new BitReader(stream);
            map.Statements.Add(new CodeAssignStatement(dataArray, new CodeArgumentReferenceExpression("data")));
            map.Statements.Add(new CodeAssignStatement(result, new CodeObjectCreateExpression(_mappedType)));
            
            map.Statements.Add(new CodeVariableDeclarationStatement(
                                   typeof(MemoryStream), "stream", new CodeObjectCreateExpression(typeof(MemoryStream), dataArray)));
            CodeExpression stream = new CodeVariableReferenceExpression("stream");
            
            map.Statements.Add(new CodeAssignStatement(reader, new CodeObjectCreateExpression(typeof(BitReader), stream)));

            CodeGenerationContext ctx = new CodeGenerationContext();
            ctx.Builder = this;
            ctx.MapInfo = _mapInfo;
            ctx.MappedObjectType = _mappedType;
            ctx.DataArray = dataArray;
            ctx.DataReader = reader;
            ctx.MappedObject = result;
            ctx.ResultObject = result;
            ctx.ResultObjectName = result.VariableName;

            map.Statements.AddRange(NewElementsMappingCode(ctx, ctx.MapInfo.Nodes));

            map.Statements.Add(new CodeMethodInvokeExpression(reader, "Close"));
            map.Statements.Add(new CodeMethodInvokeExpression(stream, "Close"));
            map.Statements.Add(new CodeMethodReturnStatement(result));
        }

        public void AddReference(Type type)
        {
            var assemblyLocation = type.Assembly.Location;
            if (!_compileParameters.ReferencedAssemblies.Contains(assemblyLocation))
                _compileParameters.ReferencedAssemblies.Add(assemblyLocation);
        }

        public void AddInheritedReferences(Type type)
        {
            if (type != typeof(object))
            {
                AddReference(type);
                AddInheritedReferences(type.BaseType);
            }
        }

        public CodeMemberMethod AddNewMethod()
        {
            return AddNewMethod(typeof(void));
        }

        private int _methodIndex = 0;
        public CodeMemberMethod AddNewMethod(Type returnType)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "GeneratedMethod" + _methodIndex++;
            method.Attributes = MemberAttributes.Private;
            method.ReturnType = new CodeTypeReference(returnType);

            _generatedMapper.Members.Add(method);
            return method;
        }

        private int _fieldIndex = 0;
        public CodeVariableReferenceExpression AddNewField(Type type)
        {
            string name = "_field" + _fieldIndex++;
            _generatedMapper.Members.Add(new CodeMemberField(type, name));
            return new CodeVariableReferenceExpression(name);
        }

        public CodeStatementCollection NewElementsMappingCode(CodeGenerationContext ctx, IList<ICodeGeneratorNode> elements)
        {
            CodeStatementCollection statements = new CodeStatementCollection();
            foreach (ICodeGeneratorNode element in elements)
            {
                CodeMemberMethod mapElementMethod = NewElementMappingMethod(ctx, element);
                statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), mapElementMethod.Name));
            }
            return statements;
        }

        public CodeMemberMethod NewElementMappingMethod(CodeGenerationContext ctx, ICodeGeneratorNode element)
        {
            CodeMemberMethod method = AddNewMethod();
            element.GenerateMappingCode(ctx, method);
            return method;
        }

        private string MapperTypeName
        {
            get { return String.Format(_typeFormat, _mappedType.Name); }
        }

        private string MapperAssemblyName
        {
            get { return String.Format(_assemblyFormat, _mappedType.FullName); }
        }
    }
}
#endif