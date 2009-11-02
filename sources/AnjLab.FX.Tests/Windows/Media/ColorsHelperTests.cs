using AnjLab.FX.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AnjLab.FX.Tests.Windows.Media
{
    [TestFixture]
    public class ColorsHelperTests
    {
        [Test]
        public void TestGetColors()
        {
            Assert.That(ColorsHelper.Colors.Length, Is.GreaterThan(0));
        }
    }
}
