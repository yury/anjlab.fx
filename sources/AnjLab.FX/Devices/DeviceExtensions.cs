namespace AnjLab.FX.Devices
{
    public static class DeviceExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return Convert.BytesToHexString(bytes).ToString();
        }

        public static byte[] ToBytes(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
                return new byte[0];
            return Convert.HexStringToBytes(sourceString);
        }
    }
}
