using System;
using AnjLab.FX.Devices;
using NUnit.Framework;
using Convert=AnjLab.FX.Devices.Convert;

namespace AnjLab.FX.Tests.Devices
{
    [TestFixture]
    public class BytesFilterTests
    {
        [Test]
        public void TestFiltration()
        {
            BytesFilter f = new BytesFilter(0x7e, 0x7e);

            Assert.AreEqual(1, f.Proccess(Convert.HexStringToBytes("7e45447e65787e3456")).Length);
            Assert.AreEqual(3, f.Buffer.Length);
            Console.WriteLine("Buffer: " + Convert.BytesToHexString(f.Buffer));
        }
    }
}