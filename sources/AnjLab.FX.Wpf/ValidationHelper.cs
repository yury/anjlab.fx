using System.Windows;

namespace AnjLab.FX.Wpf
{
    public class ValidationHelper
    {
        public static bool GetHasErrors(FrameworkElement root)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(root))
            {
                FrameworkElement element = child as FrameworkElement;
                if (element == null)
                    continue;

                if (System.Windows.Controls.Validation.GetHasError(element))
                    return true;

                if (GetHasErrors(element))
                    return true;
            }
            return false;
        }
    }
}
