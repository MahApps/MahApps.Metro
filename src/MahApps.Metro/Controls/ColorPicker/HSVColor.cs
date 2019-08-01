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

        public Color GetColor ()
        {
            if (double.IsNaN(Hue))
                Hue = 0;

            // Helper Values
            int h_i = (int)Math.Floor(Hue / 60);
            double f = (Hue / 60 - h_i);
            double p = Value * (1 - Saturation);
            double q = Value * (1 - Saturation * f);
            double t = Value * (1 - Saturation * (1 - f));

            double r, g, b;

            switch (h_i)
            {
                case 0:
                case 6:
                    r = Value;
                    b = t;
                    g = p;
                    break;

                case 1:
                    r = q;
                    b = Value;
                    g = p;
                    break;

                case 2:
                    r = p;
                    b = Value;
                    g = t;
                    break;

                case 3:
                    r = p;
                    b = q;
                    g = Value;
                    break;

                case 4:
                    r = t;
                    b = p;
                    g = Value;
                    break;

                case 5:
                    r = Value;
                    b = p;
                    g = q;
                    break;

                default:
                    throw new ArgumentException();
            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
