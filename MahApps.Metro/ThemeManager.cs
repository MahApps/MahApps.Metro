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
            ThemeIsDark = (theme == Theme.Dark);
            var themeResource = (theme == Theme.Light) ? LightResource : DarkResource;
            ApplyResourceDictionary(themeResource, r);
            ApplyResourceDictionary(accent.Resources, r);
        }

        public static bool ThemeIsDark { get; private set; }

        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                    oldRd.Remove(r.Key);

                oldRd.Add(r.Key, r.Value);
            }
        }
    }
}