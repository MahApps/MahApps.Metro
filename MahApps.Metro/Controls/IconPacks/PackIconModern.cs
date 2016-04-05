using System;
using System.Windows;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Icon from the Material Design Icons project, <see><cref>https://materialdesignicons.com/</cref></see>.
    /// </summary>
    public class PackIconModern : PackIconBase<PackIconModernKind>
    {        
        static PackIconModern()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconModern), new FrameworkPropertyMetadata(typeof(PackIconModern)));
        }     

        public PackIconModern() : base(PackIconModernDataFactory.Create)
        { }    
    }
}
