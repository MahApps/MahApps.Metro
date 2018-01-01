using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class ListItemHelper
    {
        /// <summary>
        /// Gets or sets the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        public static readonly DependencyProperty ActiveSelectionBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("ActiveSelectionBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetActiveSelectionBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(ActiveSelectionBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the brush the background brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
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
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the brush the foreground brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetActiveSelectionForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(ActiveSelectionForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the brush the foreground brush which will be used for the active selected item (if the keyboard focus is within).
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
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
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetSelectedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(SelectedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
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
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetSelectedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(SelectedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for a selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static void SetSelectedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(SelectedForegroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for an mouse hovered item.
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("MouseOverBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetMouseOverBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(MouseOverBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static void SetMouseOverBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(MouseOverBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used for selected disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledSelectedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledSelectedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the background brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetDisabledSelectedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledSelectedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
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
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetDisabledSelectedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledSelectedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for selected disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static void SetDisabledSelectedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledSelectedForegroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used for disabled items.
        /// </summary>
        public static readonly DependencyProperty DisabledForegroundBrushProperty
            = DependencyProperty.RegisterAttached("DisabledForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ListItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static Brush GetDisabledForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(DisabledForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for disabled items.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        public static void SetDisabledForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(DisabledForegroundBrushProperty, value);
        }
    }
}