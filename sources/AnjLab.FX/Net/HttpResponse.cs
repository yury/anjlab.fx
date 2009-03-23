using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Net
{
    public class HttpResponse
    {
        private readonly HttpWebResponse _res;
        private readonly IWebProxy _proxy;
        private readonly string _userAgent = string.Empty;

        internal HttpResponse(HttpWebResponse response, IWebProxy proxy, string userAgent)
        {
            Guard.ArgumentNotNull("response", response);
            Guard.ArgumentNotNull("proxy", proxy);

            _res = response;
            _proxy = proxy;
            _userAgent = userAgent;
        }

        public string Server
        {
            get
            {
                return _res.Server;
            }
        }

        public bool IsOk()
        {
            return _res.StatusCode == HttpStatusCode.OK;
        }

        public Uri ResponseUri
        {
            get
            {
                return _res.ResponseUri;
            }
        }

        public string CharacterSet
        {
            get
            {
                return _res.CharacterSet;
            }
        }

        public CookieCollection Cookies
        {
            get
            {
                return _res.Cookies;
            }
        }

        public HttpRequest NewGet(string uri, params Pair<string, string> [] vars)
        {
            HttpRequest req = HttpRequest.NewGet(uri, vars);
            InitRequest(req);
            return req;
        }

        public HttpRequest NewPost(string uri, params Pair<string, string> [] vars)
        {
            HttpRequest req = HttpRequest.NewPost(uri, vars);
            InitRequest(req);
            return req;
        }

        private void InitRequest(HttpRequest req)
        {
            req.Cookies.Add(_res.Cookies);
            req.Proxy = _proxy;
            if (_userAgent != null)
                req.UserAgent = _userAgent;
        }

        public Stream GetResponseStream()
        {
            return _res.GetResponseStream();
        }

        private string _responseText;

        public string GetResponseText()
        {
            if (_responseText != null)
                return _responseText;

            using (StreamReader reader = new StreamReader(_res.GetResponseStream(), Encoding.GetEncoding(_res.CharacterSet)))
            {
                return _responseText = reader.ReadToEnd();
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return _res.StatusCode;
            }
        }

        public string StatusDescription
        {
            get
            {
                return _res.StatusDescription;
            }
        }
    }
}
