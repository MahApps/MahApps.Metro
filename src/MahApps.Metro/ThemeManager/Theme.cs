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
    [DebuggerDisplay("DisplayName={DisplayName}, Name={Name}, Source={Resources.Source}")]
    public class Theme
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="resourceAddress">The URI of the theme ResourceDictionary.</param>
        public Theme([NotNull] Uri resourceAddress)
        {
            if (resourceAddress == null)
            {
                throw new ArgumentNullException(nameof(resourceAddress));
            }

            this.Resources = new ResourceDictionary { Source = resourceAddress };
            this.Name = (string)this.Resources["Theme.Name"];
            this.DisplayName = (string)this.Resources["Theme.DisplayName"];
            this.BaseColorScheme = (string)this.Resources["Theme.BaseColorScheme"];
            this.ColorScheme = (string)this.Resources["Theme.ColorScheme"];
            this.ShowcaseBrush = (SolidColorBrush)this.Resources["Theme.ShowcaseBrush"];
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="resourceDictionary">The ResourceDictionary of the theme.</param>
        public Theme([NotNull] ResourceDictionary resourceDictionary)
        {
            this.Resources = resourceDictionary ?? throw new ArgumentNullException(nameof(resourceDictionary));
            this.Name = (string)this.Resources["Theme.Name"];
            this.DisplayName = (string)this.Resources["Theme.DisplayName"];
            this.BaseColorScheme = (string)this.Resources["Theme.BaseColorScheme"];
            this.ColorScheme = (string)this.Resources["Theme.ColorScheme"];
            this.ShowcaseBrush = (SolidColorBrush)this.Resources["Theme.ShowcaseBrush"];
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