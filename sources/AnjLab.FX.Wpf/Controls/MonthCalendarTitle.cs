using System.Windows;
using System.Windows.Controls;

namespace AnjLab.FX.Wpf.Controls
{
    public class MonthCalendarTitle : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarTitle), new FrameworkPropertyMetadata(typeof(MonthCalendarTitle)));
        }
    }
}