using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AnjLab.FX.Html
{
    public class HtmlProcessor
    {
        private static readonly Regex stripHtmlRegex =
            new Regex(@"<!--.*?-->|<script.*?>.*?</script>|<style.*?>.*?</style>|<[^>]*>|& nbsp;|&nbsp;|&lt;|&rt;", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex doubleSpacesRegex =
            new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);
        
        public static string StripHtml(string html)
        {
            if (String.IsNullOrEmpty(html))
                return String.Empty;

            string withoutTags = stripHtmlRegex.Replace(html, " ");
            return doubleSpacesRegex.Replace(withoutTags, " ");
        }

        public static string GetOutsideTagContent(string html, string tag)
        {
            String regExp = String.Format(@"\s+|<{0}[^>]*>(?'content'[^<]+)</{0}>", tag);

            Regex tagContentExpression = new Regex(regExp, RegexOptions.IgnoreCase);
            string withoutTag = tagContentExpression.Replace(html, String.Empty);
            return StripHtml(withoutTag);
        }

        public static string GetTagContent(string html, string tag)
        {
            String regExp = String.Format(@"<{0}[^>]*>(?'content'[^<]+)</{0}>", tag);
            Regex tagContentExpression = new Regex(regExp, RegexOptions.IgnoreCase);

            StringBuilder result = new StringBuilder();
            foreach (Match m in tagContentExpression.Matches(html))
                result.Append(m.Groups["content"].Value);

            return result.ToString();
        }

        public static IList<string> GetTagAttributeValues(string html, string tag, string attr)
        {
            String regExp = String.Format(@"<{0}.*? {1}\s*=\s*[""'](?'attr'.*?)[""']", tag, attr);
            Regex tagAttrValueExpression = new Regex(regExp, RegexOptions.IgnoreCase);

            List<string> result = new List<string>();
            foreach (Match m in tagAttrValueExpression.Matches(html))
                result.Add(m.Groups["attr"].Value);

            return result;
        }
    }
}