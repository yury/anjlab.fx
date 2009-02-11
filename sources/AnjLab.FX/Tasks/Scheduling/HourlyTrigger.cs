using System;
using System.Globalization;
using AnjLab.FX.Properties;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class HourlyTrigger : ITrigger
    {
        private readonly string _tag;
        private readonly int _minutes;

        public HourlyTrigger(string tag, int minutes)
        {
            Guard.ArgumentBetweenInclusive("minutes", minutes, 0, 59);

            _tag = tag;
            _minutes = minutes % 60;
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            if (currentTime.Minute == _minutes && currentTime.Second == 0 && currentTime.Millisecond == 0)
                return currentTime;
            if (currentTime.Minute < _minutes)
                return new DateTime( currentTime.Year
                                    , currentTime.Month
                                    , currentTime.Day
                                    , currentTime.Hour
                                    , _minutes
                                    , 0);
            else
                return new DateTime( currentTime.Year
                                   , currentTime.Month
                                   , currentTime.Day
                                   , currentTime.Hour
                                   , _minutes
                                   , 0)
                                   .AddHours(1);
        }

        public string Tag
        {
            get { return _tag; }
        }

        public int Minutes
        {
            get { return _minutes; }
        }


        public override string ToString()
        {
            return string.Format(Resources.HourlyAt_Minutes_Tag, _minutes, _tag);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("Каждый час в {0} минут", _minutes);
            else
                return ToString();
        }
    }
}
