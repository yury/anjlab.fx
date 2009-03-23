using System.Text;

namespace AnjLab.FX.Devices
{
    public static class Md5
    {
        public static string HashString(string Value)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(Value);
            data = x.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }
}
