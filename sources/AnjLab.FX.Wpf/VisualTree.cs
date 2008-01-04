using System;
using System.Windows;
using System.Windows.Media;

namespace AnjLab.FX.Wpf
{
    public class VisualTree
    {
        public static DependencyObject GetDependencyObject(DependencyObject startObject, Type type)
        {
            //Walk the visual tree to get the parent(ItemsControl) 
            //of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }
    }
}
