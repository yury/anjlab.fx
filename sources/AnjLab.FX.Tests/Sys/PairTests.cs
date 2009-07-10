using System.Text;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class PairTests
    {
        [Test]
        public void TestEquals()
        {
            Pair<object, object> tom = new Pair<object, object>(null, null);
            Pair<object, object> bill = new Pair<object, object>(null, null);
            
            Assert.AreEqual(tom, bill);
            
            tom.A = 2;
            Assert.AreNotEqual(tom, bill);

            bill.A = 2;
            Assert.AreEqual(tom, bill);

            tom.B = 2;
            Assert.AreNotEqual(tom, bill);

            bill.B = 2;
            Assert.AreEqual(tom, bill);
        }
    }
}
