using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using ControlzEx.Native;
using ControlzEx.Standard;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SourceInitialized += this.AssociatedObject_SourceInitialized;
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            this.CleanUp();
            base.OnDetaching();
        }

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            this.CleanUp();
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveWindowState();
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.LoadWindowState();

            this.AssociatedObject.StateChanged += this.AssociatedObject_StateChanged;
            this.AssociatedObject.Closing += this.AssociatedObject_Closing;
            this.AssociatedObject.Closed += this.AssociatedObject_Closed;
        }

        private void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            // save the settings on this state change, because hidden windows gets no window placements
            // all the saving stuff could be so much easier with ReactiveUI :-D 
            if (this.AssociatedObject.WindowState == WindowState.Minimized)
            {
                this.SaveWindowState();
            }
        }

        private void CleanUp()
        {
            this.AssociatedObject.StateChanged -= this.AssociatedObject_StateChanged;
            this.AssociatedObject.Closing -= this.AssociatedObject_Closing;
            this.AssociatedObject.Closed -= this.AssociatedObject_Closed;
            this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
        }

#pragma warning disable 618
        private void LoadWindowState()
        {
            var settings = this.AssociatedObject.GetWindowPlacementSettings();
            if (settings == null || !this.AssociatedObject.SaveWindowPosition)
            {
                return;
            }

            try
            {
                settings.Reload();
            }
            catch (Exception e)
            {
                Trace.TraceError($"The settings could not be reloaded! {e}");
                return;
            }

            // check for existing placement and prevent empty bounds
            if (settings.Placement == null || settings.Placement.normalPosition.IsEmpty)
            {
                return;
            }

            try
            {
                var wp = settings.Placement;
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == SW.SHOWMINIMIZED ? SW.SHOWNORMAL : wp.showCmd);
                var hwnd = new WindowInteropHelper(this.AssociatedObject).Handle;
                if (!NativeMethods.SetWindowPlacement(hwnd, wp))
                {
                    Trace.TraceWarning($"The WINDOWPLACEMENT {wp} could not be set by SetWindowPlacement.");
                }
            }
            catch (Exception ex)
            {
                throw new MahAppsException("Failed to set the window state from the settings file", ex);
            }
        }

        private void SaveWindowState()
        {
            var settings = this.AssociatedObject.GetWindowPlacementSettings();
            if (settings == null || !this.AssociatedObject.SaveWindowPosition)
            {
                return;
            }
            var hwnd = new WindowInteropHelper(this.AssociatedObject).Handle;
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
                    settings.Placement = wp;
                }
            }

            try
            {
                settings.Save();
            }
            catch (Exception e)
            {
                Trace.TraceError($"The settings could not be saved! {e}");
            }
        }
#pragma warning restore 618
    }
}