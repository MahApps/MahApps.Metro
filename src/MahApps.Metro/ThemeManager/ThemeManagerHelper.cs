using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using generator = XamlColorSchemeGenerator;

namespace MahApps.Metro
{
    internal static class ThemeManagerHelper
    {
        private static string GetThemeTemplateContent()
        {
            using (var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("MahApps.Metro.Styles.Themes.Theme.Template.xaml"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        ///     Determining Ideal Text Color Based on Specified Background Color
        ///     http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name="color">The bg.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color color)
        {
            const int nThreshold = 105;
            var bgDelta = Convert.ToInt32((color.R * 0.299) + (color.G * 0.587) + (color.B * 0.114));
            var foreColor = 255 - bgDelta < nThreshold
                ? Colors.Black
                : Colors.White;
            return foreColor;
        }


        private static generator.GeneratorParameters GetGeneratorParameters()
        {
            return JsonConvert.DeserializeObject<generator.GeneratorParameters>(GetGeneratorParametersJson());
        }

        private static string GetGeneratorParametersJson()
        {
            using (var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("MahApps.Metro.Styles.Themes.GeneratorParameters.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static ResourceDictionary GetThemeResourceDictionary(string baseColorScheme, Color accentBaseColor, Color? highlightColor = null, string AccentName = null)
        {
            var generatorParameters = GetGeneratorParameters();
            var themeTemplateContent = GetThemeTemplateContent();

            var variant = generatorParameters.BaseColorSchemes.First(x => x.Name == baseColorScheme);
            var colorScheme = new generator.ColorScheme();
            colorScheme.Name = AccentName;
            var values = colorScheme.Values;
            values.Add("AccentBaseColor", accentBaseColor.ToString());
            values.Add("AccentColor", Color.FromArgb(204, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("AccentColor2", Color.FromArgb(153, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("AccentColor3", Color.FromArgb(102, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("AccentColor4", Color.FromArgb(51, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());

            values.Add("HighlightColor", highlightColor != null ? highlightColor.ToString() : accentBaseColor.ToString());
            values.Add("IdealForegroundColor", IdealTextColor(accentBaseColor).ToString());

            // Strings
            values.Add("ColorScheme", AccentName);

            var xamlContent = new generator.ColorSchemeGenerator().GenerateColorSchemeFileContent(generatorParameters, variant, colorScheme, themeTemplateContent, $"{baseColorScheme}.{AccentName}", $"{AccentName} ({baseColorScheme})");

            var resourceDictionary = (ResourceDictionary)XamlReader.Parse(xamlContent);

            return resourceDictionary;
        }

    }
}
