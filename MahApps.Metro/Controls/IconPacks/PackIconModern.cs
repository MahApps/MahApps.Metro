using System.Windows;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Icons from the Modern UI Icons project, <see><cref>http://modernuiicons.com</cref></see>.
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
