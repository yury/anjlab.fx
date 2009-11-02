using System;
using System.Collections.Generic;
using AnjLab.FX.Tasks.Scheduling;

using NUnit.Framework;

namespace AnjLab.FX.Tests.Tasks.Scheduling
{
    [TestFixture]
    public class TriggerTests
    {
        [Test]
        public void TestOnceTrigger()
        {
            DateTime now = DateTime.Now;
            ITrigger once = Trigger.Once("bla", now);
            Assert.IsNotNull(once);

            Assert.AreEqual(now, once.GetNextTriggerTime(now));

            Assert.AreEqual(now, once.GetNextTriggerTime(now.AddMilliseconds(-1)));
            Assert.AreEqual(now, once.GetNextTriggerTime(now.AddDays(-1)));

            Assert.IsNull(once.GetNextTriggerTime(now.AddDays(1)));
            Assert.IsNull(once.GetNextTriggerTime(now.AddMilliseconds(1)));

            now = new DateTime(633252388592031250);
            once = Trigger.Once("bla", new DateTime(633252388402031250));
            Assert.IsNull(once.GetNextTriggerTime(now));
        }

        [Test]
        public void TestDailyTrigger()
        {
            DateTime now = DateTime.Now;
            ITrigger daily = Trigger.Daily("bla", now.TimeOfDay);
            Assert.IsNotNull(daily);

            Assert.AreEqual(now, daily.GetNextTriggerTime(now));
            Assert.AreEqual(now, daily.GetNextTriggerTime(now.AddDays(-0.5)));
            Assert.AreEqual(now.AddDays(-1), daily.GetNextTriggerTime(now.AddDays(-1)));

            Assert.AreEqual(now.AddDays(1), daily.GetNextTriggerTime(now.AddMilliseconds(1)));
            Assert.AreEqual(now.AddDays(1), daily.GetNextTriggerTime(now.AddDays(1)));
            Assert.AreEqual(now.AddDays(5), daily.GetNextTriggerTime(now.AddDays(4.5)));
        }

        [Test]
        public void TestWeeklyTrigger()
        {
            DateTime now = DateTime.Now;
            ITrigger weekly = Trigger.Weekly("bla", now.TimeOfDay, now.DayOfWeek);
            Assert.IsNotNull(weekly);

            Assert.AreEqual(now, weekly.GetNextTriggerTime(now));
            Assert.AreEqual(now.AddDays(7), weekly.GetNextTriggerTime(now.AddMilliseconds(1)));

            Assert.AreEqual(now, weekly.GetNextTriggerTime(now.AddMilliseconds(-1)));

            now = new DateTime(2000, 1, 1, 1, 1, 1, 1); // Saturday
            weekly = Trigger.Weekly("bla", now.TimeOfDay, DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday);

            // monday
            Assert.AreEqual(DayOfWeek.Monday, now.AddDays(2).DayOfWeek);
            Assert.AreEqual(now.AddDays(2), weekly.GetNextTriggerTime(now));
            Assert.AreEqual(now.AddDays(2), weekly.GetNextTriggerTime(now.AddDays(2)));

            // wendesday
            Assert.AreEqual(DayOfWeek.Wednesday, now.AddDays(4).DayOfWeek);
            Assert.AreEqual(now.AddDays(4), weekly.GetNextTriggerTime(now.AddDays(3)));
            Assert.AreEqual(now.AddDays(4), weekly.GetNextTriggerTime(now.AddDays(4)));

            // friday
            Assert.AreEqual(DayOfWeek.Friday, now.AddDays(6).DayOfWeek);
            Assert.AreEqual(now.AddDays(6), weekly.GetNextTriggerTime(now.AddDays(5)));
            Assert.AreEqual(now.AddDays(6), weekly.GetNextTriggerTime(now.AddDays(6)));

            // monday again
            Assert.AreEqual(DayOfWeek.Monday, now.AddDays(9).DayOfWeek);
            Assert.AreEqual(now.AddDays(9), weekly.GetNextTriggerTime(now.AddDays(6.5)));
        }

        [Test]
        public void TestHourlyTrigger()
        {
            DateTime now = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            
            ITrigger hourly = Trigger.Hourly("bla", 0);
            Assert.IsNotNull(hourly);
            Assert.AreEqual(now, hourly.GetNextTriggerTime(now));
            Assert.AreEqual(now, hourly.GetNextTriggerTime(now.AddMilliseconds(-1)));
            
            Assert.AreEqual(now.AddHours(1), hourly.GetNextTriggerTime(now.AddMilliseconds(1)));
            Assert.AreEqual(now.AddHours(1), hourly.GetNextTriggerTime(now.AddHours(1)));
        }

        [Test]
        public void TestIntervalTrigger()
        {
            DateTime now = new DateTime(2000, 1, 1, 0, 0, 0, 0);

            ITrigger interval = Trigger.Interval("bla", now, TimeSpan.FromMinutes(1));
            Assert.IsNotNull(interval);
            Assert.AreEqual(now, interval.GetNextTriggerTime(now));
            Assert.AreEqual(now, interval.GetNextTriggerTime(now.AddMilliseconds(-1)));

            Assert.AreEqual(now.AddMinutes(1), interval.GetNextTriggerTime(now.AddMilliseconds(1)));
            Assert.AreEqual(now.AddMinutes(1), interval.GetNextTriggerTime(now.AddMilliseconds(2)));
            Assert.AreEqual(now.AddMinutes(2), interval.GetNextTriggerTime(now.AddMilliseconds(60 * 1000)));
        }

        [Test]
        public void TestMonthlyTrigger()
        {
            DateTime now = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            ITrigger monthly = Trigger.Monthly("bla", 31, TimeSpan.FromHours(1));
            Assert.IsNotNull(monthly);

            // 2000-01-01 00:00 -> 2000-01-31 01:00
            Assert.AreEqual(now.AddDays(30).AddHours(1), monthly.GetNextTriggerTime(now));

            // 2000-02-01 00:00 -> 2000-03-31 01:00
            Assert.AreEqual(now.AddMonths(2).AddDays(30).AddHours(1), monthly.GetNextTriggerTime(now.AddMonths(1)));

            // 2000-12-01 00:00 -> 2000-12-31 01:00
            Assert.AreEqual(now.AddMonths(11).AddDays(30).AddHours(1), monthly.GetNextTriggerTime(now.AddMonths(11)));

            // 2000-12-31 02:00 -> 2001-01-31 01:00
            Assert.AreEqual(now.AddMonths(12).AddDays(30).AddHours(1), monthly.GetNextTriggerTime(now.AddMonths(11).AddDays(30).AddHours(2)));
        }

        [Test]
        public void TestReadTriggers()
        {
            string xml =
@"<triggers>
        <daily    tag='restoreDB'    timeOfDay='23:00'/>
        <weekly   tag='backupDB'     timeOfDay='01:30' weekDays='monday,friday'/>
        <hourly   tag='delTempFiles' minutes='30'/>
        <interval tag='dumpLog'      interval='00:05:00'/>
        <once     tag='upgradeDB'    dateTime='01/15/2007 23:00'/>
        <monthly  tag='archiveDB'    monthDay='29' timeOfDay='23:00'/>
</triggers>";

            IList<ITrigger> schedule = Trigger.ReadTriggers(xml);
            Assert.IsNotNull(schedule);
            Assert.AreEqual(6, schedule.Count);

            Assert.IsAssignableFrom(typeof(DailyTrigger)   , schedule[0]);
            Assert.IsAssignableFrom(typeof(WeeklyTrigger)  , schedule[1]);
            Assert.IsAssignableFrom(typeof(HourlyTrigger)  , schedule[2]);
            Assert.IsAssignableFrom(typeof(IntervalTrigger), schedule[3]);
            Assert.IsAssignableFrom(typeof(OnceTrigger)    , schedule[4]);
            Assert.IsAssignableFrom(typeof(MonthlyTrigger) , schedule[5]);

            DailyTrigger daily = schedule[0] as DailyTrigger;
            Assert.AreEqual("restoreDB", daily.Tag);
            Assert.AreEqual(TimeSpan.FromHours(23), daily.TimeOfDay);

            WeeklyTrigger weekly = schedule[1] as WeeklyTrigger;
            Assert.AreEqual("backupDB", weekly.Tag);
            Assert.AreEqual(TimeSpan.FromMinutes(90), weekly.TimeOfDay);

            HourlyTrigger hourly = schedule[2] as HourlyTrigger;
            Assert.AreEqual("delTempFiles", hourly.Tag);
            Assert.AreEqual(30, hourly.Minutes);

            IntervalTrigger interval = schedule[3] as IntervalTrigger;
            Assert.AreEqual("dumpLog", interval.Tag);
            Assert.AreEqual(TimeSpan.FromMinutes(5), interval.Interval);

            OnceTrigger once = schedule[4] as OnceTrigger;
            Assert.AreEqual("upgradeDB", once.Tag);
            Assert.AreEqual(new DateTime(2007, 1, 15, 23, 0, 0), once.DateTime);

            MonthlyTrigger monthly = schedule[5] as MonthlyTrigger;
            Assert.AreEqual("archiveDB", monthly.Tag);
            Assert.AreEqual(29, monthly.MonthDay);
            Assert.AreEqual(TimeSpan.FromHours(23), monthly.TimeOfDay);
        }

        [Test]
        public void TestWriteTriggers()
        {
            var triggers = new List<ITrigger>
                               {
                                   Trigger.Daily("restoreDB", new TimeSpan(5, 4, 3, 4)),
                                   Trigger.Hourly("hourlyTrigger", 56),
                                   Trigger.Interval("interval", new DateTime(2008, 1, 5), new TimeSpan(5, 4, 5)),
                                   Trigger.Monthly("monthly", 5, new TimeSpan(5, 3, 4)),
                                   Trigger.Once("once", new DateTime(2008, 4, 5, 4, 3, 4)),
                                   Trigger.Weekly("weekly", new TimeSpan(4, 5, 4), DayOfWeek.Monday, DayOfWeek.Sunday,
                                                  DayOfWeek.Wednesday)
                               };

            var result = Trigger.SaveTriggers(triggers);

            var readedTriggers = Trigger.ReadTriggers(result);

            for(int i=0; i < triggers.Count; i++)
                Assert.AreEqual(triggers[i].ToString(), readedTriggers[i].ToString());
        }
    }
}
