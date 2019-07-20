// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that allows for the detection and alteration of a theme.
    /// </summary>
    public static partial class ThemeManager
    {
        // Note: add more checks if these keys aren't sufficient
        private static readonly List<string> styleKeys = new List<string>(new[]
                                                                          {
                                                                              "MahApps.Colors.Highlight",
                                                                              "MahApps.Colors.AccentBase",
                                                                              "MahApps.Brushes.Highlight",
                                                                              "MahApps.Brushes.AccentBase"
                                                                          });
    }
}