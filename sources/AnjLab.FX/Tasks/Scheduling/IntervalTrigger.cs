using System;
using AnjLab.FX.System;
using AnjLab.FX.System;
using AnjLab.FX.System;

namespace AnjLab.FX.Tasks.Scheduling
{
    internal class IntervalTrigger: ITrigger
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


        public override string ToString()
        {
            return string.Format("[{1}] every {0} starting from {2}", _interval, _tag, _startTime);
        }
    }
}
