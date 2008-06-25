using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AnjLab.FX.Sys;
using System.Reflection;
using System.ComponentModel;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class ReflectorTests
    {
        public class TestClass1
        { 
        }

        public class TestClass2
        {
            public string TestField1;
        }

        public class TestClass3
        {
            [System.ComponentModel.Description("Тестовое поле")]
            public string TestField1 = "Test";

            [Browsable(true)]
            public string TestProperty1 { get; set; }

            [Browsable(false)]
            public string TestProperty2 { get; set; }
        }

        [Test]
        public void TestGetMemberEnumerable()
        {
            var result = Reflector.GetMemberEnumerable(new TestClass1());
            Assert.AreEqual(0, result.Count());

            result = Reflector.GetMemberEnumerable(new TestClass2());
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("TestField1", result.First().Key);
            Assert.AreEqual(null, result.First().Value);

            result = Reflector.GetMemberEnumerable(new TestClass3 { TestProperty1 = "Test"});
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Тестовое поле", result.First().Key);
            Assert.AreEqual("Test", result.First().Value);

            Assert.AreEqual("TestProperty1", result.ToArray()[1].Key);
            Assert.AreEqual("Test", result.ToArray()[1].Value);
        }
    }
}
