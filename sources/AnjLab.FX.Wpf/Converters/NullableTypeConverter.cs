using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    public class NullableTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(int?)) return System.Convert.ToInt32(value);
            if (targetType == typeof(short?)) return System.Convert.ToInt16(value);
            if (targetType == typeof(long?)) return System.Convert.ToInt64(value);
            if (targetType == typeof(decimal?)) return System.Convert.ToDecimal(value);
           
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(int?)) return System.Convert.ToInt32(value);
            if (targetType == typeof(short?)) return System.Convert.ToInt16(value);
            if (targetType == typeof(long?)) return System.Convert.ToInt64(value);
            if (targetType == typeof(decimal?)) return System.Convert.ToDecimal(value);

            return value;
        }
    }
}
