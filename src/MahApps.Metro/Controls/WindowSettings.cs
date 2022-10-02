// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
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

        internal static WindowPlacementSetting FromWINDOWPLACEMENT(WINDOWPLACEMENT windowplacement)
        {
            return new WindowPlacementSetting
                    {
                        showCmd = (uint)windowplacement.showCmd,
                        minPosition = new Point(windowplacement.ptMinPosition.X, windowplacement.ptMinPosition.Y),
                        maxPosition = new Point(windowplacement.ptMaxPosition.X, windowplacement.ptMaxPosition.Y),
                        normalPosition = new Rect(windowplacement.rcNormalPosition.left, windowplacement.rcNormalPosition.top, windowplacement.rcNormalPosition.GetWidth(), windowplacement.rcNormalPosition.GetHeight()),
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