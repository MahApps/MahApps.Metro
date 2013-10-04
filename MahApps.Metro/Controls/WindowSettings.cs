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
    public interface IWindowPlacementSettings
    {
        WINDOWPLACEMENT? Placement { get; set; }
        void Reload();
        void Save();
    }

    /// <summary>
    /// this settings class is the default way to save the placement of the window
    /// </summary>
    internal class WindowApplicationSettings : ApplicationSettingsBase, IWindowPlacementSettings
    {
        public WindowApplicationSettings(Window window)
            : base(window.GetType().FullName) {
        }

        [UserScopedSetting]
        public WINDOWPLACEMENT? Placement {
            get {
                if (this["Placement"] != null) {
                    return ((WINDOWPLACEMENT)this["Placement"]);
                }
                return null;
            }
            set {
                this["Placement"] = value;
            }
        }
    }
    
    public class WindowSettings
    {
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.RegisterAttached("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(WindowSettings), new FrameworkPropertyMetadata(OnWindowPlacementSettingsInvalidated));

        public static void SetSave(DependencyObject dependencyObject, IWindowPlacementSettings windowPlacementSettings)
        {
            dependencyObject.SetValue(WindowPlacementSettingsProperty, windowPlacementSettings);
        }

        private static void OnWindowPlacementSettingsInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            var window = dependencyObject as Window;
            if (window == null || !(e.NewValue is IWindowPlacementSettings))
                return;

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

        protected virtual void LoadWindowState()
        {
            if (_settings == null) return;
            _settings.Reload();

            if (_settings.Placement == null) 
                return;

            try
            {
                var wp = _settings.Placement.Value;

                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == Constants.SW_SHOWMINIMIZED ? Constants.SW_SHOWNORMAL : wp.showCmd);
                var hwnd = new WindowInteropHelper(_window).Handle;
                UnsafeNativeMethods.SetWindowPlacement(hwnd, ref wp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load window state:\r\n{0}", ex);
            }
        }

        protected virtual void SaveWindowState()
        {
            if (_settings == null) return;
            WINDOWPLACEMENT wp;
            var hwnd = new WindowInteropHelper(_window).Handle;
            UnsafeNativeMethods.GetWindowPlacement(hwnd, out wp);
            _settings.Placement = wp;
            _settings.Save();
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
            _settings = null;
        }
    }
}
