using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using AnjLab.FX.Properties;

namespace AnjLab.FX.Sys
{
    public class DataTableAdapterFactory
    {
        private static readonly DataTableAdapterFactory _singletonInstance = new DataTableAdapterFactory();

        private const string _namespace = "AnjLab.Adapters";
        private readonly Dictionary<string, object> _generatedAdapters = new Dictionary<string, object>();

        #region Singleton point
        public static DataTableAdapterFactory Singleton
        {
            get { return _singletonInstance; }
        }
        #endregion

        #region Template method
        public IDataTableAdapter<T> New<T>(IEnumerable<PropertyColumn> columns)
        {
            string adapterTypeName = GetTypeName(typeof(T), columns);

            if (_generatedAdapters.ContainsKey(adapterTypeName))
                return (IDataTableAdapter<T>)_generatedAdapters[adapterTypeName];
            else
            {
                Type type = BuildAdapter(typeof(T), adapterTypeName, columns);
                IDataTableAdapter<T> adapter = (IDataTableAdapter<T>)Activator.CreateInstance(type);
                _generatedAdapters.Add(adapterTypeName, adapter);
                return adapter;
            }
        }
        #endregion

        private static Type BuildAdapter(Type type, string name, IEnumerable<PropertyColumn> columns)
        {
            CodeDomProvider cdp = new Microsoft.CSharp.CSharpCodeProvider();

            CodeCompileUnit ccu = BuildCompileUnit(type, name, columns);
            CompilerParameters param = new CompilerParameters();

            List<Type> allTypes = new TypeReflector().GetAllTypes(type);

            foreach (Type t in allTypes)
            {
                string assemblyPath = t.Assembly.Location;
                if (param.ReferencedAssemblies.Contains(assemblyPath))
                    continue;
                param.ReferencedAssemblies.Add(assemblyPath);
            }

            param.ReferencedAssemblies.Add(typeof(IDataTableAdapter<>).Assembly.Location);
            param.ReferencedAssemblies.Add(typeof(DataTable).Assembly.Location);
            param.ReferencedAssemblies.Add(typeof(IListSource).Assembly.Location);
            param.ReferencedAssemblies.Add(typeof(IXmlSerializable).Assembly.Location);

#if DEBUG
            param.TempFiles.KeepFiles = true;
            param.IncludeDebugInformation = true;
#endif
            CompilerResults cr = cdp.CompileAssemblyFromDom(param, ccu);

            if (cr.Errors.Count > 0)
            {
                foreach (object error in cr.Errors)
                    global::System.Diagnostics.Debug.WriteLine(error);
                throw new Exception(Resources.CodeGenerationError);
            }
            return cr.CompiledAssembly.GetType(_namespace + "." + name);
        }

        private static CodeCompileUnit BuildCompileUnit(Type type, string name, IEnumerable<PropertyColumn> columns)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();

            ccu.Namespaces.Add(BuildNamespace(type, name, columns));

            return ccu;
        }

        private static CodeNamespace BuildNamespace(Type type, string name, IEnumerable<PropertyColumn> columns)
        {
            CodeNamespace cn = new CodeNamespace(_namespace);
            cn.Imports.Add(new CodeNamespaceImport("System"));
            cn.Types.Add(Build(type, name, columns));
            return cn;
        }

        private static CodeTypeDeclaration Build(Type type, string name, IEnumerable<PropertyColumn> columns)
        {
            CodeTypeDeclaration ctd = new CodeTypeDeclaration(name);
            ctd.IsClass = true;
            ctd.BaseTypes.Add(typeof(IDataTableAdapter<>).MakeGenericType(new Type[] { type }));

            CodeConstructor cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            ctd.Members.Add(cc);
            ctd.Members.Add(CreateGetMethod(type, columns));
            return ctd;
        }

        private static CodeMemberMethod CreateGetMethod(Type type, IEnumerable<PropertyColumn> columns)
        {
            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public;
            cmm.Name = "Get";
            cmm.ReturnType = new CodeTypeReference(typeof(DataTable));
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(CodeBuilder.GetGenericType(typeof(IList<>), type), "list"));

            cmm.Statements.Add(CodeBuilder.BuildStatement("System.Data.DataTable dt = new System.Data.DataTable();"));
            foreach (PropertyColumn column in columns)
                cmm.Statements.Add(BuildAddColumnStatement(type, column));

            cmm.Statements.Add(BuildAddColumnStatement(type, new PropertyColumn("", "DataItem")));

            cmm.Statements.Add(CodeBuilder.BuildStatement("foreach({0} element in list)", GetTypeStringReference(type)));
            cmm.Statements.Add(CodeBuilder.BuildStatement("{"));

            cmm.Statements.Add(CodeBuilder.BuildStatement("System.Data.DataRow dr = dt.NewRow();"));
            foreach (PropertyColumn column in columns)
            {

                string memberName = string.Empty;
                string checkExp = string.Empty;
                foreach (string s in column.PropertyName.Split('.'))
                {
                    checkExp += string.Format(" && element.{1}{0} != null", s, memberName);
                    memberName += s + ".";
                }
                cmm.Statements.Add(CodeBuilder.BuildStatement("if(true {0})", checkExp));

                cmm.Statements.Add(CodeBuilder.BuildStatement("dr[\"{0}\"] = element.{1};", column.ColumnName, column.PropertyName));
                cmm.Statements.Add(CodeBuilder.BuildStatement("dr[\"DataItem\"] = element;"));
            }

            cmm.Statements.Add(CodeBuilder.BuildStatement("dt.Rows.Add(dr);"));
            cmm.Statements.Add(CodeBuilder.BuildStatement("}"));

            cmm.Statements.Add(CodeBuilder.BuildStatement("return dt;"));
            return cmm;
        }

        public static CodeStatement BuildAddColumnStatement(Type type, PropertyColumn column)
        {
            Type memberType = TypeReflector.GetMemberType(type, column.PropertyName);
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
                memberType = memberType.GetGenericArguments()[0];

            CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(new CodeSnippetExpression("dt.Columns"), "Add", new CodeExpression[] {
                new CodePrimitiveExpression(column.ColumnName),
                new CodeTypeOfExpression(memberType)
                });


            return new CodeExpressionStatement(e);
        }

        private static string GetTypeStringReference(Type type)
        {
            return type.FullName.Replace("+", ".");
        }

        private static string GetTypeName(Type type, IEnumerable<PropertyColumn> columns)
        {
            string hashString = string.Empty;
            foreach (PropertyColumn column in columns)
                hashString += column.ColumnName;

            return type.Name.Replace("[]", "_array").Replace("`", "_T") + "_" + Math.Abs(hashString.GetHashCode()) + "_DataTableAdapter";
        }
    }

    public class PropertyColumn
    {
        private string _propertyName;
        private string _columnName;


        public PropertyColumn()
        {
        }

        public PropertyColumn(string propertyName, string columnName)
        {
            _propertyName = propertyName;
            _columnName = columnName;
        }

        [NotifyParentProperty(true)]
        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        [NotifyParentProperty(true)]
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
    }
}
