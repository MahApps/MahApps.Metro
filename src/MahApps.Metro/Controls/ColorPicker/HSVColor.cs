using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MahApps.Metro.Controls.ColorPicker
{
    public struct HSVColor
    {

        public double Hue { get; private set; }
        public double Saturation { get; private set; }
        public double Value { get; private set; }

        public HSVColor(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            Hue = 0;
            Saturation = 0;
            Value = 0;

            var max = Math.Max(r, Math.Max(g,b));
            var min = Math.Min(r, Math.Min(g,b));

            // H
            if (max == min)
            {
                // Do nothing
            }
            else if (max == r)
            {
                Hue = 60 * (0 + (g-b) / (max - min));
            }
            else if (max == g)
            {
                Hue = 60 * (2 + (b - r) / (max - min));
            }
            else if (max == b)
            {
                Hue = 60 * (4 + (r - g) / (max - min));
            }

            if (Hue < 0)
            {
                Hue = Hue + 360;
            }

            // S 
            if (max == 0)
            {
                Saturation = 0;
            }
            else
            {
                Saturation = (max - min) / max;
            }

            // V
            Value = max;
        }

        public HSVColor(double hue, double saturation, double value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        public Color ToColor ()
        {
            return ToColor(255);
        }
        public Color ToColor(byte AlphaChannel)
        {
            if (double.IsNaN(Hue))
                Hue = 0;

            // Helper Values
            int h_i = (int)Math.Floor(Hue / 60);
            double C = Value * Saturation;
            double X = C * (1 - Math.Abs((Hue / 60) % 2 - 1));
            double m = Value - C;

            double r, g, b;

            switch (h_i)
            {
                case 0:
                case 6:
                    r = C;
                    g = X;
                    b = 0;
                    break;

                case 1:
                    r = X;
                    g = C;
                    b = 0;
                    break;

                case 2:
                    r = 0;
                    g = C;
                    b = X;
                    break;

                case 3:
                    r = 0;
                    g = X;
                    b = C;
                    break;

                case 4:
                    r = X;
                    g = 0;
                    b = C;
                    break;

                case 5:
                    r = C;
                    g = 0;
                    b = X;
                    break;

                default:
                    throw new ArgumentException();
            }

            r = r + m;
            g = g + m;
            b = b + m;

            return Color.FromArgb(AlphaChannel, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
