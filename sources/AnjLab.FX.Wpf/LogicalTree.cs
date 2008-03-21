using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnjLab.FX.Wpf
{
    public static class LogicalTree
    {
        public static TParent GetParent<TParent>(DependencyObject childObject) where TParent : DependencyObject
        {
            DependencyObject parent = childObject;
            while (parent != null)
            {
                if (parent is TParent)
                    break;
                else
                    parent = LogicalTreeHelper.GetParent(parent);
            }

            if (parent is TParent)
                return (TParent)parent;
            else
                return null;
        }
    }
}
