// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using JetBrains.Annotations;

    /// <summary>
    /// Represents the background theme of the application.
    /// </summary>
    [DebuggerDisplay("DisplayName={" + nameof(DisplayName) + "}, Name={" + nameof(Name) + "}, Source={Resources.Source}")]
    public class Theme
    {
        /// <summary>
        /// Gets the key for the theme name.
        /// </summary>
        public const string ThemeNameKey = "Theme.Name";

        /// <summary>
        /// Gets the key for the theme display name.
        /// </summary>
        public const string ThemeDisplayNameKey = "Theme.DisplayName";

        /// <summary>
        /// Gets the key for the theme base color scheme.
        /// </summary>
        public const string ThemeBaseColorSchemeKey = "Theme.BaseColorScheme";

        /// <summary>
        /// Gets the key for the theme color scheme.
        /// </summary>
        public const string ThemeColorSchemeKey = "Theme.ColorScheme";

        /// <summary>
        /// Gets the key for the theme showcase brush.
        /// </summary>
        public const string ThemeShowcaseBrushKey = "Theme.ShowcaseBrush";

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="resourceAddress">The URI of the theme ResourceDictionary.</param>
        public Theme([NotNull] Uri resourceAddress)
            : this(new ResourceDictionary { Source = resourceAddress })
        {
            if (resourceAddress == null)
            {
                throw new ArgumentNullException(nameof(resourceAddress));
            }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="resourceDictionary">The ResourceDictionary of the theme.</param>
        public Theme([NotNull] ResourceDictionary resourceDictionary)
        {
            this.Resources = resourceDictionary ?? throw new ArgumentNullException(nameof(resourceDictionary));

            this.Name = (string)this.Resources[ThemeNameKey];
            this.DisplayName = (string)this.Resources[ThemeDisplayNameKey];
            this.BaseColorScheme = (string)this.Resources[ThemeBaseColorSchemeKey];
            this.ColorScheme = (string)this.Resources[ThemeColorSchemeKey];
            this.ShowcaseBrush = (SolidColorBrush)this.Resources[ThemeShowcaseBrushKey];
        }

        /// <summary>
        /// The ResourceDictionary that represents this application theme.
        /// </summary>
        public ResourceDictionary Resources { get; }

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the display name of the theme.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Get the base color scheme for this theme.
        /// </summary>
        public string BaseColorScheme { get; }

        /// <summary>
        /// Gets the color scheme for this theme.
        /// </summary>
        public string ColorScheme { get; }

        /// <summary>
        /// Gets a brush which can be used to showcase this theme.
        /// </summary>
        public SolidColorBrush ShowcaseBrush { get; }
    }
}