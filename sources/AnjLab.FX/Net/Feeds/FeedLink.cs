using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Net.Feeds
{
    public class FeedLink
    {
        private string _rel;
        private string _href;
        private string _type;

        public string Rel
        {
            get { return _rel; }
            set { _rel = value; }
        }

        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
