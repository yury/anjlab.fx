using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using AnjLab.FX.System;
using Microsoft.CSharp;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    class AssemblyBuilder
    {
        private readonly CodeCompileUnit _compileUnit = new CodeCompileUnit();
        private readonly UniqueList<string> _references = new UniqueList<string>();
        private int _namespaceIndex = 0;
        private readonly List<Assembly> _visibleAssemblies = new List<Assembly>();
        private readonly Dictionary<string, string> _factories = new Dictionary<string, string>();

        public void AddReference(Type type)
        {
            _references.Add(type.Assembly.Location);
        }

        static public Dictionary<string, Type> Build(Module module)
        {
            AssemblyBuilder ab = new AssemblyBuilder();

            ab.AddReference(ab.GetType());

            foreach (Scope scope in module.Bindings)
            {
                foreach (Bind bind in scope)
                {
                    ab.AddReference(bind.Type);
                }
            }
            
            foreach (Definition definition in module.GetDifinitions())
            {
                ab.AddReference(definition.Type);
                definition.Build(ab);
            }

            

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters pars = new CompilerParameters();
            pars.ReferencedAssemblies.AddRange(Lst.ToArray(ab._references));
            pars.GenerateInMemory = true;
            pars.OutputAssembly = ObjectFactory.GeneratedAssemblyName + ".dll";
            
            CompilerResults results = provider.CompileAssemblyFromDom(pars, ab._compileUnit);
            if (results.Errors.HasErrors)
                throw new Exception(results.Errors[0].ToString());
            
            Dictionary<string, Type> factories = new Dictionary<string, Type>();
            foreach (KeyValuePair<string, string> pair in ab._factories)
            {
                Type type = results.CompiledAssembly.GetType(pair.Value);
                factories.Add(pair.Key, type);
            }
            return factories;
        }

        public void BuildFromConstructorDefinition(Ctor ctor)
        {
            CodeNamespace cn = new CodeNamespace("Generated" + _namespaceIndex++ );
            CodeTypeDeclaration ctd = new CodeTypeDeclaration("Factory");
            ctd.Attributes = MemberAttributes.Public;
            ctd.BaseTypes.Add(typeof (IObjectFactory));
            cn.Types.Add(ctd);

            BuildNewMethodFromConstructor(ctd, ctor);
            BuildInitMethod(ctd, ctor);
            _factories.Add(ctor.Key, cn.Name+".Factory");

            _compileUnit.Namespaces.Add(cn);
        }

        private void BuildInitMethod(CodeTypeDeclaration ctd, Definition def)
        {
            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Name = "Init";
            cmm.Attributes = MemberAttributes.Public;
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IDictionary<string, object>), "initData"));
            ctd.Members.Add(cmm);
        }

        private void BuildNewMethodFromConstructor(CodeTypeDeclaration ctd, Ctor ctor)
        {
            ConstructorInfo ci = GetConstructor(ctor);
            if (ci == null)
                throw new InvalidOperationException(string.Format("Can't find suitable constructor for {0}", ctor.Key));

            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Name = "New";
            cmm.Attributes = MemberAttributes.Public;
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]), "args"));
            cmm.ReturnType = new CodeTypeReference(typeof(object));

            if (IsGeneratorVisible(ctor.Type))
            {
                
                ParameterInfo[] pars = ci.GetParameters();
                List<CodeExpression> varRefs = new List<CodeExpression>();
                for (int i = 0; i < pars.Length; i++)
                {
                    cmm.Statements.Add(
                        new CodeVariableDeclarationStatement(pars[i].ParameterType, "p" + i,
                                                             new CodeCastExpression(pars[i].ParameterType,
                                                                                    new CodeArrayIndexerExpression(
                                                                                        new CodeArgumentReferenceExpression
                                                                                            ("args"),
                                                                                        new CodePrimitiveExpression(i)))));
                    varRefs.Add(new CodeVariableReferenceExpression("p"+i));
                }
                cmm.Statements.Add(
                    new CodeMethodReturnStatement(new CodeObjectCreateExpression(ctor.Type, varRefs.ToArray())));
            }
            else
            {
                cmm.Statements.Add(
                    new CodeMethodReturnStatement(
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(typeof (global::System.Activator)), "CreateInstance",
                            new CodeTypeOfExpression(ctor.Type),
                            new CodeArgumentReferenceExpression("args"))));
            }
            ctd.Members.Add(cmm);
        }

        private bool IsGeneratorVisible(Type type)
        {
            if (type.IsPublic)
                return true;
            if (type.IsNotPublic)
            {
                if (type.Assembly == GetType().Assembly)
                    return true;

                if (_visibleAssemblies.Contains(type.Assembly))
                    return true;
                
                object[] attributes = type.Assembly.GetCustomAttributes(typeof (InternalsVisibleToAttribute), false);
                foreach (InternalsVisibleToAttribute o in attributes)
                {
                    if (o.AssemblyName == ObjectFactory.GeneratedAssemblyName)
                    {
                        _visibleAssemblies.Add(type.Assembly);
                        return true;
                    }
                }
            }
            return false;
        }

        private static ConstructorInfo GetConstructor(Ctor ctor)
        {
            ConstructorInfo[] constructors = ctor.Type.GetConstructors();
            foreach (ConstructorInfo info in constructors)
            {
                if (!info.IsPublic && !info.IsAssembly)
                    continue;

                if (ParametersMatched(ctor, info.GetParameters()))
                    return info;
            }
            return null;
        }

        private static bool ParametersMatched(Definition def, ParameterInfo [] parms)
        {
            if (parms.Length != def.Args.Count)
                return false;

            if (parms.Length == 0 && def.Args.Count == 0)
                return true;

            for (int i = 0; i < parms.Length; i++)
            {
                object value = def.Args[i];
                ParameterInfo param = parms[i];
                if (value == null && param.ParameterType.IsValueType)
                    return false;

                if (!parms[i].ParameterType.IsAssignableFrom(def.Args[i].GetType()))
                    return false;
            }

            return true;
        }

        public static MethodInfo GetMethod(Method method)
        {
            MethodInfo[] methods = method.Type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo info in methods)
            {
                if (info.Name != method.Name)
                    continue;
                if (ParametersMatched(method, info.GetParameters()))
                    return info;
            }
            return null;
        }

        public void BuildFromMethodDefinition(Method method)
        {
            CodeNamespace cn = new CodeNamespace("Generated" + _namespaceIndex++);
            CodeTypeDeclaration ctd = new CodeTypeDeclaration("Factory");
            ctd.Attributes = MemberAttributes.Public;
            ctd.BaseTypes.Add(typeof(IObjectFactory));
            cn.Types.Add(ctd);

            BuildNewMethodFromMethod(ctd, method);
            BuildInitMethod(ctd, method);
            _factories.Add(method.Key, cn.Name + ".Factory");

            _compileUnit.Namespaces.Add(cn);
        }

        private static void BuildNewMethodFromMethod(CodeTypeDeclaration ctd, Method method)
        {
            MethodInfo mi = GetMethod(method);
            if (mi == null)
                throw new InvalidOperationException(
                    string.Format("Can't find suitable static method for {0}", method.Key));

            CodeMemberMethod cmm = new CodeMemberMethod();
            cmm.Name = "New";
            cmm.Attributes = MemberAttributes.Public;
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]), "args"));
            cmm.ReturnType = new CodeTypeReference(typeof(object));

            ParameterInfo[] pars = mi.GetParameters();
            List<CodeExpression> varRefs = new List<CodeExpression>();
            for (int i = 0; i < pars.Length; i++)
            {
                cmm.Statements.Add(
                    new CodeVariableDeclarationStatement(pars[i].ParameterType, "p" + i,
                                                         new CodeCastExpression(pars[i].ParameterType,
                                                                                new CodeArrayIndexerExpression(
                                                                                    new CodeArgumentReferenceExpression
                                                                                        ("args"),
                                                                                    new CodePrimitiveExpression(i)))));
                varRefs.Add(new CodeVariableReferenceExpression("p" + i));
            }
            cmm.Statements.Add(new CodeMethodReturnStatement(
                                   new CodeMethodInvokeExpression(
                                       new CodeTypeReferenceExpression(method.Type), method.Name,
                                       varRefs.ToArray())));


            ctd.Members.Add(cmm);
        }
    }
}
