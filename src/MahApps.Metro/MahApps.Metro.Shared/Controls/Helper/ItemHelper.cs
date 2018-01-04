using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class ItemHelper
    {
        /// <summary>
        /// Gets or sets the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        public static readonly DependencyProperty ActiveSelectionBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("ActiveSelectionBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetActiveSelectionBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(ActiveSelectionBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the brush the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetActiveSelectionBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(ActiveSelectionBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        public static readonly DependencyProperty ActiveSelectionForegroundBrushProperty
            = DependencyProperty.RegisterAttached("ActiveSelectionForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush the foreground brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetActiveSelectionForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(ActiveSelectionForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the brush the foreground brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetActiveSelectionForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(ActiveSelectionForegroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for a selected item.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("SelectedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetSelectedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(SelectedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetSelectedBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(SelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used for a selected item.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundBrushProperty
            = DependencyProperty.RegisterAttached("SelectedForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetSelectedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(SelectedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetSelectedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(SelectedForegroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for an mouse hovered item.
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("HoverBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetHoverBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(HoverBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetHoverBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(HoverBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for an mouse hovered and selected item.
        /// </summary>
        public static readonly DependencyProperty HoverSelectedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("HoverSelectedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for an mouse hovered and selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetHoverSelectedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(HoverSelectedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for an mouse hovered and selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetHoverSelectedBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(HoverSelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for selected disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledSelectedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledSelectedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetDisabledSelectedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledSelectedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetDisabledSelectedBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledSelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used for selected disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledSelectedForegroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledSelectedForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetDisabledSelectedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledSelectedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetDisabledSelectedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledSelectedForegroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetDisabledBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetDisabledBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used for disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledForegroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetDisabledForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetDisabledForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledForegroundBrushProperty, value);
        }
    }
}