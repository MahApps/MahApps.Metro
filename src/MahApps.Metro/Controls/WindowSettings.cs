// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace MahApps.Metro.Controls
{
    public class WindowPlacementSetting
    {
        public uint showCmd;
        public Point minPosition;
        public Point maxPosition;
        public Rect normalPosition;

        internal WINDOWPLACEMENT ToWINDOWPLACEMENT()
        {
            return new WINDOWPLACEMENT
                   {
                       length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                       showCmd = (SHOW_WINDOW_CMD)this.showCmd,
                       ptMinPosition = new System.Drawing.Point { X = (int)this.minPosition.X, Y = (int)this.minPosition.Y },
                       ptMaxPosition = new System.Drawing.Point { X = (int)this.maxPosition.X, Y = (int)this.maxPosition.Y },
                       rcNormalPosition = new RECT { left = (int)this.normalPosition.X, top = (int)this.normalPosition.Y, right = (int)this.normalPosition.Right, bottom = (int)this.normalPosition.Bottom }
                   };
        }

        internal static WindowPlacementSetting FromWINDOWPLACEMENT(Window window, WINDOWPLACEMENT placement)
        {
            // Get the current DPI scale factor
            var dpiScale = VisualTreeHelper.GetDpi(window);

            // Adjust the size of the window for DPI scaling
            var adjustedWidth = placement.rcNormalPosition.GetWidth() / dpiScale.DpiScaleX;
            var adjustedHeight = placement.rcNormalPosition.GetHeight() / dpiScale.DpiScaleY;

            return new WindowPlacementSetting
                   {
                       showCmd = (uint)placement.showCmd,
                       minPosition = new Point(placement.ptMinPosition.X, placement.ptMinPosition.Y),
                       maxPosition = new Point(placement.ptMaxPosition.X, placement.ptMaxPosition.Y),
                       normalPosition = new Rect(placement.rcNormalPosition.left, placement.rcNormalPosition.top, adjustedWidth, adjustedHeight),
                   };
        }
    }

    public interface IWindowPlacementSettings
    {
        WindowPlacementSetting? Placement { get; set; }

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

        /// <summary>
        /// Calls Reset on the providers.
        /// Providers must implement IApplicationSettingsProvider to support this.
        /// </summary>
        void Reset();
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

        [UserScopedSetting]
        public WindowPlacementSetting? Placement
        {
            get
            {
                try
                {
                    return this[nameof(Placement)] as WindowPlacementSetting;
                }
                catch (ConfigurationErrorsException? ex)
                {
                    string? filename = null;
                    while (ex != null && (filename = ex.Filename) == null)
                    {
                        ex = ex.InnerException as ConfigurationErrorsException;
                    }

                    throw new MahAppsException($"The settings file '{filename ?? "<unknown>"}' seems to be corrupted", ex);
                }
            }
            set => this[nameof(Placement)] = value;
        }

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
                    return (this[nameof(UpgradeSettings)] as bool?).GetValueOrDefault(true);
                }
                catch (ConfigurationErrorsException? ex)
                {
                    string? filename = null;
                    while (ex != null && (filename = ex.Filename) == null)
                    {
                        ex = ex.InnerException as ConfigurationErrorsException;
                    }

                    throw new MahAppsException($"The settings file '{filename ?? "<unknown>"}' seems to be corrupted", ex);
                }
            }
            set => this[nameof(UpgradeSettings)] = value;
        }
    }
}