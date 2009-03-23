using System;
using System.IO;
using AnjLab.FX.IO;

namespace AnjLab.FX.Devices
{
    public class Ax25
    {
        public static byte[] Encode(byte[] data)
        {
            return Encode(data, 5);
        }

        public static byte[] Encode(byte[] data, int bitsInterval)
        {
            byte[] reverseBytes = Convert.ReverseBitsInBytes(data);

            using (var s = new MemoryStream(reverseBytes))
            using (var reader = new BitReader(s))
            {
                using (var encoded = new MemoryStream())
                using (var writer = new BitWriter(encoded))
                {
                    int counter = 0;
                    while (reader.CanRead)
                    {
                        byte bit = reader.ReadBits(1);
                        writer.WriteBit(bit);
                        counter = (bit == 1) ? counter + 1 : 0;
                        if (counter == bitsInterval)
                        {
                            writer.WriteBit(0);
                            counter = 0;
                        }
                    }

                    writer.FlushBits();
                    return Convert.ReverseBitsInBytes(encoded.ToArray());
                }
            }
        }

        public static byte[] Decode(byte[] data)
        {
            return Decode(data, 5);
        }

        public static byte[] Decode(byte[] data, int bitsInterval)
        {
            return Decode(data, 0, data.Length - 1, bitsInterval);
        }

        public static byte[] Decode(byte[] data, int from, int to)
        {
            return Decode(data, from, to, 5);
        }

        public static byte[] Decode(byte[] data, int from, int to, int bitsInterval)
        {
            var copiedData = new byte[to - from + 1];
            Array.Copy(data, from, copiedData, 0, copiedData.Length);

            byte[] reverseBytes = Convert.ReverseBitsInBytes(copiedData);

            using (var s = new MemoryStream(reverseBytes))
            using (var reader = new BitReader(s))
            {
                using (var decoded = new MemoryStream())
                using (var writer = new BitWriter(decoded))
                {
                    int counter = 0;
                    while (reader.CanRead)
                    {
                        byte bit = reader.ReadBits(1);
                        writer.WriteBit(bit);
                        counter = (bit == 1) ? counter + 1 : 0;
                        if (counter == bitsInterval)
                        {
                            reader.ReadBits(1); // skip next 0 bit
                            counter = 0;
                        }
                    }

                    // !! do not flush last bits
                    return Convert.ReverseBitsInBytes(decoded.ToArray());
                }
            }
        }
    }
}