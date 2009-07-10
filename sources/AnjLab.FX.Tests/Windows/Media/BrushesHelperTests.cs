using AnjLab.FX.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AnjLab.FX.Tests.Windows.Media
{
    [TestFixture]
    public class BrushesHelperTests
    {
        [Test]
        public void TestBrushes()
        {
            Assert.That(BrushesHelper.Brushes.Length, Is.GreaterThan(0));
        }
    }
}