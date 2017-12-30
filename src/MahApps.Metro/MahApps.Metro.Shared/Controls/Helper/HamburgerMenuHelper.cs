using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class HamburgerMenuHelper
    {
        /// <summary>
        /// Defines the sidebar brush beside the <see cref="HamburgerMenu"/>.
        /// </summary>
        public static readonly DependencyProperty SideBarBrushProperty =
            DependencyProperty.RegisterAttached("SideBarBrush",
                                                typeof(Brush),
                                                typeof(HamburgerMenuHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetSideBarBrush(UIElement element)
        {
            return (Brush)element.GetValue(SideBarBrushProperty);
        }

        public static void SetSideBarBrush(UIElement element, Brush value)
        {
            element.SetValue(SideBarBrushProperty, value);
        }

        /// <summary>
        /// Defines the sidebar brush below the <see cref="HamburgerMenu"/> of an selected item.
        /// </summary>
        public static readonly DependencyProperty SideBarSelectedBrushProperty =
            DependencyProperty.RegisterAttached("SideBarSelectedBrush",
                                                typeof(Brush),
                                                typeof(HamburgerMenuHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetSideBarSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(SideBarSelectedBrushProperty);
        }

        public static void SetSideBarSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(SideBarSelectedBrushProperty, value);
        }

        /// <summary>
        /// Defines the sidebar brush below the <see cref="HamburgerMenu"/> if the mouse is over an item.
        /// </summary>
        public static readonly DependencyProperty SideBarMouseOverBrushProperty =
            DependencyProperty.RegisterAttached("SideBarMouseOverBrush",
                                                typeof(Brush),
                                                typeof(HamburgerMenuHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetSideBarMouseOverBrush(UIElement element)
        {
            return (Brush)element.GetValue(SideBarMouseOverBrushProperty);
        }

        public static void SetSideBarMouseOverBrush(UIElement element, Brush value)
        {
            element.SetValue(SideBarMouseOverBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="HamburgerMenu"/> if the mouse is over a selected item.
        /// </summary>
        public static readonly DependencyProperty SideBarMouseOverSelectedBrushProperty =
            DependencyProperty.RegisterAttached("SideBarMouseOverSelectedBrush",
                                                typeof(Brush),
                                                typeof(HamburgerMenuHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetSideBarMouseOverSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(SideBarMouseOverSelectedBrushProperty);
        }

        public static void SetSideBarMouseOverSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(SideBarMouseOverSelectedBrushProperty, value);
        }
    }
}
