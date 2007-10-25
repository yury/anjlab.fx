using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Elements")]
    public class MapInfo : IMapInfo
    {
        private List<IMapInfoElement> _elements = new List<IMapInfoElement>();
        private Type _mapedType;

        public Type MapedType
        {
            get { return _mapedType; }
            set { _mapedType = value; }
        }

        public List<IMapInfoElement> Elements
        {
            get { return _elements; }
            set { _elements = value;}
        }
    }
}