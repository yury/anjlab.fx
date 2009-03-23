using System.Drawing;

namespace AnjLab.FX.Drawing
{
    /// <summary>
    /// Contains Color and Location information for an individual pixel in an Image.
    /// </summary>
    public class Pixel
    {
        private readonly int colorValue;
        private readonly Point location;
        private Color color = Color.Empty;

        public Pixel(int x, int y, int colorValue)
        {
            this.colorValue = colorValue;
            location = new Point(x, y);
        }

        /// <summary>
        /// Gets the location of this pixel in the Image.
        /// </summary>
        /// <value>The location of this pixel in the Image.</value>
        public Point Location
        {
            get
            {
                return location;
            }
        }

        /// <summary>
        /// Gets the int value of the Color of this pixel.
        /// </summary>
        /// <value>The int value of the Color of this pixel.</value>
        public int ColorValue
        {
            get
            {
                return colorValue;
            }
        }

        public Color Color
        {
            get
            {
                if (color == Color.Empty)
                {
                    color = Color.FromArgb(colorValue);
                }

                return color;
            }
        }
    }
}
