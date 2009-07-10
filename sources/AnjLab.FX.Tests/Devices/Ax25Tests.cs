using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Devices;
using NUnit.Framework;
using Convert=AnjLab.FX.Devices.Convert;

namespace AnjLab.FX.Tests.Devices
{
    [TestFixture]
    public class Ax25Tests
    {
        [Test]
        public void TestEncodeDecode()
        {
            string[][] testCases = new string[][]
            {
                new string[2]{"feff00ffff", "beef03f8be2f"},
                new string[2]{"abcd", "abcd"},
                new string[2]{"1234567890", "1234567890"},
                new string[2]{"FFFF0000", "DFF7050000"},
                new string[2]{"EFFECC00", "EFBE990100"},
            };

            foreach (string[] testCase in testCases)
            {
                byte[] original = Convert.HexStringToBytes(testCase[0]);
                byte[] expected = Convert.HexStringToBytes(testCase[1]);

                byte[] result = Ax25.Encode(original);
                string expectedBits = Convert.BytesToBitString(expected, 0, expected.Length, false).ToString();
                string resultBits = Convert.BytesToBitString(result, 0, result.Length, false).ToString();

                Assert.AreEqual(expectedBits, resultBits, String.Format("Expected:{0}, Result:{1}. Original:{2}",
                    expectedBits, resultBits, testCase[0]));

                byte[] decodeResult = Ax25.Decode(result);
                string originalBits = Convert.BytesToBitString(original, 0, original.Length, false).ToString();
                string decodeBits = Convert.BytesToBitString(decodeResult, 0, decodeResult.Length, false).ToString();

                Assert.AreEqual(originalBits, decodeBits, String.Format("Expected:{0}, Result:{1}. Original:{2}",
                    originalBits, decodeBits, testCase[0]));
            }
        }
    }
}