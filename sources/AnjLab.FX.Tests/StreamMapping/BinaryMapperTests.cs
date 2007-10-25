using AnjLab.FX.Devices;
using AnjLab.FX.StreamMapping;
using NUnit.Framework;

namespace AnjLab.FX.Tests.StreamMapping
{
    [TestFixture]
    public class BinaryMapperTests
    {
        [Test]
        public void TestDeserialize()
        {
            BinaryMapper<TestObject> bd = new BinaryMapper<TestObject>();
            TestObject testObj = bd.Map(Convert.HexStringToBytes("0AAA"));

            Assert.IsNotNull(testObj);
        }
    }
}