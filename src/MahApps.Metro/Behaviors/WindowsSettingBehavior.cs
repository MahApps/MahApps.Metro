// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
#if !NET462
using System.Text.Json;
#endif
using System.Windows;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{
    public class WindowsSettingBehavior : Behavior<MetroWindow>
    {
        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            this.LoadWindowState();

            var window = this.AssociatedObject;
            if (window is null)
            {
                // if the associated object is null at this point, then there is really something wrong!
                Trace.TraceError($"{this}: Can not attach to nested events, cause the AssociatedObject is null.");
                return;
            }

            window.StateChanged += this.AssociatedObject_StateChanged;
            window.Closing += this.AssociatedObject_Closing;
            window.Closed += this.AssociatedObject_Closed;

            // This operation must be thread safe. It is possible, that the window is running in a different Thread.
            Application.Current?.BeginInvoke(app =>
                {
                    if (app != null)
                    {
                        app.SessionEnding += this.CurrentApplicationSessionEnding;
                    }
                });
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            this.CleanUp("from OnDetaching");
            base.OnDetaching();
        }

        private void AssociatedObject_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveWindowState();
        }

        private void AssociatedObject_Closed(object? sender, EventArgs e)
        {
            this.CleanUp("from AssociatedObject closed event");
        }

        private void CurrentApplicationSessionEnding(object? sender, SessionEndingCancelEventArgs e)
        {
            this.SaveWindowState();
        }

        private void AssociatedObject_StateChanged(object? sender, EventArgs e)
        {
            // save the settings on this state change, because hidden windows gets no window placements
            // all the saving stuff could be so much easier with ReactiveUI :-D
            if (this.AssociatedObject?.WindowState == WindowState.Minimized)
            {
                this.SaveWindowState();
            }
        }

        private void CleanUp(string fromWhere)
        {
            var window = this.AssociatedObject;
            if (window is null)
            {
                // it's bad if the associated object is null, so trace this here
                Trace.TraceWarning($"{this}: Can not clean up {fromWhere}, cause the AssociatedObject is null. This can maybe happen if this Behavior was already detached.");
                return;
            }

            Debug.WriteLine($"{this}: Clean up {fromWhere}.");

            window.StateChanged -= this.AssociatedObject_StateChanged;
            window.Closing -= this.AssociatedObject_Closing;
            window.Closed -= this.AssociatedObject_Closed;

            // This operation must be thread safe
            Application.Current?.BeginInvoke(app =>
                {
                    if (app != null)
                    {
                        app.SessionEnding -= this.CurrentApplicationSessionEnding;
                    }
                });
        }

        private void LoadWindowState()
        {
            var window = this.AssociatedObject;
            if (window is null)
            {
                return;
            }

            var settings = window.GetWindowPlacementSettings();
            if (settings is null || !window.SaveWindowPosition)
            {
                return;
            }

            try
            {
                settings.Reload();
            }
            catch (Exception e)
            {
                Trace.TraceError($"{this}: The settings for {window} could not be reloaded! {e}");
                // Don't return yet — try JSON fallback below
            }

            // check for existing placement and prevent empty bounds
            var placement = settings.Placement;
            if (placement is null || placement.normalPosition.IsEmpty)
            {
#if !NET462
                // Fallback: try to load from JSON backup if settings has no valid placement
                if (TryLoadFromJsonFallback(window, out var fallbackPlacement))
                {
                    placement = fallbackPlacement;
                    settings.Placement = fallbackPlacement;
                }
                else
                {
                    return;
                }
#else
                return;
#endif
            }

            try
            {
                var wp = placement!.ToWINDOWPLACEMENT();
                WinApiHelper.SetWindowPlacement(window, wp);
            }
            catch (Exception ex)
            {
                throw new MahAppsException($"Failed to set the window state for {window} from the settings.", ex);
            }
        }

        private void SaveWindowState()
        {
            var window = this.AssociatedObject;
            if (window is null)
            {
                return;
            }

            var settings = window.GetWindowPlacementSettings();
            if (settings is null || !window.SaveWindowPosition)
            {
                return;
            }

            var windowHandle = new WindowInteropHelper(window).EnsureHandle();
            var wp = new WINDOWPLACEMENT
                     {
                         length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>()
                     };
            unsafe
            {
                PInvoke.GetWindowPlacement(new HWND(windowHandle), &wp);
            }

            // check for saveable values
            if (wp.showCmd != SHOW_WINDOW_CMD.SW_HIDE && wp.length > 0)
            {
                if (wp.showCmd == SHOW_WINDOW_CMD.SW_NORMAL)
                {
                    unsafe
                    {
                        var rect = new RECT();
                        PInvoke.GetWindowRect(new HWND(windowHandle), &rect);
                        if (rect.left != 0
                            || rect.top != 0
                            || rect.right != 0
                            || rect.bottom != 0)
                        {
                            var monitor = PInvoke.MonitorFromWindow(new HWND(windowHandle), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
                            if (monitor != IntPtr.Zero)
                            {
                                var monitorInfo = new MONITORINFO
                                                  {
                                                      cbSize = (uint)Marshal.SizeOf<MONITORINFO>()
                                                  };
                                PInvoke.GetMonitorInfo(monitor, &monitorInfo);
                                rect.Offset(monitorInfo.rcMonitor.left - monitorInfo.rcWork.left, monitorInfo.rcMonitor.top - monitorInfo.rcWork.top);
                            }

                            wp.rcNormalPosition = rect;
                        }
                    }
                }

                if (!wp.rcNormalPosition.IsEmpty())
                {
                    settings.Placement = WindowPlacementSetting.FromWINDOWPLACEMENT(window, wp);
                }
            }

            try
            {
                settings.Save();

#if !NET462
                // On successful save, also save to JSON fallback for .NET version resilience
                SaveToJsonFallback(window, settings.Placement);
#endif
            }
            catch (Exception e)
            {
                Trace.TraceError($"{this}: The settings could not be saved! {e}");
#if !NET462
                // Fallback: save to JSON file when ApplicationSettingsBase.Save() fails
                // (e.g. .NET 9 < 9.0.3 bug where ClientConfigurationHost fails on UNC paths)
                try
                {
                    SaveToJsonFallback(window, settings.Placement);
                    Trace.TraceInformation($"{this}: Window placement saved to JSON fallback instead.");
                }
                catch (Exception fallbackEx)
                {
                    Trace.TraceError($"{this}: The JSON fallback save also failed! {fallbackEx}");
                }
#endif
            }
        }

#if !NET462
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IncludeFields = true
        };

        private void SaveToJsonFallback(Window window, WindowPlacementSetting? placement)
        {
            if (placement is null)
            {
                return;
            }

            var filePath = GetFallbackFilePath(window);
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(placement, JsonOptions);
            File.WriteAllText(filePath, json);
        }

        private bool TryLoadFromJsonFallback(Window window, out WindowPlacementSetting? placement)
        {
            placement = null;

            try
            {
                var filePath = GetFallbackFilePath(window);
                if (!File.Exists(filePath))
                {
                    return false;
                }

                var json = File.ReadAllText(filePath);
                placement = JsonSerializer.Deserialize<WindowPlacementSetting>(json, JsonOptions);
                return placement != null;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"{this}: Could not load JSON fallback for {window}: {ex.Message}");
                return false;
            }
        }

        private static string GetFallbackFilePath(Window window)
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "MahApps.Metro";
            var windowTypeName = window.GetType().FullName?.Replace('.', '_') ?? "MetroWindow";
            return Path.Combine(localAppData, appName, "WindowPlacement", $"{windowTypeName}.json");
        }
#endif
    }
}