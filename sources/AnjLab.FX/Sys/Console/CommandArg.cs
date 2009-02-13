namespace AnjLab.FX.Sys.Console
{
    public abstract class CommandArg<TType> : ICommandArg
    {
        private readonly string _name;
        private TType _value = default(TType);
        private bool _found;
        private bool _required;

        public CommandArg(string name)
        {
            _name = name;
        }

        protected CommandArg(string name, bool required)
        {
            _required = required;
            _name = name;
        }

        public abstract void ParseValue(string str);

        public virtual bool NeedValue
        {
            get { return true; }
        }

        public bool Found
        {
            get { return _found; }
            set { _found = value; }
        }

        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        public TType Value
        {
            get { return _value; }
            protected set { _value = value; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}