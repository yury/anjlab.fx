using System;
using AnjLab.FX.Collections;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Collections
{
    [TestFixture]
    public class ByteBitArrayTests
    {
        public void Test()
        {
            for (byte i = 0; i < 255; i++ )
                Assert.AreEqual(i, new ByteBitArray(i).GetByte());

            ByteBitArray array = new ByteBitArray(0x01);
            Assert.IsTrue(array[0]);
            Assert.IsFalse(array[1]);

            array = new ByteBitArray(0x03);
            Assert.IsTrue(array[0]);
            Assert.IsTrue(array[1]);

            array = new ByteBitArray(0x08);
            Assert.IsFalse(array[0]);
            Assert.IsFalse(array[1]);
            Assert.IsFalse(array[2]);
            Assert.IsTrue(array[3]);

            array = new ByteBitArray();
            array[0] = true;
            Assert.AreEqual(1, array.GetByte());
        }
    }
}
