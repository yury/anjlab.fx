using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace AnjLab.FX.Windows.Media
{
    public class ColorsHelper
    {
        public static Color[] GetColors()
        {
            var list = new List<Color>();
            foreach (PropertyInfo propInfo in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                object instance = null;

                if (propInfo.PropertyType.Equals(typeof(Color)))
                    list.Add((Color)propInfo.GetValue(instance, null));
            }
            return list.ToArray();
        }
    }
}
