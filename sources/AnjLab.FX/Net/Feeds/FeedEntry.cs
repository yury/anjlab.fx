using System;
using System.Collections.Generic;

namespace AnjLab.FX.Net.Feeds
{
    public class FeedEntry
    {
        private readonly List<string> _categories = new List<string>();
        private string _id;
        private string _title;
        private DateTime _updated;
        private DateTime _published;
        private readonly FeedLink _link = new FeedLink();
        private readonly FeedAuthor _author = new FeedAuthor();
        private readonly FeedContent _content = new FeedContent();

        public List<string> Categories
        {
            get { return _categories; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public DateTime Updated
        {
            get { return _updated; }
            set { _updated = value; }
        }

        public DateTime Published
        {
            get { return _published; }
            set { _published = value; }
        }

        public FeedLink Link
        {
            get { return _link; }
        }

        public FeedAuthor Author
        {
            get { return _author; }
        }

        public FeedContent Content
        {
            get { return _content; }
        }
    }
}
