using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Net;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Net
{
    [TestFixture]
    public class CookieHelperTests
    {
        [Test]
        public void ParseCookieTest()
        {
            var raw = "user_results_id=95462764506733551158; path=/; expires=Fri, 13-May-2011 10:24:36 GMT,MicexTrackID=62.182.64.55.85171218882275711; path=/; expires=Sat, 13-Sep-08 10:24:35 GMT";

            var cookie = CookieHelper.ParseCookie(raw);
            Assert.AreEqual("user_results_id", cookie.Name);
            Assert.AreEqual("95462764506733551158", cookie.Value);
            Assert.AreEqual(new DateTime(2008, 09, 13, 14, 24, 35), cookie.Expires);
            Assert.AreEqual("/", cookie.Path);
        }
    }
}
