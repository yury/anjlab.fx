using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Data
{
    public class RepositoryFilter
    {
        public static DateTime TrimDateTime(DateTime dateTime, RepositoryFilterType filterType)
        {
            switch(filterType)
            {
                case RepositoryFilterType.ByWeek:
                    return dateTime.Date.AddDays(-(int) dateTime.DayOfWeek);
                default:
                    string format = GetDateFormatByRepositoryFilter(filterType);
                    return DateTime.ParseExact(dateTime.ToString(format), format, CultureInfo.InvariantCulture);
            }
        }

        public static string GetDateFormatByRepositoryFilter(RepositoryFilterType filterType)
        {
            switch (filterType)
            {
                case RepositoryFilterType.None:
                    return "yyyy.MM.dd H:mm:ss";
                case RepositoryFilterType.ByMinute:
                    return "yyyy.MM.dd H:mm";
                case RepositoryFilterType.ByHour:
                    return "yyyy.MM.dd H:00";
                case RepositoryFilterType.ByWeek:
                case RepositoryFilterType.ByDay:
                    return "yyyy.MM.dd";
                case RepositoryFilterType.ByMonth:
                    return "yyyy.MM.01";
                case RepositoryFilterType.ByYear:
                    return "yyyy.01.01";
                default:
                    throw new NotImplementedException();
            }
        }

        public static DateTime IncrementDateByRepositoryFilter(DateTime dateTime, RepositoryFilterType filterType)
        {
            switch (filterType)
            {
                case RepositoryFilterType.None:
                    return dateTime;
                case RepositoryFilterType.ByMinute:
                    return dateTime.AddMinutes(1);
                case RepositoryFilterType.ByHour:
                    return dateTime.AddHours(1);
                case RepositoryFilterType.ByDay:
                    return dateTime.AddDays(1);
                case RepositoryFilterType.ByMonth:
                    return dateTime.AddMonths(1);
                case RepositoryFilterType.ByYear:
                    return dateTime.AddYears(1);
                case RepositoryFilterType.ByWeek:
                    return dateTime.AddDays(7);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
