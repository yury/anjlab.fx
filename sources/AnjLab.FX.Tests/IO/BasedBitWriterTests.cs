using System.IO;
using AnjLab.FX.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    using FX.IO;

    [TestFixture]
    public class BasedBitWriterTests: IOTestFixture
    {
        [Test]
        public void TestWriteReduced()
        {
            MemoryStream stream = new MemoryStream();
            BasedBitWriter writer = new BasedBitWriter(stream, 4);

            writer.WriteReduced(1);
            ExpectFlushedPosition(1, stream);
            stream.Seek(0, SeekOrigin.Begin);
            writer.WriteReduced(127);
            ExpectFlushedPosition(1, stream);
            stream.Seek(0, SeekOrigin.Begin);
            writer.WriteReduced(0);
            ExpectFlushedPosition(1, stream);
            stream.Seek(0, SeekOrigin.Begin);
            writer.WriteReduced(1000);
            ExpectFlushedPosition(2, stream);
            stream.Seek(0, SeekOrigin.Begin);
            writer.WriteReduced(-1);
            ExpectFlushedPosition(5, stream);
        }

        [Test]
        public void TestWriteBits()
        {
            MemoryStream stream = new MemoryStream();
            BasedBitWriter writer = new BasedBitWriter(stream, 4);
            writer.WriteBits(0);
            ExpectPosition(0, stream);
            writer.WriteBits(1);
            ExpectPosition(0, stream);
            writer.WriteBits(2);
            ExpectPosition(0, stream);
            writer.WriteBits(3);
            ExpectPosition(1, stream);
            Expect(stream.GetBuffer()[0], EqualTo(0x1b));

            writer.WriteBits(3);
            ExpectPosition(1, stream);
            writer.WriteBits(2);
            ExpectPosition(1, stream);
            writer.WriteBits(1);
            ExpectPosition(1, stream);
            writer.WriteBits(0);
            ExpectPosition(2, stream);
            Expect(stream.GetBuffer()[1], EqualTo(0xe4));

            stream = new MemoryStream();
            writer = new BasedBitWriter(stream, 16);
            writer.WriteBits(0x4);
            ExpectPosition(0, stream);
            writer.WriteBits(0xf);
            ExpectPosition(1, stream);
            Expect(stream.GetBuffer()[0], EqualTo(0x4f));

            writer.WriteBits(0xb);
            ExpectPosition(1, stream);
            writer.WriteBits(0xa);
            ExpectPosition(2, stream);
            Expect(stream.GetBuffer()[1], EqualTo(0xba));
        }

        private static void ExpectFlushedPosition(int expectedPos, MemoryStream stream)
        {
            stream.Flush();
            ExpectPosition(expectedPos, stream);
        }
    }
}
