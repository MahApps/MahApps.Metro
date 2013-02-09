using System.Windows;
using MahApps.Metro.Controls;

namespace MetroDemo
{
    /// <summary>
    /// use this custom window settings class if you don't want to save your settings with ApplicationSettingsBase
    /// </summary>
    public class CustomWindowSettings : IWindowsSettings
    {
        public void SetSave(DependencyObject dependencyObject, bool enabled) {
            // now handle the custom window settings
            // to save your settings to what you want (eg as json...)

            /*
            var window = dependencyObject as Window;
            if (window == null || !enabled)
                return;

            var settings = new CustomWindowSettings(window);
            settings.Attach();
            */
        }

        private static CustomWindowSettings instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CustomWindowSettings() {
        }

        private CustomWindowSettings() {
        }

        public static CustomWindowSettings Instance {
            get { return instance ?? (instance = new CustomWindowSettings()); }
        }
    }
}