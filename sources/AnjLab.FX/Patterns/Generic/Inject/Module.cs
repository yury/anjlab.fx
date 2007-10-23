using System;
using System.Collections.Generic;
using System.Linq;
namespace AnjLab.FX.Patterns.Generic.Inject
{
    public class Module
    {
        private List<string> _imports = new List<string>();
        private Dictionary<string,Definition> _definitions = new Dictionary<string, Definition>();
        private List<Scope> _bindings = new List<Scope>();
        private List<string> _machines = new List<string>();

        public List<string> Imports
        {
            get { return _imports; }
            set { _imports = value; }
        }

        public Dictionary<string,Definition> Definitions
        {
            get { return _definitions; }
            set { _definitions = value; }
        }

        internal ICollection<Definition> GetDifinitions()
        {
            foreach (KeyValuePair<string, Definition> pair in _definitions)
            {
                pair.Value.Key = pair.Key;
            }
            return _definitions.Values;
        }

        public List<Scope> Bindings
        {
            get { return _bindings; }
            set { _bindings = value; }
        }

        public List<string> Machines
        {
            get { return _machines; }
            set { _machines = value; }
        }
    }
}
