using System.Reflection;
using System.Security;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MahApps.Metro
{
    using System.Threading.Tasks;

    /// <summary>
    /// A class that allows for the detection and alteration of a MetroWindow's theme and accent.
    /// </summary>
    public static class ThemeManager
    {
        internal static readonly ResourceDictionary LightResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") };
        internal static readonly ResourceDictionary DarkResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml") };

        private static IList<Accent> _accents;

        /// <summary>
        /// Cached method info's for use in OnThemeChanged
        /// </summary>
        private static readonly MethodInfo SystemColors_InvalidateColors;
        private static readonly MethodInfo SystemResources_OnThemeChanged;
        private static readonly MethodInfo SystemResources_InvalidateResources;

        static ThemeManager()
        {
            SystemColors_InvalidateColors = typeof(SystemColors).GetMethod("InvalidateCache", BindingFlags.Static | BindingFlags.NonPublic);
            var assembly = Assembly.GetAssembly(typeof(Window));
            if (assembly != null)
            {
                var systemResources = assembly.GetType("System.Windows.SystemResources");
                if (systemResources != null)
                {
                    SystemResources_OnThemeChanged = systemResources.GetMethod("OnThemeChanged", BindingFlags.Static | BindingFlags.NonPublic);
                    SystemResources_InvalidateResources = systemResources.GetMethod("InvalidateResources", BindingFlags.Static | BindingFlags.NonPublic);
                }
            }
        }

        /// <summary>
        /// Gets a list of all of default themes.
        /// </summary>
        public static IList<Accent> DefaultAccents
        {
            get
            {
                if (_accents != null)
                    return _accents;

                var colors = new[]{"Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", 
                    "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Sienna"};

                _accents = new List<Accent>(colors.Length);

                foreach (string color in colors)
                {
                    _accents.Add(new Accent(color, new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", color))));
                }

                return _accents;
            }
        }

        /// <summary>
        /// Change accent and theme for the whole application.
        /// </summary>
        /// <param name="app">The instance of Application to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme(Application app, Accent newAccent, Theme newTheme)
        {
            if (app == null) throw new ArgumentNullException("app");

            var oldTheme = DetectTheme(app);
            ChangeTheme(app.Resources, oldTheme, newAccent, newTheme);
        }

        /// <summary>
        /// Change accent and theme for the given window.
        /// </summary>
        /// <param name="window">The Window to change.</param>
        /// <param name="newAccent">The accent to apply.</param>
        /// <param name="newTheme">The theme to apply.</param>
        [SecurityCritical]
        public static void ChangeTheme(Window window, Accent newAccent, Theme newTheme)
        {
            if (window == null) throw new ArgumentNullException("window");

            var oldTheme = DetectTheme(window);
            ChangeTheme(window.Resources, oldTheme, newAccent, newTheme);
        }

        [SecurityCritical]
        private static void ChangeTheme(ResourceDictionary resources, Tuple<Theme, Accent> oldThemeInfo, Accent newAccent, Theme newTheme)
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
                    var oldThemeResource = (oldTheme == Theme.Light) ? LightResource : DarkResource;
                    var md = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldThemeResource.Source);
                    if (md != null)
                    {
                        var newThemeResource = (newTheme == Theme.Light) ? LightResource : DarkResource;
                        resources.MergedDictionaries.Add(newThemeResource);
                        var ok = resources.MergedDictionaries.Remove(md);

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
        public static void ChangeTheme(ResourceDictionary resources, Accent newAccent, Theme newTheme)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            var themeResource = (newTheme == Theme.Light) ? LightResource : DarkResource;
            ApplyResourceDictionary(newAccent.Resources, resources);
            ApplyResourceDictionary(themeResource, resources);
        }

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

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        /// <param name="window">The Window to scan.</param>
        public static Tuple<Theme, Accent> DetectTheme(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");

            return DetectTheme(window.Resources);
        }

        /// <summary>
        /// Scans the application resources and returns it's accent and theme.
        /// </summary>
        /// <param name="app">The Application instance to scan.</param>
        public static Tuple<Theme, Accent> DetectTheme(Application app)
        {
            if (app == null) throw new ArgumentNullException("app");

            return DetectTheme(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's accent and theme.
        /// </summary>
        /// <param name="resources">The ResourceDictionary to check.</param>
        private static Tuple<Theme, Accent> DetectTheme(ResourceDictionary resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            Theme currentTheme = Theme.Light;
            ResourceDictionary themeDictionary = null;
            Tuple<Theme, Accent> detectedAccentTheme = null;


            if (DetectThemeFromResources(ref currentTheme, ref themeDictionary, resources))
            {
                if (GetThemeFromResources(currentTheme, resources, ref detectedAccentTheme))
                    return new Tuple<Theme, Accent>(detectedAccentTheme.Item1, detectedAccentTheme.Item2);
            }

            return null;
        }

        internal static bool DetectThemeFromAppResources(out Theme detectedTheme, out ResourceDictionary themeRd)
        {
            detectedTheme = Theme.Light;
            themeRd = null;

            return DetectThemeFromResources(ref detectedTheme, ref themeRd, Application.Current.Resources);
        }

        private static bool DetectThemeFromResources(ref Theme detectedTheme, ref ResourceDictionary themeRd, ResourceDictionary dict)
        {
            var enumerator = dict.MergedDictionaries.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRd = enumerator.Current;

                if (currentRd.Source == LightResource.Source || currentRd.Source == DarkResource.Source)
                {
                    detectedTheme = currentRd.Source == LightResource.Source ? Theme.Light : Theme.Dark;
                    themeRd = currentRd;

                    enumerator.Dispose();
                    return true;
                }

                if (DetectThemeFromResources(ref detectedTheme, ref themeRd, currentRd))
                {
                    return true;
                }
            }

            enumerator.Dispose();
            return false;
        }

        internal static bool GetThemeFromResources(Theme presetTheme, ResourceDictionary dict, ref Tuple<Theme, Accent> detectedAccentTheme)
        {
            Theme currentTheme = presetTheme;
            Accent currentAccent = null;

            Accent matched = null;
            if ((matched = ((List<Accent>)DefaultAccents).Find(x => x.Resources.Source == dict.Source)) != null)
            {
                currentAccent = matched;
                detectedAccentTheme = Tuple.Create<Theme, Accent>(currentTheme, currentAccent);
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
        private static void OnThemeChanged(Accent newAccent, Theme newTheme)
        {
            SafeRaise.Raise(IsThemeChanged, Application.Current, new OnThemeChangedEventArgs() { Theme = newTheme, Accent = newAccent });

            Action apply = () =>
                {
                    if (SystemColors_InvalidateColors != null)
                    {
                        SystemColors_InvalidateColors.Invoke(null, null);
                    }

                    // See: https://github.com/MahApps/MahApps.Metro/issues/923
                    //var invalidateParameters = typeof(SystemParameters).GetMethod("InvalidateCache", BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    //if (invalidateParameters != null)
                    //{
                    //    invalidateParameters.Invoke(null, null);
                    //}

                    if (SystemResources_OnThemeChanged != null)
                    {
                        SystemResources_OnThemeChanged.Invoke(null, null);
                    }

                    if (SystemResources_InvalidateResources != null)
                    {
                        SystemResources_InvalidateResources.Invoke(null, new object[] { false });
                    }
                };

            if (InvalidateSystemResourcesOnBackgroundThread)
            {
                Task.Factory.StartNew(apply);
            }
            else
            {
                apply();
            }
        }

        public static bool InvalidateSystemResourcesOnBackgroundThread { get; set; }
    }

    public class OnThemeChangedEventArgs : EventArgs
    {
        public Theme Theme { get; set; }
        public Accent Accent { get; set; }
    }
}