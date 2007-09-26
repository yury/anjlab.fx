using System;
using System.Collections.Generic;

namespace AnjLab.FX.Patterns.Generic
{
    public class KeyedFactory<TKey, TProduct>
    {
        public delegate TProduct FactoryMethod();

        public delegate TCProduct PrototypeFactoryMethod<TCProduct>()
            where TCProduct: ICloneable;

        readonly Dictionary<TKey, FactoryMethod> _factoryMethods =  new Dictionary<TKey, FactoryMethod>();

        public ICollection<TKey> Keys
        {
            get
            {
                return _factoryMethods.Keys;
            }
        }

        public TProduct Create(TKey key)
        {
            FactoryMethod method;
            if (_factoryMethods.TryGetValue(key, out method))
                return method();
            else
                return default(TProduct);
        }

        public bool IsRegistered(TKey key)
        {
            return _factoryMethods.ContainsKey(key);
        }

        public void Unregister(TKey key)
        {
            if (_factoryMethods.ContainsKey(key))
                _factoryMethods.Remove(key);
        }

        public void RegisterMethod(TKey key, FactoryMethod method)
        {
            _factoryMethods.Add(key, method);
        }

        public void RegisterType<TConcreteProduct>(TKey key)
            where TConcreteProduct: TProduct, new()
        {
            RegisterMethod(key, delegate { return new TConcreteProduct();});
        }

        public void RegisterImmutable(TKey key, TProduct instance)
        {
            RegisterMethod(key, delegate { return instance; });
        }

        public void RegisterPrototype<TConcreteProduct>(TKey key, TConcreteProduct instance)
            where TConcreteProduct: TProduct, ICloneable
        {
            RegisterMethod(key, delegate { return (TProduct) instance.Clone(); });   
        }

        public void RegisterLasyImmutable(TKey key, FactoryMethod method)
        {
            RegisterMethod(key, delegate {
                                    TProduct p = method();
                                    Unregister(key);
                                    RegisterImmutable(key, p);
                                    return p;
                                });
        }

        public void RegisterLasyPrototype<TConcreteProduct>(TKey key, PrototypeFactoryMethod<TConcreteProduct> method)
            where TConcreteProduct: TProduct, ICloneable
        {
            RegisterMethod(key, delegate {
                                    TConcreteProduct p = method();
                                    Unregister(key);
                                    RegisterPrototype(key, p);
                                    return (TProduct) p.Clone();
                                });
        }
    }
}