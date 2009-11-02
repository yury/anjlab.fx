using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AnjLab.FX.Data;
using System.Globalization;

namespace AnjLab.FX.Tests.Data
{
    [TestFixture]
    public class RepositoryFilterTests
    {
        [Test]
        public void TestIncrementDate()
        {
            Assert.AreEqual(new DateTime(2008, 01, 02), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01), RepositoryFilterType.ByDay));
            Assert.AreEqual(new DateTime(2008, 01, 01, 02, 00, 00), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01, 01, 00, 00), RepositoryFilterType.ByHour));
            Assert.AreEqual(new DateTime(2008, 01, 01, 01, 01, 00), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01, 01, 00, 00), RepositoryFilterType.ByMinute));
            Assert.AreEqual(new DateTime(2008, 02, 01), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01), RepositoryFilterType.ByMonth));
            Assert.AreEqual(new DateTime(2008, 01, 08), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01), RepositoryFilterType.ByWeek));
            Assert.AreEqual(new DateTime(2009, 01, 01), RepositoryFilter.IncrementDateByRepositoryFilter(new DateTime(2008, 01, 01), RepositoryFilterType.ByYear));
        }

        [Test]
        public void TestTrimDate()
        {
            DateTime t = new DateTime(2008, 02, 01, 15, 33, 14);
            Assert.AreEqual(new DateTime(2008, 02, 01), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByDay));
            Assert.AreEqual(new DateTime(2008, 02, 01, 15, 00, 00), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByHour));
            Assert.AreEqual(new DateTime(2008, 02, 01, 15, 33, 00), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByMinute));
            Assert.AreEqual(new DateTime(2008, 02, 01, 00, 00, 00), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByMonth));
            Assert.AreEqual(new DateTime(2008, 01, 27, 00, 00, 00), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByWeek));
            Assert.AreEqual(new DateTime(2008, 01, 01, 00, 00, 00), RepositoryFilter.TrimDateTime(t, RepositoryFilterType.ByYear));
        }
    }
}
