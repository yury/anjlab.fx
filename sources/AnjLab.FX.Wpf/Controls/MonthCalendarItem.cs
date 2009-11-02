using System.Windows;
using System.Windows.Controls;

namespace AnjLab.FX.Wpf.Controls
{
    public class MonthCalendarItem : ListBoxItem
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarItem), new FrameworkPropertyMetadata(typeof(MonthCalendarItem)));
        }

        /// <summary>
        /// This is the method that responds to the MouseButtonEvent event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            //In ListBox, right click will select the item, override this method to remove this feature
        }
    }
}