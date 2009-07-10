using System.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    using FX.IO;

    [TestFixture]
    public class BitReaderTests : IOTestFixture
    {
        [Test]
        public void TestReadBits()
        {
            MemoryStream stream = new MemoryStream(new byte[] { 0xe4, 0x1b, 0x1b, 0xf1, 0xff, 0xbb}); // 11100100, 00011011,  00011011
            BitReader reader = new BitReader(stream);

            Expect(reader.ReadBits(1), EqualTo(0x1));
            Expect(reader.ReadBits(2), EqualTo(0x3));
            Expect(reader.ReadBits(5), EqualTo(0x4));
            ExpectPosition(1, stream);
            Expect(reader.ReadBits(7), EqualTo(0xD));
            ExpectPosition(2, stream);
            Expect(reader.ReadBits(8), EqualTo(0x8D));
            ExpectPosition(3, stream);

            Expect(reader.ReadBits(2), EqualTo(0x3));
            ExpectPosition(4, stream);

            Expect(reader.ReadByte(), EqualTo(0xff));
            ExpectPosition(5, stream);

            Expect(reader.ReadBits(4), EqualTo(0xb));
            Expect(reader.ReadBits(4), EqualTo(0xb));
        }
    }
}
