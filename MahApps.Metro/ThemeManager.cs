using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace MahApps.Metro
{
    public static class ThemeManager
    {
        public static Accent appAccent;
        public static Theme appTheme;

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
                    });
            }
        }

        public static void ChangeAppTheme(Window window)
        {
            if (appAccent != null && appTheme != null)
                ChangeTheme(window.Resources, appAccent, appTheme);
        }

        public static void ChangeAppTheme(Window window, Accent accent, Theme theme)
        {
            appTheme = theme;
            appAccent = accent;
            ChangeTheme(window.Resources, appAccent, appTheme);
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
    }
}
