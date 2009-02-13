using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnjLab.FX.Sys.Console
{
    /// <summary>
    /// Usage example:
    /// 
    /// private static StringArg _startUrl = new StringArg("url", true);
    /// private static IntArg _depth = new IntArg("depth", true);
    /// private static StringArg _file = new StringArg("tofile", true);
    /// 
    /// static void Main(string[] args)
    /// {
    ///     try
    ///     {
    ///         CommandArgs.Parse(args, _startUrl, _depth, _file);
    ///         DoWork();
    ///     }
    ///     catch (InvalidDataException ex)
    ///     {
    ///         Console.WriteLine(ex.Message);
    ///     }
    ///     finally
    ///     {
    ///         Console.ReadKey();
    ///     }
    /// }
    /// 
    /// Sample cammand line: file.exe url http://yandex.ru depth 3 tofile 1.txt someBoolArg
    /// </summary>
    public class CommandArgs
    {
        public static void Parse(string[] strArgs, params ICommandArg[] expected)
        {
            Dictionary<string, ICommandArg> args = new Dictionary<string, ICommandArg>();

            foreach (ICommandArg e in expected)
                args.Add(e.Name, e);

            for (int i = 0; i < strArgs.Length; i++)
                if (args.ContainsKey(strArgs[i]))
                {
                    ICommandArg arg = args[strArgs[i]];
                    arg.Found = true;
                    arg.ParseValue((arg.NeedValue) ? strArgs[++i] : "");
                }

            foreach (ICommandArg arg in expected)
                if (arg.Required && !arg.Found)
                    throw new InvalidDataException(string.Format("Usage:{0}", GenerateHelp(expected)));
        }

        public static string GenerateHelp(params ICommandArg[] args)
        {
            StringBuilder help = new StringBuilder();

            foreach (ICommandArg arg in args)
                if (arg.Required)
                    help.AppendFormat("{0} ", GenerateHelp(arg));

            foreach (ICommandArg arg in args)
                if (!arg.Required)
                    help.AppendFormat("{0} ", GenerateHelp(arg));
            
            return help.ToString();
        }

        public static string GenerateHelp(ICommandArg arg)
        {
            StringBuilder help = new StringBuilder(arg.Name);
            if (arg.NeedValue)
                help.Append(" x");

            if (!arg.Required)
            {
                help.Insert(0, "[");
                help.Append("]");
            }
            return help.ToString();
        }
    }
}