// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    /// A class that allows for the detection and alteration of a theme.
    /// </summary>
    public static partial class ThemeManager
    {
        /// <summary>
        /// Gets the name for the light base color.
        /// </summary>
        public static readonly string BaseColorLight = "Light";

        /// <summary>
        /// Gets the name for the dark base color.
        /// </summary>
        public static readonly string BaseColorDark = "Dark";

        private static readonly ObservableCollection<Theme> themesInternal;
        private static readonly ReadOnlyObservableCollection<Theme> themes;

        private static readonly ObservableCollection<string> baseColorsInternal;
        private static readonly ReadOnlyObservableCollection<string> baseColors;

        private static readonly ObservableCollection<ColorScheme> colorSchemesInternal;
        private static readonly ReadOnlyObservableCollection<ColorScheme> colorSchemes;

        static ThemeManager()
        {
            {
                themesInternal = new ObservableCollection<Theme>();
                themes = new ReadOnlyObservableCollection<Theme>(themesInternal);

                var collectionView = CollectionViewSource.GetDefaultView(themes);
                collectionView.SortDescriptions.Add(new SortDescription(nameof(Theme.DisplayName), ListSortDirection.Ascending));

                themesInternal.CollectionChanged += ThemesInternalCollectionChanged;
            }

            {
                baseColorsInternal = new ObservableCollection<string>();
                baseColors = new ReadOnlyObservableCollection<string>(baseColorsInternal);

                var collectionView = CollectionViewSource.GetDefaultView(baseColors);
                collectionView.SortDescriptions.Add(new SortDescription(null, ListSortDirection.Ascending));
            }

            {
                colorSchemesInternal = new ObservableCollection<ColorScheme>();
                colorSchemes = new ReadOnlyObservableCollection<ColorScheme>(colorSchemesInternal);

                var collectionView = CollectionViewSource.GetDefaultView(colorSchemes);
                collectionView.SortDescriptions.Add(new SortDescription(nameof(ColorScheme.Name), ListSortDirection.Ascending));
            }
        }

        /// <summary>
        /// Gets a list of all themes.
        /// </summary>
        public static ReadOnlyObservableCollection<Theme> Themes
        {
            get
            {
                EnsureThemes();

                return themes;
            }
        }

        /// <summary>
        /// Gets a list of all available base colors.
        /// </summary>
        public static ReadOnlyObservableCollection<string> BaseColors
        {
            get
            {
                EnsureThemes(); 

                return baseColors; 
            }
        }

        /// <summary>
        /// Gets a list of all available color schemes.
        /// </summary>
        public static ReadOnlyObservableCollection<ColorScheme> ColorSchemes
        {
            get 
            { 
                EnsureThemes(); 

                return colorSchemes; 
            }
        }

        private static void EnsureThemes()
        {
            if (themes.Count > 0)
            {
                return;
            }

            try
            {
                var assembly = typeof(ThemeManager).Assembly;
                var assemblyName = assembly.GetName().Name;
                var resourceNames = assembly.GetManifestResourceNames();

                foreach (var resourceName in resourceNames)
                {
                    if (resourceName.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        continue;
                    }

                    var info = assembly.GetManifestResourceInfo(resourceName);
                    if (info.IsNull()
                        || info.ResourceLocation == ResourceLocation.ContainedInAnotherAssembly)
                    {
                        continue;
                    }

                    var resourceStream = assembly.GetManifestResourceStream(resourceName);

                    if (resourceStream.IsNull())
                    {
                        continue;
                    }

                    using (var reader = new ResourceReader(resourceStream))
                    {
                        foreach (DictionaryEntry entry in reader)
                        {
                            var stringKey = entry.Key as string;
                            if (stringKey.IsNull()
                                || stringKey.IndexOf("/themes/", StringComparison.OrdinalIgnoreCase) == -1
                                || stringKey.EndsWith(".baml", StringComparison.OrdinalIgnoreCase) == false
                                || stringKey.EndsWith("generic.baml", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            var resourceDictionary = new ResourceDictionary
                            {
                                Source = new Uri($"pack://application:,,,/{assemblyName};component/{stringKey.Replace(".baml", ".xaml")}")
                            };

                            if (resourceDictionary.MergedDictionaries.Count == 0
                                && resourceDictionary.Contains(Theme.ThemeNameKey))
                            {
                                themesInternal.Add(new Theme(resourceDictionary));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("This exception happens because you are maybe running that code out of the scope of a WPF application. Most likely because you are testing your configuration inside a unit test.", e);
            }
        }

        private static void ThemesInternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems.OfType<Theme>())
                    {
                        if (baseColorsInternal.Contains(newItem.BaseColorScheme) == false)
                        {
                            baseColorsInternal.Add(newItem.BaseColorScheme);
                        }

                        if (colorSchemesInternal.Any(x => x.Name == newItem.ColorScheme) == false)
                        {
                            colorSchemesInternal.Add(new ColorScheme(newItem));
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var newItem in e.OldItems.OfType<Theme>())
                    {
                        baseColorsInternal.Remove(newItem.BaseColorScheme);

                        var colorScheme = colorSchemesInternal.FirstOrDefault(x => x.Name == newItem.ColorScheme);
                        if (colorScheme != null)
                        {
                            colorSchemesInternal.Remove(colorScheme);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    baseColorsInternal.Clear();
                    colorSchemesInternal.Clear();
                    break;
            }
        }

        /// <summary>
        /// Clears the internal themes list.
        /// </summary>
        public static void ClearThemes()
        {
            themesInternal?.Clear();
        }

        /// <summary>
        /// Adds an theme.
        /// </summary>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddTheme([NotNull] Uri resourceAddress)
        {
            var theme = new Theme(resourceAddress);

            var themeExists = GetTheme(theme.Name).IsNotNull();
            if (themeExists)
            {
                return false;
            }

            themesInternal.Add(new Theme(resourceAddress));
            return true;
        }

        /// <summary>
        /// Adds an theme.
        /// </summary>
        /// <param name="resourceDictionary">The ResourceDictionary of the theme.</param>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddTheme([NotNull] ResourceDictionary resourceDictionary)
        {
            var theme = new Theme(resourceDictionary);

            var themeExists = GetTheme(theme.Name).IsNotNull();
            if (themeExists)
            {
                return false;
            }

            themesInternal.Add(theme);
            return true;
        }

        /// <summary>
        /// Gets the <see cref="Theme"/> with the given name.
        /// </summary>
        /// <returns>The <see cref="Theme"/> or <c>null</c>, if the theme wasn't found</returns>
        public static Theme GetTheme([NotNull] string name)
        {
            if (name.IsNull())
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Themes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the <see cref="Theme"/> with the given resource dictionary.
        /// </summary>
        /// <param name="resources"><see cref="ResourceDictionary"/> from which the theme should be retrieved.</param>
        /// <returns>The <see cref="Theme"/> or <c>null</c>, if the theme wasn't found.</returns>
        public static Theme GetTheme([NotNull] ResourceDictionary resources)
        {
            if (resources.IsNull())
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var builtInTheme = Themes.FirstOrDefault(x => AreResourceDictionarySourcesEqual(x.Resources, resources));
            if (builtInTheme.IsNotNull())
            {
                return builtInTheme;
            }

            // support dynamically created runtime resource dictionaries
            if (resources.Source.IsNull())
            {
                if (IsThemeDictionary(resources))
                {
                    return new Theme(resources);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the inverse <see cref="Theme" /> of the given <see cref="Theme"/>.
        /// This method relies on the "Dark" or "Light" affix to be present.
        /// </summary>
        /// <param name="theme">The app theme.</param>
        /// <returns>The inverse <see cref="Theme"/> or <c>null</c> if it couldn't be found.</returns>
        /// <remarks>
        /// Returns BaseLight, if BaseDark is given or vice versa.
        /// Custom Themes must end with "Dark" or "Light" for this to work, for example "CustomDark" and "CustomLight".
        /// </remarks>
        public static Theme GetInverseTheme([NotNull] Theme theme)
        {
            if (theme.IsNull())
            {
                throw new ArgumentNullException(nameof(theme));
            }

            if (theme.Name.StartsWith("dark.", StringComparison.OrdinalIgnoreCase))
            {
                return GetTheme("Light." + theme.Name.Substring("dark.".Length));
            }

            if (theme.Name.StartsWith("light.", StringComparison.OrdinalIgnoreCase))
            {
                return GetTheme("Dark." + theme.Name.Substring("light.".Length));
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified resource dictionary represents an <see cref="Theme"/>.
        /// <para />
        /// This might include runtime themes which do not have a resource uri.
        /// </summary>
        /// <param name="resources">The resources.</param>
        /// <returns><c>true</c> if the resource dictionary is an <see cref="Theme"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">resources</exception>
        public static bool IsThemeDictionary([NotNull] ResourceDictionary resources)
        {
            if (resources.IsNull())
            {
                throw new ArgumentNullException(nameof(resources));
            }

            foreach (var styleKey in styleKeys)
            {
                // Note: do not use contains, because that will look in all merged dictionaries as well. We need to check
                // out the actual keys of the current resource dictionary
                if (!(from object resourceKey in resources.Keys
                      select resourceKey as string).Any(keyAsString => string.Equals(keyAsString, styleKey, StringComparison.Ordinal)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a resource from the detected AppStyle.
        /// </summary>
        /// <param name="window">The window to check. If this is null, the Application's sources will be checked.</param>
        /// <param name="key">The key to check against.</param>
        /// <returns>The resource object or null, if the resource wasn't found.</returns>
        public static object GetResourceFromAppStyle(Window window, string key)
        {
            var appStyle = (window.IsNull()
                                ? DetectTheme(Application.Current)
                                : DetectTheme(window))
                           ?? DetectTheme(Application.Current);

            var resource = appStyle?.Resources[key];

            return resource;
        }

        /// <summary>
        /// Change the theme for the whole application.
        /// </summary>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Application app, [NotNull] string themeName)
        {
            if (app.IsNull())
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (themeName.IsNull())
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectTheme(app);
            Theme matched;
            if ((matched = GetTheme(themeName)).IsNotNull())
            {
                return ChangeTheme(app.Resources, oldTheme, matched);
            }

            return oldTheme;
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Window window, [NotNull] string themeName)
        {
            if (window.IsNull())
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (themeName.IsNull())
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectTheme(window);
            Theme matched;
            if ((matched = GetTheme(themeName)).IsNotNull())
            {
                return ChangeTheme(window.Resources, oldTheme, matched);
            }

            return oldTheme;
        }

        /// <summary>
        /// Change theme for the whole application.
        /// </summary>
        /// <param name="app">The instance of Application to change.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Application app, [NotNull] Theme newTheme)
        {
            if (app.IsNull())
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (newTheme.IsNull())
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectTheme(app);
            return ChangeTheme(app.Resources, oldTheme, newTheme);
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Window window, [NotNull] Theme newTheme)
        {
            if (window.IsNull())
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (newTheme.IsNull())
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectTheme(window);
            return ChangeTheme(window.Resources, oldTheme, newTheme);
        }

        /// <summary>
        /// Change theme for the given ResourceDictionary.
        /// </summary>
        /// <param name="resourceDictionary">The Window to change.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] ResourceDictionary resourceDictionary, [NotNull] Theme newTheme)
        {
            if (resourceDictionary.IsNull())
            {
                throw new ArgumentNullException(nameof(resourceDictionary));
            }

            if (newTheme.IsNull())
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectTheme(resourceDictionary);
            return ChangeTheme(resourceDictionary, oldTheme, newTheme);
        }

        [SecurityCritical]
        private static Theme ChangeTheme(ResourceDictionary resources, Theme oldTheme, Theme newTheme)
        {
            var themeChanged = false;

            if (oldTheme != newTheme)
            {
                resources.BeginInit();

                ResourceDictionary oldThemeResource = null;
                if (oldTheme.IsNotNull())
                {
                    oldThemeResource = resources.MergedDictionaries.FirstOrDefault(d => AreResourceDictionarySourcesEqual(d, oldTheme.Resources));
                }

                resources.MergedDictionaries.Add(newTheme.Resources);

                if (oldThemeResource.IsNotNull())
                {
                    resources.MergedDictionaries.Remove(oldThemeResource);
                }

                themeChanged = true;
                resources.EndInit();
            }

            if (themeChanged)
            {
                OnThemeChanged(newTheme);
            }

            return newTheme;
        }

        /// <summary>
        /// Change base color and color scheme of for the given application.
        /// </summary>
        /// <param name="app">The application to modify.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Application app, [NotNull] string baseColor, [NotNull] string colorScheme)
        {
            var currentTheme = DetectTheme(app);

            if (currentTheme.IsNull())
            {
                return null;
            }

            var newTheme = Themes.FirstOrDefault(x => x.BaseColorScheme == baseColor && x.ColorScheme == colorScheme);

            if (newTheme.IsNull())
            {
                Trace.TraceError($"Could not find a theme with base color scheme '{baseColor}' and color scheme '{currentTheme.ColorScheme}'.");
                return null;
            }

            return ChangeTheme(app.Resources, currentTheme, newTheme);
        }

        /// <summary>
        /// Change base color and color scheme of for the given window.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] Window window, [NotNull] string baseColor, [NotNull] string colorScheme)
        {
            var currentTheme = DetectTheme(window);

            if (currentTheme.IsNull())
            {
                return null;
            }

            var newTheme = Themes.FirstOrDefault(x => x.BaseColorScheme == baseColor && x.ColorScheme == colorScheme);

            if (newTheme.IsNull())
            {
                Trace.TraceError($"Could not find a theme with base color scheme '{baseColor}' and color scheme '{currentTheme.ColorScheme}'.");
                return null;
            }

            return ChangeTheme(window.Resources, currentTheme, newTheme);
        }

        /// <summary>
        /// Change base color and color scheme of for the given ResourceDictionary.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="oldTheme">The old/current theme.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeTheme([NotNull] ResourceDictionary resources, Theme oldTheme, [NotNull] string baseColor, [NotNull] string colorScheme)
        {
            var newTheme = Themes.FirstOrDefault(x => x.BaseColorScheme == baseColor && x.ColorScheme == colorScheme);

            if (newTheme.IsNull())
            {
                Trace.TraceError($"Could not find a theme with base color scheme '{baseColor}' and color scheme '{oldTheme.ColorScheme}'.");
                return null;
            }

            return ChangeTheme(resources, oldTheme, newTheme);
        }

        /// <summary>
        /// Change base color for the given application.
        /// </summary>
        /// <param name="app">The application to change.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeBaseColor([NotNull] Application app, [NotNull] string baseColor)
        {
            var currentTheme = DetectTheme(app);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(app.Resources, currentTheme, baseColor, currentTheme.ColorScheme);
        }

        /// <summary>
        /// Change base color for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeBaseColor([NotNull] Window window, [NotNull] string baseColor)
        {
            var currentTheme = DetectTheme(window);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(window.Resources, currentTheme, baseColor, currentTheme.ColorScheme);
        }

        /// <summary>
        /// Change base color of for the given ResourceDictionary.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="oldTheme">The old/current theme.</param>
        /// <param name="baseColor">The base color to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeBaseColor([NotNull] ResourceDictionary resources, [CanBeNull] Theme oldTheme, [NotNull] string baseColor)
        {
            var currentTheme = oldTheme ?? DetectTheme(resources);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(resources, currentTheme, baseColor, currentTheme.ColorScheme);
        }

        /// <summary>
        /// Change color scheme for the given application.
        /// </summary>
        /// <param name="app">The application to change.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeColorScheme([NotNull] Application app, [NotNull] string colorScheme)
        {
            var currentTheme = DetectTheme(app);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(app.Resources, currentTheme, currentTheme.BaseColorScheme, colorScheme);
        }

        /// <summary>
        /// Change color scheme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeColorScheme([NotNull] Window window, [NotNull] string colorScheme)
        {
            var currentTheme = DetectTheme(window);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(window.Resources, currentTheme, currentTheme.BaseColorScheme, colorScheme);
        }

        /// <summary>
        /// Change color scheme for the given ResourceDictionary.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="oldTheme">The old/current theme.</param>
        /// <param name="colorScheme">The color scheme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static Theme ChangeThemeColorScheme([NotNull] ResourceDictionary resources, [CanBeNull] Theme oldTheme, [NotNull] string colorScheme)
        {
            var currentTheme = oldTheme ?? DetectTheme(resources);

            if (currentTheme.IsNull())
            {
                return null;
            }

            return ChangeTheme(resources, currentTheme, currentTheme.BaseColorScheme, colorScheme);
        }

        /// <summary>
        /// Changes the theme of a ResourceDictionary directly.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="newTheme">The theme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static void ApplyThemeResourcesFromTheme([NotNull] ResourceDictionary resources, [NotNull] Theme newTheme)
        {
            if (resources.IsNull())
            {
                throw new ArgumentNullException(nameof(resources));
            }

            if (newTheme.IsNull())
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            ApplyResourceDictionary(newTheme.Resources, resources);
        }

        [SecurityCritical]
        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            oldRd.BeginInit();

            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                {
                    oldRd.Remove(r.Key);
                }

                oldRd.Add(r.Key, r.Value);
            }

            oldRd.EndInit();
        }

        /// <summary>
        /// Scans the window resources and returns it's theme.
        /// </summary>
        /// <remarks>If the theme can't be detected from the <see cref="Application.MainWindow"/> we try to detect it from <see cref="Application.Current"/>.</remarks>
        [CanBeNull]
        public static Theme DetectTheme()
        {
            var mainWindow = Application.Current?.MainWindow;

            if (mainWindow.IsNotNull())
            {
                try
                {
                    var style = DetectTheme(mainWindow);

                    if (style.IsNotNull())
                    {
                        return style;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Failed to detect app style on main window.{Environment.NewLine}{ex}");
                }
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return DetectTheme(Application.Current);
        }

        /// <summary>
        /// Scans the window resources and returns it's theme.
        /// </summary>
        /// <param name="window">The Window to scan.</param>
        /// <remarks>If the theme can't be detected from the <paramref name="window"/> we try to detect it from <see cref="Application.Current"/>.</remarks>
        [CanBeNull]
        public static Theme DetectTheme([NotNull] Window window)
        {
            if (window.IsNull())
            {
                throw new ArgumentNullException(nameof(window));
            }

            var detectedStyle = DetectTheme(window.Resources)
                                ?? DetectTheme(Application.Current.Resources);

            return detectedStyle;
        }

        /// <summary>
        /// Scans the application resources and returns it's theme.
        /// </summary>
        /// <param name="app">The Application instance to scan.</param>
        [CanBeNull]
        public static Theme DetectTheme([NotNull] Application app)
        {
            if (app.IsNull())
            {
                throw new ArgumentNullException(nameof(app));
            }

            return DetectTheme(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's theme.
        /// </summary>
        /// <param name="resourceDictionary">The ResourceDictionary to scan.</param>
        [CanBeNull]
        public static Theme DetectTheme([NotNull] ResourceDictionary resourceDictionary)
        {
            if (resourceDictionary.IsNull())
            {
                throw new ArgumentNullException(nameof(resourceDictionary));
            }

            if (DetectThemeFromResources(out var currentTheme, resourceDictionary))
            {
                return currentTheme;
            }

            return null;
        }

        private static bool DetectThemeFromResources(out Theme detectedTheme, ResourceDictionary dict)
        {
            using (var enumerator = dict.MergedDictionaries.Reverse().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var currentRd = enumerator.Current;

                    if (currentRd.IsNull())
                    {
                        continue;
                    }

                    Theme matched;
                    if ((matched = GetTheme(currentRd)).IsNotNull())
                    {
                        detectedTheme = matched;
                        return true;
                    }

                    if (DetectThemeFromResources(out detectedTheme, currentRd))
                    {
                        return true;
                    }
                }
            }

            detectedTheme = null;
            return false;
        }

        /// <summary>
        /// This event fires if the theme was changed
        /// this should be using the weak event pattern, but for now it's enough
        /// </summary>
        public static event EventHandler<OnThemeChangedEventArgs> IsThemeChanged;

        /// <summary>
        /// Invalidates global colors and resources.
        /// Sometimes the ContextMenu is not changing the colors, so this will fix it.
        /// </summary>
        [SecurityCritical]
        private static void OnThemeChanged(Theme newTheme)
        {
            IsThemeChanged?.Invoke(Application.Current, new OnThemeChangedEventArgs(newTheme));
        }

        private static bool AreResourceDictionarySourcesEqual(ResourceDictionary first, ResourceDictionary second)
        {
            if (first.IsNull()
                || second.IsNull())
            {
                return false;
            }

            if (first.Source.IsNull()
                || second.Source.IsNull())
            {
                try
                {
                    foreach (var key in first.Keys)
                    {
                        var isTheSame = second.Contains(key)
                                        && Equals(first[key], second[key]);
                        if (!isTheSame)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"Could not compare resource dictionaries: {exception} {Environment.NewLine} {exception.StackTrace}");
                    return false;
                }

                return true;
            }

            return Uri.Compare(first.Source, second.Source, UriComponents.Host | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

#region WindowsAppModeSetting

        /// <summary>
        /// Synchronizes the current <see cref="Theme"/> with the "app mode" setting from windows.
        /// </summary>
        public static void SyncThemeWithWindowsAppModeSetting()
        {
            var baseColor = AppsUseLightTheme()
                               ? BaseColorLight
                               : BaseColorDark;

            ChangeThemeBaseColor(Application.Current, baseColor);
        }

        private static bool isAutomaticWindowsAppModeSettingSyncEnabled;

        /// <summary>
        /// Gets or sets whether changes to the "app mode" setting from windows should be detected at runtime and the current <see cref="Theme"/> be changed accordingly.
        /// </summary>
        public static bool IsAutomaticWindowsAppModeSettingSyncEnabled
        {
            get { return isAutomaticWindowsAppModeSettingSyncEnabled; }

            set
            {
                if (value == isAutomaticWindowsAppModeSettingSyncEnabled)
                {
                    return;
                }

                isAutomaticWindowsAppModeSettingSyncEnabled = value;

                if (isAutomaticWindowsAppModeSettingSyncEnabled)
                {
                    SystemEvents.UserPreferenceChanged += HandleUserPreferenceChanged;
                }
                else
                {
                    SystemEvents.UserPreferenceChanged -= HandleUserPreferenceChanged;
                }
            }
        }

        private static void HandleUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                if (Application.Current.IsNull())
                {
#if DEBUG
                    Trace.TraceWarning("ThemeManager (UserPreferenceChanged): Can not sync theme with windows app mode settings, because the current application is NULL!");
#endif
                    return;
                }

                SyncThemeWithWindowsAppModeSetting();
            }
        }

        private static bool AppsUseLightTheme()
        {
            try
            {
                var registryValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", true);
                return registryValue.IsNull()
                       || Convert.ToBoolean(registryValue);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }

            return true;
        }

#endregion WindowsAppModeSetting

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        [ContractAnnotation("obj:null => true")]
        private static bool IsNull(this object obj)
        {
            return obj is null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        [ContractAnnotation("obj:null => false")]
        private static bool IsNotNull(this object obj)
        {
            return obj is null == false;
        }
    }

    /// <summary>
    /// Class which is used as argument for an event to signal theme changes.
    /// </summary>
    public class OnThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public OnThemeChangedEventArgs(Theme theme)
        {
            this.Theme = theme;
        }

        /// <summary>
        /// The new theme.
        /// </summary>
        public Theme Theme { get; set; }
    }

    /// <summary>
    /// Helper class for displaying color schemes.
    /// </summary>
    [DebuggerDisplay("Name={" + nameof(Name) + "}")]
    public class ColorScheme
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ColorScheme(Theme theme)
            : this(theme.ColorScheme, theme.ShowcaseBrush)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ColorScheme(string name, Brush showcaseBrush)
        {
            this.Name = name;
            this.ShowcaseBrush = showcaseBrush;
        }

        /// <summary>
        /// Gets the name for this color scheme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the showcase brush for this color scheme.
        /// </summary>
        public Brush ShowcaseBrush { get; }
    }
}