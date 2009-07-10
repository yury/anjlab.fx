using System.Collections.Generic;
using AnjLab.FX.Collections;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Collections
{
    [TestFixture]
    public class UniqueListTests
    {
        [Test]
        public void TestValueTypeAddInsert()
        {
            IList<int> uniqueList = new UniqueList<int>();
            uniqueList.Add(1);
            uniqueList.Add(2);
            uniqueList.Add(3);
            uniqueList.Add(3);
            uniqueList.Add(2);
            uniqueList.Add(1);

            Assert.AreEqual(3, uniqueList.Count);

            uniqueList.Insert(0, 1);
            uniqueList.Insert(0, 2);
            uniqueList.Insert(0, 3);

            Assert.AreEqual(3, uniqueList.Count);
        }

        public void TestReferenceTypeAddInsert()
        {
            IList<string> uniqueList = new UniqueList<string>();
            uniqueList.Add(null);
            uniqueList.Add("x");
            uniqueList.Add("X");

            Assert.AreEqual(3, uniqueList.Count);

            uniqueList.Add(null);
            uniqueList.Add("x");
            uniqueList.Add("X");

            Assert.AreEqual(3, uniqueList.Count);

            uniqueList.Insert(0, null);
            uniqueList.Insert(0, "x");
            uniqueList.Insert(0, "X");

            Assert.AreEqual(3, uniqueList.Count);
        }
    }
}