using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    /// <summary>
    /// example: ConverterParameter='\{0:d\}'
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class FormattingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var formatString = parameter as string;
            return formatString != null ? string.Format(culture, formatString, value) : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }
}
