namespace AnjLab.FX.Devices
{
    public static class DeviceExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return Convert.BytesToHexString(bytes).ToString();
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
    }
}
