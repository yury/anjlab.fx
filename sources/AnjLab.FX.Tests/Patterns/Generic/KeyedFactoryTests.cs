using AnjLab.FX.Patterns.Generic;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Patterns.Generic
{
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
    }
}
