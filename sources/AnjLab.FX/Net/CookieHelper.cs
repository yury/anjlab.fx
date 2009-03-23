using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace AnjLab.FX.Net
{
    public static class CookieHelper
    {
        public static Cookie ParseCookie(string cookieString)
        {
            var cookie = new Cookie();
            var tokens = cookieString.Split(';');
            foreach(var token in tokens)
            {
                var keyValue = token.Split('=');
                if(keyValue.Length != 2) continue;

                switch(keyValue[0].Trim().ToLower())
                {
                    case "expires":
                        DateTime expires;
                        if (DateTime.TryParse(keyValue[1].Trim(), out expires))
                            cookie.Expires = expires;
                        break;
                    case "path":
                        cookie.Path = keyValue[1].Trim();
                        break;
                    default:
                        cookie.Name = keyValue[0].Trim();
                        cookie.Value = keyValue[1].Trim();
                        break;
                }
            }

            return cookie;
        }
    }
}
