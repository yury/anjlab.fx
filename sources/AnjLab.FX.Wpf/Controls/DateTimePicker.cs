using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnjLab.FX.Wpf.Controls
{
    public class DateTimePicker : DatePicker
    {
        static DateTimePicker()
        {
            DatePicker.CanEditProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
            DatePicker.MaskProperty.OverrideMetadata(typeof(DateTimePicker), new PropertyMetadata("00/00/0000 00:00:00"));
        }

        protected override string GetDateFormat(System.Globalization.CultureInfo cultureInfo)
        {
            return string.Format("{0} HH:mm:ss", cultureInfo.DateTimeFormat.ShortDatePattern);
        }
    }
}
