using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace MahApps.Metro
{
    public static class ThemeManager
    {
        private static readonly ResourceDictionary LightResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") };
        private static readonly ResourceDictionary DarkResource = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml") };

        private static IEnumerable<Accent> _accents;
        public static IEnumerable<Accent> DefaultAccents
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

        public static void ChangeTheme(Application app, Accent accent, Theme theme)
        {
            ChangeTheme(app.Resources, accent, theme);
        }

        public static void ChangeTheme(Window window, Accent accent, Theme theme)
        {
            ChangeTheme(window.Resources, accent, theme);
        }

        public static void ChangeTheme(ResourceDictionary r, Accent accent, Theme theme)
        {
            var themeResource = (theme == Theme.Light) ? LightResource : DarkResource;
            ApplyResourceDictionary(themeResource, r);
            ApplyResourceDictionary(accent.Resources, r);
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
        /// Scans a Window's resources and returns it's accent and theme.
        /// </summary>
        /// <param name="window">The Window to check.</param>
        /// <returns></returns>
        public static Tuple<Theme, Accent> DetectTheme(MahApps.Metro.Controls.MetroWindow window)
        {
            if (window == null) throw new ArgumentNullException("window");

            Theme currentTheme = Theme.Light;
            ResourceDictionary themeDictionary = null;
            Tuple<Theme, Accent> detectedAccentTheme = null;


            if (DetectThemeFromResources(ref currentTheme, ref themeDictionary, window.Resources))
            {
                detectedAccentTheme = GetThemeFromResources(currentTheme, window.Resources);

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
            }

            enumerator.Dispose();
            return false;
        }
        internal static Tuple<Theme, Accent> GetThemeFromResources(Theme presetTheme, ResourceDictionary dict)
        {
            Theme currentTheme = 0;
            Accent currentAccent = null;

            currentTheme = presetTheme;

            foreach (ResourceDictionary rd in dict.MergedDictionaries)
            {
                Accent matched = null;
                if ((matched = ((List<Accent>)_accents).Find(x => x.Resources.Source == rd.Source)) != null)
                {
                    currentAccent = matched;
                    break;
                }
            }

            return Tuple.Create<Theme, Accent>(currentTheme, currentAccent);
        }
    }
}