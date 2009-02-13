using AnjLab.FX.Sys.Console;

namespace AnjLab.FX.Sys.Console
{
    public class StringArg : CommandArg<string>
    {
        public StringArg(string name, bool required) : base(name, required)
        {
        }

        public StringArg(string name)
            : base(name)
        {
        }

        public override void ParseValue(string str)
        {
            Value = str;
        }
    }
}