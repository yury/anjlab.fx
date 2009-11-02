using System;
using System.Globalization;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool) value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }
    }
}
