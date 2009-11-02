using System;
using System.Globalization;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Converters
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter
    {
        private string _format = "dd.MM.yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

            DateTime date = (DateTime)value;
            return date.ToString(_format, CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            return DateTime.ParseExact(strValue, _format, CultureInfo.InvariantCulture);
        }

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
    }
}