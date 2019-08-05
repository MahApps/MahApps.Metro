using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MahApps.Metro.Controls.ColorPicker
{
    public static class ColorHelper
    {
        #region Constructors

        static ColorHelper()
        {
            ColorNames = new Dictionary<object, string>();

            var rm = new ResourceManager(typeof(Lang.ColorNames));
            ResourceSet resourceSet =rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (var entry in resourceSet.OfType<DictionaryEntry>())
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(entry.Key.ToString());
                    ColorNames.Add(color, entry.Value.ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine(entry.Key.ToString() + " is not a valid color-key");
                }
            }            
        }
        #endregion

        public static int ToInt32(this Color color)
        {
            byte[] channels = new byte[4];
            channels[0] = color.B;
            channels[1] = color.G;
            channels[2] = color.R;
            channels[3] = color.A;
            return BitConverter.ToInt32(channels, 0);
        }

        public static Color ColorFromInt32 (int ColorNumber)
        {
            var bytes = BitConverter.GetBytes(ColorNumber);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public static Color? ColorFromString (string ColorName)
        {
            Color? result = null;

            try
            {
                if (! ColorName.StartsWith("#"))
                {
                    result = ColorNames.FirstOrDefault(x => string.Equals(x.Value, ColorName, StringComparison.OrdinalIgnoreCase)).Key as Color?;
                }
                if (!result.HasValue)
                {
                    result = ColorConverter.ConvertFromString(ColorName) as Color?;
                }
            }
            catch (FormatException e)
            {
                if (!result.HasValue && !ColorName.StartsWith("#"))
                {
                    result = ColorFromString("#" + ColorName);
                }
            }

            return result;
        }


        public static Dictionary<object, string> ColorNames { get; set; }

        public static string GetColorName(Color color)
        {
            return ColorNames.TryGetValue(color, out string name) ? name : null;
        }

    }
}
