using System.Security.Cryptography;
using System.Text;

namespace AnjLab.FX.Devices
{
    public static class DES
    {
        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
        /// </summary>
        /// <param name="toEncrypt">string to be encrypted</param>
        /// <param name="securityKey">string to generate encryption key</param>
        public static byte[] EncryptToBytes(string toEncrypt, string securityKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(PrepareKey(securityKey));
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var tdes = new TripleDESCryptoServiceProvider
                           {
                               Key = keyArray,
                               Mode = CipherMode.ECB,
                               Padding = PaddingMode.PKCS7
                           };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return resultArray;
        }

        public static string EncryptToBase64(string toEncrypt, string securityKey)
        {
            byte[] resultArray = EncryptToBytes(toEncrypt, securityKey);
            return System.Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherBytes">encrypted bytes</param>
        /// <param name="securityKey">string to generate encryption key</param>
        public static string DecryptFromBytes(byte[] cipherBytes, string securityKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(PrepareKey(securityKey));

            var tdes = new TripleDESCryptoServiceProvider
                           {
                               Key = keyArray,
                               Mode = CipherMode.ECB,
                               Padding = PaddingMode.PKCS7
                           };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            tdes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }

        public static string DecryptFromBase64(string cipherString, string securityKey)
        {
            byte[] toEncryptArray = System.Convert.FromBase64String(cipherString);
            return DecryptFromBytes(toEncryptArray, securityKey);
        }

        private static string PrepareKey(string key)
        {
            if (key.Length < 24)
                key += "012345678901234567891234";
            if (key.Length > 24)
                return key.Substring(0, 24);
            return key;
        }
    }
}
