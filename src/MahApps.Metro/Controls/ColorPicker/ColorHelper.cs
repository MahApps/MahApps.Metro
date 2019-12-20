using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls.ColorPicker
{

    /// <summary>
    /// A Helper class for the Color-Struct
    /// </summary>
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

        /// <summary>
        /// Converts this Color to its Int32-Value
        /// </summary>
        /// <param name="color">the color to convert</param>
        /// <returns>32 bit Integer</returns>
        public static int ToInt32(this Color color)
        {
            byte[] channels = new byte[4];
            channels[0] = color.B;
            channels[1] = color.G;
            channels[2] = color.R;
            channels[3] = color.A;
            return BitConverter.ToInt32(channels, 0);
        }

        /// <summary>
        /// Creats an Int32 into a Color
        /// </summary>
        /// <param name="ColorNumber">the Int32 representation of the color</param>
        /// <returns>Color</returns>
        public static Color ColorFromInt32 (int ColorNumber)
        {
            var bytes = BitConverter.GetBytes(ColorNumber);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        /// <summary>
        /// This function tries to convert a given string into a Color in the following order:
        ///    1. If the string starts with '#' the function tries to get the color from the hex-code
        ///    2. else the function tries to find the color in the color names Dictionary
        ///    3. If 1. & 2. were not successfull the function adds a '#' sign and tries 1. & 2. again
        /// </summary>
        /// <param name="ColorName">The localized name of the color, the hex-code of the color or the internal colorname</param>
        /// <returns>the Color if successfull, else null</returns>
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


        /// <summary>
        /// A Dictionary with localized Color Names
        /// </summary>
        public static Dictionary<object, string> ColorNames { get; set; }


        /// <summary>
        /// Searches for the localized name of a given <paramref name="color"/>
        /// </summary>
        /// <param name="color">color</param>
        /// <returns>the local color name or null if the given color doesn't have a name</returns>
        public static string GetColorName(Color color)
        {
            return ColorNames.TryGetValue(color, out string name) ? name : null;
        }

    }
}
