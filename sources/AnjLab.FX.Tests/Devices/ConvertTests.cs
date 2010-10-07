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

        [Test]
        public void TestSplitToBytes()
        {
            CollectionAssert.AreEqual(new [] {0, 0}, Convert.SplitToBytes(0));
            CollectionAssert.AreEqual(new[] { 0, 0xF1 }, Convert.SplitToBytes(0x00F1));
            CollectionAssert.AreEqual(new[] { 0x43, 0x21 }, Convert.SplitToBytes(0x4321));
        }

        [Test]
        public void TestReverseBytes()
        {
            CollectionAssert.AreEqual(null, Convert.ReverseBytes(null));
            CollectionAssert.AreEqual(new[] { 0, 0 }, Convert.ReverseBytes(new byte[] { 0, 0 }));
            CollectionAssert.AreEqual(new[] { 0x6A, 0xA6 }, Convert.ReverseBytes(new byte[] { 0xA6, 0x6A }));
            CollectionAssert.AreEqual(new[] { 0x81, 0 }, Convert.ReverseBytes(new byte[] { 0, 0x81 }));
        }

        [Test]
        public void TestReverseWordBytes()
        {
            Assert.AreEqual(0, Convert.ReverseWordBytes(0));
            Assert.AreEqual(0xFF00, Convert.ReverseWordBytes(0x00FF));
            Assert.AreEqual(0x3412, Convert.ReverseWordBytes(0x1234));
        }

        [Test]
        public void TestShortToBitsArray()
        {
            CollectionAssert.AreEqual(Convert.IntToBitsArray(0, 16), new [] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false});
            CollectionAssert.AreEqual(Convert.IntToBitsArray(1, 16), new[] { true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false });
            CollectionAssert.AreEqual(Convert.IntToBitsArray(8, 16), new[] { false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false });
            CollectionAssert.AreEqual(Convert.IntToBitsArray(8, 8), new[] { false, false, false, true, false, false, false, false });
        }

        [Test]
        public void TestBitsArrayToShort()
        {
            Assert.AreEqual(0, Convert.BitsArrayToShort(new[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }));
            Assert.AreEqual(2, Convert.BitsArrayToShort(new[] { false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false }));
            Assert.AreEqual(10, Convert.BitsArrayToShort(new[] { false, true, false, true, false, false, false, false, false, false, false, false, false, false, false, false }));
            Assert.AreEqual(3, Convert.BitsArrayToShort(new[] { true, true, false }));
        }

    }
}
