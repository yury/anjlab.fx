using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Net.Feeds
{
    public class AtomFeed
    {
        readonly FeedGenerator _generator = new FeedGenerator();
        private string _id;
        private string _title;
        private readonly FeedAuthor _author = new FeedAuthor();
        private DateTime _updated;
        private readonly FeedLink _link = new FeedLink();
        private readonly List<FeedEntry> _entries = new List<FeedEntry>();

        public FeedGenerator Generator
        {
            get { return _generator; }
        }

        public string ID
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public FeedAuthor Author
        {
            get { return _author; }
        }

        public DateTime Updated
        {
            get { return _updated; }
        }

        public FeedLink Link
        {
            get { return _link; }
        }

        public List<FeedEntry> Entries
        {
            get { return _entries; }
        }

        public static AtomFeed ReadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            AtomFeed result = new AtomFeed();
            XmlElement feed = doc.DocumentElement;
            result._id = feed["id"].InnerText;
            result._title = feed["title"].InnerText;

            ReadAuthor(feed["author"], result._author);
            ReadLink(feed["link"], result._link);
            ReadGenerator(feed["generator"], result._generator);
            ReadDate(feed["updated"], out result._updated);
            ReadEntries(feed.GetElementsByTagName("entry"), result._entries);
            return result;
        }

        private static void ReadEntries(XmlNodeList nodes, ICollection<FeedEntry> entries)
        {
            Guard.ArgumentNotNull("entries", entries);

            if (nodes == null)
                return;
            foreach (XmlNode node in nodes)
            {
                FeedEntry entry = new FeedEntry();
                entry.ID = node["id"].InnerText;
                entry.Title = node["title"].InnerText;

                ReadAuthor(node["author"], entry.Author);
                DateTime updated, published;
                ReadDate(node["updated"], out updated);
                ReadDate(node["published"], out published);
                entry.Published = published;
                entry.Updated = updated;
                ReadLink(node["link"], entry.Link);
                ReadContent(node["content"], entry.Content);
                ReadCategories(((XmlElement)node).GetElementsByTagName("category"), entry.Categories);


                entries.Add(entry);
            }
        }

        private static void ReadCategories(XmlNodeList nodes, ICollection<string> categories)
        {
            foreach (XmlNode node in nodes)
            {
                XmlAttribute label = node.Attributes["label"];
                if (label != null)
                {
                    categories.Add(label.Value);
                    continue;
                }
                XmlAttribute term = node.Attributes["term"];
                if (term != null)
                {
                    categories.Add(term.Value);
                }
            }
        }

        private static void ReadContent(XmlNode node, FeedContent content)
        {
            Guard.ArgumentNotNull("content", content);

            if (node == null)
                return;

            XmlAttribute type = node.Attributes["type"];
            if (type != null)
                content.Type = type.Value;
            content.Value = node.InnerXml;
        }

        private static void ReadDate(XmlNode node, out DateTime updated)
        {
            updated = DateTime.MinValue;

            if (node == null)
                return;

            DateTime.TryParse(node.InnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out updated);
        }

        private static void ReadAuthor(XmlNode node, FeedAuthor author)
        {
            Guard.ArgumentNotNull("author", author);

            if (node == null)
                return;
            XmlElement name = node["name"];
            if (name == null)
                return;
            author.Name = name.InnerText;
        }

        private static void ReadLink(XmlNode node, FeedLink link)
        {
            Guard.ArgumentNotNull("link", link);

            if (node == null)
                return;
            XmlAttribute rel = node.Attributes["rel"];
            if (rel != null)
                link.Rel = rel.Value;
            XmlAttribute href = node.Attributes["href"];
            if (href != null)
                link.Href = href.Value;
            XmlAttribute type = node.Attributes["type"];
            if (type != null)
                link.Type = type.Value;
        }

        private static void ReadGenerator(XmlNode node, FeedGenerator generator)
        {
            Guard.ArgumentNotNull("generator", generator);

            if (node == null)
                return;
            XmlAttribute uri = node.Attributes["uri"];
            if (uri != null)
                generator.Uri = uri.Value;
            generator.Name = node.InnerText;
        }
    }
}
