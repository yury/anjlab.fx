using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AnjLab.FX.Windows.Data;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tests.Windows.Data
{
    [TestFixture]
    public class EnumHelperValueConverterTests
    {
        public enum TestEnum
        {
            CoolDay,
            BadDay,
            AnotherDay
        }

        [Test]
        public void Test()
        {
            EnumHelper helper = new EnumHelper(new Enum[] {TestEnum.CoolDay, TestEnum.BadDay, TestEnum.AnotherDay},
                new string[] {"This is a cool day", "This is a bad day", "This is another day"});
            
            EnumHelperValueConverter conv = new EnumHelperValueConverter();

            Assert.AreEqual(helper.GetPair(TestEnum.CoolDay), conv.Convert(TestEnum.CoolDay, null, helper, null));
            Assert.AreEqual(helper.GetPair(TestEnum.BadDay), conv.Convert(TestEnum.BadDay, null, helper, null));
            Assert.AreEqual(helper.GetPair(TestEnum.AnotherDay), conv.Convert(TestEnum.AnotherDay, null, helper, null));

            Assert.AreEqual(TestEnum.CoolDay, conv.ConvertBack(helper.GetPair(TestEnum.CoolDay), null, helper, null));
            Assert.AreEqual(TestEnum.BadDay, conv.ConvertBack(helper.GetPair(TestEnum.BadDay), null, helper, null));
            Assert.AreEqual(TestEnum.AnotherDay, conv.ConvertBack(helper.GetPair(TestEnum.AnotherDay), null, helper, null));
        }
    }
}
