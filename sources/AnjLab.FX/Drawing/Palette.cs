using System.Drawing;

namespace AnjLab.FX.Drawing
{
    public class Palette
    {
        public static Color GetReadableColor(Color backgroundColor)
        {
            if ((backgroundColor.R + backgroundColor.G + backgroundColor.B) > 254)
            {
                return Color.Black;//for light colors
            }
            else
            {
                return Color.White;//dark colors
            }
        }
    }
}
