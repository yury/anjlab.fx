//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using AnjLab.FX.IO;

//using AnjLab.FX.Patterns.Generic.Inject;
//using NUnit.Framework;

//namespace AnjLab.FX.Tests.Patterns.Generic.Inject
//{
//    public class A
//    {
        
//    }

//    internal class B: A
//    {
        
//    }

//    internal class InternalClass
//    {
//    } 

//    class PrivateClass
//    {
//    }

//    [TestFixture]
//    public class ObjectFactoryTests
//    {
//        [Test]
//        public void TestAccess()
//        {
//            Assert.IsTrue(GetType().IsPublic);
//            Assert.IsTrue(typeof(InternalClass).IsNotPublic);
//            Assert.IsTrue(typeof(PrivateClass).IsNotPublic);
//        }

//        [Test]
//        public void TestLoading()
//        {
//            ObjectFactory.ReadXaml(@"Properties\sample.xaml");
//            Assert.IsNotNull(ObjectFactory.Module);
//            Assert.AreEqual(1, ObjectFactory.Module.Bindings.Count);
//        }

//        [Test]
//        public void TestSingletonActivation()
//        {
//            ObjectFactory.Clear();
//            ObjectFactory.ReadXaml(@"Properties\sample.xaml");
//            ILog log1 = ObjectFactory.Get<ILog>();
//            ILog log2 = ObjectFactory.Get<ILog>();
//            ILog log3 = ObjectFactory.Get<ILog>();
//            Assert.AreSame(log1, log2);
//            Assert.AreSame(log3, log2);
//        }

//        [Test]
//        public void TestPerCallActivation()
//        {
//            ObjectFactory.Clear();
//            ObjectFactory.ReadXaml(@"Properties\sample.xaml");

//            A a1 = ObjectFactory.Get<A>();
//            A a2 = ObjectFactory.Get<A>();

//            Assert.AreNotSame(a1, a2);
//        }

//        [Test]
//        public void TestExceptions()
//        {
//            ObjectFactory.Clear();
//            try
//            {
//                ObjectFactory.Get<ILog>();
//                Assert.Fail("Should throw exception");
//            } 
//            catch(KeyNotFoundException)
//            {
//            }
//        }
//    }
//}
