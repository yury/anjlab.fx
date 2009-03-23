using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

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


        public static TParent GetParent<TParent>(DependencyObject childObject) where TParent : DependencyObject
        {
            DependencyObject parent = childObject;
            while (parent != null)
            {
                if (parent is TParent)
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }

            if(parent is TParent)
                return (TParent)parent;
            else
                return null;
        }

        public static TChild GetChild<TChild>(DependencyObject parentObject) where TChild : DependencyObject
        {
            TChild result = default(TChild);
            for(int i=0; i<VisualTreeHelper.GetChildrenCount(parentObject); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parentObject, i);
                if(child is TChild)
                    return (TChild)child;
            }

            return result;
        }

        public static IList<TChild> FindChildren<TChild>(DependencyObject parentObject) where TChild : DependencyObject
        {
            List<TChild> result = new List<TChild>();
            FindChildren(parentObject, result);
            return result;
        }

        private static void FindChildren<TChild>(DependencyObject parentObject, IList<TChild> result) where TChild : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObject); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parentObject, i);
                if(child is TChild)
                    result.Add((TChild)child);

                FindChildren(child, result);
            }
        }

        public static IEnumerable<DependencyObject> GetChildren(DependencyObject parentObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObject); i++)
            {
                yield return VisualTreeHelper.GetChild(parentObject, i);
            }
        }
    }
}
