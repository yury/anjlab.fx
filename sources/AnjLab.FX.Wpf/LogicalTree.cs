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

        public static IList<TChild> FindChildren<TChild>(DependencyObject parentObject) where TChild : DependencyObject
        {
            List<TChild> result = new List<TChild>();
            FindChildren(parentObject, result);
            return result;
        }

        private static void FindChildren<TChild>(DependencyObject parentObject, IList<TChild> result) where TChild : DependencyObject
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parentObject))
            {
                var obj = child as DependencyObject;
                if(obj == null) continue;

                if (obj is TChild)
                    result.Add((TChild)obj);

                FindChildren(obj, result);
            }
        }
    }
}
