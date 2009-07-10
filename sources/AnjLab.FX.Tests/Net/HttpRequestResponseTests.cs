using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Net;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Net
{
    [TestFixture]
    public class HttpRequestResponseTests
    {
        [Test]
        public void Test()
        {
            HttpRequest get = HttpRequest.NewGet("http://ya.ru");
            Assert.IsNotNull(get);
            HttpResponse res = get.GetResponse();
            Assert.IsNotNull(res);

            Assert.IsTrue(res.IsOk());
            Console.WriteLine(res.Server);

            Assert.AreEqual(new Uri("http://ya.ru"), res.ResponseUri);

            Assert.AreEqual("windows-1251", res.CharacterSet);
            Console.WriteLine(res.Cookies.Count);

            //Console.WriteLine(res.GetResponseText());

            Console.WriteLine((int)res.StatusCode);
            Console.WriteLine(res.StatusDescription);

            get = res.NewGet("http://www.yandex.ru/yandsearch",
                                Pair.New("text", "тест"),
                                Pair.New("rpt", "rad"));
            res = get.GetResponse();
            Assert.IsTrue(res.IsOk());
            Console.WriteLine(res.Cookies.Count);
            Console.WriteLine(res.GetResponseText());

        }
    }
}
