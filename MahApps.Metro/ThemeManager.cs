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

        internal static bool DetectThemeFromAppResources(out Theme detectedTheme, out ResourceDictionary themeRd)
        {
            detectedTheme = Theme.Light;
            themeRd = null;

            var enumerator = Application.Current.Resources.MergedDictionaries.GetEnumerator();
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
        internal static Tuple<Theme, Accent> GetThemeFromAppResources(Nullable<Theme> presetTheme = null)
        {
            Theme currentTheme = 0;
            Accent currentAccent = null;

            if (presetTheme == null)
            {
                ResourceDictionary rd = null;
                if (DetectThemeFromAppResources(out currentTheme, out rd) == false)
                    throw new InvalidOperationException("No theme was specified in App.xaml!");
            }
            else
            {
                currentTheme = presetTheme.Value;
            }


            foreach (ResourceDictionary rd in Application.Current.Resources.MergedDictionaries)
            {
                Accent matched = null;
                if ((matched = ((List<Accent>)_accents).Find(x => x.Resources.Source == rd.Source)) != null)
                {
                    currentAccent = matched;
                    break;
                }
            }

            if (currentAccent == null)
                throw new InvalidOperationException("No accent was specified in App.xaml!");

            return Tuple.Create<Theme, Accent>(currentTheme, currentAccent);
        }

        internal static void RemoveThemeFromAppResources()
        {
            Theme currentTheme = 0;
            Accent currentAccent = null;
            ResourceDictionary rd = null;

            if (DetectThemeFromAppResources(out currentTheme, out rd) == false)
                throw new InvalidOperationException("No theme was specified in App.xaml!");

a:          var enumerator = Application.Current.Resources.MergedDictionaries.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRd = enumerator.Current;

                if (currentRd.Source.ToString() == "pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml" 
                    || currentRd.Source.ToString() == "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml"
                    || currentRd.Source.ToString() == "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml")
                {
                    Application.Current.Resources.MergedDictionaries.Remove(currentRd);

                    goto a; //yes, goto is bad
                }
            }

            enumerator.Dispose();

            Application.Current.Resources.MergedDictionaries.Remove(rd);
        }

        internal static ResourceDictionary ApplyEssentialResourceDictionaries(ResourceDictionary resourceDict, Theme theme, Accent accent)
        {
            var rd = resourceDict;
            rd.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
            rd.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml") });
            rd.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });

            var themeResource = (theme == Theme.Light) ? LightResource : DarkResource;
            rd.MergedDictionaries.Add(themeResource);
            rd.MergedDictionaries.Add(accent.Resources);

            return rd;
        }
    }
}