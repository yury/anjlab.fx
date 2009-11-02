using System;
using System.Collections.Generic;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace Framework.Tests.System
{
    [TestFixture]
    public class ComparerFactoryTests
    {
        public class TestClass1
        {
            public int field1;
            public string field2;

            public TestClass1(int field1, string field2)
            {
                this.field1 = field1;
                this.field2 = field2;
            }
        }

        public class TestClass2
        {
            public TestClass1 class1;


            public TestClass2(TestClass1 class1)
            {
                this.class1 = class1;
            }
        }

        [Test]
        public void TestComparerFactory()
        {
            ComparerFactory f = new ComparerFactory();

            List<TestClass1> list = new List<TestClass1>();
            list.Add(new TestClass1(5, "1"));
            list.Add(new TestClass1(45, "2"));
            list.Add(new TestClass1(40, "233"));

            list.Sort(f.New<TestClass1>("field1"));

            Assert.IsTrue(list[0].field1 == 5);
            Assert.IsTrue(list[1].field1 == 40);
            Assert.IsTrue(list[2].field1 == 45);

            List<TestClass2> list2 = new List<TestClass2>();
            list2.Add(new TestClass2(new TestClass1(5, "3434")));
            list2.Add(new TestClass2(new TestClass1(45, "4")));
            list2.Add(new TestClass2(new TestClass1(10, "342334")));

            list2.Sort(f.New<TestClass2>("class1.field1"));

            Assert.IsTrue(list2[0].class1.field1 == 5);
            Assert.IsTrue(list2[1].class1.field1 == 10);
            Assert.IsTrue(list2[2].class1.field1 == 45);
        }
    }
}
