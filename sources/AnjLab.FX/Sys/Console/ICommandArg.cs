namespace AnjLab.FX.Sys.Console
{
    public interface ICommandArg
    {
        string Name { get; }
        void ParseValue(string str);
        bool NeedValue { get; }
        bool Found { get; set; }
        bool Required { get; set; }
    }
}