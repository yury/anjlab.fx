using System;
using AnjLab.FX.Devices;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Devices
{
    [TestFixture]
    public class BytesFilterTests
    {
        [Test]
        public void TestFiltration()
        {
            var f = new BytesFilter(0x7e, 0x7e);

            Assert.AreEqual(1, f.Proccess("7e45447e65787e3456".ToBytes()).Length);
            Assert.AreEqual(3, f.Buffer.Length);
            Console.WriteLine("Buffer: " + f.Buffer.ToHexString());
        }

        [Test]
        public void Test2BytesFiltration()
        {
            var f = new BytesFilter("10".ToBytes(), "1003".ToBytes());

            var res = f.Proccess("100102031003040506101003".ToBytes());
            Assert.AreEqual(2, res.Length);
            Assert.AreEqual(6, res[0].Length);
            Assert.AreEqual(3, res[1].Length);
        }

        [Test]
        public void DataStartsFromTests()
        {
            Assert.IsTrue(BytesFilter.DataStartsFrom("0506".ToBytes(), "0506".ToBytes(), 0));
            Assert.IsTrue(BytesFilter.DataStartsFrom("0506".ToBytes(), "010203040506".ToBytes(), 4));
            Assert.IsFalse(BytesFilter.DataStartsFrom("0506".ToBytes(), "010203040506".ToBytes(), 2));
            Assert.IsFalse(BytesFilter.DataStartsFrom("0506".ToBytes(), "010203040506".ToBytes(), 5));
        }
    }
}