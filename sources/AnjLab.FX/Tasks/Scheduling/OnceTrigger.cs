using System;
using System.Globalization;
using AnjLab.FX.Properties;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class OnceTrigger : ITrigger
    {
        private readonly string _tag;
        private readonly DateTime _dateTime;

        public OnceTrigger(string tag, DateTime dateTime)
        {
            _tag = tag;
            _dateTime = dateTime;
        }

        public DateTime? GetNextTriggerTime(DateTime currentTime)
        {
            // if we are in the past now - just return null

            if (currentTime.Ticks > _dateTime.Ticks)
                return null;
            else
            {
                return _dateTime;
            }
        }

        public string Tag
        {
            get { return _tag; }
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
        }


        public override string ToString()
        {
            return string.Format(Resources.OnceAt_Time_Tag, _dateTime, _tag);
        }

        public string ToString(CultureInfo culture)
        {
            if (culture.Name == "ru-RU")
                return string.Format("ќдин раз в {0}", _dateTime);
            else
                return ToString();
        }
    }
}
