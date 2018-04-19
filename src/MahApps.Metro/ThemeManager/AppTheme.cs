// ReSharper disable once CheckNamespace
namespace MahApps.Metro
{
    using System;
    using System.Diagnostics;
    using System.Windows;

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
        public ResourceDictionary Resources {get; }

        /// <summary>
        /// Gets the name of the application theme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the AppTheme class.
        /// </summary>
        /// <param name="name">The name of the new AppTheme.</param>
        /// <param name="resourceAddress">The URI of the accent ResourceDictionary.</param>
        public AppTheme(string name, Uri resourceAddress)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (resourceAddress == null) throw new ArgumentNullException(nameof(resourceAddress));

            this.Name = name;
            this.Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}