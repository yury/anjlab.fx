using AnjLab.FX.System.Console;

namespace AnjLab.FX.System.Console
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