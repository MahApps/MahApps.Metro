using System;
using System.Diagnostics;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// An object that represents the foreground color for a Metro <see cref="AppTheme"/>.
    /// </summary>
    [DebuggerDisplay("accent={Name}, res={Resources.Source}")]
    public class Accent
    {
        /// <summary>
        /// The ResourceDictionary that represents this Accent.
        /// </summary>
        public ResourceDictionary Resources;
        /// <summary>
        /// Gets/sets the name of the Accent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Accent class.
        /// </summary>
        public Accent()
        { }

        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Accent class.
        /// </summary>
        /// <param name="name">The name of the new Accent.</param>
        /// <param name="resourceAddress">The URI of the accent ResourceDictionary.</param>
        public Accent(string name, Uri resourceAddress)
        {
            if (name == null) throw new ArgumentException("name");
            if (resourceAddress == null) throw new ArgumentNullException("resourceAddress");

            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}