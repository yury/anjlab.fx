using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Windows.Data
{
    [ValueConversion(typeof(Enum), typeof(EnumHelperPair))]
    public class EnumHelperValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum e = value as Enum;
            EnumHelper helper = parameter as EnumHelper;

            if(e == null || helper == null)
                return null;

            return helper.GetPair(e);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnumHelperPair pair = value as EnumHelperPair;
            if (pair == null)
                return null;
            else
                return pair.Enum;
        }
    }
}
