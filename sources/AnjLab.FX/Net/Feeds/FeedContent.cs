using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Net.Feeds
{
    public class FeedContent
    {
        private string _type;
        private string _value;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
