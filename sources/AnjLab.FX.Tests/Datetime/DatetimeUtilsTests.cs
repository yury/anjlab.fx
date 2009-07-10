using System;
using AnjLab.FX.Datetime;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Datetime
{
    [TestFixture]
    public class DateUtilsTest
    {
        [Test]
        public void DaysInYearTest()
        {
            Assert.AreEqual(365, DateUtils.DaysInYear(2003));
            Assert.AreEqual(366, DateUtils.DaysInYear(2004));
        }

        [Test]
        public void BeginEndOfDayTest()
        {
            Assert.IsTrue(DateUtils.BeginOfDay(DateTime.Now) < DateTime.Now);
            Assert.IsTrue(DateUtils.EndOfDay(DateTime.Now) > DateTime.Now);
        }
    }
}
