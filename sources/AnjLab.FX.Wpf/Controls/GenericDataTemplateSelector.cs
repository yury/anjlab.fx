using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Wpf.Controls
{
    public class GenericDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplateSelectorItem[] DataTemplateSelectorItems { get; set; }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item != null && DataTemplateSelectorItems != null)
            {
                foreach(var selectorItem in DataTemplateSelectorItems)
                {
                    var value = Reflector.GetValue(item, selectorItem.PropertyName);
                    if (selectorItem.Value.Equals(Convert.ToString(value)))
                        return selectorItem.Template;
                }
            }

            return null;

        }
    }

    public class DataTemplateSelectorItem
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public DataTemplate Template { get; set; } 
    }
}
