using ControlzEx.Native;
using ControlzEx.Standard;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Interop;

namespace MahApps.Metro.Controls
{
    public interface IWindowPlacementSettings
    {
#pragma warning disable 618
        WINDOWPLACEMENT Placement { get; set; }
#pragma warning restore 618

        /// <summary>
        /// Refreshes the application settings property values from persistent storage.
        /// </summary>
        void Reload();

        /// <summary>
        /// Upgrades the application settings on loading.
        /// </summary>
        bool UpgradeSettings { get; set; }

        /// <summary>
        /// Updates application settings to reflect a more recent installation of the application.
        /// </summary>
        void Upgrade();

        /// <summary>
        /// Stores the current values of the settings properties.
        /// </summary>
        void Save();
    }

    /// <summary>
    /// this settings class is the default way to save the placement of the window
    /// </summary>
    internal class WindowApplicationSettings : ApplicationSettingsBase, IWindowPlacementSettings
    {
        public WindowApplicationSettings(Window window)
            : base(window.GetType().FullName)
        {
        }

#pragma warning disable 618
        [UserScopedSetting]
        public WINDOWPLACEMENT Placement
        {
            get
            {
                if (this["Placement"] != null)
                {
                    return ((WINDOWPLACEMENT)this["Placement"]);
                }
                return null;
            }
            set { this["Placement"] = value; }
        }
#pragma warning restore 618

        /// <summary>
        /// Upgrades the application settings on loading.
        /// </summary>
        [UserScopedSetting]
        public bool UpgradeSettings
        {
            get
            {
                try
                {
                    if (this["UpgradeSettings"] != null)
                    {
                        return (bool)this["UpgradeSettings"];
                    }
                }
                catch (ConfigurationErrorsException ex)
                {
                    string filename = null;
                    while (ex != null && (filename = ex.Filename) == null)
                    {
                        ex = ex.InnerException as ConfigurationErrorsException;
                    }
                    throw new MahAppsException(string.Format("The settings file '{0}' seems to be corrupted", filename ?? "<unknown>"), ex);
                }
                return true;
            }
            set { this["UpgradeSettings"] = value; }
        }
    }

    [Obsolete("This class is obsolete and will be deleted at the next major release.")]
    public class WindowSettings
    {
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.RegisterAttached("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(WindowSettings), new FrameworkPropertyMetadata(OnWindowPlacementSettingsInvalidated));

        public static void SetSave(DependencyObject dependencyObject, IWindowPlacementSettings windowPlacementSettings)
        {
            SetWindowPlacementSettings(dependencyObject, windowPlacementSettings);
        }

        public static void SetWindowPlacementSettings(DependencyObject dependencyObject, IWindowPlacementSettings windowPlacementSettings)
        {
            dependencyObject.SetValue(WindowPlacementSettingsProperty, windowPlacementSettings);
        }

        private static void OnWindowPlacementSettingsInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null || !(e.NewValue is IWindowPlacementSettings))
            {
                return;
            }

            var windowSettings = new WindowSettings(window, (IWindowPlacementSettings)e.NewValue);
            windowSettings.Attach();
        }

        private Window _window;
        private IWindowPlacementSettings _settings;

        public WindowSettings(Window window, IWindowPlacementSettings windowPlacementSettings)
        {
            _window = window;
            _settings = windowPlacementSettings;
        }

#pragma warning disable 618
        protected virtual void LoadWindowState()
        {
            if (_settings == null)
            {
                return;
            }
            _settings.Reload();

            // check for existing placement and prevent empty bounds
            if (_settings.Placement == null || _settings.Placement.normalPosition.IsEmpty) return;

            try
            {
                var wp = _settings.Placement;
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == SW.SHOWMINIMIZED ? SW.SHOWNORMAL : wp.showCmd);
                var hwnd = new WindowInteropHelper(_window).Handle;
                NativeMethods.SetWindowPlacement(hwnd, wp);
            }
            catch (Exception ex)
            {
                throw new MahAppsException("Failed to set the window state from the settings file", ex);
            }
        }

        protected virtual void SaveWindowState()
        {
            if (_settings == null)
            {
                return;
            }
            var hwnd = new WindowInteropHelper(_window).Handle;
            var wp = NativeMethods.GetWindowPlacement(hwnd);
            // check for saveable values
            if (wp.showCmd != SW.HIDE && wp.length > 0)
            {
                if (wp.showCmd == SW.NORMAL)
                {
                    RECT rect;
                    if (UnsafeNativeMethods.GetWindowRect(hwnd, out rect))
                    {
                        wp.normalPosition = rect;
                    }
                }
                if (!wp.normalPosition.IsEmpty)
                {
                    _settings.Placement = wp;
                }
            }
            _settings.Save();
        }
#pragma warning restore 618

        private void Attach()
        {
            if (_window != null)
            {
                _window.SourceInitialized += WindowSourceInitialized;
                _window.Closed += WindowClosed;
            }
        }

        void WindowSourceInitialized(object sender, EventArgs e)
        {
            LoadWindowState();
            _window.StateChanged += WindowStateChanged;
            _window.Closing += WindowClosing;
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            // save the settings on this state change, because hidden windows gets no window placements
            // all the saving stuff could be so much easier with ReactiveUI :-D 
            if (_window.WindowState == WindowState.Minimized)
            {
                SaveWindowState();
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            SaveWindowState();
            _window.StateChanged -= WindowStateChanged;
            _window.Closing -= WindowClosing;
            _window.Closed -= WindowClosed;
            _window.SourceInitialized -= WindowSourceInitialized;
            _window = null;
            _settings = null;
        }
    }
}
