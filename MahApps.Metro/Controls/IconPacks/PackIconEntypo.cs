using System.Windows;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Icons from Entypo+ Icons Font - <see><cref>http://www.entypo.com</cref></see>.
    /// </summary>
    public class PackIconEntypo : PackIconBase<PackIconEntypoKind>
    {        
        static PackIconEntypo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconEntypo), new FrameworkPropertyMetadata(typeof(PackIconEntypo)));
        }     

        public PackIconEntypo() : base(PackIconEntypoDataFactory.Create)
        { }    
    }
}
