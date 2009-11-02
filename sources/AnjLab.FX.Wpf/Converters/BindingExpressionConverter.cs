using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnjLab.FX.Wpf.Converters
{
    public class BindingExpressionConverter : ExpressionConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
                return true;
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {
                var bindingExpression = value as BindingExpression;
                if (bindingExpression == null)
                    throw new Exception();
                return bindingExpression.ParentBinding;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public static void RegisterExpression<T>()
        {
            var attribute = new Attribute[1];
            var vConv = new TypeConverterAttribute(typeof(BindingExpressionConverter));
            attribute[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attribute);
        }
    }
}
