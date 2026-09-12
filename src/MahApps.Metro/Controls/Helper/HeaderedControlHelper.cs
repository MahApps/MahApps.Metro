// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class HeaderedControlHelper
    {
        public static readonly DependencyProperty HeaderForegroundProperty
            = DependencyProperty.RegisterAttached(
                "HeaderForeground",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(Brushes.White));

        /// <summary>
        /// Gets the value of the Foreground for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderForegroundProperty);
        }

        /// <summary>
        /// Sets the value of the Foreground for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(HeaderForegroundProperty, value);
        }

        public static readonly DependencyProperty HeaderBackgroundProperty
            = DependencyProperty.RegisterAttached(
                "HeaderBackground",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

        /// <summary>
        /// Gets the value of the Background for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetHeaderBackground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderBackgroundProperty);
        }

        /// <summary>
        /// Sets the value of the Background for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderBackground(UIElement element, Brush value)
        {
            element.SetValue(HeaderBackgroundProperty, value);
        }

        public static readonly DependencyProperty HeaderBackgroundMouseOverProperty
            = DependencyProperty.RegisterAttached(
                "HeaderBackgroundMouseOver",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the value of the background the header is painted with while the mouse is over it. Left unset, the header keeps
        /// the one it has at rest.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush? GetHeaderBackgroundMouseOver(UIElement element)
        {
            return (Brush?)element.GetValue(HeaderBackgroundMouseOverProperty);
        }

        /// <summary>
        /// Sets the value of the background the header is painted with while the mouse is over it.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderBackgroundMouseOver(UIElement element, Brush? value)
        {
            element.SetValue(HeaderBackgroundMouseOverProperty, value);
        }

        public static readonly DependencyProperty HeaderBackgroundPressedProperty
            = DependencyProperty.RegisterAttached(
                "HeaderBackgroundPressed",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the value of the background the header is painted with while it is the one selected. Left unset, the header keeps
        /// the one it has at rest.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush? GetHeaderBackgroundPressed(UIElement element)
        {
            return (Brush?)element.GetValue(HeaderBackgroundPressedProperty);
        }

        /// <summary>
        /// Sets the value of the background the header is painted with while it is the one selected.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderBackgroundPressed(UIElement element, Brush? value)
        {
            element.SetValue(HeaderBackgroundPressedProperty, value);
        }

        public static readonly DependencyProperty HeaderForegroundMouseOverProperty
            = DependencyProperty.RegisterAttached(
                "HeaderForegroundMouseOver",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the value of the foreground the header is painted with while the mouse is over it. Left unset, the header keeps
        /// the one it has at rest.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush? GetHeaderForegroundMouseOver(UIElement element)
        {
            return (Brush?)element.GetValue(HeaderForegroundMouseOverProperty);
        }

        /// <summary>
        /// Sets the value of the foreground the header is painted with while the mouse is over it.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderForegroundMouseOver(UIElement element, Brush? value)
        {
            element.SetValue(HeaderForegroundMouseOverProperty, value);
        }

        public static readonly DependencyProperty HeaderForegroundPressedProperty
            = DependencyProperty.RegisterAttached(
                "HeaderForegroundPressed",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the value of the foreground the header is painted with while it is the one selected. Left unset, the header keeps
        /// the one it has at rest.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush? GetHeaderForegroundPressed(UIElement element)
        {
            return (Brush?)element.GetValue(HeaderForegroundPressedProperty);
        }

        /// <summary>
        /// Sets the value of the foreground the header is painted with while it is the one selected.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderForegroundPressed(UIElement element, Brush? value)
        {
            element.SetValue(HeaderForegroundPressedProperty, value);
        }

        public static readonly DependencyProperty HeaderFontFamilyProperty
            = DependencyProperty.RegisterAttached(
                "HeaderFontFamily",
                typeof(FontFamily),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the FontFamily for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontFamily GetHeaderFontFamily(UIElement element)
        {
            return (FontFamily)element.GetValue(HeaderFontFamilyProperty);
        }

        /// <summary>
        /// Sets the value of the FontFamily for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontFamily(UIElement element, FontFamily value)
        {
            element.SetValue(HeaderFontFamilyProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty
            = DependencyProperty.RegisterAttached(
                "HeaderFontSize",
                typeof(double),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the FontSize for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        /// <summary>
        /// Sets the value of the FontSize for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderFontStretchProperty
            = DependencyProperty.RegisterAttached(
                "HeaderFontStretch",
                typeof(FontStretch),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the FontStretch for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontStretch GetHeaderFontStretch(UIElement element)
        {
            return (FontStretch)element.GetValue(HeaderFontStretchProperty);
        }

        /// <summary>
        /// Sets the value of the FontStretch for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontStretch(UIElement element, FontStretch value)
        {
            element.SetValue(HeaderFontStretchProperty, value);
        }

        public static readonly DependencyProperty HeaderFontWeightProperty
            = DependencyProperty.RegisterAttached(
                "HeaderFontWeight",
                typeof(FontWeight),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the FontWeight for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontWeight GetHeaderFontWeight(UIElement element)
        {
            return (FontWeight)element.GetValue(HeaderFontWeightProperty);
        }

        /// <summary>
        /// Sets the value of the FontWeight for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontWeight(UIElement element, FontWeight value)
        {
            element.SetValue(HeaderFontWeightProperty, value);
        }

        public static readonly DependencyProperty HeaderMarginProperty
            = DependencyProperty.RegisterAttached(
                "HeaderMargin",
                typeof(Thickness),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the outer margin for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Thickness GetHeaderMargin(UIElement element)
        {
            return (Thickness)element.GetValue(HeaderMarginProperty);
        }

        /// <summary>
        /// Sets or sets the outer margin for the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderMargin(UIElement element, Thickness value)
        {
            element.SetValue(HeaderMarginProperty, value);
        }

        public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "HeaderHorizontalContentAlignment",
                typeof(HorizontalAlignment),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Gets the horizontal alignment of the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static HorizontalAlignment GetHeaderHorizontalContentAlignment(UIElement element)
        {
            return (HorizontalAlignment)element.GetValue(HeaderHorizontalContentAlignmentProperty);
        }

        /// <summary>
        /// Sets the horizontal alignment of the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderHorizontalContentAlignment(UIElement element, HorizontalAlignment value)
        {
            element.SetValue(HeaderHorizontalContentAlignmentProperty, value);
        }

        public static readonly DependencyProperty HeaderVerticalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "HeaderVerticalContentAlignment",
                typeof(VerticalAlignment),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(VerticalAlignment.Stretch));

        /// <summary>
        /// Gets the vertical alignment of the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static VerticalAlignment GetHeaderVerticalContentAlignment(UIElement element)
        {
            return (VerticalAlignment)element.GetValue(HeaderVerticalContentAlignmentProperty);
        }

        /// <summary>
        /// Sets the vertical alignment of the header.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ColorPalette))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderVerticalContentAlignment(UIElement element, VerticalAlignment value)
        {
            element.SetValue(HeaderVerticalContentAlignmentProperty, value);
        }
    }
}