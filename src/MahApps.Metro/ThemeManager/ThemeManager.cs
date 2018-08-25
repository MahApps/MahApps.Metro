// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security;
    using System.Windows;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    /// A class that allows for the detection and alteration of a theme.
    /// </summary>
    public static class ThemeManager
    {
        private const string BaseResourcePath = "pack://application:,,,/MahApps.Metro;component/Styles/Themes/";

        private const string BaseColorLight = "Light";
        private const string BaseColorDark = "Dark";

        private static IList<Theme> themes;

        /// <summary>
        /// Gets a list of all default themes.
        /// </summary>
        public static IEnumerable<Theme> Themes
        {
            get
            {
                if (themes != null)
                {
                    return themes;
                }

                var baseColors = new[] { BaseColorLight, BaseColorDark };
                var colors = new[]
                             {
                                 "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt",
                                 "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
                             };

                themes = new List<Theme>(baseColors.Length + colors.Length);

                try
                {
                    foreach (var baseColor in baseColors)
                    {
                        foreach (var color in colors)
                        {
                            var resourceAddress = new Uri($"{BaseResourcePath}{baseColor}.{color}.xaml");
                            themes.Add(new Theme(resourceAddress));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("This exception happens because you are maybe running that code out of the scope of a WPF application. Most likely because you are testing your configuration inside a unit test.", e);
                }

                return themes;
            }
        }

        /// <summary>
        /// Adds an theme.
        /// </summary>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddTheme([NotNull] Uri resourceAddress)
        {
            var theme = new Theme(resourceAddress);

            var themeExists = GetTheme(theme.Name) != null;
            if (themeExists)
            {
                return false;
            }

            themes.Add(new Theme(resourceAddress));
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

            var themeExists = GetTheme(theme.Name) != null;
            if (themeExists)
            {
                return false;
            }

            themes.Add(theme);
            return true;
        }

        /// <summary>
        /// Gets the <see cref="Theme"/> with the given name.
        /// </summary>
        /// <returns>The <see cref="Theme"/> or <c>null</c>, if the theme wasn't found</returns>
        public static Theme GetTheme([NotNull] string name)
        {
            if (name == null)
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
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            var builtInTheme = Themes.FirstOrDefault(x => AreResourceDictionarySourcesEqual(x.Resources, resources));
            if (builtInTheme != null)
            {
                return builtInTheme;
            }

            // support dynamically created runtime resource dictionaries
            if (resources.Source == null)
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
            if (theme == null)
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
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            // Note: add more checks if these keys aren't sufficient
            var styleKeys = new List<string>(new[]
                                             {
                                                 "HighlightColor",
                                                 "AccentBaseColor",
                                                 "HighlightBrush",
                                                 "AccentBaseColorBrush"
                                             });

            foreach (var styleKey in styleKeys)
            {
                // Note: do not use contains, because that will look in all merged dictionaries as well. We need to check
                // out the actual keys of the current resource dictionary
                if (!(from object resourceKey in resources.Keys
                     select resourceKey as string).Any(keyAsString => string.Equals(keyAsString, styleKey)))
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
            var appStyle = window != null
                               ? DetectTheme(window)
                               : DetectTheme(Application.Current);

            if (appStyle == null
                && window != null)
            {
                appStyle = DetectTheme(Application.Current); //no resources in the window's resources.
            }

            var resource = appStyle?.Resources[key];

            return resource;
        }

        /// <summary>
        /// Change the theme for the whole application.
        /// </summary>
        [SecurityCritical]
        public static void ChangeTheme([NotNull] Application app, [NotNull] string themeName)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (themeName == null)
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectTheme(app);
            Theme matched;
            if ((matched = GetTheme(themeName)) != null)
            {
                ChangeTheme(app.Resources, oldTheme, matched);
            }
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        [SecurityCritical]
        public static void ChangeTheme([NotNull] Window window, [NotNull] string themeName)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (themeName == null)
            {
                throw new ArgumentNullException(nameof(themeName));
            }

            var oldTheme = DetectTheme(window);
            Theme matched;
            if ((matched = GetTheme(themeName)) != null)
            {
                ChangeTheme(window.Resources, oldTheme, matched);
            }
        }

        /// <summary>
        /// Change theme for the whole application.
        /// </summary>
        /// <param name="app">The instance of Application to change.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme([NotNull] Application app, [NotNull] Theme newTheme)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectTheme(app);
            ChangeTheme(app.Resources, oldTheme, newTheme);
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme([NotNull] Window window, [NotNull] Theme newTheme)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            var oldTheme = DetectTheme(window);
            ChangeTheme(window.Resources, oldTheme, newTheme);
        }

        [SecurityCritical]
        private static void ChangeTheme(ResourceDictionary resources, Theme oldTheme, Theme newTheme)
        {
            var themeChanged = false;

            if (oldTheme != newTheme)
            {
                resources.BeginInit();

                ResourceDictionary oldThemeResource = null;
                if (oldTheme != null)
                {
                    oldThemeResource = resources.MergedDictionaries.FirstOrDefault(d => AreResourceDictionarySourcesEqual(d, oldTheme.Resources));
                }

                resources.MergedDictionaries.Add(newTheme.Resources);

                if (oldThemeResource != null)
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
        }

        /// <summary>
        /// Changes the theme of a ResourceDictionary directly.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="newTheme">The theme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static void ChangeTheme([NotNull] ResourceDictionary resources, [NotNull] Theme newTheme)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            if (newTheme == null)
            {
                throw new ArgumentNullException(nameof(newTheme));
            }

            ApplyResourceDictionary(newTheme.Resources, resources);
        }

        [SecurityCritical]
        public static void ChangeThemeBaseColor([NotNull] ResourceDictionary resources, string baseColor)
        {
            var currentTheme = DetectTheme(resources);

            if (currentTheme == null)
            {
                return;
            }

            var newTheme = Themes.FirstOrDefault(x => x.BaseColorScheme == baseColor && x.ColorScheme == currentTheme.ColorScheme);

            if (newTheme == null)
            {
                Trace.TraceError($"Could not find a theme with base color scheme '{baseColor}' and color scheme '{currentTheme.ColorScheme}'.");
                return;
            }

            ChangeTheme(resources, currentTheme, newTheme);
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
        /// Copies all resource keys from one resource to another.
        /// </summary>
        /// <param name="fromRD">The source resource dictionary.</param>
        /// <param name="toRD">The destination resource dictionary.</param>
        /// <exception cref="System.ArgumentNullException">
        /// fromRD
        /// or
        /// toRD
        /// </exception>
        internal static void CopyResource([NotNull] ResourceDictionary fromRD, [NotNull] ResourceDictionary toRD)
        {
            if (fromRD == null)
            {
                throw new ArgumentNullException(nameof(fromRD));
            }

            if (toRD == null)
            {
                throw new ArgumentNullException(nameof(toRD));
            }

            ApplyResourceDictionary(fromRD, toRD);
            foreach (var rd in fromRD.MergedDictionaries)
            {
                CopyResource(rd, toRD);
            }
        }

        /// <summary>
        /// Scans the window resources and returns it's theme.
        /// </summary>
        [CanBeNull]
        public static Theme DetectTheme()
        {
            var mainWindow = Application.Current?.MainWindow;

            if (mainWindow != null)
            {
                try
                {
                    var style = DetectTheme(mainWindow);

                    if (style != null)
                    {
                        return style;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Failed to detect app style on main window.{Environment.NewLine}{ex}");
                }
            }

            return DetectTheme(Application.Current);
        }

        /// <summary>
        /// Scans the window resources and returns it's theme.
        /// </summary>
        /// <param name="window">The Window to scan.</param>
        public static Theme DetectTheme([NotNull] Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var detectedStyle = DetectTheme(window.Resources);
            if (detectedStyle == null)
            {
                detectedStyle = DetectTheme(Application.Current.Resources);
            }

            return detectedStyle;
        }

        /// <summary>
        /// Scans the application resources and returns it's theme.
        /// </summary>
        /// <param name="app">The Application instance to scan.</param>
        [CanBeNull]
        public static Theme DetectTheme([NotNull] Application app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return DetectTheme(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's theme.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to check.</param>
        [CanBeNull]
        private static Theme DetectTheme([NotNull] ResourceDictionary resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            Theme currentTheme = null;

            if (DetectThemeFromResources(ref currentTheme, resources))
            {
                return currentTheme;
            }

            return null;
        }

        internal static bool DetectThemeFromAppResources(out Theme detectedTheme)
        {
            detectedTheme = null;

            return DetectThemeFromResources(ref detectedTheme, Application.Current.Resources);
        }

        private static bool DetectThemeFromResources(ref Theme detectedTheme, ResourceDictionary dict)
        {
            var enumerator = dict.MergedDictionaries.Reverse().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRd = enumerator.Current;

                if (currentRd == null)
                {
                    continue;
                }

                Theme matched;
                if ((matched = GetTheme(currentRd)) != null)
                {
                    detectedTheme = matched;
                    enumerator.Dispose();
                    return true;
                }

                if (DetectThemeFromResources(ref detectedTheme, currentRd))
                {
                    return true;
                }
            }

            enumerator.Dispose();
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
            if (first == null
                || second == null)
            {
                return false;
            }

            if (first.Source == null
                || second.Source == null)
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

            ChangeThemeBaseColor(Application.Current.Resources, baseColor);
        }

        private static bool isAutomaticWindowsAppModeSettingSyncEnabled;

        /// <summary>
        /// Gets or sets wether changes to the "app mode" setting from windows should be detected at runtime and the current <see cref="Theme"/> be changed accordingly.
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
                SyncThemeWithWindowsAppModeSetting();
            }
        }

        private static bool AppsUseLightTheme()
        {
            try
            {
                return Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", true));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }

            return true;
        }

        #endregion WindowsAppModeSetting
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
}