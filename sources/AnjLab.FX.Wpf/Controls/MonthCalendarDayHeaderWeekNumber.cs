using System.Windows;
using System.Windows.Controls;

namespace AnjLab.FX.Wpf.Controls
{
    public class MonthCalendarDayHeader : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarDayHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarDayHeader), new FrameworkPropertyMetadata(typeof(MonthCalendarDayHeader)));
        }
    }

    public class MonthCalendarWeekNumber : Control
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendarWeekNumber()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarWeekNumber), new FrameworkPropertyMetadata(typeof(MonthCalendarWeekNumber)));
        }
    }
}
