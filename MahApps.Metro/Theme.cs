using System;
using System.Diagnostics;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// An enum that represents the two Metro styles: Light and Dark.
    /// </summary>
    [Obsolete("This class is obsolete and no longer used")]
    public enum Theme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Represents the background theme of the application.
    /// </summary>
    [DebuggerDisplay("apptheme={Name}, theme={Theme}, res={Resources.Source}")]
    public class AppTheme
    {
        /// <summary>
        /// The ResourceDictionary that represents this application theme.
        /// </summary>
        public ResourceDictionary Resources {get; private set;}

        /// <summary>
        /// Gets the name of the application theme.
        /// </summary>
        public string Name { get; private set; }

        public AppTheme(string name, Uri resourceAddress)
        {
            if(name == null)
                throw new ArgumentException("name");

            if(resourceAddress == null)
                throw new ArgumentNullException("resourceAddress");

            this.Name = name;
            this.Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}