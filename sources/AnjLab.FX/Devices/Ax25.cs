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

            using (MemoryStream s = new MemoryStream(reverseBytes))
            using (BitReader reader = new BitReader(s))
            {
                using (MemoryStream encoded = new MemoryStream())
                using (BitWriter writer = new BitWriter(encoded))
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
            byte[] reverseBytes = Convert.ReverseBitsInBytes(data);

            using (MemoryStream s = new MemoryStream(reverseBytes))
            using (BitReader reader = new BitReader(s))
            {
                using (MemoryStream decoded = new MemoryStream())
                using (BitWriter writer = new BitWriter(decoded))
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