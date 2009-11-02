using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AnjLab.FX.Sys;
using System.Data;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class DataTableAdapterTests
    {
        public class TestClass1
        {
            public int? Field;
            private string property;

            public TestClass1(int field, string property)
            {
                this.Field = field;
                this.property = property;
            }

            public string Property
            {
                get { return property; }
            }
        }

        public class TestClass2
        {
            private TestClass1 class1;
            public DateTime Time;

            public TestClass2()
            {
                
            }

            public TestClass2(TestClass1 class1)
            {
                this.class1 = class1;
            }

            public TestClass1 Class1
            {
                get { return class1; }
            }

        }

        [Test]
        public void TestCreateAdapter()
        {
            DataTableAdapterFactory factory = new DataTableAdapterFactory();
            PropertyColumn[] columns = new PropertyColumn[]
                {
                    new PropertyColumn("Class1.Field", "Field"),
                    new PropertyColumn("Class1.Property", "Property"),
                    new PropertyColumn("Time", "Time")
                };

            IDataTableAdapter<TestClass2> adapter = factory.New<TestClass2>(columns);

            List<TestClass2> list = new List<TestClass2>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new TestClass2(new TestClass1(4, "4")));
                list.Add(new TestClass2(new TestClass1(40, "40")));
                list.Add(new TestClass2(new TestClass1(12, "12")));
                list.Add(new TestClass2());
            }

            DataTable dt = adapter.Get(list);

            Assert.IsNotNull(dt);
            Assert.AreEqual(list.Count, dt.Rows.Count);
            Assert.AreEqual(4, dt.Columns.Count);

            Assert.AreEqual(list[0].Class1.Property, dt.Rows[0]["Property"]);
            Assert.AreEqual(list[0].Class1.Field, dt.Rows[0]["Field"]);
            Assert.AreEqual(list[0].Time, dt.Rows[0]["Time"]);
            Assert.AreSame(list[0], dt.Rows[0]["DataItem"]);

            IDataTableAdapter<TestClass2> adapter2 = factory.New<TestClass2>(columns);
            Assert.AreSame(adapter, adapter2);

            PropertyColumn[] columns2 = new PropertyColumn[]
                {
                    new PropertyColumn("Class1.Field", "Field")
                };

            IDataTableAdapter<TestClass2> adapter3 = factory.New<TestClass2>(columns2);
            Assert.AreNotSame(adapter, adapter3);
        }
    }
}
