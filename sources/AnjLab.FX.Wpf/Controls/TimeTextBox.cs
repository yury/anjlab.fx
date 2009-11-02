using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnjLab.FX.Wpf.Controls
{
    public class TimeTextBox : MaskedTextBox
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan?),
                                                                                              typeof(TimeTextBox),
                                                                                              new FrameworkPropertyMetadata(ValueChanged) { BindsTwoWayByDefault = true });

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeTextBox = (TimeTextBox)d;
            timeTextBox.SetValue(e.NewValue);
        }

        private void SetValue(object newValue)
        {
            if (newValue == null)
                Clear();
            else
            {
                var time = (TimeSpan)newValue;
                Text = time.ToString().Substring(0, Mask.Length);
            }
        }

        public TimeSpan? Value
        {
            get { return (TimeSpan?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        static TimeTextBox()
        {
            MaskedTextBox.MaskProperty.OverrideMetadata(typeof(TimeTextBox), new UIPropertyMetadata("00:00"));
            MaskedTextBox.PromptCharProperty.OverrideMetadata(typeof(TimeTextBox), new UIPropertyMetadata('0'));
        }

        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            TimeSpan result;
            if (TimeSpan.TryParse(Text, out result))
                Value = result;
            else
            {
                e.Handled = true;
                SetValue(Value);
            }
        }
    }
}
