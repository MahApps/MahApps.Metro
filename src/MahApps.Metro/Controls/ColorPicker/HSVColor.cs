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
            Hue = 0;
            Saturation = 0;
            Value = 0;

            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            // H

            if (max == color.R)
            {
                Hue = 60 * (0 + (color.G - color.B) * 1.0 / (max - min));
            }
            else if (max == color.G)
            {
                Hue = 60 * (2 + (color.B - color.R) * 1.0 / (max - min));
            }
            else if (max == color.B)
            {
                Hue = 60 * (4 + (color.R - color.G) * 1.0 / (max - min));
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
                Saturation = (max - min) * 1d / max;
            }

            // V
            Value = max / 255d;
        }
    }
}
