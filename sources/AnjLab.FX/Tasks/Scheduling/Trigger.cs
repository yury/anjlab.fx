using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class Trigger
    {
        private const string TriggersTag = "triggers";
        private const string DailyTag = "daily";
        private const string TagTag = "tag";
        private const string TimeOfDayTag = "timeOfDay";
        private const string WeeklyTag = "weekly";
        private const string WeekDaysTag = "weekDays";
        private const string HourlyTag = "hourly";
        private const string MinutesTag = "minutes";
        private const string OnceTag = "once";
        private const string DateTimeTag = "dateTime";
        private const string MontlyTag = "monthly";
        private const string MonthDayTag = "monthDay";
        private const string IntervalTag = "interval";
        private const string StartTimeTag = "startTime";

        public static ITrigger Daily(string tag, TimeSpan timeOfDay)
        {
            return new DailyTrigger(tag, timeOfDay);
        }

        public static ITrigger Weekly(string tag, TimeSpan timeOfDay, params DayOfWeek[] weekDays)
        {
            return new WeeklyTrigger(tag, timeOfDay, weekDays);
        }

        public static ITrigger Hourly(string tag, int minutes)
        {
            return new HourlyTrigger(tag, minutes);
        }

        public static ITrigger Interval(string tag, DateTime startTime, TimeSpan interval)
        {
            return new IntervalTrigger(tag, startTime, interval);
        }

        public static ITrigger Once(string tag, DateTime datetime)
        {
            return new OnceTrigger(tag, datetime);
        }

        public static ITrigger Monthly(string tag, int monthDay, TimeSpan timeOfDay)
        {
            return new MonthlyTrigger(tag, monthDay, timeOfDay);
        }

        /// <summary>Reads triggers from xml stream</summary>
        /// <triggers>
        ///     <daily tag="restoreDB" timeOfDay="23:00"/>
        ///     <weekly tag="backupDB" timeOfDay="21:30" weekDays="monday,friday"/>
        ///     <hourly tag="delTempFiles" minutes="30"/>
        ///     <interval tag="dumpLog" interval="00:05:00"/>
        ///     <once tag="upgradeDB" dateTime="01/15/2007 23:00"/>
        ///     <monthly tag="archiveDB" monthDay="29" timeOfDay="23:00"/>
        /// </triggers>
        public static IList<ITrigger> ReadTriggers(XmlReader reader)
        {
            IList<ITrigger> triggers = new List<ITrigger>();

            if (reader.Name != TriggersTag)
                reader.Read();
            reader.MoveToContent();
            
            int depth = reader.Depth;

            do
            {
                reader.Read();
                
                switch(reader.Name)
                {
                    case DailyTag:
                        triggers.Add(Daily(reader[TagTag], TimeSpan.Parse(reader[TimeOfDayTag])));
                        break;
                    case WeeklyTag:
                        TimeSpan timeOfDay = TimeSpan.Parse(reader[TimeOfDayTag]);
                        string[] days = reader[WeekDaysTag].Split(',');
                        DayOfWeek [] weekDays = new DayOfWeek[days.Length];

                        for(int i = 0; i < days.Length; i++)
                            weekDays[i] = (DayOfWeek)Enum.Parse(typeof (DayOfWeek), days[i], true);

                        triggers.Add(Weekly(reader[TagTag], timeOfDay, weekDays));
                        break;
                    case HourlyTag:
                        int minutes = int.Parse(reader[MinutesTag]);
                        triggers.Add(Hourly(reader[TagTag], minutes));
                        break;
                    case OnceTag:
                        DateTime dateTime = DateTime.Parse(reader[DateTimeTag], CultureInfo.InvariantCulture);
                        triggers.Add(Once(reader[TagTag], dateTime));
                        break;
                    case MontlyTag:
                        int monthDay = int.Parse(reader[MonthDayTag]);
                        timeOfDay = TimeSpan.Parse(reader[TimeOfDayTag]);
                        triggers.Add(Monthly(reader[TagTag], monthDay, timeOfDay));
                        break;
                    case IntervalTag:
                        TimeSpan interval = TimeSpan.Parse(reader[IntervalTag]);
                        DateTime startTime;
                        if (!DateTime.TryParse(reader[StartTimeTag], out startTime))
                            startTime = DateTime.Now;
                        triggers.Add(Interval(reader[TagTag], startTime, interval));
                        break;
                }
            } while (depth != reader.Depth);

            if(reader.NodeType != XmlNodeType.None)
                reader.ReadEndElement();

            return triggers;
        }

        public static void SaveTriggers(IEnumerable<ITrigger> triggers, XmlWriter writer)
        {
            writer.WriteStartElement(TriggersTag);

            foreach(var trigger in triggers)
            {
                var dailyTrigger = trigger as DailyTrigger;
                if(dailyTrigger != null)
                {
                    writer.WriteStartElement(DailyTag);
                    writer.WriteAttributeString(TagTag, dailyTrigger.Tag);
                    writer.WriteAttributeString(TimeOfDayTag, dailyTrigger.TimeOfDay.ToString());
                    writer.WriteEndElement();
                    continue;
                }
                var weeklyTrigger = trigger as WeeklyTrigger;
                if (weeklyTrigger != null)
                {
                    writer.WriteStartElement(WeeklyTag);
                    writer.WriteAttributeString(TagTag, weeklyTrigger.Tag);
                    writer.WriteAttributeString(WeekDaysTag, string.Join(",", weeklyTrigger.WeekDays.Select(wd => wd.ToString()).ToArray()));
                    writer.WriteAttributeString(TimeOfDayTag, weeklyTrigger.TimeOfDay.ToString());
                    writer.WriteEndElement();
                    continue;
                }
                var hourlyTrigger = trigger as HourlyTrigger;
                if (hourlyTrigger != null)
                {
                    writer.WriteStartElement(HourlyTag);
                    writer.WriteAttributeString(TagTag, hourlyTrigger.Tag);
                    writer.WriteAttributeString(MinutesTag, hourlyTrigger.Minutes.ToString());
                    writer.WriteEndElement();
                    continue;
                }
                var onceTrigger = trigger as OnceTrigger;
                if (onceTrigger != null)
                {
                    writer.WriteStartElement(OnceTag);
                    writer.WriteAttributeString(TagTag, onceTrigger.Tag);
                    writer.WriteAttributeString(DateTimeTag, onceTrigger.DateTime.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                    continue;
                }
                var monthlyTrigger = trigger as MonthlyTrigger;
                if (monthlyTrigger != null)
                {
                    writer.WriteStartElement(MontlyTag);
                    writer.WriteAttributeString(TagTag, monthlyTrigger.Tag);
                    writer.WriteAttributeString(MonthDayTag, monthlyTrigger.MonthDay.ToString());
                    writer.WriteAttributeString(TimeOfDayTag, monthlyTrigger.TimeOfDay.ToString());
                    writer.WriteEndElement();
                    continue;
                }
                var intervalTrigger = trigger as IntervalTrigger;
                if (intervalTrigger != null)
                {
                    writer.WriteStartElement(IntervalTag);
                    writer.WriteAttributeString(TagTag, intervalTrigger.Tag);
                    writer.WriteAttributeString(IntervalTag, intervalTrigger.Interval.ToString());
                    writer.WriteAttributeString(StartTimeTag, intervalTrigger.StartTime.ToString());
                    writer.WriteEndElement();
                    continue;
                }
            }
        }

        public static string SaveTriggers(IEnumerable<ITrigger> triggers)
        {
            var builder = new StringBuilder();
            using(var writer = XmlWriter.Create(builder))
                SaveTriggers(triggers, writer);

            return builder.ToString();
        }

        public static IList<ITrigger> ReadTriggers(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return new List<ITrigger>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                return ReadTriggers(reader);    
            }
        }
    }
}
