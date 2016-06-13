using System;
using System.Windows;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Icon from the FontAwesome Icons project, <see><cref>http://fontawesome.io</cref></see>.
    /// </summary>
    public class PackIconFontAwesome : PackIconBase<PackIconFontAwesomeKind>
    {        
        static PackIconFontAwesome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconFontAwesome), new FrameworkPropertyMetadata(typeof(PackIconFontAwesome)));
        }     

        public PackIconFontAwesome() : base(PackIconFontAwesomeDataFactory.Create)
        { }    
    }
}
