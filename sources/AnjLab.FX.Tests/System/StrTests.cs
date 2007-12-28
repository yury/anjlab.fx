using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AnjLab.FX.System;
using NUnit.Framework;

namespace AnjLab.FX.Tests.System
{
    [TestFixture]
    public class StrTests
    {
        [Test]
        public void TestLinesFrom()
        {
            string src = @"line1

line2
line3
";
            string res = "";
            foreach (string line in Str.LinesFrom(src))
            {
                res += line + Environment.NewLine;
            }

            Assert.AreEqual(src, res);
        }

        [Test]
        public void TestContainsInvariantIgnoreCase()
        {
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "x"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "yx"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "xy"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "xyx"));

            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "x", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "yx", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "xy", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "xyx", "y"));

            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "z", "x", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "z", "yx", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "z", "xy", "y"));
            Assert.IsTrue(Str.ContainsInvariantIgnoreCase("x", "z", "xyx", "y"));
        }
    }
}
