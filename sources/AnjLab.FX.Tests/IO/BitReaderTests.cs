using System.IO;
using AnjLab.FX.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    using FX.IO;

    [TestFixture]
    public class BitReaderTests: IOTestFixture
    {
        [Test]
        public void TestReadReduced()
        {
            MemoryStream stream = new MemoryStream(new byte[] {0x01, 0x04, 0xff, 0x05, });
            BitReader reader = new BitReader(stream, 4);
            Expect(reader.ReadReduced(), EqualTo(0x01));
            Expect(reader.ReadReduced(), EqualTo(0x04));
            Expect(reader.ReadReduced(), EqualTo(0x2ff));
            ExpectPosition(4, stream);   
        }

        [Test]
        public void TestReadBits()
        {
            MemoryStream stream = new MemoryStream(new byte[] {0xe4, 0x1b });
            BitReader reader = new BitReader(stream, 4);
            Expect(reader.ReadBits(), EqualTo(3));
            ExpectPosition(1, stream);
            Expect(reader.ReadBits(), EqualTo(2));
            ExpectPosition(1, stream);
            Expect(reader.ReadBits(), EqualTo(1));
            ExpectPosition(1, stream);
            Expect(reader.ReadBits(), EqualTo(0));
            ExpectPosition(1, stream);

            Expect(reader.ReadBits(), EqualTo(0));
            ExpectPosition(2, stream);
            Expect(reader.ReadBits(), EqualTo(1));
            ExpectPosition(2, stream);
            Expect(reader.ReadBits(), EqualTo(2));
            ExpectPosition(2, stream);
            Expect(reader.ReadBits(), EqualTo(3));
            ExpectPosition(2, stream);

            stream.Seek(0, SeekOrigin.Begin);
            reader = new BitReader(stream, 16);
            Expect(reader.ReadBits(), EqualTo(0xe));
            ExpectPosition(1, stream);
            Expect(reader.ReadBits(), EqualTo(0x4));
            ExpectPosition(1, stream);

            Expect(reader.ReadBits(), EqualTo(0x1));
            ExpectPosition(2, stream);
            Expect(reader.ReadBits(), EqualTo(0xb));
            ExpectPosition(2, stream);
        }
    }
}
