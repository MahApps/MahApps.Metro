using System.Security;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// A class that allows for the detection and alteration of a MetroWindow's theme and accent.
    /// </summary>
    public static class ThemeManager
    {
        private static IList<Accent> _accents;
        private static IList<AppTheme> _appThemes;

        /// <summary>
        /// Gets a list of all of default themes.
        /// </summary>
        public static IEnumerable<Accent> Accents
        {
            get
            {
                if (_accents != null)
                    return _accents;

                var colors = new[] {
                                       "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt",
                                       "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
                                   };

                _accents = new List<Accent>(colors.Length);

                foreach (var color in colors)
                {
                    _accents.Add(new Accent(color, new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", color))));
                }

                return _accents;
            }
        }

        /// <summary>
        /// Gets a list of all of default metro themes.
        /// </summary>
        public static IEnumerable<AppTheme> AppThemes
        {
            get
            {
                if (_appThemes != null)
                    return _appThemes;

                var themes = new[] { "BaseLight", "BaseDark" };

                _appThemes = new List<AppTheme>(themes.Length);

                foreach (var color in themes)
                {
                    Theme theme = color.ToLower().Contains("light") ? Theme.Light : Theme.Dark;
                    var appTheme = new AppTheme(color, new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", color)), theme);

                    _appThemes.Add(appTheme);
                }

                return _appThemes;
            }
        }

        /// <summary>
        /// Adds an accent with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="resourceAddress"></param>
        /// <returns>true if the accent does not exists and can be added.</returns>
        public static bool AddAccent(string name, Uri resourceAddress)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (resourceAddress == null) throw new ArgumentNullException("resourceAddress");

            var accentExists = GetAccent(name) != null;
            if (accentExists)
            {
                return false;
            }

            _accents.Add(new Accent(name, resourceAddress));
            return true;
        }

        /// <summary>
        /// Adds an app theme with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="resourceAddress"></param>
        /// <param name="theme"></param>
        /// <returns>true if the app theme does not exists and can be added.</returns>
        public static bool AddAppTheme(string name, Uri resourceAddress, Theme theme)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (resourceAddress == null) throw new ArgumentNullException("resourceAddress");

            var appThemeExists = GetAppTheme(name) != null;
            if (appThemeExists)
            {
                return false;
            }

            _appThemes.Add(new AppTheme(name, resourceAddress, theme));
            return true;
        }

        /// <summary>
        /// Gets app theme with the given name.
        /// </summary>
        /// <param name="appThemeName"></param>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme(string appThemeName)
        {
            if (appThemeName == null) throw new ArgumentNullException("appThemeName");

            return AppThemes.FirstOrDefault(x => x.Name == appThemeName);
        }

        /// <summary>
        /// Gets app theme with the given resource dictionary.
        /// </summary>
        /// <param name="resources"></param>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme(ResourceDictionary resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");
            
            return AppThemes.FirstOrDefault(x => x.Resources.Source == resources.Source);
        }

        /// <summary>
        /// Gets app theme with the given name and theme type (light or dark).
        /// </summary>
        /// <param name="appThemeName"></param>
        /// <param name="theme"></param>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme(string appThemeName, Theme theme)
        {
            if (appThemeName == null) throw new ArgumentNullException("appThemeName");

            appThemeName = appThemeName.ToLower().Replace("light", "").Replace("dark", "");
            return AppThemes.FirstOrDefault(x => x.Name.ToLower().Contains(appThemeName) && x.Theme == theme);
        }

        /// <summary>
        /// Gets app theme with the given app theme and theme type (light or dark).
        /// </summary>
        /// <param name="currentAppTheme"></param>
        /// <param name="theme"></param>
        /// <returns>AppTheme</returns>
        public static AppTheme GetAppTheme(AppTheme currentAppTheme, Theme theme)
        {
            return GetAppTheme(currentAppTheme.Name, theme);
        }

        /// <summary>
        /// Gets accent with the given name.
        /// </summary>
        /// <param name="accentName"></param>
        /// <returns>AppTheme</returns>
        public static Accent GetAccent(string accentName)
        {
            if (accentName == null) throw new ArgumentNullException("accentName");

            return Accents.FirstOrDefault(x => x.Name == accentName);
        }

        /// <summary>
        /// Gets accent with the given resource dictionary.
        /// </summary>
        /// <param name="resources"></param>
        /// <returns>Accent</returns>
        public static Accent GetAccent(ResourceDictionary resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            return Accents.FirstOrDefault(x => x.Resources.Source == resources.Source);
        }

        /// <summary>
        /// Change theme for the whole application.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="themeName"></param>
        [SecurityCritical]
        public static void ChangeAppTheme(Application app, string themeName)
        {
            if (app == null) throw new ArgumentNullException("app");
            if (themeName == null) throw new ArgumentNullException("themeName");

            var oldTheme = DetectAppTheme(app);
            AppTheme matched;
            if ((matched = GetAppTheme(themeName)) != null)
            {
                ChangeTheme(app.Resources, oldTheme, oldTheme.Item2, matched);
            }
        }

        /// <summary>
        /// Change theme for the given window.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="themeName"></param>
        [SecurityCritical]
        public static void ChangeAppTheme(Window window, string themeName)
        {
            if (window == null) throw new ArgumentNullException("window");
            if (themeName == null) throw new ArgumentNullException("themeName");

            var oldTheme = DetectAppTheme(window);
            AppTheme matched;
            if ((matched = GetAppTheme(themeName)) != null)
            {
                ChangeTheme(window.Resources, oldTheme, oldTheme.Item2, matched);
            }
        }

        /// <summary>
        /// Change accent and theme for the whole application.
        /// </summary>
        /// <param name="app">The instance of Application to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme(Application app, Accent newAccent, AppTheme newTheme)
        {
            if (app == null) throw new ArgumentNullException("app");

            var oldTheme = DetectAppTheme(app);
            ChangeTheme(app.Resources, oldTheme, newAccent, newTheme);
        }

        /// <summary>
        /// Change accent and theme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme(Window window, Accent newAccent, AppTheme newTheme)
        {
            if (window == null) throw new ArgumentNullException("window");

            var oldTheme = DetectAppTheme(window);
            ChangeTheme(window.Resources, oldTheme, newAccent, newTheme);
        }

        [SecurityCritical]
        private static void ChangeTheme(ResourceDictionary resources, Tuple<AppTheme, Accent> oldThemeInfo, Accent newAccent, AppTheme newTheme)
        {
            var themeChanged = false;
            if (oldThemeInfo != null)
            {
                var oldAccent = oldThemeInfo.Item2;
                if (oldAccent != null && oldAccent.Name != newAccent.Name)
                {
                    var oldAccentResource = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldAccent.Resources.Source);
                    if (oldAccentResource != null)
                    {
                        resources.MergedDictionaries.Add(newAccent.Resources);
                        var ok = resources.MergedDictionaries.Remove(oldAccentResource);

                        themeChanged = true;
                    }
                }

                var oldTheme = oldThemeInfo.Item1;
                if (oldTheme != null && oldTheme != newTheme)
                {
                    var oldThemeResource = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldTheme.Resources.Source);
                    if (oldThemeResource != null)
                    {
                        resources.MergedDictionaries.Add(newTheme.Resources);
                        var ok = resources.MergedDictionaries.Remove(oldThemeResource);

                        themeChanged = true;
                    }
                }
            }
            else
            {
                ChangeTheme(resources, newAccent, newTheme);

                themeChanged = true;
            }

            if (themeChanged)
            {
                OnThemeChanged(newAccent, newTheme);
            }
        }

        /// <summary>
        /// Changes the theme of a ResourceDictionary directly.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to modify.</param>
        /// <param name="newAccent">The accent to apply to the ResourceDictionary.</param>
        /// <param name="newTheme">The theme to apply to the ResourceDictionary.</param>
        [SecurityCritical]
        public static void ChangeTheme(ResourceDictionary resources, Accent newAccent, AppTheme newTheme)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            ApplyResourceDictionary(newAccent.Resources, resources);
            ApplyResourceDictionary(newTheme.Resources, resources);
        }
        [SecurityCritical]

        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            oldRd.BeginInit();

            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                    oldRd.Remove(r.Key);

                oldRd.Add(r.Key, r.Value);
            }

            oldRd.EndInit();
        }

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        public static Tuple<AppTheme, Accent> DetectAppTheme()
        {
            try
            {
                return DetectAppTheme(Application.Current.MainWindow);
            }
            catch (Exception)
            {
                return DetectAppTheme(Application.Current);
            }
        }

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        /// <param name="window">The Window to scan.</param>
        public static Tuple<AppTheme, Accent> DetectAppTheme(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");

            return DetectAppTheme(window.Resources);
        }

        /// <summary>
        /// Scans the application resources and returns it's accent and theme.
        /// </summary>
        /// <param name="app">The Application instance to scan.</param>
        public static Tuple<AppTheme, Accent> DetectAppTheme(Application app)
        {
            if (app == null) throw new ArgumentNullException("app");

            return DetectAppTheme(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's accent and theme.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to check.</param>
        private static Tuple<AppTheme, Accent> DetectAppTheme(ResourceDictionary resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            AppTheme currentTheme = null;
            Tuple<AppTheme, Accent> detectedAccentTheme = null;


            if (DetectThemeFromResources(ref currentTheme, resources))
            {
                if (GetThemeFromResources(currentTheme, resources, ref detectedAccentTheme))
                    return new Tuple<AppTheme, Accent>(detectedAccentTheme.Item1, detectedAccentTheme.Item2);
            }

            return null;
        }

        internal static bool DetectThemeFromAppResources(out AppTheme detectedTheme)
        {
            detectedTheme = null;

            return DetectThemeFromResources(ref detectedTheme, Application.Current.Resources);
        }

        private static bool DetectThemeFromResources(ref AppTheme detectedTheme, ResourceDictionary dict)
        {
            var enumerator = dict.MergedDictionaries.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRd = enumerator.Current;

                AppTheme matched = null;
                if ((matched = GetAppTheme(currentRd)) != null)
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

        internal static bool GetThemeFromResources(AppTheme presetTheme, ResourceDictionary dict, ref Tuple<AppTheme, Accent> detectedAccentTheme)
        {
            AppTheme currentTheme = presetTheme;

            Accent matched = null;
            if ((matched = GetAccent(dict)) != null)
            {
                detectedAccentTheme = Tuple.Create<AppTheme, Accent>(currentTheme, matched);
                return true;
            }

            foreach (ResourceDictionary rd in dict.MergedDictionaries)
            {
                if (GetThemeFromResources(presetTheme, rd, ref detectedAccentTheme))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// This event fires if accent color and theme was changed
        /// this should be using the weak event pattern, but for now it's enough
        /// </summary>
        public static event EventHandler<OnThemeChangedEventArgs> IsThemeChanged;

        /// <summary>
        /// Invalidates global colors and resources.
        /// Sometimes the ContextMenu is not changing the colors, so this will fix it.
        /// </summary>
        [SecurityCritical]
        private static void OnThemeChanged(Accent newAccent, AppTheme newTheme)
        {
            SafeRaise.Raise(IsThemeChanged, Application.Current, new OnThemeChangedEventArgs() { AppTheme = newTheme, Accent = newAccent });
        }

        #region obsoletes

        [Obsolete("This will be deleted in next release.")]
        public static IList<Accent> DefaultAccents
        {
            get
            {
                if (_accents != null)
                    return _accents;

                var colors = new[] {
                                       "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt",
                                       "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
                                   };

                _accents = new List<Accent>(colors.Length);

                foreach (var color in colors)
                {
                    _accents.Add(new Accent(color, new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", color))));
                }

                return _accents;
            }
        }

        [Obsolete("This will be deleted in next release.")]
        public static IList<AppTheme> DefaultAppThemes
        {
            get
            {
                if (_appThemes != null)
                    return _appThemes;

                var themes = new[] { "BaseLight", "BaseDark" };

                _appThemes = new List<AppTheme>(themes.Length);

                foreach (var color in themes)
                {
                    Theme theme = color.ToLower().Contains("light") ? Theme.Light : Theme.Dark;
                    var appTheme = new AppTheme(color, new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", color)), theme);
                    _appThemes.Add(appTheme);
                }

                return _appThemes;
            }
        }

        [SecurityCritical]
        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static void ChangeTheme(Application app, Accent newAccent, Theme newTheme)
        {
            if (app == null) throw new ArgumentNullException("app");

            var oldTheme = DetectTheme(app);
            AppTheme oldAppTheme = AppThemes.First(x => x.Theme == oldTheme.Item1);
            AppTheme newAppTheme = AppThemes.First(x => x.Theme == newTheme);

            ChangeTheme(app.Resources, Tuple.Create(oldAppTheme, oldTheme.Item2), newAccent, newAppTheme);
        }

        [SecurityCritical]
        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static void ChangeTheme(Window window, Accent newAccent, Theme newTheme)
        {
            if (window == null) throw new ArgumentNullException("window");

            var oldTheme = DetectTheme(window);
            AppTheme oldAppTheme = AppThemes.First(x => x.Theme == oldTheme.Item1);
            AppTheme newAppTheme = AppThemes.First(x => x.Theme == newTheme);

            ChangeTheme(window.Resources, Tuple.Create(oldAppTheme, oldTheme.Item2), newAccent, newAppTheme);
        }

        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static void ChangeTheme(ResourceDictionary resources, Accent newAccent, Theme newTheme)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            AppTheme appTheme = AppThemes.First(x => x.Theme == newTheme);

            ChangeTheme(resources, newAccent, appTheme);
        }

        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static Tuple<Theme, Accent> DetectTheme()
        {
            try
            {
                return DetectTheme(Application.Current.MainWindow);
            }
            catch (Exception)
            {
                return DetectTheme(Application.Current);
            }
        }

        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static Tuple<Theme, Accent> DetectTheme(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");

            var lookAndFeel = DetectAppTheme(window);

            return Tuple.Create(lookAndFeel.Item1.Theme, lookAndFeel.Item2);
        }

        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public static Tuple<Theme, Accent> DetectTheme(Application app)
        {
            if (app == null) throw new ArgumentNullException("app");

            var lookAndFeel = DetectAppTheme(app);

            return Tuple.Create(lookAndFeel.Item1.Theme, lookAndFeel.Item2);
        }


        [Obsolete("This will be deleted in next release.")]
        public static bool InvalidateSystemResourcesOnBackgroundThread { get; set; }

        #endregion
    }

    public class OnThemeChangedEventArgs : EventArgs
    {
        public AppTheme AppTheme { get; set; }
        [Obsolete("This will be deleted in next release. ThemeManager provides now a class called AppTheme to handle custome app themes!")]
        public Theme Theme { get; set; }
        public Accent Accent { get; set; }
    }
}