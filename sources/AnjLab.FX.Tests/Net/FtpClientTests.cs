using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Net
{
    [TestFixture]
    public class FtpClientTests
    {
        [Test, Ignore]
        public void TestUpload()
        {
            //UploadFile(@"C:\data\work\pochtalion\sites\doforme\page9990.html", "ftp://doforme.ru/www/", "doforme", "2005ajax", 2048);
            //UploadFile(@"C:\data\work\pochtalion\sites\doforme\page9991.html", "ftp://doforme.ru/www/", "doforme", "2005ajax", 4096);

            NetworkCredential login = new NetworkCredential("doforme", "2005ajax");
            Stopwatch timer = new Stopwatch();
            int count = 0;
            timer.Start();
            foreach (string fileName in Directory.GetFiles(@"C:\data\work\pochtalion\sites\doforme"))
            {
                if (Path.GetFileName(fileName).StartsWith("page5"))
                {
                    bool uploaded = false;
                    int timeout = (count == 0) ? 0 : 1000;
                    do
                    {
                        try
                        {
                            UploadFile(fileName, "ftp://doforme.ru/www/", login, 4096, 0);
                            count++;
                            uploaded = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Timeout for {0} with {1}. Try once more.", Path.GetFileName(fileName), 0);
                            timeout += 1000;
                        }
                    } while (!uploaded);
                }
            }
            timer.Stop();

            Console.WriteLine("Files uploaded:{0}, time:{1}", count, timer.Elapsed.TotalMilliseconds);
        }

        private void UploadFile(string fileName, string baseUrl, NetworkCredential credentials, int bufferLength, int timeout)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(Path.Combine(baseUrl, Path.GetFileName(fileName)));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = true;
            request.UsePassive = true;
            request.Credentials = credentials;
            
            if (timeout > 0)
                request.Timeout = timeout;

            Stopwatch timer = new Stopwatch();
            Stream uploadStream;
            timer.Start();
            try
            {
                uploadStream = request.GetRequestStream();
            }
            catch(Exception e)
            {
                request.Abort();
                throw;
            }

            if (timeout > 0)
                request.Timeout = timeout * 100;
            timer.Stop();
            Console.WriteLine("Get upload stream for {0}ms", timer.Elapsed.TotalMilliseconds);
            timer.Start();
            try
            {
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                FileStream stream = File.OpenRead(fileName);
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    uploadStream.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);
                timer.Stop();
                Console.WriteLine("File {0} of size {1} was uploaded for {2}ms", Path.GetFileName(fileName), count, timer.Elapsed.TotalMilliseconds);
                uploadStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("status:{0}. message:{1}", response.StatusCode, response.ExitMessage);
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get the request stream. Error:{0}", e);
                return;
            }
        }
    }
}
