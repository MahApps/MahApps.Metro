using System;
using System.Windows;

namespace MahApps.Metro
{
    /// <summary>
    /// An object that represents the foreground color for a Metro <see cref="Theme"/>.
    /// </summary>
    public class Accent
    {
        public ResourceDictionary Resources;
        public string Name { get; set; }

        public Accent()
        { }

        public Accent(string name, Uri resourceAddress)
        {
            Name = name;
            Resources = new ResourceDictionary {Source = resourceAddress};
        }
    }
}