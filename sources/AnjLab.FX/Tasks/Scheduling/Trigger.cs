using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class Trigger
    {
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

            if (reader.Name != "triggers")
                reader.Read();
            reader.MoveToContent();
            
            int depth = reader.Depth;
            do
            {
                reader.Read();
                
                switch(reader.Name)
                {
                    case "daily":
                        triggers.Add(Daily(reader["tag"], TimeSpan.Parse(reader["timeOfDay"])));
                        break;
                    case "weekly":
                        TimeSpan timeOfDay = TimeSpan.Parse(reader["timeOfDay"]);
                        string[] days = reader["weekDays"].Split(',');
                        DayOfWeek [] weekDays = new DayOfWeek[days.Length];

                        for(int i = 0; i < days.Length; i++)
                            weekDays[i] = (DayOfWeek)Enum.Parse(typeof (DayOfWeek), days[i], true);

                        triggers.Add(Weekly(reader["tag"], timeOfDay, weekDays));
                        break;
                    case "hourly":
                        int minutes = int.Parse(reader["minutes"]);
                        triggers.Add(Hourly(reader["tag"], minutes));
                        break;
                    case "once":
                        DateTime dateTime = DateTime.Parse(reader["dateTime"], CultureInfo.InvariantCulture);
                        triggers.Add(Once(reader["tag"], dateTime));
                        break;
                    case "monthly":
                        int monthDay = int.Parse(reader["monthDay"]);
                        timeOfDay = TimeSpan.Parse(reader["timeOfDay"]);
                        triggers.Add(Monthly(reader["tag"], monthDay, timeOfDay));
                        break;
                    case "interval":
                        TimeSpan interval = TimeSpan.Parse(reader["interval"]);
                        DateTime startTime;
                        if (!DateTime.TryParse(reader["startTime"], out startTime))
                            startTime = DateTime.Now;
                        triggers.Add(Interval(reader["tag"], startTime, interval));
                        break;
                }
            } while (depth != reader.Depth);
            reader.ReadEndElement();

            return triggers;
        }

        public static IList<ITrigger> ReadTriggers(string xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                return ReadTriggers(reader);    
            }
        }
    }
}
