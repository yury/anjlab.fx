using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using AnjLab.FX.Properties;
using Diag = System.Diagnostics;

namespace AnjLab.FX.Sys
{
    public class ComparerFactory
    {
        private const string _namespace = "AnjLab.FX.Comparers";
        private readonly Dictionary<string, object> _generatedComparers = new Dictionary<string, object>();

        #region Template methods
        /// <summary>
        /// Created a new comparer.
        /// </summary>
        /// <param name="compareBy">The compare by string. Can contain 'asc' or 'desc' parameter to specify direction.</param>
        /// <returns></returns>
        public IComparer<T> New<T>(string compareBy)
        {
            string comparerTypeName = GetTypeName(typeof(T), compareBy);

            if (_generatedComparers.ContainsKey(comparerTypeName))
                return (IComparer<T>)_generatedComparers[comparerTypeName];
            else
            {
                Type type = BuildComparer(typeof(T), comparerTypeName, compareBy);
                IComparer<T> comparer = (IComparer<T>)Activator.CreateInstance(type);
                _generatedComparers.Add(comparerTypeName, comparer);
                return comparer;
            }
        }
        #endregion

        private static Type BuildComparer(Type type, string name, string compareBy)
        {
            CodeDomProvider cdp = new Microsoft.CSharp.CSharpCodeProvider();
            
            CodeCompileUnit ccu = BuildCompileUnit(type, name, compareBy);
            CompilerParameters param = new CompilerParameters();

            List<Type> allTypes = new TypeReflector().GetAllTypes(type);

            foreach (Type t in allTypes)
            {
                string assemblyPath = t.Assembly.Location;
                if (param.ReferencedAssemblies.Contains(assemblyPath))
                    continue;
                param.ReferencedAssemblies.Add(assemblyPath);
            }

            param.ReferencedAssemblies.Add(typeof(IComparer<>).Assembly.Location);

#if DEBUG
            param.TempFiles.KeepFiles = true;
            param.IncludeDebugInformation = true;
#endif
            CompilerResults cr = cdp.CompileAssemblyFromDom(param, ccu);

            if (cr.Errors.Count > 0)
            {
                foreach (object error in cr.Errors)
                    Diag.Debug.WriteLine(error);
                throw new Exception(Resources.CodeGenerationError);
            }
            return cr.CompiledAssembly.GetType(_namespace + "." + name);
        }

        private static string GetTypeName(Type type, string compareBy)
        {
            return type.Name.Replace("[]", "_array").Replace("`", "_T") + "_" + compareBy.Replace(".", "") + "_Comparer";
        }

        private static CodeCompileUnit BuildCompileUnit(Type type, string name, string compareBy)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();

            ccu.Namespaces.Add(BuildNamespace(type, name, compareBy));

            return ccu;
        }

        private static CodeNamespace BuildNamespace(Type type, string name, string compareBy)
        {
            CodeNamespace cn = new CodeNamespace(_namespace);
            cn.Types.Add(Build(type, name, compareBy));
            return cn;
        }

        private static CodeTypeDeclaration Build(Type type, string name, string compareBy)
        {
            CodeTypeDeclaration ctd = new CodeTypeDeclaration(name);
            ctd.IsClass = true;
            ctd.BaseTypes.Add(typeof (IComparer<>).MakeGenericType(new Type[] {type}));

            CodeConstructor cc = new CodeConstructor();
            cc.Attributes = MemberAttributes.Public;
            ctd.Members.Add(cc);

            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Attributes = MemberAttributes.Public;
            cmm.Name = "Compare";
            cmm.ReturnType = new CodeTypeReference(typeof(int));
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(type, "xObj"));
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(type, "yObj"));
            cmm.Statements.Add(BuildStatement("object x = xObj.{0};", compareBy));
            cmm.Statements.Add(BuildStatement("object y = yObj.{0};", compareBy));
            cmm.Statements.Add(BuildStatement("if(x is global::System.IComparable) { return ((global::System.IComparable)x).CompareTo(y);}"));
            cmm.Statements.Add(BuildStatement("if(x == null && y == null) { return 0; }"));
            cmm.Statements.Add(BuildStatement("if(x == null) { return -1; }"));
            cmm.Statements.Add(BuildStatement("if(y == null) { return 1; }"));
            cmm.Statements.Add(BuildStatement("if(x.Equals(y)) { return 0; }"));
            cmm.Statements.Add(BuildStatement("return x.ToString().CompareTo(y.ToString());"));

            ctd.Members.Add(cmm);
            return ctd;
        }

        private static CodeSnippetStatement BuildStatement(string statement)
        {
            return new CodeSnippetStatement(statement);
        }

        private static CodeSnippetStatement BuildStatement(string format, params string[] args)
        {
            return BuildStatement(string.Format(format, args));
        }
    }

    class tc : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            throw new NotImplementedException();
        }
    }
}
