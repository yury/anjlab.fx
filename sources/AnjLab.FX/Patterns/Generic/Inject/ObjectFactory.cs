using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using System.Xml;
using AnjLab.FX.System;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public static class ObjectFactory
    {
        public const string GeneratedAssemblyName = "AnjLab.FX.Inject.Generated";
        private static Module _module;
        private static Dictionary<string, KeyedFactory<Type, Activator>> _scopedFactories = new Dictionary<string, KeyedFactory<Type, Activator>>();
        private static readonly KeyedFactory<Type, Activator> _defaultScopeFactory = new KeyedFactory<Type, Activator>();
        
        internal static Module Module
        {
            get { return _module; }
        }

        public static TProduct Get<TProduct>()
        {
            Activator activator = _defaultScopeFactory.Create(typeof (TProduct));
            if (activator == null)
                throw new KeyNotFoundException();
            return (TProduct)activator.GetInstance();
        }

        public static TProduct Get<TProduct>(params object[] args)
        {
            Activator activator = _defaultScopeFactory.Create(typeof (TProduct));
            if (activator == null)
                throw new KeyNotFoundException();
            return default(TProduct);
        }

        public static TProduct GetScoped<TProduct>(string scope)
        {
            return default(TProduct);
        }

        public static TProduct GetScoped<TProduct>(string scope, params object [] args)
        {
            return default(TProduct);
        }

        public static void ReadXaml(string pathToXaml)
        {
            ParserContext context = new ParserContext();
            
            using (Stream stream = new FileStream(pathToXaml, FileMode.Open))
            {
                _module = (Module) XamlReader.Load(stream, context);
            }
            foreach (Definition def in _module.Definitions.Values)
            {
                Ctor c = def as Ctor;
                if (c == null)
                    continue;
                if (!string.IsNullOrEmpty(c.InternalType))
                {
                    c.Type = Type.GetType(c.InternalType);
                }
            }
            Dictionary<string, Type> build = AssemblyBuilder.Build(_module);
            foreach (Scope scope in _module.Bindings)
            {
                foreach (Bind bind in scope)
                {
                    Definition def = _module.Definitions[bind.To];
                    Type factoryType = build[bind.To];
                    _defaultScopeFactory.RegisterLasyImmutable(bind.Type, delegate
                                                                              {
                                                                                  return
                                                                                      bind.Activation.Create(def,
                                                                                          (IObjectFactory)global::System.Activator.
                                                                                              CreateInstance(factoryType));
                                                                              });
                }
            }
        }

        public static void Clear()
        {
            _module = null;
            foreach (Type key in _defaultScopeFactory.Keys)
            {
                _defaultScopeFactory.Unregister(key);
            }
            _scopedFactories.Clear();
        }
    }
}
