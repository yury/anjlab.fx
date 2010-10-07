using System;
using System.Globalization;
using System.Text;
using AnjLab.FX.Sys;

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
            str = str.Replace("0x", "");

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

        public static string UInt16ToReversedHexString(ushort n)
        {
            byte high = (byte)(n >> 8);
            byte low = (byte)((n << 8) >> 8);
            return ByteToHexString(high) + ByteToHexString(low);
        }

        public static ushort ReverseWordBytes(ushort sourceWord)
        {
            var high = (byte)(sourceWord >> 8);
            var low = (byte)(sourceWord & 0xFF);
            return (ushort)((low << 8) + high);
        }

        public static byte[] SplitToBytes(short word)
        {
            return new [] { (byte)(word >> 8), (byte)(word & 0xFF) };
        }

        public static byte[] ReverseBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;
            var newBytes = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                newBytes[i] = bytes[bytes.Length - i - 1];
            return newBytes;
        }

        public static bool[] IntToBitsArray(int s, byte size)
        {
            if (size > 16)
                size = 16;
            int mask = 1;
            var array = new bool[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = (s & mask) != 0;
                mask = mask << 1;
            }
            return array;
        }

        public static short BitsArrayToShort(bool[] array)
        {
            short word = 0;
            for (int i = array.Length - 1; i >= 0 ; i--)
            {
                word |= System.Convert.ToInt16(array[i]);
                word = (short)(word << 1);
            }
            word = (short)(word >> 1);
            return word;
        }

        public static string IntToHexString(int value)
        {
            var bytes = new []
                            {
                                (byte) (value >> 24),
                                (byte) ((value & 0x00ff0000) >> 16),
                                (byte) ((value & 0x0000ff00) >> 8),
                                (byte) (value & 0x000000ff),
                            };
            return BytesToHexString(bytes).ToString();
        }

        public static int HexStringToInt(string hex)
        {
            var bytes = HexStringToBytes(hex);
            int result = 0;
            if (bytes.Length >= 4)
                result = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
            else
                if (bytes.Length >= 2)
                    result = (bytes[0] << 8) + bytes[1];
                else
                    if (bytes.Length == 1)
                        result = bytes[0];
            return result;
        }

        public static unsafe float UIntToSingle(uint num)
        {
            return *(((float*) &num));
        }
        
        public static float IntToSingle(int num)
        {
            return UIntToSingle((uint) num);
        }
    }
}
