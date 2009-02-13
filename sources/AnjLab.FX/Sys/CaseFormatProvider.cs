using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace AnjLab.FX.Sys
{
    public class CaseFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return this;
        }

        private static readonly Regex _caseFormatStringRegex = new Regex("(?<case>[^=;]+)=(?<value>[^=;]+)", RegexOptions.Compiled);

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return string.Format("{0}", arg);

            string argStr = Convert.ToString(arg);
            bool isCaseFormatting = false;
            foreach (Match match in _caseFormatStringRegex.Matches(format))
            {
                isCaseFormatting = true;

                if (match.Groups["case"].Value == argStr)
                    return match.Groups["value"].Value;
            }

            if (!isCaseFormatting && arg is IFormattable)
            {
                return ((IFormattable) arg).ToString(format, null);
            }

            return argStr;
        }
    }
}
