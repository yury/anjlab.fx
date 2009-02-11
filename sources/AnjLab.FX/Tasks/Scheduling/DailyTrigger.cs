using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AnjLab.FX.Properties;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class DailyTrigger: ITrigger
    {
        private readonly string _tag;
        private readonly TimeSpan _timeOfDay;

        public DailyTrigger(string tag, TimeSpan timeOfDay)
        {
            _tag = tag;
            _timeOfDay = timeOfDay;
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            DateTime date = currentTime.Date;
            if (currentTime.TimeOfDay <= _timeOfDay)
                return date.Add(_timeOfDay);
            else
                return date.AddDays(1).Add(_timeOfDay);
        }

        public string Tag
        {
            get { return _tag; }
        }

        public TimeSpan TimeOfDay
        {
            get { return _timeOfDay; }
        }

        public override string ToString()
        {
            return string.Format(Resources.DailyAt_TimeOfDay_Tag, _timeOfDay, _tag);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("≈жедневно в {0}", _timeOfDay);
            else
                return ToString();
        }
    }
}
