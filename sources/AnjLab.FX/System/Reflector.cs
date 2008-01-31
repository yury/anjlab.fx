using System;
using System.Reflection;

namespace AnjLab.FX.System
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
                    obj = obj.GetType().GetProperty(pathPart, flags).GetValue(obj, null);

                return obj;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
