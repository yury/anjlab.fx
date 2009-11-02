using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(FontWeight))]
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true.Equals(value) ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
