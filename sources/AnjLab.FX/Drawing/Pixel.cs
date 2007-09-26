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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Pixel"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="colorValue">The color value.</param>
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

        /// <summary>
        /// Gets the Color of this pixel.  Use <see cref="T:ColorValue"/> when possible as it is more efficient.
        /// </summary>
        /// <value>The Color of this pixel.</value>
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
