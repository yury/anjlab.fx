using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace AnjLab.FX.Windows.Media
{
    public class BrushesHelper
    {
        public static BrushData[] Brushes
        {
            get
            {
                var list = new List<BrushData>(1) { new BrushData { Name = "None", Brush = null } };
                foreach (PropertyInfo propInfo in typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    object instance = null;

                    list.Add(new BrushData
                    {
                        Brush = (Brush)propInfo.GetValue(instance, null),
                        Name = propInfo.Name
                    });
                }
                return list.ToArray();
            }
        }
    }
}
