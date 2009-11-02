using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AnjLab.FX.Web
{
    public class Validator
    {
        private static Regex emailRegEx = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled);

        public static bool IsEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            return emailRegEx.IsMatch(email);
        }
    }
}
