using System.Windows;
using System.Windows.Controls;

namespace AnjLab.FX.Wpf
{
    public class ValidationHelper
    {
        public static bool GetHasErrors(FrameworkElement root)
        {
            return GetHasErrors(root, false);
        }

        public static bool GetHasErrors(FrameworkElement root, bool checkUnvisible)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root))
            {
                var element = child as FrameworkElement;
                if (element == null)
                    continue;

                if (element.IsVisible && Validation.GetHasError(element))
                    return true;

                if (element.IsVisible && GetHasErrors(element, checkUnvisible))
                    return true;
            }
            return false;
        }
    }
}
