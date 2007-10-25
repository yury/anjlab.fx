using System;
using AnjLab.FX.StreamMapping;
using NUnit.Framework;
using Convert=AnjLab.FX.Devices.Convert;

namespace AnjLab.FX.Tests.StreamMapping
{
    [TestFixture]
    public class BinaryMapperTests
    {
        [Test]
        public void TestMap()
        {
            BinaryMapper<TestObject> bd = new BinaryMapper<TestObject>();
            TestObject testObj = bd.Map(Convert.HexStringToBytes("0AAAFFFFFFFFFF"));

            Assert.IsNotNull(testObj);

            Assert.AreEqual(0x0A, testObj.ByteProperty);
            Assert.AreEqual(0xFFAA, testObj.ShortProperty);
            Assert.AreEqual(0xFFFFFFFF, testObj.IntProperty);
        }
    }
}