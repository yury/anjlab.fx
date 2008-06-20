using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Globalization;

namespace AnjLab.FX.Wpf.Controls
{
    public class NumericTextBox : MaskedTextBox
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (object),
                                                                                              typeof (NumericTextBox),
                                                                                              new PropertyMetadata(
                                                                                                  Value_Changed));

        public NumericTextBox()
        {
            Mask = "0.00";
            PromptChar = '0';
            TextAlignment = System.Windows.TextAlignment.Right;
            ForceCaretPosition = false;
        }

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        
        private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (NumericTextBox) d;
            if (instance._isTextChanged) return;

            
            var value = Convert.ToDouble(e.NewValue);

            var mask = numRegex.Replace(value.ToString(CultureInfo.InvariantCulture), "0");
            instance.Mask = mask;
            instance.Text = value.ToString(instance.Mask);
        }

        private bool _isTextChanged;
        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            _isTextChanged = true;

            base.OnTextChanged(e);
            
            double result;
            Value = double.TryParse(Text, out result) ? result : result;

            _isTextChanged = false;
        }


        static readonly Regex numRegex = new Regex(@"\d", RegexOptions.Compiled);

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var position = SelectionStart;
            
            if ((Mask.Length > position && Mask[position] == '.') || (Mask.Length == position && Mask.IndexOf('.') == -1))
            {
                IncreaseMask(position);
            }
           
            base.OnPreviewTextInput(e);
        }

        private void IncreaseMask(int position)
        {
            var provider = MaskProvider;

            var str = provider.ToDisplayString().Insert(position, PromptChar.ToString()).Replace(" ", string.Empty);

            Mask = FormatMask(Mask.Insert(position, PromptChar.ToString()));
            Text = str;

            CaretIndex = GetDecimalSeparatorPosition(Mask) - 1;
        }

        private string FormatMask(string inputMask)
        {
            inputMask = inputMask.Replace(" ", string.Empty);

            for (var i = GetDecimalSeparatorPosition(inputMask) - 3; i > 0; i = i - 3)
            {
                inputMask = inputMask.Insert(i, " ");
            }

            return inputMask;
        }

        private void DecreaseMask(int position, int length)
        {
           var decPos = GetDecimalSeparatorPosition(Mask);
            var str = MaskProvider.ToDisplayString();

            if(position > decPos)
            {
                Text = str.Remove(position, length).Insert(position, 0.ToString("D" + length));
                CaretIndex = position;
                return;
            }

            if (position + length > decPos)
            {
                var decLength = position + length - decPos - 1;
                var decPosition = decPos + 1;
                str = str.Remove(decPosition, decLength).Insert(decPosition, 0.ToString("D" + decLength));
                
                length = decPos - position;
            }

            if (length > 0)
            {
                str = str.Remove(position, length).Replace(" ", string.Empty);
                if (str.Length == 0) str = PromptChar.ToString();

                var mask = FormatMask(Mask.Remove(position, length));
                if (mask.Length == 0 || mask.StartsWith("."))
                {
                    if (mask.StartsWith("."))
                        str = PromptChar + str;

                    mask = PromptChar + mask;
                }

                Mask = mask;

                Text = str;
            }

            CaretIndex = position;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete && SelectionStart < Text.Length)//handle the delete key
            {
                DecreaseMask(SelectionStart, SelectionLength);

                e.Handled = true;
            }

            else if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            else if (e.Key == Key.Back && SelectionStart > 0)//handle the back space
            {
                DecreaseMask(SelectionStart-1, 1);

                e.Handled = true;
            }
        }

        private int GetDecimalSeparatorPosition(string input)
        {
            var dsPos = input.IndexOf('.');
            if (dsPos == -1) dsPos = input.Length;
            return dsPos;
        }
    }
}
