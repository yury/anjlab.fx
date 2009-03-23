using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Net
{
    public class HttpRequest
    {
        private readonly HttpWebRequest _req;

        public IWebProxy Proxy
        {
            get { return _req.Proxy; }
            set { _req.Proxy = value; }
        }

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
            
            if (vars != null && vars.Keys.Count > 0 && Str.EqualsInvariantIgnoreCase(method, "POST"))
            {
                using (StreamWriter writer = new StreamWriter(_req.GetRequestStream()))
                {
                    writer.Write(Lst.ToString(vars, "{0}={1}", "&"));
                    writer.Flush();
                }
            }
        }

        public static HttpRequest NewGet(string uri, params Pair<string, string>[] vars)
        {
            return new HttpRequest(uri, "GET", Pair.ToDictionary(vars));
        }

        public HttpResponse GetResponse()
        {
            return new HttpResponse((HttpWebResponse) _req.GetResponse(), Proxy, UserAgent);
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

        public string UserAgent
        {
            get
            {
                return _req.UserAgent;
            }
            set
            {
                _req.UserAgent = value;
            }
        }
    }
}
