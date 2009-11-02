using System.Web.UI;
using AnjLab.FX.Web.Scripts;

[assembly: WebResource(Namespaces.Sys, "text/javascript")]
[assembly: WebResource(Namespaces.Core, "text/javascript")]
[assembly: WebResource(Namespaces.UnitTesting, "text/javascript")]
[assembly: WebResource(Namespaces.Browser, "text/javascript")]
[assembly: WebResource(Namespaces.Finance, "text/javascript")]
[assembly: WebResource(Namespaces.Dom, "text/javascript")]

namespace AnjLab.FX.Web.Scripts
{
    public class Namespaces
    {
        public const string Sys = "AnjLab.FX.Web.Scripts.sys.js";
        public const string Browser = "AnjLab.FX.Web.Scripts.browser.js";
        public const string Core = "AnjLab.FX.Web.Scripts.core.js";
        public const string UnitTesting = "AnjLab.FX.Web.Scripts.unitTesting.js";
        public const string Finance = "AnjLab.FX.Web.Scripts.finance.js";
        public const string Dom = "AnjLab.FX.Web.Scripts.dom.js";
    }
}
