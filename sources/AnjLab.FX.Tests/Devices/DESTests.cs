using AnjLab.FX.Devices;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Devices
{
    [TestFixture]
    public class DESTests
    {
        [Test]
        public void TestEncryptAndDecrypt()
        {
            const string key = "test key 0";
            const string source = "source string";
            byte[] encryptedData = DES.EncryptToBytes(source, key);
            string encryptedString = DES.EncryptToBase64(source, key);

            Assert.IsNotEmpty(encryptedString);
            Assert.IsNotEmpty(encryptedString);

            Assert.AreEqual(source, DES.DecryptFromBytes(encryptedData, key));
            Assert.AreEqual(source, DES.DecryptFromBase64(encryptedString, key));

            try
            {
                DES.DecryptFromBase64(encryptedString, key.Replace("0", "4"));
            }
            catch
            {
                return;
            }
            Assert.Fail();
        }
    }
}
