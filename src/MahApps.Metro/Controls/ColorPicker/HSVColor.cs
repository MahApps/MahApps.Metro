using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{

    /// <summary>
    /// This struct represent a Color in HSV (Hue, Saturation, Value)
    /// 
    /// For more information visit: https://en.wikipedia.org/wiki/HSL_and_HSV
    /// </summary>
    public struct HSVColor : IEquatable<HSVColor>
    {
        /// <summary>
        /// Gets the Alpha channel.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// Gets the Hue channel.
        /// </summary>
        public double Hue { get; private set; }

        /// <summary>
        /// Gets the Saturation channel
        /// </summary>
        public double Saturation { get; private set; }

        /// <summary>
        /// Gets the Value channel
        /// </summary>
        public double Value { get; private set; }


        /// <summary>
        /// Creates a new HSV Color from a given <see cref="Color"/>
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert</param>
        public HSVColor(Color color)
        {
            A = color.A / 255d;
            Hue = 0;
            Saturation = 0;
            Value = 0;

            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            var delta = max - min;

            // H
            if (delta == 0)
            {
                // Do nothing, because Hue is already 0
            }
            else if (max == color.R)
            {
                Hue = 60 * ((double)(color.G - color.B) / delta % 6);
            }
            else if (max == color.G)
            {
                Hue = 60 * (2 + (double)(color.B - color.R) / delta);
            }
            else if (max == color.B)
            {
                Hue = 60 * (4 + (double)(color.R - color.G) / delta);
            }

            if (Hue < 0)
            {
                Hue += 360;
            }

            // S 
            if (max == 0)
            {
                Saturation = 0;
            }
            else
            {
                Saturation = (double)delta / max;
            }

            // V
            Value = max / 255d;
        }

        /// <summary>
        /// Creates a new HSV Color 
        /// </summary>
        /// <param name="hue"><see cref="Hue"/> channel [0;360]</param>
        /// <param name="saturation"><see cref="Saturation"/> channel [0;1]</param>
        /// <param name="value"><see cref="Value"/> channel [0;1]</param>
        public HSVColor(double hue, double saturation, double value)
        {
            A = 1;
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        /// <summary>
        /// Creates a new HSV Color 
        /// </summary>
        /// <param name="a"><see cref="A"/> (Alpha) channel [0;1]</param>
        /// <param name="hue"><see cref="Hue"/> channel [0;360]</param>
        /// <param name="saturation"><see cref="Saturation"/> channel [0;1]</param>
        /// <param name="value"><see cref="Value"/> channel [0;1]</param>
        public HSVColor(double a, double hue, double saturation, double value)
        {
            A = a;
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        /// <summary>
        /// Gets the <see cref="Color"/> for this HSV Color struct
        /// </summary>
        /// <returns><see cref="Color"/></returns>
        public Color ToColor()
        {             
            return Color.FromArgb((byte)Math.Round(A * 255), GetColorComponent(5), GetColorComponent(3), GetColorComponent(1));
        }

        byte GetColorComponent (int n)
        {
            var k = (n + Hue / 60d) % 6;
            return (byte)Math.Round((Value - Value * Saturation * Math.Max(0, Math.Min(k, Math.Min(4 - k, 1)))) * 255);
        }

        public bool Equals(HSVColor other)
        {
            return Hue.IsCloseTo(other.Hue) && A.IsCloseTo(other.A) && Saturation.IsCloseTo(other.Saturation) && Value.IsCloseTo(other.Value);
        }
    }
}
