using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Net.Feeds
{
    public class FeedGenerator
    {
        private string _uri;
        private string _name;

        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
