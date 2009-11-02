using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
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
        public void TestInvariantIgnoreCaseIsPartAnyOf()
        {
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "x"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "yx"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "xy"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "xyx"));

            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "x", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "yx", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "xy", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "xyx", "y"));

            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "z", "x", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "z", "yx", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "z", "xy", "y"));
            Assert.IsTrue(Str.InvariantIgnoreCaseIsPartAnyOf("x", "z", "xyx", "y"));
        }

        [Test]
        public void TestInvariantIgnoreCaseContainsAnyOf()
        {
            Assert.IsFalse(Str.InvariantIgnoreCaseContainsAnyOf("a", "b"));

            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("ab", "a"));
            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("ba", "a"));
            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("bab", "bab"));

            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("a", "a", "b"));
            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("b", "b", "a"));
            Assert.IsTrue(Str.InvariantIgnoreCaseContainsAnyOf("ab", "b", "a", "b"));
        }
    }
}
