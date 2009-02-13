using AnjLab.FX.Sys.Console;

namespace AnjLab.FX.Sys.Console
{
    public class IntArg : CommandArg<int>
    {
        public IntArg(string name, bool required) : base(name, required)
        {
        }

        public IntArg(string name)
            : base(name)
        {
        }

        public override void ParseValue(string str)
        {
            Value = int.Parse(str);
        }
    }
}