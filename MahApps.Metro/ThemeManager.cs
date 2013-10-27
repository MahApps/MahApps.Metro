using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MahApps.Metro
{
    public static class ThemeManager
    {
        private static readonly ResourceDictionary LightResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") };
        private static readonly ResourceDictionary DarkResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml") };

        private static IList<Accent> _accents;
        public static IList<Accent> DefaultAccents
        {
            get
            {
                return _accents ?? (_accents =
                    new List<Accent>{
                        new Accent("Red", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml")),
                        new Accent("Green", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml")),
                        new Accent("Blue", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")),
                        new Accent("Purple", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Purple.xaml")),
                        new Accent("Orange", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Orange.xaml")),

                        new Accent("Lime", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Lime.xaml")),
                        new Accent("Emerald", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Emerald.xaml")),
                        new Accent("Teal", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Teal.xaml")),
                        new Accent("Cyan", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Cyan.xaml")),
                        new Accent("Cobalt", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Cobalt.xaml")),
                        new Accent("Indigo", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Indigo.xaml")),
                        new Accent("Violet", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Violet.xaml")),
                        new Accent("Pink", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Pink.xaml")),
                        new Accent("Magenta", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Magenta.xaml")),
                        new Accent("Crimson", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Crimson.xaml")),
                        new Accent("Amber", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Amber.xaml")),
                        new Accent("Yellow", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Yellow.xaml")),
                        new Accent("Brown", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Brown.xaml")),
                        new Accent("Olive", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Olive.xaml")),
                        new Accent("Steel", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Steel.xaml")),
                        new Accent("Mauve", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Mauve.xaml")),
                        new Accent("Sienna", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Sienna.xaml")),
                    });
            }
        }

        /// <summary>
        /// change accent and theme for the hole application
        /// </summary>
        public static void ChangeTheme(Application app, Accent accent, Theme theme)
        {
            var oldTheme = DetectTheme(app);
            ChangeTheme(app.Resources, oldTheme, accent, theme);
        }

        /// <summary>
        /// change accent and theme for the given window
        /// </summary>
        public static void ChangeTheme(Window window, Accent accent, Theme theme)
        {
            var oldTheme = DetectTheme(window);
            ChangeTheme(window.Resources, oldTheme, accent, theme);
        }

        private static void ChangeTheme(ResourceDictionary resources, Tuple<Theme, Accent> oldThemeInfo, Accent accent, Theme newTheme)
        {
            if (oldThemeInfo != null)
            {
                var oldAccent = oldThemeInfo.Item2;
                if (oldAccent != null && oldAccent.Name != accent.Name)
                {
                    var accentResource = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldAccent.Resources.Source);
                    if (accentResource != null) {
                        var ok = resources.MergedDictionaries.Remove(accentResource);
                        // really need this???
                        foreach (DictionaryEntry r in accentResource)
                        {
                            if (resources.Contains(r.Key))
                                resources.Remove(r.Key);
                        }

                        resources.MergedDictionaries.Add(accent.Resources);
                    }
                }

                var oldTheme = oldThemeInfo.Item1;
                if (oldTheme != null && oldTheme != newTheme)
                {
                    var oldThemeResource = (oldTheme == Theme.Light) ? LightResource : DarkResource;
                    var md = resources.MergedDictionaries.FirstOrDefault(d => d.Source == oldThemeResource.Source);
                    if (md != null)
                    {
                        var ok = resources.MergedDictionaries.Remove(md);
                        var newThemeResource = (newTheme == Theme.Light) ? LightResource : DarkResource;
                        // really need this???
                        foreach (DictionaryEntry r in oldThemeResource)
                        {
                            if (resources.Contains(r.Key))
                                resources.Remove(r.Key);
                        }

                        resources.MergedDictionaries.Add(newThemeResource);
                    }
                }
            }
            else
            {
                ChangeTheme(resources, accent, newTheme);
            }
        }

        public static void ChangeTheme(ResourceDictionary r, Accent accent, Theme theme)
        {
            var themeResource = (theme == Theme.Light) ? LightResource : DarkResource;
            ApplyResourceDictionary(accent.Resources, r);
            ApplyResourceDictionary(themeResource, r);
        }

        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                    oldRd.Remove(r.Key);

                oldRd.Add(r.Key, r.Value);
            }
        }

        /// <summary>
        /// Scans the window resources and returns it's accent and theme.
        /// </summary>
        public static Tuple<Theme, Accent> DetectTheme(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");

            return DetectTheme(window.Resources);
        }

        /// <summary>
        /// Scans the application resources and returns it's accent and theme.
        /// </summary>
        public static Tuple<Theme, Accent> DetectTheme(Application app)
        {
            if (app == null) throw new ArgumentNullException("app");
            
            return DetectTheme(app.Resources);
        }

        /// <summary>
        /// Scans a resources and returns it's accent and theme.
        /// </summary>
        /// <param name="window">The resource to check.</param>
        private static Tuple<Theme, Accent> DetectTheme(ResourceDictionary resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            Theme currentTheme = Theme.Light;
            ResourceDictionary themeDictionary = null;
            Tuple<Theme, Accent> detectedAccentTheme = null;


            if (DetectThemeFromResources(ref currentTheme, ref themeDictionary, resources)) {
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

                if (DetectThemeFromResources(ref detectedTheme, ref themeRd, currentRd)) {
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
    }
}