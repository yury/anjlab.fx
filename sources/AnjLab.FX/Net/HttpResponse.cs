using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AnjLab.FX.System;

namespace AnjLab.FX.Net
{
    public class HttpResponse
    {
        private HttpWebResponse _res;

        internal HttpResponse(HttpWebResponse response)
        {
            _res = response;   
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
            req.Cookies.Add(_res.Cookies);
            return req;
        }

        public HttpRequest NewPost(string uri, params Pair<string, string> [] vars)
        {
            HttpRequest req = HttpRequest.NewPost(uri, vars);
            req.Cookies.Add(_res.Cookies);
            return req;
        }

        public Stream GetResponseStream()
        {
            return _res.GetResponseStream();
        }

        public string GetResponseText()
        {
            using (StreamReader reader = new StreamReader(_res.GetResponseStream(), Encoding.GetEncoding(_res.CharacterSet)))
            {
                return reader.ReadToEnd();
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
