using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO.Compression
{
    public sealed class GZip
    {
        public static Stream Compress(Stream source)
        {
            var ms = new MemoryStream();
            Compress(source, ms);
            ms.Position = 0;
            return ms;
        }

        public static Stream Decompress(Stream source)
        {
            var ms = new MemoryStream();
            Decompress(source, ms);
            ms.Position = 0;
            return ms;
        }

        public static void Compress(Stream source, Stream destination)
        {
            using(var output = new GZipStream(destination, CompressionMode.Compress, true))
            {
                Pump(source, output);
            }
        }

        private static void Decompress(Stream source, Stream destination)
        {
            using (var input = new GZipStream(source, CompressionMode.Decompress, true))
            {
                Pump(input, destination);
            }
        }

        private static void Pump(Stream input, Stream output)
        {
            var bytes = new byte[4096];
            int n;
            while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, n);
            }
        }
    }
}
