// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
        public double A { get; }

        /// <summary>
        /// Gets the Hue channel.
        /// </summary>
        public double Hue { get; }

        /// <summary>
        /// Gets the Saturation channel
        /// </summary>
        public double Saturation { get; }

        /// <summary>
        /// Gets the Value channel
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Creates a new HSV Color from a given <see cref="Color"/>
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert</param>
        public HSVColor(Color color)
        {
            this.A = color.A / 255d;
            this.Hue = 0;
            this.Saturation = 0;
            this.Value = 0;

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
                this.Hue = 60 * ((double)(color.G - color.B) / delta % 6);
            }
            else if (max == color.G)
            {
                this.Hue = 60 * (2 + (double)(color.B - color.R) / delta);
            }
            else if (max == color.B)
            {
                this.Hue = 60 * (4 + (double)(color.R - color.G) / delta);
            }

            if (this.Hue < 0)
            {
                this.Hue += 360;
            }

            // S 
            if (max == 0)
            {
                this.Saturation = 0;
            }
            else
            {
                this.Saturation = (double)delta / max;
            }

            // V
            this.Value = max / 255d;
        }

        /// <summary>
        /// Creates a new HSV Color 
        /// </summary>
        /// <param name="hue"><see cref="Hue"/> channel [0;360]</param>
        /// <param name="saturation"><see cref="Saturation"/> channel [0;1]</param>
        /// <param name="value"><see cref="Value"/> channel [0;1]</param>
        public HSVColor(double hue, double saturation, double value)
        {
            this.A = 1;
            this.Hue = hue;
            this.Saturation = saturation;
            this.Value = value;
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
            this.A = a;
            this.Hue = hue;
            this.Saturation = saturation;
            this.Value = value;
        }

        /// <summary>
        /// Gets the <see cref="Color"/> for this HSV Color struct
        /// </summary>
        /// <returns><see cref="Color"/></returns>
        public Color ToColor()
        {
            return Color.FromArgb((byte)Math.Round(this.A * 255), this.GetColorComponent(5), this.GetColorComponent(3), this.GetColorComponent(1));
        }

        private byte GetColorComponent(int n)
        {
            var k = (n + this.Hue / 60d) % 6;
            return (byte)Math.Round((this.Value - this.Value * this.Saturation * Math.Max(0, Math.Min(k, Math.Min(4 - k, 1)))) * 255);
        }

        public bool Equals(HSVColor other)
        {
            return this.Hue.IsCloseTo(other.Hue) && this.A.IsCloseTo(other.A) && this.Saturation.IsCloseTo(other.Saturation) && this.Value.IsCloseTo(other.Value);
        }
    }
}