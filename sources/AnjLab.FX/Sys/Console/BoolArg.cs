namespace AnjLab.FX.Sys.Console
{
    public class BoolArg : CommandArg<bool>
    {
        public BoolArg(string name, bool required) : base(name, required)
        {
        }

        public BoolArg(string name)
            : base(name)
        {
        }

        public override void ParseValue(string str)
        {
            Value = true;
        }

        public override bool NeedValue
        {
            get { return false; }
        }
    }
}