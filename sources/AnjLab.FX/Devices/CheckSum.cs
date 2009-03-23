namespace AnjLab.FX.Devices
{
    public class CheckSum
    {
        /// <summary>
        /// CRC-16-IBM : G(x) = x16 + x15 + x2 + 1 (USB, many others; also known as "CRC-16"). Reverse -  0xA001	
        /// CRC-16 polinom = 0xFFFF
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static short Crc16(byte[] bytes)
        {
            return Crc16(bytes, 0, bytes.Length);
        }

        public static short Crc16(byte[] bytes, int from, int to)
        {
            int sum = 0xFFFF;

            for (int i = from; i < to; i++)
            {
                sum = sum ^ bytes[i];

                for (int j = 0; j <= 7; j++)
                {
                    int lastBit = sum & 0x0001;
                    sum = sum >> 1;
                    if (lastBit != 0)
                        sum = sum ^ 0xA001;
                }
            }
            return (short)sum;
        }

        public static byte NmeaChecksum(string sentence)
        {
            byte actualCheckSum = 0;
            int dollar = sentence.IndexOf('$');
            int asterix = sentence.IndexOf('*');
            if (dollar != -1 && asterix != -1)
                sentence = sentence.Substring(dollar + 1, asterix - dollar - 1);

            foreach (char symbol in sentence)
                actualCheckSum = (byte)(actualCheckSum ^ (byte)symbol);

            return actualCheckSum;
        }
    }
}
