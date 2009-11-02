using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(DateTime))]
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? DateTime.Now.Date.Add(((TimeSpan)value)) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? ((DateTime)value).TimeOfDay : value; 
        }
    }
}
