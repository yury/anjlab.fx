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
        private FeedLink _link = new FeedLink();
        private FeedAuthor _author = new FeedAuthor();

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
            set { _link = value; }
        }

        public FeedAuthor Author
        {
            get { return _author; }
            set { _author = value; }
        }
    }
}
