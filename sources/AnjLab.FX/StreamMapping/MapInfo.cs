using System.Collections.Generic;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Elements")]
    public class MapInfo
    {
        private List<IMapInfoElement> _elements = new List<IMapInfoElement>();
        public List<IMapInfoElement> Elements
        {
            get { return _elements; }
            set { _elements = value;}
        }
    }
}