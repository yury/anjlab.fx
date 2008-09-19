using AnjLab.FX.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AnjLab.FX.Tests.Windows.Media
{
    [TestFixture]
    public class BrushesHelperTests
    {
        [Test]
        public void TestGetColors()
        {
            Assert.That(BrushesHelper.GetBrushes().Length, Is.GreaterThan(0));
        }
    }
}