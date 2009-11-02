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
            Assert.AreEqual("Google Reader", feed.Generator.Name);
            Assert.AreEqual("http://www.google.com/reader", feed.Generator.Uri);
            Assert.AreEqual("tag:google.com,2005:reader/user/08735471787465251095/label/test", feed.ID);
            Assert.AreEqual("\"test\" via Yury in Google Reader", feed.Title);
            Assert.AreEqual("Yury", feed.Author.Name);
            Assert.AreEqual("self", feed.Link.Rel);
            Assert.AreEqual("http://www.google.com/reader/atom/user/08735471787465251095/label/test", feed.Link.Href);
            
            
            Assert.AreEqual(new DateTime(2008, 02, 04, 21, 14, 27).ToLocalTime(), feed.Updated);
            Assert.AreEqual(0, feed.Entries.Count);
        }

        [Test]
        public void TestRead2()
        {
            AtomFeed feed = AtomFeed.ReadXml(TestRes.AtomFeed2);
            Assert.IsNotNull(feed);
            Assert.AreEqual("Google Reader", feed.Generator.Name);
            Assert.AreEqual("http://www.google.com/reader", feed.Generator.Uri);
            Assert.AreEqual("tag:google.com,2005:reader/user/08735471787465251095/label/test", feed.ID);
            Assert.AreEqual("\"test\" via Yury in Google Reader", feed.Title);
            Assert.AreEqual("Yury", feed.Author.Name);
            Assert.AreEqual("self", feed.Link.Rel);
            Assert.AreEqual("http://www.google.com/reader/atom/user/08735471787465251095/label/test", feed.Link.Href);
            Assert.AreEqual(new DateTime(2008, 02, 04, 21, 14, 27).ToLocalTime(), feed.Updated);
            Assert.AreEqual(2, feed.Entries.Count);

            FeedEntry entryA = feed.Entries[0];

            Assert.AreEqual("seregaborzov", entryA.Author.Name);
            Assert.AreEqual("tag:google.com,2005:reader/item/f301e5f8beb266f0", entryA.ID);
            Assert.AreEqual("Полезный SEO блог…", entryA.Title);

            Assert.AreEqual(new DateTime(2008, 02, 04, 21, 13, 21).ToLocalTime(), entryA.Published);
            Assert.AreEqual(new DateTime(2008, 03, 04, 21, 13, 21).ToLocalTime(), entryA.Updated);
            
            Assert.AreEqual("alternate", entryA.Link.Rel);
            Assert.AreEqual("http://feeds.feedburner.com/~r/seregaborzov/~3/229146423/", entryA.Link.Href);
            Assert.AreEqual("text/html", entryA.Link.Type);

            Assert.AreEqual("html", entryA.Content.Type);
            Assert.IsTrue(entryA.Content.Value.IndexOf("&lt;div&gt;&lt;br&gt;&lt;p&gt;Их конечно очень много, но я вот &lt;")> 0);

            Assert.IsTrue(entryA.Categories.Count == 5, "no categories in entry");
            Assert.AreEqual("read", entryA.Categories[0]);
            Assert.AreEqual("reading-list", entryA.Categories[1]);
            Assert.AreEqual("test", entryA.Categories[2]);
            Assert.AreEqual("fresh", entryA.Categories[3]);
            Assert.AreEqual("SEO", entryA.Categories[4]);

            FeedEntry entryB = feed.Entries[1];
            Assert.AreEqual(6, entryB.Categories.Count);
            Assert.AreEqual("read", entryB.Categories[0]);
            Assert.AreEqual("reading-list", entryB.Categories[1]);
            Assert.AreEqual("test", entryB.Categories[2]);
            Assert.AreEqual("fresh", entryB.Categories[3]);
            Assert.AreEqual("Blogging", entryB.Categories[4]);
            Assert.AreEqual("Юмор", entryB.Categories[5]);
        }
    }
}
