using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AnjLab.FX.System;

namespace AnjLab.FX.Net
{
    public class HttpRequest
    {
        private readonly HttpWebRequest _req;

        public HttpRequest(string uri, string method, IDictionary<string, string> vars)
        {
            if (method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                if (vars != null && vars.Keys.Count > 0)
                    uri += "?" + Lst.ToString(vars, "{0}={1}", "&");
            }
            _req = (HttpWebRequest) WebRequest.Create(uri);
            _req.Method = method;
            _req.CookieContainer = new CookieContainer();
            if (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                if (vars != null && vars.Keys.Count > 0)
                {
                    using (StreamWriter writer = new StreamWriter(_req.GetRequestStream()))
                    {
                        writer.Write(Lst.ToString(vars, "{0}={1}", "&"));
                        writer.Flush();
                    }        
                }
            }
            
        }

        public static HttpRequest NewGet(string uri, params Pair<string, string>[] vars)
        {
            return new HttpRequest(uri, "GET", Pair.ToDictionary(vars));
        }

        public HttpResponse GetResponse()
        {
            return new HttpResponse((HttpWebResponse) _req.GetResponse());
        }

        public CookieContainer Cookies
        {
            get
            {
                return _req.CookieContainer;
            }
            set
            {
                _req.CookieContainer = value;
            }
        }

        public static HttpRequest NewPost(string uri, params Pair<string, string>[] vars)
        {
            return new HttpRequest(uri, "POST", Pair.ToDictionary(vars));
        }
    }
}
