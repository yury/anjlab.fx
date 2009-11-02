using System;
using System.Diagnostics;
using System.Reflection;
using AnjLab.FX.Patterns.Generic;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Patterns.Generic
{
    public class Product: ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [TestFixture]
    public class KeyedFactoryTests
    {
        [Test]
        public void TestFactory()
        {
            KeyedFactory<string, string> f = new KeyedFactory<string, string>();
            f.RegisterImmutable("bla", "bla");
            f.RegisterMethod("bla1", delegate { return "bla1"; });

            Assert.AreEqual("bla", f.Create("bla"));
            Assert.AreEqual("bla1", f.Create("bla1"));
        }

        [Test, Ignore]
        public void TestFactorySpeed()
        {
            KeyedFactory<int, Product> factory = new KeyedFactory<int, Product>();
            int key = 0;
            factory.RegisterType<Product>(key++);
            factory.RegisterMethod(key++, delegate { return Activator.CreateInstance<Product>(); });
            factory.RegisterMethod(key++, delegate { return (Product) Activator.CreateInstance(typeof (Product));}) ;
            factory.RegisterMethod(key++, delegate { return new Product(); });
            factory.RegisterImmutable(key++, new Product());
            factory.RegisterPrototype(key++, new Product());

            ConstructorInfo ci = typeof (Product).GetConstructor(Type.EmptyTypes);
            factory.RegisterMethod(key++, delegate { return (Product) ci.Invoke(null); });

            foreach (int k in factory.Keys)
                factory.Create(k);

            int iterationCount = 1000000;

            foreach (int k in factory.Keys)
            {
                Stopwatch watch = Stopwatch.StartNew();
                for (int i = 0; i < iterationCount; i++)
                    factory.Create(k);
                Console.WriteLine("{0} {1}", k, watch.ElapsedMilliseconds);
            }
        }
    }
}
