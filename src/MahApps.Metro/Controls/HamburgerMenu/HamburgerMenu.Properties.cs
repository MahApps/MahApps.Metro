// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    [StyleTypedProperty(Property = nameof(ResizeThumbStyle), StyleTargetType = typeof(MetroThumb))]
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    [StyleTypedProperty(Property = nameof(HeaderItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    [StyleTypedProperty(Property = nameof(SeparatorItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    [StyleTypedProperty(Property = nameof(ItemFocusVisualStyle), StyleTargetType = typeof(Control))]
    public partial class HamburgerMenu
    {
        private ControlTemplate _defaultItemFocusVisualTemplate = null;

        /// <summary>Identifies the <see cref="OpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty OpenPaneLengthProperty
            = DependencyProperty.Register(nameof(OpenPaneLength),
                                          typeof(double),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(240.0, OpenPaneLengthPropertyChangedCallback, OnOpenPaneLengthCoerceValueCallback));

        private static object OnOpenPaneLengthCoerceValueCallback(DependencyObject dependencyObject, object inputValue)
        {
            if (dependencyObject is HamburgerMenu hamburgerMenu && hamburgerMenu.ActualWidth > 0 && inputValue is double openPaneLength)
            {
                // Get the minimum needed width
                var minWidth = hamburgerMenu.DisplayMode == SplitViewDisplayMode.CompactInline || hamburgerMenu.DisplayMode == SplitViewDisplayMode.CompactOverlay
                    ? Math.Max(hamburgerMenu.CompactPaneLength, hamburgerMenu.MinimumOpenPaneLength)
                    : Math.Max(0, hamburgerMenu.MinimumOpenPaneLength);

                if (minWidth < 0)
                {
                    minWidth = 0;
                }

                // Get the maximum allowed width
                var maxWidth = Math.Min(hamburgerMenu.ActualWidth, hamburgerMenu.MaximumOpenPaneLength);

                // Check if max < min
                if (maxWidth < minWidth)
                {
                    minWidth = maxWidth;
                }

                // Check is OpenPaneLength is valid
                if (openPaneLength < minWidth)
                {
                    return minWidth;
                }

                if (openPaneLength > maxWidth)
                {
                    return maxWidth;
                }

                return openPaneLength;
            }
            else
            {
                return inputValue;
            }
        }

        private static void OpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Gets or sets the width of the pane when it's fully expanded.
        /// </summary>
        public double OpenPaneLength
        {
            get => (double)this.GetValue(OpenPaneLengthProperty);
            set => this.SetValue(OpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="CompactPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty CompactPaneLengthProperty
            = DependencyProperty.Register(nameof(CompactPaneLength),
                                          typeof(double), typeof(HamburgerMenu),
                                          new PropertyMetadata(48.0, OnCompactPaneLengthPropertyChangedCallback));

        private static void OnCompactPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.CoerceValue(OpenPaneLengthProperty);
                hamburgerMenu.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Gets or sets the width of the pane when in its compact display mode.
        /// </summary>
        public double CompactPaneLength
        {
            get => (double)this.GetValue(CompactPaneLengthProperty);
            set => this.SetValue(CompactPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="MinimumOpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumOpenPaneLengthProperty
            = DependencyProperty.Register(nameof(MinimumOpenPaneLength),
                                          typeof(double),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(100d, MinimumOpenPaneLengthPropertyChangedCallback));

        private static void MinimumOpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.CoerceValue(OpenPaneLengthProperty);
                hamburgerMenu.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        public double MinimumOpenPaneLength
        {
            get => (double)this.GetValue(MinimumOpenPaneLengthProperty);
            set => this.SetValue(MinimumOpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="MaximumOpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumOpenPaneLengthProperty
            = DependencyProperty.Register(nameof(MaximumOpenPaneLength),
                                          typeof(double),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(500d, OnMaximumOpenPaneLengthPropertyChangedCallback));

        private static void OnMaximumOpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.CoerceValue(OpenPaneLengthProperty);
                hamburgerMenu.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        public double MaximumOpenPaneLength
        {
            get => (double)this.GetValue(MaximumOpenPaneLengthProperty);
            set => this.SetValue(MaximumOpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="CanResizeOpenPane"/> dependency property.</summary>
        public static readonly DependencyProperty CanResizeOpenPaneProperty
            = DependencyProperty.Register(nameof(CanResizeOpenPane),
                                          typeof(bool),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnCanResizeOpenPanePropertyChangedCallback));

        private static void OnCanResizeOpenPanePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.CoerceValue(OpenPaneLengthProperty);
            }
        }

        /// <summary>
        /// Gets or Sets whether the open pane can be resized by the user. The default value is false.
        /// </summary>
        public bool CanResizeOpenPane
        {
            get => (bool)this.GetValue(CanResizeOpenPaneProperty);
            set => this.SetValue(CanResizeOpenPaneProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ResizeThumbStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ResizeThumbStyleProperty
            = DependencyProperty.Register(nameof(ResizeThumbStyle),
                                          typeof(Style),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the <see cref="Style"/> for the resizing Thumb (type of <see cref="MetroThumb"/>)
        /// </summary>
        public Style ResizeThumbStyle
        {
            get => (Style)this.GetValue(ResizeThumbStyleProperty);
            set => this.SetValue(ResizeThumbStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="PanePlacement"/> dependency property.</summary>
        public static readonly DependencyProperty PanePlacementProperty
            = DependencyProperty.Register(nameof(PanePlacement),
                                          typeof(SplitViewPanePlacement),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(SplitViewPanePlacement.Left));

        /// <summary>
        /// Gets or sets a value that specifies whether the pane is shown on the right or on left side of the HamburgerMenu.
        /// </summary>
        public SplitViewPanePlacement PanePlacement
        {
            get => (SplitViewPanePlacement)this.GetValue(PanePlacementProperty);
            set => this.SetValue(PanePlacementProperty, value);
        }

        /// <summary>Identifies the <see cref="DisplayMode"/> dependency property.</summary>
        public static readonly DependencyProperty DisplayModeProperty
            = DependencyProperty.Register(nameof(DisplayMode),
                                          typeof(SplitViewDisplayMode),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(SplitViewDisplayMode.CompactInline, OnDisplayModePropertyChangedCallback));

        private static void OnDisplayModePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is SplitViewDisplayMode && dependencyObject is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.CoerceValue(OpenPaneLengthProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how the pane and content areas are shown.
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            get => (SplitViewDisplayMode)this.GetValue(DisplayModeProperty);
            set => this.SetValue(DisplayModeProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneMargin"/> dependency property.</summary>
        public static readonly DependencyProperty PaneMarginProperty
            = DependencyProperty.Register(nameof(PaneMargin),
                                          typeof(Thickness),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the margin for the <see cref="SplitView"/> pane.
        /// </summary>
        public Thickness PaneMargin
        {
            get => (Thickness)this.GetValue(PaneMarginProperty);
            set => this.SetValue(PaneMarginProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneHeaderMargin"/> dependency property.</summary>
        public static readonly DependencyProperty PaneHeaderMarginProperty
            = DependencyProperty.Register(nameof(PaneHeaderMargin),
                                          typeof(Thickness),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the margin for the pane header.
        /// </summary>
        public Thickness PaneHeaderMargin
        {
            get => (Thickness)this.GetValue(PaneHeaderMarginProperty);
            set => this.SetValue(PaneHeaderMarginProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneBackground"/> dependency property.</summary>
        public static readonly DependencyProperty PaneBackgroundProperty
            = DependencyProperty.Register(nameof(PaneBackground),
                                          typeof(Brush),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Brush to apply to the background of the pane area of the HamburgerMenu.
        /// </summary>
        public Brush PaneBackground
        {
            get => (Brush)this.GetValue(PaneBackgroundProperty);
            set => this.SetValue(PaneBackgroundProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneForeground"/> dependency property.</summary>
        public static readonly DependencyProperty PaneForegroundProperty
            = DependencyProperty.Register(nameof(PaneForeground),
                                          typeof(Brush),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Brush to apply to the foreground of the pane area of the HamburgerMenu.
        /// </summary>
        public Brush PaneForeground
        {
            get => (Brush)this.GetValue(PaneForegroundProperty);
            set => this.SetValue(PaneForegroundProperty, value);
        }

        /// <summary>Identifies the <see cref="IsPaneOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsPaneOpenProperty
            = DependencyProperty.Register(nameof(IsPaneOpen),
                                          typeof(bool),
                                          typeof(HamburgerMenu),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsPaneOpenPropertyChangedCallback));

        private static void IsPaneOpenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                (dependencyObject as HamburgerMenu)?.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pane is expanded to its full width.
        /// </summary>
        public bool IsPaneOpen
        {
            get => (bool)this.GetValue(IsPaneOpenProperty);
            set => this.SetValue(IsPaneOpenProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(nameof(ItemsSource),
                                          typeof(object),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets an object source used to generate the content of the menu.
        /// </summary>
        public object ItemsSource
        {
            get => this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ItemContainerStyleProperty
            = DependencyProperty.Register(nameof(ItemContainerStyle),
                                          typeof(Style),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Style"/> used for each item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get => (Style)this.GetValue(ItemContainerStyleProperty);
            set => this.SetValue(ItemContainerStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="HeaderItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderItemContainerStyleProperty
            = DependencyProperty.Register(nameof(HeaderItemContainerStyle),
                                          typeof(Style),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>   
        /// Gets or sets the <see cref="Style"/> used for each header item.
        /// </summary>
        public Style HeaderItemContainerStyle
        {
            get => (Style)this.GetValue(HeaderItemContainerStyleProperty);
            set => this.SetValue(HeaderItemContainerStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="SeparatorItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorItemContainerStyleProperty
            = DependencyProperty.Register(nameof(SeparatorItemContainerStyle),
                                          typeof(Style),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>   
        /// Gets or sets the <see cref="Style"/> used for each separator item.
        /// </summary>
        public Style SeparatorItemContainerStyle
        {
            get => (Style)this.GetValue(SeparatorItemContainerStyleProperty);
            set => this.SetValue(SeparatorItemContainerStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ItemTemplateProperty
            = DependencyProperty.Register(nameof(ItemTemplate),
                                          typeof(DataTemplate),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => this.SetValue(ItemTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty
            = DependencyProperty.Register(nameof(ItemTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> used to display each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(ItemTemplateSelectorProperty);
            set => this.SetValue(ItemTemplateSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register(nameof(SelectedItem),
                                          typeof(object),
                                          typeof(HamburgerMenu),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or sets the selected menu item.
        /// </summary>
        public object SelectedItem
        {
            get => this.GetValue(SelectedItemProperty);
            set => this.SetValue(SelectedItemProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedIndex"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedIndexProperty
            = DependencyProperty.Register(nameof(SelectedIndex),
                                          typeof(int),
                                          typeof(HamburgerMenu),
                                          new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>
        /// Gets or sets the selected menu index.
        /// </summary>
        public int SelectedIndex
        {
            get => (int)this.GetValue(SelectedIndexProperty);
            set => this.SetValue(SelectedIndexProperty, value);
        }

        /// <summary>Identifies the <see cref="ContentTransition"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTransitionProperty
            = DependencyProperty.Register(nameof(ContentTransition),
                                          typeof(TransitionType),
                                          typeof(HamburgerMenu),
                                          new FrameworkPropertyMetadata(TransitionType.Normal));

        /// <summary>
        /// Gets or sets the <see cref="TransitionType"/> for the <see cref="TransitioningContentControl"/> which shows the selected menu item content.
        /// </summary>
        public TransitionType ContentTransition
        {
            get => (TransitionType)this.GetValue(ContentTransitionProperty);
            set => this.SetValue(ContentTransitionProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemCommand"/> dependency property.</summary>
        public static readonly DependencyProperty ItemCommandProperty
            = DependencyProperty.Register(nameof(ItemCommand),
                                          typeof(ICommand),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a <see cref="ICommand"/> which will be executed if an item was clicked by the user.
        /// </summary>
        public ICommand ItemCommand
        {
            get => (ICommand)this.GetValue(ItemCommandProperty);
            set => this.SetValue(ItemCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty ItemCommandParameterProperty
            = DependencyProperty.Register(nameof(ItemCommandParameter),
                                          typeof(object),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> parameter which will be passed by the <see cref="ItemCommand"/>.
        /// </summary>
        public object ItemCommandParameter
        {
            get => (object)this.GetValue(ItemCommandParameterProperty);
            set => this.SetValue(ItemCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="VerticalScrollBarOnLeftSide"/> dependency property.</summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty
            = DependencyProperty.Register(nameof(VerticalScrollBarOnLeftSide),
                                          typeof(bool),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the ScrollBar of the HamburgerMenu is on the left side or on the right side of the menu items.
        /// </summary>
        public bool VerticalScrollBarOnLeftSide
        {
            get => (bool)this.GetValue(VerticalScrollBarOnLeftSideProperty);
            set => this.SetValue(VerticalScrollBarOnLeftSideProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowSelectionIndicator"/> dependency property.</summary>
        public static readonly DependencyProperty ShowSelectionIndicatorProperty
            = DependencyProperty.Register(nameof(ShowSelectionIndicator),
                                          typeof(bool),
                                          typeof(HamburgerMenu),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether a selection indicator will be shown on the HamburgerMenuItem.
        /// </summary>
        public bool ShowSelectionIndicator
        {
            get => (bool)this.GetValue(ShowSelectionIndicatorProperty);
            set => this.SetValue(ShowSelectionIndicatorProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ItemFocusVisualStyle"/> dependency property.</summary>
        private static readonly DependencyPropertyKey ItemFocusVisualStylePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(ItemFocusVisualStyle),
                                                  typeof(Style),
                                                  typeof(HamburgerMenu),
                                                  new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ItemFocusVisualStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ItemFocusVisualStyleProperty = ItemFocusVisualStylePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the default FocusVisualStyle for a HamburgerMenuItem.
        /// This style can be override at the HamburgerMenuItem style by setting the FocusVisualStyle property.
        /// </summary>
        public Style ItemFocusVisualStyle
        {
            get => (Style)this.GetValue(ItemFocusVisualStyleProperty);
            protected set => this.SetValue(ItemFocusVisualStylePropertyKey, value);
        }

        /// <summary>
        /// Gets the collection used to generate the content of the items list.
        /// </summary>
        /// <exception cref="Exception">
        /// Exception thrown if ButtonsListView is not yet defined.
        /// </exception>
        public ItemCollection Items
        {
            get
            {
                if (this.buttonsListView == null)
                {
                    throw new Exception("ButtonsListView is not defined yet. Please use ItemsSource instead.");
                }

                return this.buttonsListView.Items;
            }
        }

        /// <summary>
        /// Executes the <see cref="ItemCommand"/>.
        /// </summary>
        public void RaiseItemCommand()
        {
            var command = this.ItemCommand;
            var commandParameter = this.ItemCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        private void ChangeItemFocusVisualStyle()
        {
            this._defaultItemFocusVisualTemplate = this._defaultItemFocusVisualTemplate ?? this.TryFindResource("MahApps.Templates.HamburgerMenuItem.FocusVisual") as ControlTemplate;
            if (this._defaultItemFocusVisualTemplate != null)
            {
                var focusVisualStyle = new Style(typeof(Control));
                focusVisualStyle.Setters.Add(new Setter(TemplateProperty, this._defaultItemFocusVisualTemplate));
                focusVisualStyle.Setters.Add(new Setter(WidthProperty, this.IsPaneOpen ? this.OpenPaneLength : this.CompactPaneLength));
                focusVisualStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
                focusVisualStyle.Seal();

                this.SetValue(ItemFocusVisualStylePropertyKey, focusVisualStyle);
            }
        }
    }
}