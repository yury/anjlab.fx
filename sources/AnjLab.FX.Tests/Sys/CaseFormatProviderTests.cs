using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class CaseFormatProviderTests
    {
        [Test]
        public void TestProvider()
        {
            CaseFormatProvider provider = new CaseFormatProvider();

            Assert.AreEqual("Истина где-то рядом", string.Format(provider, "{0:False=Ложь;True=Истина} где-то рядом", true));
            Assert.AreEqual("Ложь где-то рядом", string.Format(provider, "{0:False=Ложь;True=Истина} где-то рядом", false));
            Assert.AreEqual("Zero", string.Format(provider, "{0:0=Zero;1=One;2=Two;we}", 0));
            Assert.AreEqual("Two", string.Format(provider, "{0:0=Zero;1=One;2=Two}", 2));
            Assert.AreEqual("2", string.Format(provider, "{0:0=Zero;1=One;23=Two}", 2));
            Assert.AreEqual(2.2222.ToString("F2"), string.Format(provider, "{0:F2}", 2.2222));
            Assert.AreEqual(2.2299.ToString("0.00"), string.Format(provider, "{0:0.00}", 2.2299));
            Assert.AreEqual("2=Two", string.Format("{0:2=Two}", 2));
            Assert.AreEqual(0.57.ToString("p0"), string.Format(provider, "{0:p0}", 0.57));
            Assert.AreEqual(string.Empty, string.Format("{0:2=Two}", new object[] {null}));
        }
    }
}
