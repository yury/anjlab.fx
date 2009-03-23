using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO
{
    public static class StreamExtensions
    {
        public static void WriteTo(this Stream stream, Stream destStream)
        {
            stream.WriteTo(1024, destStream);
        }

        public static void WriteTo(this Stream stream, int buferLength, Stream destStream)
        {
            var buffer = new byte[buferLength];
            var bytesRead = stream.Read(buffer, 0, buferLength);
            while (bytesRead > 0)
            {
                destStream.Write(buffer, 0, bytesRead);
                bytesRead = stream.Read(buffer, 0, buferLength);
            }
            
            destStream.Flush();
        }
    }
}
