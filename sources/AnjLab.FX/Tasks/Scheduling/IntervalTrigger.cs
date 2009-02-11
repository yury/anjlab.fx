using System;
using System.Globalization;
using AnjLab.FX.Properties;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class IntervalTrigger : ITrigger
    {
        private readonly string _tag;
        private DateTime _startTime;
        private TimeSpan _interval;

        public IntervalTrigger(string tag, DateTime startTime, TimeSpan interval)
        {
            Guard.ArgumentGreaterThenZero("interval", interval);

            _tag = tag;
            _startTime = startTime;
            _interval = interval;
        }

        public IntervalTrigger(string tag, TimeSpan interval)
            :this(tag, DateTime.Now, interval)
        {
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            if (currentTime <= _startTime)
                return _startTime;

            TimeSpan ts = currentTime - _startTime;
            long count = ts.Ticks / _interval.Ticks + 1;
            return _startTime.AddTicks(count * _interval.Ticks);
        }

        public string Tag
        {
            get { return _tag; }
        }

        public TimeSpan Interval
        {
            get { return _interval; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
        }

        public override string ToString()
        {
            return string.Format(Resources.EveryTimeStartingFrom_Interval_Tag_StartTime, _interval, _tag, _startTime);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("Каждые {0} начиная с {1}", _interval, _startTime);
            else
                return ToString();
        }
    }
}
