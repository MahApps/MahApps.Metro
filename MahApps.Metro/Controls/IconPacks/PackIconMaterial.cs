using System;
using System.Windows;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Icon from the Material Design Icons project, <see><cref>https://materialdesignicons.com/</cref></see>.
    /// </summary>
    public class PackIconMaterial : PackIconBase<PackIconMaterialKind>
    {        
        static PackIconMaterial()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconMaterial), new FrameworkPropertyMetadata(typeof(PackIconMaterial)));
        }     

        public PackIconMaterial() : base(PackIconMaterialDataFactory.Create)
        { }    
    }
}
