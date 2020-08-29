// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class DataGridNumericUpDownColumn : DataGridBoundColumn
    {
        private static Style _defaultEditingElementStyle;
        private static Style _defaultElementStyle;

        static DataGridNumericUpDownColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        public static Style DefaultEditingElementStyle
        {
            get
            {
                if (_defaultEditingElementStyle == null)
                {
                    var style = new Style(typeof(NumericUpDown));

                    style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ControlsHelper.DisabledVisualElementVisibilityProperty, Visibility.Collapsed));

                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0)));
                    style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, false));
                    style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                    style.Setters.Add(new Setter(ControlsHelper.CornerRadiusProperty, new CornerRadius(0)));

                    style.Seal();
                    _defaultEditingElementStyle = style;
                }

                return _defaultEditingElementStyle;
            }
        }

        public static Style DefaultElementStyle
        {
            get
            {
                if (_defaultElementStyle == null)
                {
                    var style = new Style(typeof(NumericUpDown));

                    style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ControlsHelper.DisabledVisualElementVisibilityProperty, Visibility.Collapsed));

                    style.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
                    style.Setters.Add(new Setter(UIElement.FocusableProperty, false));

                    style.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0)));
                    style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, true));
                    style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                    style.Setters.Add(new Setter(ControlsHelper.CornerRadiusProperty, new CornerRadius(0)));

                    style.Seal();
                    _defaultElementStyle = style;
                }

                return _defaultElementStyle;
            }
        }

        private static void ApplyBinding(BindingBase binding, DependencyObject target, DependencyProperty property)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
            }
        }

        private void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            Style style = this.PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
            {
                element.Style = style;
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return this.GenerateNumericUpDown(true, cell);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return this.GenerateNumericUpDown(false, cell);
        }

        private NumericUpDown GenerateNumericUpDown(bool isEditing, DataGridCell cell)
        {
            var numericUpDown = cell?.Content as NumericUpDown ?? new NumericUpDown();

            SyncColumnProperty(this, numericUpDown, FontFamilyProperty, TextElement.FontFamilyProperty);
            SyncColumnProperty(this, numericUpDown, FontSizeProperty, TextElement.FontSizeProperty);
            SyncColumnProperty(this, numericUpDown, FontStyleProperty, TextElement.FontStyleProperty);
            SyncColumnProperty(this, numericUpDown, FontWeightProperty, TextElement.FontWeightProperty);

            SyncColumnProperty(this, numericUpDown, TextAlignmentProperty, NumericUpDown.TextAlignmentProperty);
            SyncColumnProperty(this, numericUpDown, StringFormatProperty, NumericUpDown.StringFormatProperty);
            SyncColumnProperty(this, numericUpDown, CultureProperty, NumericUpDown.CultureProperty);
            SyncColumnProperty(this, numericUpDown, MinimumProperty, NumericUpDown.MinimumProperty);
            SyncColumnProperty(this, numericUpDown, MaximumProperty, NumericUpDown.MaximumProperty);
            SyncColumnProperty(this, numericUpDown, NumericInputModeProperty, NumericUpDown.NumericInputModeProperty);
            SyncColumnProperty(this, numericUpDown, DecimalPointCorrectionProperty, NumericUpDown.DecimalPointCorrectionProperty);
            SyncColumnProperty(this, numericUpDown, IntervalProperty, NumericUpDown.IntervalProperty);
            SyncColumnProperty(this, numericUpDown, DelayProperty, NumericUpDown.DelayProperty);
            SyncColumnProperty(this, numericUpDown, SpeedupProperty, NumericUpDown.SpeedupProperty);
            SyncColumnProperty(this, numericUpDown, SnapToMultipleOfIntervalProperty, NumericUpDown.SnapToMultipleOfIntervalProperty);
            SyncColumnProperty(this, numericUpDown, InterceptArrowKeysProperty, NumericUpDown.InterceptArrowKeysProperty);
            SyncColumnProperty(this, numericUpDown, InterceptManualEnterProperty, NumericUpDown.InterceptManualEnterProperty);
            SyncColumnProperty(this, numericUpDown, InterceptMouseWheelProperty, NumericUpDown.InterceptMouseWheelProperty);
            SyncColumnProperty(this, numericUpDown, TrackMouseWheelWhenMouseOverProperty, NumericUpDown.TrackMouseWheelWhenMouseOverProperty);
            SyncColumnProperty(this, numericUpDown, HideUpDownButtonsProperty, NumericUpDown.HideUpDownButtonsProperty);
            SyncColumnProperty(this, numericUpDown, SwitchUpDownButtonsProperty, NumericUpDown.SwitchUpDownButtonsProperty);
            SyncColumnProperty(this, numericUpDown, ButtonsAlignmentProperty, NumericUpDown.ButtonsAlignmentProperty);
            SyncColumnProperty(this, numericUpDown, UpDownButtonsWidthProperty, NumericUpDown.UpDownButtonsWidthProperty);

            if (isEditing)
            {
                SyncColumnProperty(this, numericUpDown, ForegroundProperty, TextElement.ForegroundProperty);
            }
            else
            {
                if (!SyncColumnProperty(this, numericUpDown, ForegroundProperty, TextElement.ForegroundProperty))
                {
                    ApplyBinding(new Binding(Control.ForegroundProperty.Name) { Source = cell, Mode = BindingMode.OneWay }, numericUpDown, TextElement.ForegroundProperty);
                }
            }

            this.ApplyStyle(isEditing, true, numericUpDown);
            ApplyBinding(this.Binding, numericUpDown, NumericUpDown.ValueProperty);

            numericUpDown.Focusable = isEditing;
            numericUpDown.IsHitTestVisible = isEditing;

            return numericUpDown;
        }

        /// <summary>
        ///     Called when a cell has just switched to edit mode.
        /// </summary>
        /// <param name="editingElement">A reference to element returned by GenerateEditingElement.</param>
        /// <param name="editingEventArgs">The event args of the input event that caused the cell to go into edit mode. May be null.</param>
        /// <returns>The unedited value of the cell.</returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is NumericUpDown numericUpDown)
            {
                numericUpDown.Focus();
                numericUpDown.SelectAll();
                return numericUpDown.Value;
            }

            return null;
        }

        /// <summary>
        /// Synchronizes the column property. Taken from Helper code for DataGrid.
        /// </summary>
        internal static bool SyncColumnProperty(DependencyObject column, DependencyObject content, DependencyProperty columnProperty, DependencyProperty contentProperty)
        {
            if (IsDefaultValue(column, columnProperty))
            {
                content.ClearValue(contentProperty);
                return false;
            }
            else
            {
                content.SetValue(contentProperty, column.GetValue(columnProperty));
                return true;
            }
        }

        /// <summary>
        /// Taken from Helper code for DataGrid.
        /// </summary>
        private static bool IsDefaultValue(DependencyObject d, DependencyProperty dp)
        {
            return DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
        }

        private Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            Style style = isEditing ? this.EditingElementStyle : this.ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
            {
                style = this.ElementStyle;
            }

            return style;
        }

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty =
            NumericUpDown.StringFormatProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((string)NumericUpDown.StringFormatProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// Gets or sets the formatting for the displaying value.
        /// </summary>
        /// <remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx"></see>
        /// </remarks>
        public string StringFormat
        {
            get { return (string)this.GetValue(StringFormatProperty); }
            set { this.SetValue(StringFormatProperty, value); }
        }

        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty =
            NumericUpDown.CultureProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((CultureInfo)NumericUpDown.CultureProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public CultureInfo Culture
        {
            get { return (CultureInfo)this.GetValue(CultureProperty); }
            set { this.SetValue(CultureProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            NumericUpDown.TextAlignmentProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((TextAlignment)NumericUpDown.TextAlignmentProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty =
            NumericUpDown.MinimumProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.MinimumProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Minimum
        {
            get { return (double)this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty =
            NumericUpDown.MaximumProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.MaximumProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Maximum
        {
            get { return (double)this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>Identifies the <see cref="NumericInputMode"/> dependency property.</summary>
        public static readonly DependencyProperty NumericInputModeProperty =
            NumericUpDown.NumericInputModeProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((NumericInput)NumericUpDown.NumericInputModeProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public NumericInput NumericInputMode
        {
            get { return (NumericInput)this.GetValue(NumericInputModeProperty); }
            set { this.SetValue(NumericInputModeProperty, value); }
        }

        /// <summary>Identifies the <see cref="DecimalPointCorrection"/> dependency property.</summary>
        public static readonly DependencyProperty DecimalPointCorrectionProperty
            = DependencyProperty.Register(nameof(DecimalPointCorrection),
                                          typeof(DecimalPointCorrectionMode),
                                          typeof(DataGridNumericUpDownColumn),
                                          new PropertyMetadata(default(DecimalPointCorrectionMode)));

        /// <summary>
        /// Gets or sets the decimal-point correction mode. The default is <see cref="DecimalPointCorrectionMode.Inherits"/>
        /// </summary>
        public DecimalPointCorrectionMode DecimalPointCorrection
        {
            get => (DecimalPointCorrectionMode)this.GetValue(DecimalPointCorrectionProperty);
            set => this.SetValue(DecimalPointCorrectionProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty =
            NumericUpDown.IntervalProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.IntervalProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Interval
        {
            get { return (double)this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty =
            NumericUpDown.DelayProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((int)NumericUpDown.DelayProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public int Delay
        {
            get { return (int)this.GetValue(DelayProperty); }
            set { this.SetValue(DelayProperty, value); }
        }

        /// <summary>Identifies the <see cref="Speedup"/> dependency property.</summary>
        public static readonly DependencyProperty SpeedupProperty =
            NumericUpDown.SpeedupProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.SpeedupProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool Speedup
        {
            get { return (bool)this.GetValue(SpeedupProperty); }
            set { this.SetValue(SpeedupProperty, value); }
        }

        /// <summary>Identifies the <see cref="SnapToMultipleOfInterval"/> dependency property.</summary>
        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty =
            NumericUpDown.SnapToMultipleOfIntervalProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.SnapToMultipleOfIntervalProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool SnapToMultipleOfInterval
        {
            get { return (bool)this.GetValue(SnapToMultipleOfIntervalProperty); }
            set { this.SetValue(SnapToMultipleOfIntervalProperty, value); }
        }

        /// <summary>Identifies the <see cref="InterceptArrowKeys"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptArrowKeysProperty =
            NumericUpDown.InterceptArrowKeysProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.InterceptArrowKeysProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptArrowKeys
        {
            get { return (bool)this.GetValue(InterceptArrowKeysProperty); }
            set { this.SetValue(InterceptArrowKeysProperty, value); }
        }

        /// <summary>Identifies the <see cref="InterceptManualEnter"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptManualEnterProperty =
            NumericUpDown.InterceptManualEnterProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.InterceptManualEnterProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptManualEnter
        {
            get { return (bool)this.GetValue(InterceptManualEnterProperty); }
            set { this.SetValue(InterceptManualEnterProperty, value); }
        }

        /// <summary>Identifies the <see cref="InterceptMouseWheel"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelProperty =
            NumericUpDown.InterceptMouseWheelProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.InterceptMouseWheelProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptMouseWheel
        {
            get { return (bool)this.GetValue(InterceptMouseWheelProperty); }
            set { this.SetValue(InterceptMouseWheelProperty, value); }
        }

        /// <summary>Identifies the <see cref="TrackMouseWheelWhenMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty =
            NumericUpDown.TrackMouseWheelWhenMouseOverProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.TrackMouseWheelWhenMouseOverProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool TrackMouseWheelWhenMouseOver
        {
            get { return (bool)this.GetValue(TrackMouseWheelWhenMouseOverProperty); }
            set { this.SetValue(TrackMouseWheelWhenMouseOverProperty, value); }
        }

        /// <summary>Identifies the <see cref="HideUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty HideUpDownButtonsProperty =
            NumericUpDown.HideUpDownButtonsProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.HideUpDownButtonsProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool HideUpDownButtons
        {
            get { return (bool)this.GetValue(HideUpDownButtonsProperty); }
            set { this.SetValue(HideUpDownButtonsProperty, value); }
        }

        /// <summary>Identifies the <see cref="SwitchUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty SwitchUpDownButtonsProperty =
            NumericUpDown.SwitchUpDownButtonsProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.SwitchUpDownButtonsProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool SwitchUpDownButtons
        {
            get { return (bool)this.GetValue(SwitchUpDownButtonsProperty); }
            set { this.SetValue(SwitchUpDownButtonsProperty, value); }
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty =
            NumericUpDown.ButtonsAlignmentProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((ButtonsAlignment)NumericUpDown.ButtonsAlignmentProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public ButtonsAlignment ButtonsAlignment
        {
            get { return (ButtonsAlignment)this.GetValue(ButtonsAlignmentProperty); }
            set { this.SetValue(ButtonsAlignmentProperty, value); }
        }

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty =
            NumericUpDown.UpDownButtonsWidthProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.UpDownButtonsWidthProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double UpDownButtonsWidth
        {
            get { return (double)this.GetValue(UpDownButtonsWidthProperty); }
            set { this.SetValue(UpDownButtonsWidthProperty, value); }
        }

        /// <summary>Identifies the <see cref="FontFamily"/> dependency property.</summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The font family of the desired font.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        /// <summary>Identifies the <see cref="FontSize"/> dependency property.</summary>
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The size of the desired font.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        [Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        /// <summary>Identifies the <see cref="FontStyle"/> dependency property.</summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The style of the desired font.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)this.GetValue(FontStyleProperty); }
            set { this.SetValue(FontStyleProperty, value); }
        }

        /// <summary>Identifies the <see cref="FontWeight"/> dependency property.</summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The weight or thickness of the desired font.
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)this.GetValue(FontWeightProperty); }
            set { this.SetValue(FontWeightProperty, value); }
        }

        /// <summary>Identifies the <see cref="Foreground"/> dependency property.</summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// An brush that describes the foreground color. This overrides the cell foreground inherited color.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)this.GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Method used as property changed callback for properties which need RefreshCellContent to be called
        /// </summary>
        private static void NotifyPropertyChangeForRefreshContent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d is DataGridNumericUpDownColumn, "d should be a DataGridNumericUpDownColumn");
            ((DataGridNumericUpDownColumn)d).NotifyPropertyChanged(e.Property.Name);
        }

        /// <summary>
        /// Rebuilds the contents of a cell in the column in response to a binding change.
        /// </summary>
        /// <param name="element">The cell to update.</param>
        /// <param name="propertyName">The name of the column property that has changed.</param>
        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            var cell = element as DataGridCell;
            var numericUpDown = cell?.Content as NumericUpDown;
            if (numericUpDown != null)
            {
                switch (propertyName)
                {
                    case nameof(this.FontFamily):
                        SyncColumnProperty(this, numericUpDown, FontFamilyProperty, TextElement.FontFamilyProperty);
                        break;
                    case nameof(this.FontSize):
                        SyncColumnProperty(this, numericUpDown, FontSizeProperty, TextElement.FontSizeProperty);
                        break;
                    case nameof(this.FontStyle):
                        SyncColumnProperty(this, numericUpDown, FontStyleProperty, TextElement.FontStyleProperty);
                        break;
                    case nameof(this.FontWeight):
                        SyncColumnProperty(this, numericUpDown, FontWeightProperty, TextElement.FontWeightProperty);
                        break;
                    case nameof(this.TextAlignment):
                        SyncColumnProperty(this, numericUpDown, TextAlignmentProperty, NumericUpDown.TextAlignmentProperty);
                        break;
                    case nameof(this.StringFormat):
                        SyncColumnProperty(this, numericUpDown, StringFormatProperty, NumericUpDown.StringFormatProperty);
                        break;
                    case nameof(this.Culture):
                        SyncColumnProperty(this, numericUpDown, CultureProperty, NumericUpDown.CultureProperty);
                        break;
                    case nameof(this.Minimum):
                        SyncColumnProperty(this, numericUpDown, MinimumProperty, NumericUpDown.MinimumProperty);
                        break;
                    case nameof(this.Maximum):
                        SyncColumnProperty(this, numericUpDown, MaximumProperty, NumericUpDown.MaximumProperty);
                        break;
                    case nameof(this.NumericInputMode):
                        SyncColumnProperty(this, numericUpDown, NumericInputModeProperty, NumericUpDown.NumericInputModeProperty);
                        break;
                    case nameof(this.DecimalPointCorrection):
                        SyncColumnProperty(this, numericUpDown, DecimalPointCorrectionProperty, NumericUpDown.DecimalPointCorrectionProperty);
                        break;
                    case nameof(this.Interval):
                        SyncColumnProperty(this, numericUpDown, IntervalProperty, NumericUpDown.IntervalProperty);
                        break;
                    case nameof(this.Delay):
                        SyncColumnProperty(this, numericUpDown, DelayProperty, NumericUpDown.DelayProperty);
                        break;
                    case nameof(this.Speedup):
                        SyncColumnProperty(this, numericUpDown, SpeedupProperty, NumericUpDown.SpeedupProperty);
                        break;
                    case nameof(this.SnapToMultipleOfInterval):
                        SyncColumnProperty(this, numericUpDown, SnapToMultipleOfIntervalProperty, NumericUpDown.SnapToMultipleOfIntervalProperty);
                        break;
                    case nameof(this.InterceptArrowKeys):
                        SyncColumnProperty(this, numericUpDown, InterceptArrowKeysProperty, NumericUpDown.InterceptArrowKeysProperty);
                        break;
                    case nameof(this.InterceptManualEnter):
                        SyncColumnProperty(this, numericUpDown, InterceptManualEnterProperty, NumericUpDown.InterceptManualEnterProperty);
                        break;
                    case nameof(this.InterceptMouseWheel):
                        SyncColumnProperty(this, numericUpDown, InterceptMouseWheelProperty, NumericUpDown.InterceptMouseWheelProperty);
                        break;
                    case nameof(this.TrackMouseWheelWhenMouseOver):
                        SyncColumnProperty(this, numericUpDown, TrackMouseWheelWhenMouseOverProperty, NumericUpDown.TrackMouseWheelWhenMouseOverProperty);
                        break;
                    case nameof(this.HideUpDownButtons):
                        SyncColumnProperty(this, numericUpDown, HideUpDownButtonsProperty, NumericUpDown.HideUpDownButtonsProperty);
                        break;
                    case nameof(this.SwitchUpDownButtons):
                        SyncColumnProperty(this, numericUpDown, SwitchUpDownButtonsProperty, NumericUpDown.SwitchUpDownButtonsProperty);
                        break;
                    case nameof(this.ButtonsAlignment):
                        SyncColumnProperty(this, numericUpDown, ButtonsAlignmentProperty, NumericUpDown.ButtonsAlignmentProperty);
                        break;
                    case nameof(this.UpDownButtonsWidth):
                        SyncColumnProperty(this, numericUpDown, UpDownButtonsWidthProperty, NumericUpDown.UpDownButtonsWidthProperty);
                        break;
                }
            }

            base.RefreshCellContent(element, propertyName);
        }
    }
}