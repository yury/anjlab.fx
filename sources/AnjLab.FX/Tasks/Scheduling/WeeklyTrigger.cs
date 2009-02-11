using System;
using System.Globalization;
using System.Linq;
using AnjLab.FX.Properties;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class WeeklyTrigger : ITrigger
    {
        private readonly string _tag;
        private readonly TimeSpan _timeOfDay;
        private readonly DayOfWeek[] _weekDays;

        public WeeklyTrigger(string tag, TimeSpan timeOfDay, params DayOfWeek[] weekDays)
        {
            _tag = tag;
            _timeOfDay = timeOfDay;
            _weekDays = weekDays;
            if (_weekDays.Length == 0)
                _weekDays = new DayOfWeek[]{DayOfWeek.Monday};

            Array.Sort(_weekDays);
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            DateTime date = currentTime.Date;
            DayOfWeek weekDay = currentTime.DayOfWeek;

            if (IsScheduled(weekDay) && currentTime.TimeOfDay <= _timeOfDay)
                return date.Add(_timeOfDay);
            else
            {
                int wD  = (int)weekDay;
                int nwD = (int)GetNextDayOfWeek(weekDay);
                int diff = 0;

                do
                {
                    diff++;
                } while ((++wD) % 7 != nwD);

                return date.AddDays(diff).Add(_timeOfDay);
            }
        }

        private bool IsScheduled(DayOfWeek weekDay)
        {
            foreach (DayOfWeek day in _weekDays)
            {
                if (day == weekDay)
                    return true;
            }
            return false;
        }

        private DayOfWeek GetNextDayOfWeek(DayOfWeek weekDay)
        {
            foreach (DayOfWeek day in _weekDays)
            {
                if (day > weekDay)
                    return day;
            }
            return _weekDays[0];
        }

        public string Tag
        {
            get { return _tag; }
        }

        public TimeSpan TimeOfDay
        {
            get { return _timeOfDay; }
        }

        public DayOfWeek[] WeekDays
        {
            get { return _weekDays; }
        }

        public override string ToString()
        {
            return string.Format(Resources.Weekly_OnTime_AtTimeOfDay_Tag, Lst.ToString(_weekDays), _tag, _timeOfDay);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("≈женедельно по дн€м: {0} в {1}", Lst.ToString(WeekDays.Select(wd => culture.DateTimeFormat.DayNames[(int)wd])), _timeOfDay);
            else
                return ToString();
        }
    }
}
