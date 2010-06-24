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

            res = f.Proccess("106400070210031070c900ffab020009501d00000000000000001003".ToBytes());
            Assert.AreEqual(2, res.Length);
            Assert.AreEqual(7, res[0].Length);
            Assert.AreEqual(21, res[1].Length);

            f.Clear();
            f.Proccess("10".ToBytes());
            f.Proccess("64".ToBytes());
            f.Proccess("0a".ToBytes());
            f.Proccess("07".ToBytes());
            f.Proccess("02".ToBytes());
            f.Proccess("10".ToBytes());
            f.Proccess("03".ToBytes());
            f.Proccess("10".ToBytes());
            f.Proccess("6400".ToBytes());
            res = f.Proccess("07021003".ToBytes());
            Assert.AreEqual(1, res.Length);
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