using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class LstTests
    {
        [Test]
        public void TestToString()
        {
            int [] arr = new int[]{1, 2, 3, 4, 5};

            Assert.AreEqual("1, 2, 3, 4, 5", Lst.ToString(arr));

            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict["a"] = 1;
            dict["b"] = 2;

            Assert.AreEqual("a = 1, b = 2", Lst.ToString(dict, "{0} = {1}", ", "));
        }

        [Test]
        public void TestEmptyListToString()
        {
            int[] arr = new int[]{};
            Assert.AreEqual(string.Empty, Lst.ToString(arr));
        }
    }
}
