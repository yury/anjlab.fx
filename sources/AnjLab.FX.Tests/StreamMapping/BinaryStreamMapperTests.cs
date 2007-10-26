using AnjLab.FX.StreamMapping;
using NUnit.Framework;
using Convert=AnjLab.FX.Devices.Convert;

namespace AnjLab.FX.Tests.StreamMapping
{
    [TestFixture]
    public class BinaryStreamMapperTests
    {
        [Test]
        public void TestMapperFirstTime()
        {
            AssertMapper();
        }

        [Test]
        public void TestMapperSecondTime()
        {
            AssertMapper();
        }

        private void AssertMapper()
        {
            BinaryStreamMapper<TestObject> bd = new BinaryStreamMapper<TestObject>();
            TestObject testObj = bd.Map(Convert.HexStringToBytes("0AAAFFFFFFFFFF0000e77702"));

            Assert.IsNotNull(testObj);

            Assert.AreEqual(0x0a, testObj.ByteProperty);
            Assert.AreEqual(0xffaa - 128 + 1, testObj.ShortProperty);
            Assert.AreEqual(0xffffffff, testObj.IntProperty);
            Assert.AreEqual(0x7e, testObj.ShortPropertyWithTwoParts);
            Assert.AreEqual(false, testObj.BoolProperty);
            Assert.AreEqual(true, testObj.BoolProperty2);
            Assert.AreEqual(TestEnum.Second, testObj.EnumProperty);

        }
    }
}