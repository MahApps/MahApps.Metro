using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    public interface IWindowsSettings
    {
        void SetSave(DependencyObject dependencyObject, bool enabled);
    }

    public class WindowSettings : IWindowsSettings
    {
        public void SetSave(DependencyObject dependencyObject, bool enabled) {
            var window = dependencyObject as Window;
            if (window == null || !enabled)
                return;

            var settings = new WindowSettings(window);
            settings.Attach();
        }

        private static WindowSettings instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static WindowSettings() {
        }

        private WindowSettings() {
        }

        public static WindowSettings Instance {
            get { return instance ?? (instance = new WindowSettings()); }
        }

        internal class WindowApplicationSettings : ApplicationSettingsBase
        {
            public WindowApplicationSettings(WindowSettings windowSettings)
                : base(windowSettings._window.GetType().FullName)
            {
            }

            [UserScopedSetting]
            public WINDOWPLACEMENT? Placement
            {
                get
                {
                    if (this["Placement"] != null)
                    {
                        return ((WINDOWPLACEMENT)this["Placement"]);
                    }
                    return null;
                }
                set
                {
                    this["Placement"] = value;
                }
            }
        }
        private Window _window;

        public WindowSettings(Window window)
        {
            _window = window;
        }

        protected virtual void LoadWindowState()
        {
            Settings.Reload();

            if (Settings.Placement == null) 
                return;

            try
            {
                var wp = Settings.Placement.Value;

                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == Constants.SW_SHOWMINIMIZED ? Constants.SW_SHOWNORMAL : wp.showCmd);
                var hwnd = new WindowInteropHelper(_window).Handle;
                UnsafeNativeMethods.SetWindowPlacement(hwnd, ref wp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to load window state:\r\n{0}", ex));
            }
        }

        protected virtual void SaveWindowState()
        {
            WINDOWPLACEMENT wp;
            var hwnd = new WindowInteropHelper(_window).Handle;
            UnsafeNativeMethods.GetWindowPlacement(hwnd, out wp);
            Settings.Placement = wp;
            Settings.Save();
        }

        private void Attach()
        {
            if (_window == null) return;
            _window.Closing += WindowClosing;
            _window.SourceInitialized += WindowSourceInitialized;
        }

        void WindowSourceInitialized(object sender, EventArgs e)
        {
            LoadWindowState();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
            _window.Closing -= WindowClosing;
            _window.SourceInitialized -= WindowSourceInitialized;
            _window = null;
        }

        private WindowApplicationSettings _windowApplicationSettings;

        internal virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        [Browsable(false)]
        internal WindowApplicationSettings Settings
        {
            get { return _windowApplicationSettings ?? (_windowApplicationSettings = CreateWindowApplicationSettingsInstance()); }
        }
    }
}
