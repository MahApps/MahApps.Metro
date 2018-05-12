using ControlzEx.Standard;
using System.Configuration;
using System.Windows;

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
}
