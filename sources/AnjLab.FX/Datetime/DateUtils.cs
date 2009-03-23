using System;

namespace AnjLab.FX.Datetime
{
    public class DateUtils
    {
        public static int DaysInYear(DateTime date)
        {
            return DaysInYear(date.Year);
        }

        public static int DaysInYear(int year)
        {
            return DateTime.IsLeapYear(year) ? 366 : 365;
        }

        public static DateTime YearFirstDay(DateTime year)
        {
            return new DateTime(year.Year, 1, 1);
        }

        public static DateTime YearLastDay(DateTime year)
        {
            return new DateTime(year.Year, 12, 31);
        }

        public static DateTime BeginOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime EndOfDay(DateTime date)
        {
            return BeginOfDay(date).AddDays(1).AddMilliseconds(-1);
        }
    }
}
