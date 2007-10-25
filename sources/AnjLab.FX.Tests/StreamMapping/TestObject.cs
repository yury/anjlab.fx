using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Tests.StreamMapping
{
    public class TestObject
    {
        private short _shortProperty = 0;

        public short ShortProperty
        {
            get { return _shortProperty; }
            set { _shortProperty = value; }
        }
    }
}