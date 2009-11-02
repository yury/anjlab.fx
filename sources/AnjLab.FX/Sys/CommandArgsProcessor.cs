using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AnjLab.FX.Patterns.Generic;

namespace AnjLab.FX.Sys
{
    public class CommandArgsProcessor
    {
        private readonly string[] _args;
        readonly KeyedFactory<string, ICommand> _commands = new KeyedFactory<string, ICommand>();
        private readonly string NoKey = "_____no_key____";

        public CommandArgsProcessor(string[] args)
        {
            _args = args;
        }

        public bool HasKey(string key)
        {
            foreach (string prefix in new string[] { "/", "-" })
            {
                string k = prefix + key;
                foreach (string s in _args)
                {
                    if (string.Equals(k, s, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        public void MapKey(string key, ICommand cmd)
        {
            Guard.ArgumentNotNullNorEmpty("key", key);
            Guard.ArgumentNotNull("cmd", cmd);

            _commands.RegisterImmutable(key, cmd);
        }

        public void MapKey(string key, VoidAction action)
        {
            Guard.ArgumentNotNullNorEmpty("key", key);
            Guard.ArgumentNotNull("action", action);

            MapKey(key, Command.FromAction(action));
        }

        public void MapNoKey(VoidAction action)
        {
            Guard.ArgumentNotNull("action", action);

            MapKey(NoKey, action);
        }

        public void MapNoKey(ICommand cmd)
        {
            Guard.ArgumentNotNull("cmd", cmd);

            MapKey(NoKey, cmd);
        }

        public void Run()
        {
            foreach (string key in _commands.Keys)
            {
                if (!string.Equals(key, NoKey, StringComparison.InvariantCultureIgnoreCase) && HasKey(key))
                {
                    _commands.Create(key).Execute();
                    return;
                }
            }
            if (_commands.IsRegistered(NoKey))
            {
                _commands.Create(NoKey).Execute();
            }
        }

        private IDictionary<string, string> _params;

        public bool	HasParam(string key)
        {
            return GetParams().ContainsKey(key.ToLower());
        }

        private IDictionary<string, string> GetParams()
        {
            if (_params == null)
            {
                _params = ParseParams(_args);
            }
            return _params;
        }

        private static IDictionary<string, string> ParseParams(IEnumerable<string> args)
        {
            var result = new Dictionary<string, string>();

            foreach(var arg in args)
            {
                var paramRegex = new Regex("/[pP]:(?<key>.*)=(?<value>.*)");

                var match = paramRegex.Match(arg);

                if (match.Success)
                {
                    result.Add(match.Groups["key"].Value.ToLower(), match.Groups["value"].Value);
                }
            }

            return result;
        }

        public string GetParamValue(string key)
        {
            return GetParams()[key.ToLower()];
        }
    }
}
