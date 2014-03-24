using System;
using System.Diagnostics;

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

    [DebuggerDisplay("apptheme={Name}, theme={Theme}, res={Resources.Source}")]
    public class AppTheme : Accent
    {
        public AppTheme(string name, Uri resourceAddress)
            : base(name, resourceAddress)
        {
        }

        public Theme Theme { get; set; }
    }
}