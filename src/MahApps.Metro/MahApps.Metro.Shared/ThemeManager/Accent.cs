// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// An object that represents the foreground color for a <see cref="AppTheme"/>.
    /// </summary>
    [DebuggerDisplay("accent={Name}, res={Resources.Source}")]
    public class Accent
    {
        /// <summary>
        /// The ResourceDictionary that represents this Accent.
        /// </summary>
        public ResourceDictionary Resources { get; set; }

        /// <summary>
        /// Gets/sets the name of the Accent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the Accent class.
        /// </summary>
        public Accent()
        { }

        /// <summary>
        /// Initializes a new instance of the Accent class.
        /// </summary>
        /// <param name="name">The name of the new Accent.</param>
        /// <param name="resourceAddress">The URI of the accent ResourceDictionary.</param>
        public Accent(string name, Uri resourceAddress)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resourceAddress == null) throw new ArgumentNullException(nameof(resourceAddress));

            this.Name = name;
            this.Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}