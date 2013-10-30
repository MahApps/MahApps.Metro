using System;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// An object that represents the foreground color for a Metro <see cref="Theme"/>.
    /// </summary>
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
        /// Creates a new Accent object.
        /// </summary>
        public Accent()
        { }

        /// <summary>
        /// Creates a new Accent object with a name and ResourceDictionary URI.
        /// </summary>
        /// <param name="name">The name of the new Accent.</param>
        /// <param name="resourceAddress">The URI of the accent ResourceDictionary.</param>
        public Accent(string name, Uri resourceAddress)
        {
            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}