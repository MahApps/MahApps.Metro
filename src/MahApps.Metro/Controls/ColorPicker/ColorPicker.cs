using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ColorPaletteStandard", Type = typeof(ColorPalette))]
    [TemplatePart(Name = "PART_ColorPaletteAvailable", Type = typeof(ColorPalette))]
    [TemplatePart(Name = "PART_ColorPaletteCustom01", Type = typeof(ColorPalette))]
    [TemplatePart(Name = "PART_ColorPaletteCustom02", Type = typeof(ColorPalette))]
    [TemplatePart(Name = "PART_ColorPaletteRecent", Type = typeof(ColorPalette))]
    [TemplatePart(Name = "PART_PopupTabControl", Type = typeof(TabControl))]
    [TemplatePart(Name = "PART_ColorPalettesTab", Type = typeof(TabItem))]
    [TemplatePart(Name = "PART_AdvancedTab", Type = typeof(TabItem))]
    [StyleTypedProperty(Property = nameof(StandardColorPaletteStyle), StyleTargetType = typeof(ColorPalette))]
    [StyleTypedProperty(Property = nameof(AvailableColorPaletteStyle), StyleTargetType = typeof(ColorPalette))]
    [StyleTypedProperty(Property = nameof(CustomColorPalette01Style), StyleTargetType = typeof(ColorPalette))]
    [StyleTypedProperty(Property = nameof(RecentColorPaletteStyle), StyleTargetType = typeof(ColorPalette))]
    [StyleTypedProperty(Property = nameof(CustomColorPalette02Style), StyleTargetType = typeof(ColorPalette))]
    [StyleTypedProperty(Property = nameof(TabControlStyle), StyleTargetType = typeof(TabControl))]
    [StyleTypedProperty(Property = nameof(TabItemStyle), StyleTargetType = typeof(TabItem))]
    public class ColorPicker : ColorPickerBase
    {
        /// <summary>Identifies the <see cref="DropDownClosed"/> routed event.</summary>
        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent(
                                                                nameof(DropDownClosed),
                                                                RoutingStrategy.Bubble,
                                                                typeof(EventHandler<EventArgs>),
                                                                typeof(ColorPicker));

        /// <summary>Identifies the <see cref="DropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(ColorPicker), new PropertyMetadata(300d));

        /// <summary>Identifies the <see cref="DropDownOpened"/> routed event.</summary>
        public static readonly RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(DropDownOpened),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(EventHandler<EventArgs>),
                                                                        typeof(ColorPicker));

        /// <summary>Identifies the <see cref="DropDownWidth"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register(nameof(DropDownWidth), typeof(double), typeof(ColorPicker), new PropertyMetadata(300d));

        /// <summary>Identifies the <see cref="IsDropDownOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

        /// <summary>Identifies the <see cref="MaxDropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(ColorPicker), new FrameworkPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3));

        /// <summary>Identifies the <see cref="MaxDropDownWidth"/> dependency property.</summary>
        public static readonly DependencyProperty MaxDropDownWidthProperty = DependencyProperty.Register(nameof(MaxDropDownWidth), typeof(double), typeof(ColorPicker), new PropertyMetadata(MaxWidthProperty.DefaultMetadata.DefaultValue));

        /// <summary>Identifies the <see cref="SelectedColorTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorTemplateProperty = DependencyProperty.Register(nameof(SelectedColorTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="AddToRecentColorsTrigger"/> dependency property.</summary>
        public static readonly DependencyProperty AddToRecentColorsTriggerProperty = DependencyProperty.Register(nameof(AddToRecentColorsTrigger), typeof(AddToRecentColorsTrigger), typeof(ColorPicker), new PropertyMetadata(AddToRecentColorsTrigger.ColorPickerClosed));

        private Popup PART_Popup;
        private ColorPalette PART_ColorPaletteStandard;
        private ColorPalette PART_ColorPaletteAvailable;
        private ColorPalette PART_ColorPaletteCustom01;
        private ColorPalette PART_ColorPaletteCustom02;
        private ColorPalette PART_ColorPaletteRecent;
        private TabControl PART_PopupTabControl;
        private TabItem PART_ColorPalettesTab;
        private TabItem PART_AdvancedTab;

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
            EventManager.RegisterClassHandler(typeof(ColorPicker), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(ColorPicker), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true); // call us even if the transparent button in the style gets the click.
        }

        /// <summary>
        ///     Occurs when the DropDown is closed.
        /// </summary>
        public event EventHandler<EventArgs> DropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        /// <summary>
        ///     Occurs when the DropDown is opened.
        /// </summary>
        public event EventHandler<EventArgs> DropDownOpened
        {
            add { AddHandler(DropDownOpenedEvent, value); }
            remove { RemoveHandler(DropDownOpenedEvent, value); }
        }

        /// <summary>
        /// The width of the DropDown
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// The width of the DropDown
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }

        /// <summary>
        /// Whether or not the "popup" for this control is currently open
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        ///     The maximum height of the popup
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// The maximum width of the DropDown
        /// </summary>
        public double MaxDropDownWidth
        {
            get { return (double)GetValue(MaxDropDownWidthProperty); }
            set { SetValue(MaxDropDownWidthProperty, value); }
        }

        public DataTemplate SelectedColorTemplate
        {
            get { return (DataTemplate)GetValue(SelectedColorTemplateProperty); }
            set { SetValue(SelectedColorTemplateProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets when to add the <see cref="ColorPickerBase.SelectedColor"/> to the <see cref="RecentColorPaletteItemsSource"/>
        /// </summary>
        public AddToRecentColorsTrigger AddToRecentColorsTrigger
        {
            get { return (AddToRecentColorsTrigger)GetValue(AddToRecentColorsTriggerProperty); }
            set { SetValue(AddToRecentColorsTriggerProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            PART_Popup = (Popup)this.GetTemplateChild(nameof(PART_Popup));

            PART_ColorPaletteStandard = this.GetTemplateChild(nameof(PART_ColorPaletteStandard)) as ColorPalette;
            PART_ColorPaletteAvailable = this.GetTemplateChild(nameof(PART_ColorPaletteAvailable)) as ColorPalette;
            PART_ColorPaletteCustom01 = this.GetTemplateChild(nameof(PART_ColorPaletteCustom01)) as ColorPalette;
            PART_ColorPaletteCustom02 = this.GetTemplateChild(nameof(PART_ColorPaletteCustom02)) as ColorPalette;
            PART_ColorPaletteRecent = this.GetTemplateChild(nameof(PART_ColorPaletteCustom02)) as ColorPalette;

            PART_PopupTabControl = this.GetTemplateChild(nameof(PART_PopupTabControl)) as TabControl;
            PART_ColorPalettesTab = this.GetTemplateChild(nameof(PART_ColorPalettesTab)) as TabItem;
            PART_AdvancedTab = this.GetTemplateChild(nameof(PART_AdvancedTab)) as TabItem;

            base.OnApplyTemplate();

            ValidateTabItems();
        }


        internal override void OnSelectedColorChanged(Color? OldValue, Color? NewValue)
        {
            base.OnSelectedColorChanged(NewValue, OldValue);

            if (this.AddToRecentColorsTrigger == AddToRecentColorsTrigger.SelectedColorChanged && SelectedColor.HasValue)
            {
                BuildInColorPalettes.AddColorToRecentColors(NewValue, RecentColorPaletteItemsSource);
                BuildInColorPalettes.ReduceRecentColors(BuildInColorPalettes.GetMaximumRecentColorsCount(this), RecentColorPaletteItemsSource as ObservableCollection<Color?>);
            }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker colorPicker)
            {
                if ((bool)e.NewValue)
                {
                    colorPicker.RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent));

                    

                    var action = new Action(() =>
                    {
                        colorPicker.Focus();

                        Mouse.Capture(colorPicker, CaptureMode.SubTree);

                        bool isFocused = false;

                        colorPicker.ValidateTabItems();

                        if (colorPicker.PART_PopupTabControl.SelectedItem == colorPicker.PART_ColorPalettesTab)
                        {
                            if (!isFocused && colorPicker.IsStandardColorPaletteVisible && colorPicker.PART_ColorPaletteStandard != null)
                            {
                                isFocused = colorPicker.PART_ColorPaletteStandard.FocusSelectedItem();
                            }
                            else if (!isFocused && colorPicker.IsAvailableColorPaletteVisible && colorPicker.PART_ColorPaletteAvailable != null)
                            {
                                isFocused = colorPicker.PART_ColorPaletteAvailable.FocusSelectedItem();
                            }
                            else if (!isFocused && colorPicker.IsCustomColorPalette01Visible && colorPicker.PART_ColorPaletteCustom01 != null)
                            {
                                isFocused = colorPicker.PART_ColorPaletteCustom01.FocusSelectedItem();
                            }
                            else if (!isFocused && colorPicker.IsCustomColorPalette02Visible && colorPicker.PART_ColorPaletteCustom02 != null)
                            {
                                isFocused = colorPicker.PART_ColorPaletteCustom02.FocusSelectedItem();
                            }
                            else if (!isFocused && colorPicker.IsRecentColorPaletteVisible && colorPicker.PART_ColorPaletteRecent != null)
                            {
                                isFocused = colorPicker.PART_ColorPaletteRecent.FocusSelectedItem();
                            }
                        }
                        else if (colorPicker.PART_PopupTabControl.SelectedItem == colorPicker.PART_AdvancedTab)
                        {
                            colorPicker.PART_AdvancedTab.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        }
                    });

                    colorPicker.Dispatcher.BeginInvoke(DispatcherPriority.Send, action);
                }
                else
                {
                    colorPicker.RaiseEvent(new RoutedEventArgs(DropDownClosedEvent));

                    if (Mouse.Captured == colorPicker)
                    {
                        Mouse.Capture(null);
                    }

                    if (colorPicker.AddToRecentColorsTrigger == AddToRecentColorsTrigger.ColorPickerClosed && colorPicker.SelectedColor.HasValue)
                    {
                        BuildInColorPalettes.AddColorToRecentColors(colorPicker.SelectedColor, colorPicker.RecentColorPaletteItemsSource);
                        BuildInColorPalettes.ReduceRecentColors(BuildInColorPalettes.GetMaximumRecentColorsCount(colorPicker), colorPicker.RecentColorPaletteItemsSource);
                    }
                }
            }
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;

            if (Mouse.Captured != colorPicker)
            {
                if (e.OriginalSource == colorPicker)
                {
                    if (Mouse.Captured == null || !(Mouse.Captured as DependencyObject).IsDescendantOf(colorPicker))
                    {
                        colorPicker.Close();
                    }
                }
                else
                {
                    if ((e.OriginalSource as DependencyObject).IsDescendantOf(colorPicker))
                    {
                        // Take capture if one of our children gave up capture (by closing their drop down)
                        if (colorPicker.IsDropDownOpen && Mouse.Captured == null)
                        {
                            Mouse.Capture(colorPicker, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        colorPicker.Close();
                    }
                }
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;

            // If we (or one of our children) are clicked, claim the focus (don't steal focus if our context menu is clicked)
            if ((!colorPicker.ContextMenu?.IsOpen ?? true) && !colorPicker.IsKeyboardFocusWithin)
            {
                colorPicker.Focus();
            }

            e.Handled = true;   // Always handle so that parents won't take focus away

            if (Mouse.Captured == colorPicker && e.OriginalSource == colorPicker)
            {
                colorPicker.Close();
            }
        }

        private void Close()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }
        

        #region ColorPalettes

        /// <summary>Identifies the <see cref="IsAvailableColorPaletteVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsAvailableColorPaletteVisibleProperty =
            DependencyProperty.Register(nameof(IsAvailableColorPaletteVisible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        /// <summary>Identifies the <see cref="AvailableColorPaletteHeader"/> dependency property.</summary>
        public static readonly DependencyProperty AvailableColorPaletteHeaderProperty =
            DependencyProperty.Register(nameof(AvailableColorPaletteHeader), typeof(object), typeof(ColorPicker), new PropertyMetadata("Available"));

        /// <summary>Identifies the <see cref="AvailableColorPaletteItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty AvailableColorPaletteItemsSourceProperty =
            DependencyProperty.Register(nameof(AvailableColorPaletteItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="AvailableColorPaletteStyle"/> dependency property.</summary>
        public static readonly DependencyProperty AvailableColorPaletteStyleProperty =
            DependencyProperty.Register(nameof(AvailableColorPaletteStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsCustomColorPalette01Visible"/> dependency property.</summary>
        public static readonly DependencyProperty IsCustomColorPalette01VisibleProperty =
            DependencyProperty.Register(nameof(IsCustomColorPalette01Visible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        /// <summary>Identifies the <see cref="CustomColorPalette01Header"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette01HeaderProperty =
            DependencyProperty.Register(nameof(CustomColorPalette01Header), typeof(object), typeof(ColorPicker), new PropertyMetadata("Custom 01"));

        /// <summary>Identifies the <see cref="CustomColorPalette01HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette01HeaderTemplateProperty =
            DependencyProperty.Register(nameof(CustomColorPalette01HeaderTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CustomColorPalette01ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette01ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColorPalette01ItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CustomColorPalette01Style"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette01StyleProperty =
            DependencyProperty.Register(nameof(CustomColorPalette01Style), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsCustomColorPalette02Visible"/> dependency property.</summary>
        public static readonly DependencyProperty IsCustomColorPalette02VisibleProperty =
            DependencyProperty.Register(nameof(IsCustomColorPalette02Visible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        /// <summary>Identifies the <see cref="CustomColorPalette02Header"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette02HeaderProperty =
            DependencyProperty.Register(nameof(CustomColorPalette02Header), typeof(object), typeof(ColorPicker), new PropertyMetadata("Custom 02"));

        /// <summary>Identifies the <see cref="CustomColorPalette02HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette02HeaderTemplateProperty =
            DependencyProperty.Register(nameof(CustomColorPalette02HeaderTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CustomColorPalette02ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette02ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColorPalette02ItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CustomColorPalette02Style"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColorPalette02StyleProperty =
            DependencyProperty.Register(nameof(CustomColorPalette02Style), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));
        /// <summary>Identifies the <see cref="IsRecentColorPaletteVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsRecentColorPaletteVisibleProperty =
            DependencyProperty.Register(nameof(IsRecentColorPaletteVisible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));
        /// <summary>Identifies the <see cref="RecentColorPaletteHeader"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorPaletteHeaderProperty =
            DependencyProperty.Register(nameof(RecentColorPaletteHeader), typeof(object), typeof(ColorPicker), new PropertyMetadata("Recent"));

        /// <summary>Identifies the <see cref="RecentColorPaletteHeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorPaletteHeaderTemplateProperty =
            DependencyProperty.Register(nameof(RecentColorPaletteHeaderTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="RecentColorPaletteItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorPaletteItemsSourceProperty =
            DependencyProperty.Register(nameof(RecentColorPaletteItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="RecentColorPaletteStyle"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorPaletteStyleProperty =
            DependencyProperty.Register(nameof(RecentColorPaletteStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsStandardColorPaletteVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsStandardColorPaletteVisibleProperty =
            DependencyProperty.Register(nameof(IsStandardColorPaletteVisible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        /// <summary>Identifies the <see cref="StandardColorPaletteHeader"/> dependency property.</summary>
        public static readonly DependencyProperty StandardColorPaletteHeaderProperty =
                            DependencyProperty.Register(nameof(StandardColorPaletteHeader), typeof(object), typeof(ColorPicker), new PropertyMetadata("Standard"));

        /// <summary>Identifies the <see cref="StandardColorPaletteItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty StandardColorPaletteItemsSourceProperty =
            DependencyProperty.Register(nameof(StandardColorPaletteItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="StandardColorPaletteStyle"/> dependency property.</summary>
        public static readonly DependencyProperty StandardColorPaletteStyleProperty =
            DependencyProperty.Register(nameof(StandardColorPaletteStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets if the available color palette is visible
        /// </summary>
        public bool IsAvailableColorPaletteVisible
        {
            get { return (bool)GetValue(IsAvailableColorPaletteVisibleProperty); }
            set { SetValue(IsAvailableColorPaletteVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the available color palettes Header
        /// </summary>
        public object AvailableColorPaletteHeader
        {
            get { return (object)GetValue(AvailableColorPaletteHeaderProperty); }
            set { SetValue(AvailableColorPaletteHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the available color palettes ItemSource
        /// </summary>
        public IEnumerable AvailableColorPaletteItemsSource
        {
            get { return (IEnumerable)GetValue(AvailableColorPaletteItemsSourceProperty); }
            set { SetValue(AvailableColorPaletteItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the available color palettes Style
        /// </summary>
        public Style AvailableColorPaletteStyle
        {
            get { return (Style)GetValue(AvailableColorPaletteStyleProperty); }
            set { SetValue(AvailableColorPaletteStyleProperty, value); }
        }


        /// <summary>
        /// Gets or sets if the custom color palette is visible (1/2)
        /// </summary>
        public bool IsCustomColorPalette01Visible
        {
            get { return (bool)GetValue(IsCustomColorPalette01VisibleProperty); }
            set { SetValue(IsCustomColorPalette01VisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes Header (1/2)
        /// </summary>
        public object CustomColorPalette01Header
        {
            get { return (object)GetValue(CustomColorPalette01HeaderProperty); }
            set { SetValue(CustomColorPalette01HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes HeaderTemplate (1/2)
        /// </summary>
        public DataTemplate CustomColorPalette01HeaderTemplate
        {
            get { return (DataTemplate)GetValue(CustomColorPalette01HeaderTemplateProperty); }
            set { SetValue(CustomColorPalette01HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes ItemSource (1/2)
        /// </summary>
        public IEnumerable CustomColorPalette01ItemsSource
        {
            get { return (IEnumerable)GetValue(CustomColorPalette01ItemsSourceProperty); }
            set { SetValue(CustomColorPalette01ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes Style (1/2)
        /// </summary>
        public Style CustomColorPalette01Style
        {
            get { return (Style)GetValue(CustomColorPalette01StyleProperty); }
            set { SetValue(CustomColorPalette01StyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the custom color palette is visible (2/2)
        /// </summary>
        public bool IsCustomColorPalette02Visible
        {
            get { return (bool)GetValue(IsCustomColorPalette02VisibleProperty); }
            set { SetValue(IsCustomColorPalette02VisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes Header (2/2)
        /// </summary>
        public object CustomColorPalette02Header
        {
            get { return (object)GetValue(CustomColorPalette02HeaderProperty); }
            set { SetValue(CustomColorPalette02HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes HeaderTemplate (2/2)
        /// </summary>
        public DataTemplate CustomColorPalette02HeaderTemplate
        {
            get { return (DataTemplate)GetValue(CustomColorPalette02HeaderTemplateProperty); }
            set { SetValue(CustomColorPalette02HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes ItemSource (2/2)
        /// </summary>
        public IEnumerable CustomColorPalette02ItemsSource
        {
            get { return (IEnumerable)GetValue(CustomColorPalette02ItemsSourceProperty); }
            set { SetValue(CustomColorPalette02ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom color palettes Style (2/2)
        /// </summary>
        public Style CustomColorPalette02Style
        {
            get { return (Style)GetValue(CustomColorPalette02StyleProperty); }
            set { SetValue(CustomColorPalette02StyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the recent color palette is visible
        /// </summary>
        public bool IsRecentColorPaletteVisible
        {
            get { return (bool)GetValue(IsRecentColorPaletteVisibleProperty); }
            set { SetValue(IsRecentColorPaletteVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the recent color palettes Header
        /// </summary>
        public object RecentColorPaletteHeader
        {
            get { return (object)GetValue(RecentColorPaletteHeaderProperty); }
            set { SetValue(RecentColorPaletteHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the recent color palettes HeaderTemplate 
        /// </summary>
        public DataTemplate RecentColorPaletteHeaderTemplate
        {
            get { return (DataTemplate)GetValue(RecentColorPaletteHeaderTemplateProperty); }
            set { SetValue(RecentColorPaletteHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the recent color palettes ItemSource
        /// </summary>
        public IEnumerable RecentColorPaletteItemsSource
        {
            get { return (IEnumerable)GetValue(RecentColorPaletteItemsSourceProperty); }
            set { SetValue(RecentColorPaletteItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the recent color palettes Style
        /// </summary>
        public Style RecentColorPaletteStyle
        {
            get { return (Style)GetValue(RecentColorPaletteStyleProperty); }
            set { SetValue(RecentColorPaletteStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the standard color palette is visible
        /// </summary>
        public bool IsStandardColorPaletteVisible
        {
            get { return (bool)GetValue(IsStandardColorPaletteVisibleProperty); }
            set { SetValue(IsStandardColorPaletteVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the strandard color palettes Header 
        /// </summary>
        public object StandardColorPaletteHeader
        {
            get { return (object)GetValue(StandardColorPaletteHeaderProperty); }
            set { SetValue(StandardColorPaletteHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the standard color palettes HeaderTemplate
        /// </summary>
        public IEnumerable StandardColorPaletteItemsSource
        {
            get { return (IEnumerable)GetValue(StandardColorPaletteItemsSourceProperty); }
            set { SetValue(StandardColorPaletteItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the standard color palettes Style
        /// </summary>
        public Style StandardColorPaletteStyle
        {
            get { return (Style)GetValue(StandardColorPaletteStyleProperty); }
            set { SetValue(StandardColorPaletteStyleProperty, value); }
        }

        #endregion ColorPalettes

        #region TabControl

        /// <summary>Identifies the <see cref="TabControlStyle"/> dependency property.</summary>
        public static readonly DependencyProperty TabControlStyleProperty = DependencyProperty.Register(nameof(TabControlStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="TabItemStyle"/> dependency property.</summary>
        public static readonly DependencyProperty TabItemStyleProperty = DependencyProperty.Register(nameof(TabItemStyle), typeof(Style), typeof(ColorPicker), new PropertyMetadata(null));


        /// <summary>
        /// Gets or Sets the <see cref="Style"/> for the <see cref="TabControl"/>
        /// </summary>
        public Style TabControlStyle
        {
            get { return (Style)GetValue(TabControlStyleProperty); }
            set { SetValue(TabControlStyleProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Style"/> for the <see cref="TabItem"/>
        /// </summary>
        public Style TabItemStyle
        {
            get { return (Style)GetValue(TabItemStyleProperty); }
            set { SetValue(TabItemStyleProperty, value); }
        }


        /// <summary>Identifies the <see cref="ColorPalettesTabHeader"/> dependency property.</summary>
        public static readonly DependencyProperty ColorPalettesTabHeaderProperty = DependencyProperty.Register(nameof(ColorPalettesTabHeader), typeof(object), typeof(ColorPicker), new PropertyMetadata("Palettes"));

        /// <summary>Identifies the <see cref="ColorPalettesTabHeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ColorPalettesTabHeaderTemplateProperty = DependencyProperty.Register(nameof(ColorPalettesTabHeaderTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsColorPalettesTabVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsColorPalettesTabVisibleProperty = DependencyProperty.Register(nameof(IsColorPalettesTabVisible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnTabItemVisibilityChanged));

        /// <summary>
        /// Gets or Sets the Header for the <see cref="ColorPalette"/>-Tab
        /// </summary>
        public object ColorPalettesTabHeader
        {
            get { return (object)GetValue(ColorPalettesTabHeaderProperty); }
            set { SetValue(ColorPalettesTabHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the HeaderTemplate for the <see cref="ColorPalette"/>-Tab
        /// </summary>
        public DataTemplate ColorPalettesTabHeaderTemplate
        {
            get { return (DataTemplate)GetValue(ColorPalettesTabHeaderTemplateProperty); }
            set { SetValue(ColorPalettesTabHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or Sets if the <see cref="ColorPalette"/>-Tab is visible
        /// </summary>
        public bool IsColorPalettesTabVisible
        {
            get { return (bool)GetValue(IsColorPalettesTabVisibleProperty); }
            set { SetValue(IsColorPalettesTabVisibleProperty, value); }
        }


        /// <summary>Identifies the <see cref="AdvancedTabHeader"/> dependency property.</summary>
        public static readonly DependencyProperty AdvancedTabHeaderProperty = DependencyProperty.Register(nameof(AdvancedTabHeader), typeof(object), typeof(ColorPicker), new PropertyMetadata("Advanced"));

        /// <summary>Identifies the <see cref="AdvancedTabHeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty AdvancedTabHeaderTemplateProperty = DependencyProperty.Register(nameof(AdvancedTabHeaderTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsAdvancedTabVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsAdvancedTabVisibleProperty = DependencyProperty.Register(nameof(IsAdvancedTabVisible), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true, OnTabItemVisibilityChanged));

        /// <summary>
        /// Gets or Sets the Header for the <see cref="ColorCanvas"/>-Tab
        /// </summary>
        public object AdvancedTabHeader
        {
            get { return (object)GetValue(AdvancedTabHeaderProperty); }
            set { SetValue(AdvancedTabHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the HeaderTemplate for the <see cref="ColorCanvas"/>-Tab
        /// </summary>
        public DataTemplate AdvancedTabHeaderTemplate
        {
            get { return (DataTemplate)GetValue(AdvancedTabHeaderTemplateProperty); }
            set { SetValue(AdvancedTabHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or Sets if the <see cref="ColorCanvas"/>-Tab is visible
        /// </summary>
        public bool IsAdvancedTabVisible
        {
            get { return (bool)GetValue(IsAdvancedTabVisibleProperty); }
            set { SetValue(IsAdvancedTabVisibleProperty, value); }
        }


        private static void OnTabItemVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker colorPicker && colorPicker.IsInitialized)
            {
                colorPicker.ValidateTabItems();
            }
        }

        private void ValidateTabItems()
        {
            // If both TabItems are invisble we set the content to null
            if (!IsAdvancedTabVisible && !IsColorPalettesTabVisible)
            {
                PART_PopupTabControl.SelectedIndex = -1;
            }
            // If the color palettes tab is selected but not visible we select the advanced tab and vice vera
            else if (PART_PopupTabControl.SelectedItem == PART_ColorPalettesTab && !IsColorPalettesTabVisible)
            {
                PART_PopupTabControl.SelectedItem = PART_AdvancedTab;
            }
            else if (PART_PopupTabControl.SelectedItem == PART_AdvancedTab && !IsAdvancedTabVisible)
            {
                PART_PopupTabControl.SelectedItem = PART_ColorPalettesTab;
            }
            else
            {
                if (IsColorPalettesTabVisible)
                {
                    PART_ColorPalettesTab.IsSelected = true;
                }
                else if (IsAdvancedTabVisible)
                {
                    PART_AdvancedTab.IsSelected = true;
                }
            }
        }

        #endregion
    }
}