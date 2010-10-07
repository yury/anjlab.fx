using System.Text;

namespace AnjLab.FX.Devices
{
    public static class DeviceExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return Convert.BytesToHexString(bytes).ToString();
        }

        public static string ToFormattedHexString(this byte[] bytes)
        {
            var str = bytes.ToHexString().ToUpper();
            var builder = new StringBuilder();
            for (int i = 0; i < str.Length/2; i++)
                builder.Append(str.Substring(i*2, 2) + " ");
            return builder.ToString();
        }

        public static string ToHexString(this short s)
        {
            return new [] {(byte) (s >> 8), (byte)(s & 0xFF)}.ToHexString();
        }

        public static string ToHexString(this byte b)
        {
            return new[] { b }.ToHexString();
        }

        public static byte[] ToBytes(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
                return new byte[0];
            return Convert.HexStringToBytes(sourceString);
        }

        public static short ToShort(this bool[] bitsArray)
        {
            if (bitsArray == null)
                return 0;
            return Convert.BitsArrayToShort(bitsArray);
        }

        public static byte ToByte(this bool[] bitsArray)
        {
            return (byte)bitsArray.ToShort();
        }

        public static bool[] ToBitsArray(this short s, byte size)
        {
            return Convert.IntToBitsArray(s, size);
        }

        public static bool[] ToBitsArray(this byte b)
        {
            return Convert.IntToBitsArray(b, 8);
        }
    }
}
