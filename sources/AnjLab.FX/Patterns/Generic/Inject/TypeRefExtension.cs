using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using AnjLab.FX.System;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    [MarkupExtensionReturnType(typeof(string))]
    public class TypeRefExtension: MarkupExtension
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private FieldInfo _fieldInfo;
        private static Regex _nameSpaceRegex = new Regex("clr-namespace:(?<namespace>[^;]+)(;assembly=(?<assembly>.+))?", RegexOptions.IgnorePatternWhitespace);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(_name))
                throw new InvalidOperationException();

            ParserContext context = GetContext(serviceProvider);
            string[] prefixName = Name.Split(':');
            string namespce = context.XmlnsDictionary.LookupNamespace(prefixName[0]);
            Match match = _nameSpaceRegex.Match(namespce);
            if (match.Success)
            {
                return Assembly.CreateQualifiedName(match.Groups["assembly"].Value, match.Groups["namespace"].Value + "." + prefixName[1]);
            }
            return string.Empty;
        }

        private ParserContext GetContext(IServiceProvider provider)
        {
            if (_fieldInfo == null)
                _fieldInfo = provider.GetType().GetField("_context", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            return _fieldInfo.GetValue(provider) as ParserContext;
        }
    }
}
