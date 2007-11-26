using System;
using System.Globalization;
using System.Text;
using AnjLab.FX.System;

namespace AnjLab.FX.Devices
{
    public class Convert
    {
        public static StringBuilder BytesToHexString(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();
            if (bytes != null)
                foreach (byte b in bytes)
                    str.Append(ByteToHexString(b));
            return str;
        }

        public static string ByteToHexString(byte oneByte)
        {
            string s = global::System.Convert.ToString(oneByte, 16);
            return (s.Length == 1) ? "0" + s : s;
        }

        public static byte[] HexStringToBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException("str", "is null or empty");

            str = str.Trim();
            str = str.Replace(" ", "");

            if (str.Length % 2 != 0)
                throw new FormatException("String length must be even");

            byte[] res = new byte[str.Length / 2];
            for (int i = 0; i < res.Length; i++)
                res[i] = byte.Parse(str.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
            return res;
        }

        public static byte[] BitStringToBytes(StringBuilder bits, bool reverseBytes)
        {
            byte[] result = new byte[bits.Length / 8];

            for (int i = 0; i < result.Length; i++)
            {
                string byteStr = bits.ToString(i * 8, 8);
                if (reverseBytes)
                    byteStr = ReverseByte(byteStr);
                result[i] = global::System.Convert.ToByte(byteStr, 2);
            }

            return result;
        }

        public static byte[] ReverseBitsInBytes(byte[] data)
        {
            byte[] reverse = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                byte reverseByte = data[i];
                for (int k = 0; k < 8; k++)
                {
                    byte lowBit = (byte)(reverseByte & 0x01);
                    reverse[i] = (byte)(reverse[i] | (lowBit << 7 - k));
                    reverseByte = (byte)(reverseByte >> 1);
                }
            }
            return reverse;
        }

        public static StringBuilder BytesToBitString(byte[] bytes, int fromInclusive, int toExlusive, bool reverseBytes)
        {
            Guard.GreaterThan(toExlusive, fromInclusive, "fromInclusive is greater then toExlusive", fromInclusive, toExlusive);
            Guard.GreaterThan(toExlusive, bytes.Length - 1, "toExlusive is greater than bytes array length", toExlusive);

            StringBuilder str = new StringBuilder();
            for (int i = fromInclusive; i < toExlusive; i++)
            {
                string s = global::System.Convert.ToString(bytes[i], 2);
                if (reverseBytes)
                    s = ReverseByte(s);

                while (s.Length != 8)
                {
                    if (reverseBytes)
                        s += "0";
                    else
                        s = "0" + s;
                }

                str.Append(s);
            }
            return str;
        }

        public static string ReverseByte(string byteStr)
        {
            char[] res = new char[byteStr.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = byteStr[byteStr.Length - 1 - i];

            return new string(res);
        }
    }
}
