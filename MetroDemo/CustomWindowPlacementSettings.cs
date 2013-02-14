using MahApps.Metro.Controls;
using MahApps.Metro.Native;

namespace MetroDemo
{
    /// <summary>
    /// saves the window placement
    /// </summary>
    public class CustomWindowPlacementSettings : IWindowPlacementSettings
    {
        private static CustomWindowPlacementSettings instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CustomWindowPlacementSettings() {
        }

        private CustomWindowPlacementSettings() {
        }

        public static CustomWindowPlacementSettings Instance {
            get { return instance ?? (instance = new CustomWindowPlacementSettings()); }
        }

        public WINDOWPLACEMENT? Placement { get; set; }
        
        public void Reload() {
            // load the placement from your custom settings file or something else
        }

        public void Save() {
            // save the placement to your custom settings file or something else
        }
    }
}