using System;
using AnjLab.FX.System;

namespace AnjLab.FX.Tasks.Scheduling
{
    internal class HourlyTrigger: ITrigger
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
            return string.Format("[{1}] Hourly at {0} min", _minutes, _tag);
        }
    }
}
