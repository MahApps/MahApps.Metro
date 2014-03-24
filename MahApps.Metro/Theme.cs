using System;

namespace MahApps.Metro
{
    /// <summary>
    /// An enum that represents the two Metro styles: Light and Dark.
    /// </summary>
    public enum Theme
    {
        Light,
        Dark
    }

    public class AppTheme : Accent
    {
        public AppTheme(string name, Uri resourceAddress)
            : base(name, resourceAddress)
        {
        }

        public Theme Theme { get; set; }
    }
}