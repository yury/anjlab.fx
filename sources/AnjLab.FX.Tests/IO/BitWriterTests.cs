using System.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    using FX.IO;

    [TestFixture]
    public class BitWriterTests : IOTestFixture
    {
        [Test]
        public void TestWriteBits()
        {
            using (MemoryStream s = new MemoryStream())
            using(BitWriter writer = new BitWriter(s))
            {
                writer.WriteBit(1);
                writer.WriteBit(1);
                writer.WriteBit(0);
                writer.WriteBit(1);
                writer.FlushBits();

                Assert.AreEqual(new byte[] { 0xd0 }, s.ToArray());

                writer.WriteBit(1);
                writer.WriteBit(1);
                writer.WriteBit(0);
                writer.WriteBit(1);
                writer.WriteBit(0);
                writer.WriteBit(0);
                writer.WriteBit(0);
                writer.WriteBit(1);

                Assert.AreEqual(new byte[]{0xd0, 0xd1}, s.ToArray());

                writer.WriteBit(1);
                writer.WriteBit(0);
                writer.Write((byte)1);

                writer.WriteBit(1);
                writer.WriteBit(1);
                writer.FlushBits();

                Assert.AreEqual(new byte[] { 0xd0, 0xd1, 0x01, 0x80, 0xc0 }, s.ToArray());
            }
        }
    }
}
