using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace AnjLab.FX.Windows.Media
{
    public class ColorsHelper
    {
        public static ColorData[] Colors
        {
            get
            {
                var list = new List<ColorData>();
                foreach (PropertyInfo propInfo in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    object instance = null;

                    list.Add(new ColorData
                    {
                        Color = (Color)propInfo.GetValue(instance, null),
                        Name = propInfo.Name
                    });
                }
                return list.ToArray();
            }
        }
    }
}
