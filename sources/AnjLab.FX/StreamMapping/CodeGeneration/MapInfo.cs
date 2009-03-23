#if NET_3_5
using System.Collections.Generic;
using System.Windows.Markup;

namespace AnjLab.FX.StreamMapping
{
    [ContentProperty("Nodes")]
    public class MapInfo
    {
        private List<ICodeGeneratorNode> _nodes = new List<ICodeGeneratorNode>();
        public List<ICodeGeneratorNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value;}
        }

        private Dictionary<string, ICodeGeneratorNode> _namedMappings = new Dictionary<string, ICodeGeneratorNode>();
        public Dictionary<string, ICodeGeneratorNode> NamedMappings
        {
            get { return _namedMappings; }
            set { _namedMappings = value; }
        }

        public ICodeGeneratorNode GetNamedMapping(string key)
        {
            if (_namedMappings.ContainsKey(key))
                return _namedMappings[key];
            return null;
        }
    }
}
#endif