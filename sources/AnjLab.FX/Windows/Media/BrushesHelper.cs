using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace AnjLab.FX.Windows.Media
{
    public class BrushesHelper
    {
        public static Brush[] GetBrushes()
        {
            var list = new List<Brush>();
            foreach (PropertyInfo propInfo in typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                object instance = null;

                list.Add((Brush)propInfo.GetValue(instance, null));
            }
            return list.ToArray();
        }
    }
}
