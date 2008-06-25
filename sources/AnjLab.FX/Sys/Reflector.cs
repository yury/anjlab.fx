using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
namespace AnjLab.FX.Sys
{
    public class Reflector
    {
        public static object GetValue(object obj, string valuePath)
        {
            return GetValue(obj, valuePath, BindingFlags.Public | BindingFlags.Instance);
        }

        public static object GetValue(object obj, string valuePath, BindingFlags flags)
        {
            try
            {
                string[] path = valuePath.Split('.');

                foreach (string pathPart in path)
                {
                    if(obj == null) return null;
                    obj = obj.GetType().GetProperty(pathPart, flags).GetValue(obj, null);
                }

                return obj;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static IEnumerable<KeyValuePair<string, object>> GetMemberEnumerable(object obj)
        {
            return GetMemberEnumerable(obj, BindingFlags.Public | BindingFlags.Instance);
        }

        public static IEnumerable<KeyValuePair<string, object>> GetMemberEnumerable(object obj, BindingFlags flags)
        {
            foreach(var field in obj.GetType().GetFields(flags))
            {
                if(IsBrowsable(field))
                    yield return new KeyValuePair<string, object>(GetMemberDescription(field), field.GetValue(obj));
            }

            foreach(var property in obj.GetType().GetProperties(flags))
            {
                if(IsBrowsable(property))
                    yield return new KeyValuePair<string, object>(GetMemberDescription(property), property.GetValue(obj, null));
            }
        }

        private static bool IsBrowsable(MemberInfo info)
        {
            var attrs = info.GetCustomAttributes(typeof(BrowsableAttribute), false);

            if (attrs == null || attrs.Length == 0) return true;

            return ((BrowsableAttribute)attrs[0]).Browsable; 
        }

        private static string GetMemberDescription(MemberInfo info)
        {
            var attrs = info.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attrs == null || attrs.Length == 0) return info.Name;

            return ((DescriptionAttribute) attrs[0]).Description;
        }
    }
}
