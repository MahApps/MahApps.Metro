// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// Gets or sets the foreground brush which will be used for an mouse hovered item.
        /// </summary>
        public static readonly DependencyProperty HoverForegroundBrushProperty
            = DependencyProperty.RegisterAttached("HoverForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetHoverForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(HoverForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for an mouse hovered item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetHoverForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(HoverForegroundBrushProperty, value);
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
        /// Gets or sets the foreground brush which will be used for an mouse hovered and selected item.
        /// </summary>
        public static readonly DependencyProperty HoverSelectedForegroundBrushProperty
            = DependencyProperty.RegisterAttached("HoverSelectedForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the foreground brush which will be used for an mouse hovered and selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetHoverSelectedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(HoverSelectedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used for an mouse hovered and selected item.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetHoverSelectedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(HoverSelectedForegroundBrushProperty, value);
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

        private static readonly DependencyPropertyKey IsMouseLeftButtonPressedPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("IsMouseLeftButtonPressed",
                                                          typeof(bool),
                                                          typeof(ItemHelper),
                                                          new PropertyMetadata(false));

        public static readonly DependencyProperty IsMouseLeftButtonPressedProperty = IsMouseLeftButtonPressedPropertyKey.DependencyProperty;

        public static bool GetIsMouseLeftButtonPressed(UIElement element)
        {
            return (bool)element.GetValue(IsMouseLeftButtonPressedProperty);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        public static readonly DependencyProperty MouseLeftButtonPressedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("MouseLeftButtonPressedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                OnMouseLeftButtonPressedBackgroundBrushPropertyChanged));

        private static void OnMouseLeftButtonPressedBackgroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && e.OldValue != e.NewValue)
            {
                element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                element.MouseEnter -= OnLeftMouseEnter;
                element.MouseLeave -= OnLeftMouseLeave;

                if (e.NewValue is Brush || GetMouseLeftButtonPressedForegroundBrush(element) != null)
                {
                    element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                    element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                    element.MouseEnter += OnLeftMouseEnter;
                    element.MouseLeave += OnLeftMouseLeave;
                }
            }
        }

        private static void OnLeftMouseEnter(object sender, MouseEventArgs e)
        {
            var element = (UIElement)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                element.SetValue(IsMouseLeftButtonPressedPropertyKey, true);
            }
        }

        private static void OnLeftMouseLeave(object sender, MouseEventArgs e)
        {
            var element = (UIElement)sender;
            if (e.LeftButton == MouseButtonState.Pressed && GetIsMouseLeftButtonPressed(element))
            {
                element.SetValue(IsMouseLeftButtonPressedPropertyKey, false);
            }
        }

        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                element.SetValue(IsMouseLeftButtonPressedPropertyKey, true);
            }
        }

        private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            if (GetIsMouseLeftButtonPressed(element))
            {
                element.SetValue(IsMouseLeftButtonPressedPropertyKey, false);
            }
        }

        /// <summary>
        /// Gets the background brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetMouseLeftButtonPressedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(MouseLeftButtonPressedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetMouseLeftButtonPressedBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(MouseLeftButtonPressedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        public static readonly DependencyProperty MouseLeftButtonPressedForegroundBrushProperty
            = DependencyProperty.RegisterAttached("MouseLeftButtonPressedForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                OnMouseLeftButtonPressedForegroundBrushPropertyChanged));

        private static void OnMouseLeftButtonPressedForegroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && e.OldValue != e.NewValue)
            {
                element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                element.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                element.MouseEnter -= OnLeftMouseEnter;
                element.MouseLeave -= OnLeftMouseLeave;

                if (e.NewValue is Brush || GetMouseLeftButtonPressedBackgroundBrush(element) != null)
                {
                    element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                    element.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                    element.MouseEnter += OnLeftMouseEnter;
                    element.MouseLeave += OnLeftMouseLeave;
                }
            }
        }

        /// <summary>
        /// Gets the foreground brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetMouseLeftButtonPressedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(MouseLeftButtonPressedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used when an item is pressed by the left mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetMouseLeftButtonPressedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(MouseLeftButtonPressedForegroundBrushProperty, value);
        }

        private static readonly DependencyPropertyKey IsMouseRightButtonPressedPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("IsMouseRightButtonPressed",
                                                          typeof(bool),
                                                          typeof(ItemHelper),
                                                          new PropertyMetadata(false));

        public static readonly DependencyProperty IsMouseRightButtonPressedProperty = IsMouseRightButtonPressedPropertyKey.DependencyProperty;

        public static bool GetIsMouseRightButtonPressed(UIElement element)
        {
            return (bool)element.GetValue(IsMouseRightButtonPressedProperty);
        }

        /// <summary>
        /// Gets or sets the background brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        public static readonly DependencyProperty MouseRightButtonPressedBackgroundBrushProperty
            = DependencyProperty.RegisterAttached("MouseRightButtonPressedBackgroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                OnMouseRightButtonPressedBackgroundBrushPropertyChanged));

        private static void OnMouseRightButtonPressedBackgroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && e.OldValue != e.NewValue)
            {
                element.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
                element.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;

                if (e.NewValue is Brush || GetMouseRightButtonPressedForegroundBrush(element) != null)
                {
                    element.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
                    element.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
                }
            }
        }

        private static void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (element is TreeViewItem)
                {
                    Mouse.Capture(element, CaptureMode.SubTree);
                }
                else
                {
                    Mouse.Capture(element);
                }

                element.SetValue(IsMouseRightButtonPressedPropertyKey, true);
            }
        }

        private static void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            if (GetIsMouseRightButtonPressed(element))
            {
                Mouse.Capture(null);
                element.SetValue(IsMouseRightButtonPressedPropertyKey, false);
            }
        }

        /// <summary>
        /// Gets the background brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetMouseRightButtonPressedBackgroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(MouseRightButtonPressedBackgroundBrushProperty);
        }

        /// <summary>
        /// Sets the background brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetMouseRightButtonPressedBackgroundBrush(UIElement element, Brush value)
        {
            element.SetValue(MouseRightButtonPressedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        public static readonly DependencyProperty MouseRightButtonPressedForegroundBrushProperty
            = DependencyProperty.RegisterAttached("MouseRightButtonPressedForegroundBrush",
                                                  typeof(Brush),
                                                  typeof(ItemHelper),
                                                  new FrameworkPropertyMetadata(default(Brush),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                OnMouseRightButtonPressedForegroundBrushPropertyChanged));

        private static void OnMouseRightButtonPressedForegroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && e.OldValue != e.NewValue)
            {
                element.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
                element.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;

                if (e.NewValue is Brush || GetMouseRightButtonPressedBackgroundBrush(element) != null)
                {
                    element.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
                    element.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
                }
            }
        }

        /// <summary>
        /// Gets the foreground brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Brush GetMouseRightButtonPressedForegroundBrush(UIElement element)
        {
            return (Brush)element.GetValue(MouseRightButtonPressedForegroundBrushProperty);
        }

        /// <summary>
        /// Sets the foreground brush which will be used when an item is pressed by the right mouse button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetMouseRightButtonPressedForegroundBrush(UIElement element, Brush value)
        {
            element.SetValue(MouseRightButtonPressedForegroundBrushProperty, value);
        }
    }
}