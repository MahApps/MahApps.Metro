using System;
using System.Diagnostics;
using System.Windows;

namespace MahApps.Metro
{
    internal class AppName
    {
        public const string MahApps = "MahApps.Metro";
    }

    /// <summary>
    /// Represents the background theme of the application.
    /// </summary>
    [DebuggerDisplay("apptheme={Name}, res={Resources.Source}")]
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
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resourceAddress == null) throw new ArgumentNullException(nameof(resourceAddress));

            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}