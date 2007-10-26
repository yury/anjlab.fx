using System.Collections.Generic;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Elements")]
    public class MapInfo
    {
        private List<IMapElement> _elements = new List<IMapElement>();
        public List<IMapElement> Elements
        {
            get { return _elements; }
            set { _elements = value;}
        }

        private Dictionary<string, IMapElement> _namedMappings = new Dictionary<string, IMapElement>();
        public Dictionary<string, IMapElement> NamedMappings
        {
            get { return _namedMappings; }
            set { _namedMappings = value; }
        }
    }
}