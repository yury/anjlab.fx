using System;
using AnjLab.FX.Datetime;

namespace AnjLab.FX.Finance
{
    public class TimeInterval
    {
        public static decimal Get(DateTime currentDate, DateTime futureDate)
        {
            DateTime currentYearFirstDay = DateUtils.YearFirstDay(currentDate);
            DateTime futureYearLastDay = DateUtils.YearLastDay(futureDate);

            int daysInYearsInterval = (futureYearLastDay - currentYearFirstDay).Days + 1;
            if (daysInYearsInterval % 365 == 0) // no leap year in interval
            {
                return ((decimal)(futureDate - currentDate).Days) / 365;
            }
            else
            {
                if (currentDate.Year == futureDate.Year) // leap year
                    return ((decimal)(futureDate - currentDate).Days) / 366;
                else // interval contains leap years
                {
                    DateTime futureYearFirstDay = DateUtils.YearFirstDay(futureDate);
                    DateTime currentYearLastDay = DateUtils.YearLastDay(currentDate);

                    decimal firstInterval = ((decimal)(currentYearLastDay - currentDate).Days) / DateUtils.DaysInYear(currentDate);
                    decimal secondInterval = ((((decimal)(futureDate - futureYearFirstDay).Days) + 1) / DateUtils.DaysInYear(futureDate));
                    return firstInterval + secondInterval + (futureDate.Year - currentDate.Year - 1);
                }
            }
        }
    }
}
