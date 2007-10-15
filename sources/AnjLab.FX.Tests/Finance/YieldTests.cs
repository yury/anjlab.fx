using System;
using NUnit.Framework;

namespace AnjLab.FX.Finance
{
    [TestFixture]
    public class YieldTests
    {
        [Test]
        public void CalculateTest()
        {
            Assert.AreEqual(11.392, Math.Round(Yield.Calculate(DateTime.Parse("01.10.2006"), DateTime.Parse("22.9.2007"), 90), 3));
            Assert.AreEqual(52.1368, Math.Round(Yield.Calculate(DateTime.Parse("05.09.2008"), DateTime.Parse("22.11.2008"), 90), 4));
            Assert.AreEqual(2.009, Math.Round(Yield.Calculate(DateTime.Parse("03.10.2003"), DateTime.Parse("30.10.2005"), 96), 4));
            Assert.AreEqual(1.1042, Math.Round(Yield.Calculate(DateTime.Parse("02.10.1999"), DateTime.Parse("25.10.2009"), 90), 4));

            Assert.AreEqual(0, Math.Round(Yield.Calculate(DateTime.Parse("02.10.1999"), DateTime.Parse("02.10.1999"), 90), 4));
        }
    }
}
