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

        public static IEnumerable<Pair<string, object>> GetMemberEnumerable(object obj)
        {
            return GetMemberEnumerable(obj, BindingFlags.Public | BindingFlags.Instance);
        }

        public static IEnumerable<Pair<string, object>> GetMemberEnumerable(object obj, BindingFlags flags)
        {
            foreach(var field in obj.GetType().GetFields(flags))
            {
                if(IsBrowsable(field))
                    yield return new Pair<string, object>(GetMemberDescription(field), field.GetValue(obj));
            }

            foreach(var property in obj.GetType().GetProperties(flags))
            {
                if(IsBrowsable(property))
                    yield return new Pair<string, object>(GetMemberDescription(property), property.GetValue(obj, null));
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

        private static readonly Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        private static List<Type> FindTypeList(Assembly[] typeAssemblies, string typeName)
        {
            var list = new List<Type>();
            foreach (Assembly a in typeAssemblies)
            {
                var typeList = new List<Type>(a.GetTypes());
                list.AddRange(typeList.FindAll(type => type.Name.Equals(typeName)));
            }
            return list;
        }

        public static object ReadAttachedProperty(string propertyName, object propertyContainer)
        {
            return ReadAttachedProperty(propertyName, propertyContainer, loadedAssemblies);
        }

        public static object ReadAttachedProperty(string propertyName, object propertyContainer, Assembly[] typeAssemblies)
        {
            string[] propertyPath = propertyName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            foreach (Type propertyOwnerType in FindTypeList(typeAssemblies, propertyPath[0]))
            {
                var propertyMethod = propertyOwnerType.GetMethod("Get" + propertyPath[1]);
                if (propertyMethod != null)
                {
                    var result = propertyMethod.Invoke(null, new[] { propertyContainer });
                    return result;
                }
            }
            return null;
        }

        public static void WriteAttachedProperty(string propertyName, object propertyValue, object propertyContainer)
        {
            WriteAttachedProperty(propertyName, propertyValue, propertyContainer, loadedAssemblies);
        }

        public static void WriteAttachedProperty(string propertyName, object propertyValue, object propertyContainer, Assembly[] typeAssemblies)
        {
            string[] propertyPath = propertyName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            foreach (Type propertyOwnerType in FindTypeList(typeAssemblies, propertyPath[0]))
            {
                var propertyMethod = propertyOwnerType.GetMethod("Set" + propertyPath[1]);
                if (propertyMethod != null)
                {
                    propertyMethod.Invoke(null, new [] {propertyContainer, propertyValue});
                    return;
                }
            }
        }
    }
}
