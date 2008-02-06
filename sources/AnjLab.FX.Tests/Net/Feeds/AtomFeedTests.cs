using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Net.Feeds;
using NUnit.Framework;

using TestRes = AnjLab.FX.Tests.Properties.Resources;

namespace AnjLab.FX.Tests.Net.Feeds
{
    [TestFixture]
    public class AtomFeedTests
    {
        [Test]
        public void TestRead1()
        {
            AtomFeed feed = AtomFeed.ReadXml(TestRes.AtomFeed1);
            Assert.IsNotNull(feed);
            Assert.AreEqual("tag:google.com,2005:reader/user/08735471787465251095/label/test", feed.ID);
            Assert.AreEqual("\"test\" via Yury in Google Reader", feed.Title);
            Assert.AreEqual("Yury", feed.Author.Name);
            Assert.AreEqual("self", feed.Link.Rel);
            Assert.AreEqual("http://www.google.com/reader/atom/user/08735471787465251095/label/test", feed.Link.Href);
            Assert.AreEqual("http://www.google.com/reader", feed.Generator.Uri);
            Assert.AreEqual("Google Reader", feed.Generator.Name);
            Assert.AreEqual(new DateTime(2008, 02, 04, 21, 14, 27).ToLocalTime().Date, feed.Updated.Date);
            Assert.AreEqual(0, feed.Entries.Count);
        }

        [Test]
        public void TestRead2()
        {
            AtomFeed feed = AtomFeed.ReadXml(TestRes.AtomFeed2);
            Assert.IsNotNull(feed);
        }
    }
}
