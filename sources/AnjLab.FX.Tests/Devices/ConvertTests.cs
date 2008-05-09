using System;
using NUnit.Framework;
using Convert=AnjLab.FX.Devices.Convert;

namespace AnjLab.FX.Tests.Devices
{
    [TestFixture]
    public class ConvertTests
    {
        [Test]
        public void TestReverseBitsInBytes()
        {
            string[][] testCases = new string[][]
            {
                new string[2]{"feff00ffff", "7fff00ffff"},
                new string[2]{"7df7c03efbe8", "beef037cdf17"}
            };

            foreach (string[] testCase in testCases)
            {
                byte[] original = Convert.HexStringToBytes(testCase[0]);
                byte[] expected = Convert.HexStringToBytes(testCase[1]);

                byte[] result = Convert.ReverseBitsInBytes(original);
                Assert.AreEqual(expected, result, String.Format("Expected:{0}, Result:{1}. Original:{2}",
                                                                testCase[1],
                                                                Convert.BytesToHexString(result),
                                                                testCase[0]));
            }
        }

        [Test]
        public void TestHexStringToBytes()
        {
            Assert.AreEqual("ffff", Convert.BytesToHexString(Convert.HexStringToBytes("ffff")).ToString());
            Assert.AreEqual("ffff", Convert.BytesToHexString(Convert.HexStringToBytes("0xffff")).ToString());
        }

    }
}
