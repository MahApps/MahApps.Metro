namespace MahApps.Metro.Theming
{
    using System.Collections.Generic;
    using ControlzEx.Theming;

    /// <summary>
    /// Provides theme resources from Fluent.Ribbon.
    /// </summary>
    public class MahAppsLibraryThemeProvider : LibraryThemeProvider
    {
        public static readonly MahAppsLibraryThemeProvider DefaultInstance = new MahAppsLibraryThemeProvider();

        /// <inheritdoc cref="LibraryThemeProvider" />
        public MahAppsLibraryThemeProvider()
            : base(true)
        {
        }

        public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
        {
            values.Add("MahApps.Colors.AccentBase", colorValues.AccentBaseColor.ToString());
            values.Add("MahApps.Colors.Accent", colorValues.AccentColor80.ToString());
            values.Add("MahApps.Colors.Accent2", colorValues.AccentColor60.ToString());
            values.Add("MahApps.Colors.Accent3", colorValues.AccentColor40.ToString());
            values.Add("MahApps.Colors.Accent4", colorValues.AccentColor20.ToString());

            values.Add("MahApps.Colors.Highlight", colorValues.HighlightColor.ToString());
            values.Add("MahApps.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString());            
        }
    }
}