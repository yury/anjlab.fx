using System;
using System.Globalization;
using AnjLab.FX.Properties;
using AnjLab.FX.Sys;


namespace AnjLab.FX.Tasks.Scheduling
{
    public class MonthlyTrigger : ITrigger
    {
        private readonly string _tag;
        private readonly TimeSpan _timeOfDay;
        private readonly int _monthDay;


        public MonthlyTrigger(string tag, int monthDay, TimeSpan timeOfDay)
        {
            Guard.ArgumentBetweenInclusive("monthDay", monthDay, 1, 31);

            _tag = tag;
            _timeOfDay = timeOfDay;
            _monthDay = monthDay;
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            DateTime result = currentTime.Date;
            if (result.Day == _monthDay && currentTime.TimeOfDay <= _timeOfDay)
                return result.Add(_timeOfDay);
            else if (result.Day < _monthDay)
            {
                result = result.AddDays(_monthDay - result.Day);
                if (result.Month != currentTime.Month)
                {
                    return GetNextTriggerTime(currentTime.AddMonths(1));
                }
                return result.Add(_timeOfDay);
            } 
            else
            {
                result = result.AddDays(-result.Day + 1).AddMonths(1).AddDays(_monthDay - 1);
                int monthDiff = Math.Abs(currentTime.Month - result.Month);
                if (monthDiff == 1 || (monthDiff == 11 && currentTime.Month == 12))
                    return result.Add(_timeOfDay);
                else
                    return GetNextTriggerTime(currentTime.AddMonths(1));
            }
        }

        public string Tag
        {
            get { return _tag; }
        }

        public int MonthDay
        {
            get { return _monthDay; }
        }

        public TimeSpan TimeOfDay
        {
            get { return _timeOfDay; }
        }


        public override string ToString()
        {
            return string.Format(Resources.MonthlyEveryDayAt_TimeOfDay_Tag_DateTime, _timeOfDay, _tag, _monthDay);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("≈жемес€чно на {0} день в {1}", _monthDay, _timeOfDay);
            else
                return ToString();
        }
    }
}
