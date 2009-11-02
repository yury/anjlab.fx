using System.Collections.Generic;
using System.Drawing;
using AnjLab.FX.Drawing;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Drawing
{
    [TestFixture]
    public class AirplaneInfoControlTests : AssertionHelper
    {
        [Test]
        public void TestGetReadableColor()
        {
            List<Color> darkColors = new List<Color>();
            darkColors.Add(Color.Black);
            darkColors.Add(Color.DarkBlue);
            darkColors.Add(Color.DarkGreen);
            darkColors.Add(Color.DarkRed);

            Expect(darkColors.ConvertAll<Color>(Palette.GetReadableColor), All.EqualTo(Color.White));

            List<Color> lightColors = new List<Color>();
            lightColors.Add(Color.White);
            lightColors.Add(Color.Yellow);
            lightColors.Add(Color.YellowGreen);
            lightColors.Add(Color.Red);
            lightColors.Add(Color.LightBlue);
            lightColors.Add(Color.Lime);

            Expect(lightColors.ConvertAll<Color>(Palette.GetReadableColor), All.EqualTo(Color.Black));
        }
    }
}
