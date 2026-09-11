// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The half of an up-down column that does not care what kind of number the cell holds: every
    /// property it hands to the control, and the work of making and refreshing one.
    /// </summary>
    public abstract class DataGridNumericUpDownColumnBase : DataGridBoundColumn
    {
        private static Style? _defaultEditingElementStyle;
        private static Style? _defaultElementStyle;

        static DataGridNumericUpDownColumnBase()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumnBase), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumnBase), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
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
                    style.Setters.Add(new Setter(NumericUpDownBase.HideUpDownButtonsProperty, false));
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
                    style.Setters.Add(new Setter(NumericUpDownBase.HideUpDownButtonsProperty, true));
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

        private protected static void ApplyBinding(BindingBase? binding, DependencyObject target, DependencyProperty property)
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
            var style = this.PickStyle(isEditing, defaultToElementStyle);
            if (style is not null)
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
            // A cell hands what is typed on it to its column through DataGridColumn.OnInput, which is
            // where DataGridTextColumn starts editing. That one is internal to WPF and cannot be
            // overridden from here, so the cell is asked directly instead.
            cell.PreviewTextInput -= OnCellTextInput;
            cell.PreviewTextInput += OnCellTextInput;

            return this.GenerateTextBlock(cell);
        }

        /// <summary>
        /// Typing on a cell that is only showing its value starts editing it, the same way a text column
        /// behaves.
        /// </summary>
        private static void OnCellTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Handled
                || sender is not DataGridCell cell
                || cell.IsEditing
                || !HasSomethingToType(e.Text))
            {
                return;
            }

            if (cell.TryFindParent<DataGrid>()?.BeginEdit(e) == true)
            {
                // The character has been dealt with by the editor that just opened. Letting it travel on
                // would put it in a second time.
                e.Handled = true;
            }
        }

        /// <summary>
        /// Whether there is anything in the text worth starting an edit for. Escape reaches here as text
        /// when nothing else took it, and opening an editor on escape would puzzle anyone.
        /// </summary>
        private static bool HasSomethingToType(string? text)
        {
            return !string.IsNullOrEmpty(text) && text!.Any(character => character != '');
        }

        /// <summary>Says what a value looks like in this column, for the cell that only shows it.</summary>
        protected abstract IMultiValueConverter TextConverter { get; }

        /// <summary>
        /// A cell that is not being edited shows its value as text. It used to hold a whole up-down
        /// control that was made unfocusable, untouchable and stripped of its buttons, which is forty
        /// odd visual elements pretending to be one.
        /// </summary>
        private TextBlock GenerateTextBlock(DataGridCell? cell)
        {
            var textBlock = cell?.Content as TextBlock ?? new TextBlock();

            SyncColumnProperty(this, textBlock, FontFamilyProperty, TextElement.FontFamilyProperty);
            SyncColumnProperty(this, textBlock, FontSizeProperty, TextElement.FontSizeProperty);
            SyncColumnProperty(this, textBlock, FontStyleProperty, TextElement.FontStyleProperty);
            SyncColumnProperty(this, textBlock, FontWeightProperty, TextElement.FontWeightProperty);
            SyncColumnProperty(this, textBlock, TextAlignmentProperty, TextBlock.TextAlignmentProperty);

            if (!SyncColumnProperty(this, textBlock, ForegroundProperty, TextElement.ForegroundProperty))
            {
                ApplyBinding(new Binding(Control.ForegroundProperty.Name) { Source = cell, Mode = BindingMode.OneWay }, textBlock, TextElement.ForegroundProperty);
            }

            this.ApplyStyle(false, false, textBlock);
            this.BindText(textBlock);

            return textBlock;
        }

        /// <summary>Binds the text to the value, formatted the way the control would have shown it.</summary>
        private void BindText(TextBlock textBlock)
        {
            if (this.Binding is null)
            {
                BindingOperations.ClearBinding(textBlock, TextBlock.TextProperty);
                return;
            }

            var binding = new MultiBinding { Converter = this.TextConverter, Mode = BindingMode.OneWay };
            binding.Bindings.Add(this.Binding);
            binding.Bindings.Add(new Binding(nameof(this.StringFormat)) { Source = this, Mode = BindingMode.OneWay });
            binding.Bindings.Add(new Binding(nameof(this.Culture)) { Source = this, Mode = BindingMode.OneWay });

            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, binding);
        }

        /// <summary>Makes the control this column puts in a cell, or takes the one already there.</summary>
        protected abstract NumericUpDownBase TakeOrMakeControl(DataGridCell? cell);

        /// <summary>Carries the bounds and the step over, which only the typed half can name.</summary>
        protected abstract void SyncTypedProperties(NumericUpDownBase control);

        /// <summary>Carries one of those over, named by the property that changed.</summary>
        protected abstract void SyncTypedProperty(NumericUpDownBase control, string propertyName);

        /// <summary>Binds the column to the value of the control, which is typed as well.</summary>
        protected abstract void BindValue(NumericUpDownBase control);

        /// <summary>The value the control holds, for the cell to remember while it is edited.</summary>
        protected abstract object? ValueOf(NumericUpDownBase control);

        private NumericUpDownBase GenerateNumericUpDown(bool isEditing, DataGridCell cell)
        {
            var numericUpDown = this.TakeOrMakeControl(cell);

            SyncColumnProperty(this, numericUpDown, FontFamilyProperty, TextElement.FontFamilyProperty);
            SyncColumnProperty(this, numericUpDown, FontSizeProperty, TextElement.FontSizeProperty);
            SyncColumnProperty(this, numericUpDown, FontStyleProperty, TextElement.FontStyleProperty);
            SyncColumnProperty(this, numericUpDown, FontWeightProperty, TextElement.FontWeightProperty);

            SyncColumnProperty(this, numericUpDown, TextAlignmentProperty, NumericUpDownBase.TextAlignmentProperty);
            SyncColumnProperty(this, numericUpDown, StringFormatProperty, NumericUpDownBase.StringFormatProperty);
            SyncColumnProperty(this, numericUpDown, CultureProperty, NumericUpDownBase.CultureProperty);
            SyncColumnProperty(this, numericUpDown, NumericInputModeProperty, NumericUpDownBase.NumericInputModeProperty);
            SyncColumnProperty(this, numericUpDown, DecimalPointCorrectionProperty, NumericUpDownBase.DecimalPointCorrectionProperty);
            SyncColumnProperty(this, numericUpDown, DelayProperty, NumericUpDownBase.DelayProperty);
            SyncColumnProperty(this, numericUpDown, SpeedupProperty, NumericUpDownBase.SpeedupProperty);
            SyncColumnProperty(this, numericUpDown, SnapToMultipleOfIntervalProperty, NumericUpDownBase.SnapToMultipleOfIntervalProperty);
            SyncColumnProperty(this, numericUpDown, InterceptArrowKeysProperty, NumericUpDownBase.InterceptArrowKeysProperty);
            SyncColumnProperty(this, numericUpDown, InterceptManualEnterProperty, NumericUpDownBase.InterceptManualEnterProperty);
            SyncColumnProperty(this, numericUpDown, InterceptMouseWheelProperty, NumericUpDownBase.InterceptMouseWheelProperty);
            SyncColumnProperty(this, numericUpDown, TrackMouseWheelWhenMouseOverProperty, NumericUpDownBase.TrackMouseWheelWhenMouseOverProperty);
            SyncColumnProperty(this, numericUpDown, HideUpDownButtonsProperty, NumericUpDownBase.HideUpDownButtonsProperty);
            SyncColumnProperty(this, numericUpDown, SwitchUpDownButtonsProperty, NumericUpDownBase.SwitchUpDownButtonsProperty);
            SyncColumnProperty(this, numericUpDown, ButtonsAlignmentProperty, NumericUpDownBase.ButtonsAlignmentProperty);
            SyncColumnProperty(this, numericUpDown, UpDownButtonsWidthProperty, NumericUpDownBase.UpDownButtonsWidthProperty);
            this.SyncTypedProperties(numericUpDown);

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
            this.BindValue(numericUpDown);

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
        protected override object? PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is NumericUpDownBase numericUpDown)
            {
                numericUpDown.Focus();

                var unedited = this.ValueOf(numericUpDown);

                if (editingEventArgs is TextCompositionEventArgs typed && HasSomethingToType(typed.Text))
                {
                    // Typing is what started this, so what was typed is what the control shows.
                    numericUpDown.TakeTypedText(typed.Text);
                }
                else
                {
                    numericUpDown.SelectAll();
                }

                return unedited;
            }

            return null;
        }

        /// <summary>
        /// Synchronizes the column property. Taken from Helper code for DataGrid.
        /// </summary>
        private protected static bool SyncColumnProperty(DependencyObject column, DependencyObject content, DependencyProperty columnProperty, DependencyProperty contentProperty)
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

        private Style? PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            var style = isEditing ? this.EditingElementStyle : this.ElementStyle;
            if (isEditing && defaultToElementStyle && style is null)
            {
                style = this.ElementStyle;
            }

            return style;
        }

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty =
            NumericUpDownBase.StringFormatProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((string)NumericUpDownBase.StringFormatProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// Gets or sets the formatting for the displaying value.
        /// </summary>
        /// <remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx"></see>
        /// </remarks>
        public string StringFormat
        {
            get => (string)this.GetValue(StringFormatProperty);
            set => this.SetValue(StringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty =
            NumericUpDownBase.CultureProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((CultureInfo)NumericUpDownBase.CultureProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public CultureInfo? Culture
        {
            get => (CultureInfo?)this.GetValue(CultureProperty);
            set => this.SetValue(CultureProperty, value);
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            NumericUpDownBase.TextAlignmentProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((TextAlignment)NumericUpDownBase.TextAlignmentProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)this.GetValue(TextAlignmentProperty);
            set => this.SetValue(TextAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="NumericInputMode"/> dependency property.</summary>
        public static readonly DependencyProperty NumericInputModeProperty =
            NumericUpDownBase.NumericInputModeProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((NumericInput)NumericUpDownBase.NumericInputModeProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public NumericInput NumericInputMode
        {
            get => (NumericInput)this.GetValue(NumericInputModeProperty);
            set => this.SetValue(NumericInputModeProperty, value);
        }

        /// <summary>Identifies the <see cref="DecimalPointCorrection"/> dependency property.</summary>
        public static readonly DependencyProperty DecimalPointCorrectionProperty
            = DependencyProperty.Register(nameof(DecimalPointCorrection),
                                          typeof(DecimalPointCorrectionMode),
                                          typeof(DataGridNumericUpDownColumnBase),
                                          new PropertyMetadata(default(DecimalPointCorrectionMode)));

        /// <summary>
        /// Gets or sets the decimal-point correction mode. The default is <see cref="DecimalPointCorrectionMode.Inherits"/>
        /// </summary>
        public DecimalPointCorrectionMode DecimalPointCorrection
        {
            get => (DecimalPointCorrectionMode)this.GetValue(DecimalPointCorrectionProperty);
            set => this.SetValue(DecimalPointCorrectionProperty, value);
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty =
            NumericUpDownBase.DelayProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((int)NumericUpDownBase.DelayProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public int Delay
        {
            get => (int)this.GetValue(DelayProperty);
            set => this.SetValue(DelayProperty, value);
        }

        /// <summary>Identifies the <see cref="Speedup"/> dependency property.</summary>
        public static readonly DependencyProperty SpeedupProperty =
            NumericUpDownBase.SpeedupProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.SpeedupProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool Speedup
        {
            get => (bool)this.GetValue(SpeedupProperty);
            set => this.SetValue(SpeedupProperty, value);
        }

        /// <summary>Identifies the <see cref="SnapToMultipleOfInterval"/> dependency property.</summary>
        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty =
            NumericUpDownBase.SnapToMultipleOfIntervalProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.SnapToMultipleOfIntervalProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool SnapToMultipleOfInterval
        {
            get => (bool)this.GetValue(SnapToMultipleOfIntervalProperty);
            set => this.SetValue(SnapToMultipleOfIntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptArrowKeys"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptArrowKeysProperty =
            NumericUpDownBase.InterceptArrowKeysProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.InterceptArrowKeysProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptArrowKeys
        {
            get => (bool)this.GetValue(InterceptArrowKeysProperty);
            set => this.SetValue(InterceptArrowKeysProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptManualEnter"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptManualEnterProperty =
            NumericUpDownBase.InterceptManualEnterProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.InterceptManualEnterProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptManualEnter
        {
            get => (bool)this.GetValue(InterceptManualEnterProperty);
            set => this.SetValue(InterceptManualEnterProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptMouseWheel"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelProperty =
            NumericUpDownBase.InterceptMouseWheelProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.InterceptMouseWheelProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool InterceptMouseWheel
        {
            get => (bool)this.GetValue(InterceptMouseWheelProperty);
            set => this.SetValue(InterceptMouseWheelProperty, value);
        }

        /// <summary>Identifies the <see cref="TrackMouseWheelWhenMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty =
            NumericUpDownBase.TrackMouseWheelWhenMouseOverProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.TrackMouseWheelWhenMouseOverProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool TrackMouseWheelWhenMouseOver
        {
            get => (bool)this.GetValue(TrackMouseWheelWhenMouseOverProperty);
            set => this.SetValue(TrackMouseWheelWhenMouseOverProperty, value);
        }

        /// <summary>Identifies the <see cref="HideUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty HideUpDownButtonsProperty =
            NumericUpDownBase.HideUpDownButtonsProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.HideUpDownButtonsProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool HideUpDownButtons
        {
            get => (bool)this.GetValue(HideUpDownButtonsProperty);
            set => this.SetValue(HideUpDownButtonsProperty, value);
        }

        /// <summary>Identifies the <see cref="SwitchUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty SwitchUpDownButtonsProperty =
            NumericUpDownBase.SwitchUpDownButtonsProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((bool)NumericUpDownBase.SwitchUpDownButtonsProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool SwitchUpDownButtons
        {
            get => (bool)this.GetValue(SwitchUpDownButtonsProperty);
            set => this.SetValue(SwitchUpDownButtonsProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty =
            NumericUpDownBase.ButtonsAlignmentProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((ButtonsAlignment)NumericUpDownBase.ButtonsAlignmentProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public ButtonsAlignment ButtonsAlignment
        {
            get => (ButtonsAlignment)this.GetValue(ButtonsAlignmentProperty);
            set => this.SetValue(ButtonsAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty =
            NumericUpDownBase.UpDownButtonsWidthProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata((double)NumericUpDownBase.UpDownButtonsWidthProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double UpDownButtonsWidth
        {
            get => (double)this.GetValue(UpDownButtonsWidthProperty);
            set => this.SetValue(UpDownButtonsWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="FontFamily"/> dependency property.</summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The font family of the desired font.
        /// </summary>
        public FontFamily FontFamily
        {
            get => (FontFamily)this.GetValue(FontFamilyProperty);
            set => this.SetValue(FontFamilyProperty, value);
        }

        /// <summary>Identifies the <see cref="FontSize"/> dependency property.</summary>
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The size of the desired font.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        [Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get => (double)this.GetValue(FontSizeProperty);
            set => this.SetValue(FontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="FontStyle"/> dependency property.</summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The style of the desired font.
        /// </summary>
        public FontStyle FontStyle
        {
            get => (FontStyle)this.GetValue(FontStyleProperty);
            set => this.SetValue(FontStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="FontWeight"/> dependency property.</summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The weight or thickness of the desired font.
        /// </summary>
        public FontWeight FontWeight
        {
            get => (FontWeight)this.GetValue(FontWeightProperty);
            set => this.SetValue(FontWeightProperty, value);
        }

        /// <summary>Identifies the <see cref="Foreground"/> dependency property.</summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(DataGridNumericUpDownColumnBase),
                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// An brush that describes the foreground color. This overrides the cell foreground inherited color.
        /// </summary>
        public Brush Foreground
        {
            get => (Brush)this.GetValue(ForegroundProperty);
            set => this.SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// Method used as property changed callback for properties which need RefreshCellContent to be called
        /// </summary>
        private protected static void NotifyPropertyChangeForRefreshContent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d is DataGridNumericUpDownColumnBase, "d should be a DataGridNumericUpDownColumnBase");
            ((DataGridNumericUpDownColumnBase)d).NotifyPropertyChanged(e.Property.Name);
        }

        /// <summary>
        /// Rebuilds the contents of a cell in the column in response to a binding change.
        /// </summary>
        /// <param name="element">The cell to update.</param>
        /// <param name="propertyName">The name of the column property that has changed.</param>
        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            var cell = element as DataGridCell;

            if (cell?.Content is TextBlock textBlock)
            {
                switch (propertyName)
                {
                    case nameof(this.FontFamily):
                        SyncColumnProperty(this, textBlock, FontFamilyProperty, TextElement.FontFamilyProperty);
                        break;
                    case nameof(this.FontSize):
                        SyncColumnProperty(this, textBlock, FontSizeProperty, TextElement.FontSizeProperty);
                        break;
                    case nameof(this.FontStyle):
                        SyncColumnProperty(this, textBlock, FontStyleProperty, TextElement.FontStyleProperty);
                        break;
                    case nameof(this.FontWeight):
                        SyncColumnProperty(this, textBlock, FontWeightProperty, TextElement.FontWeightProperty);
                        break;
                    case nameof(this.Foreground):
                        SyncColumnProperty(this, textBlock, ForegroundProperty, TextElement.ForegroundProperty);
                        break;
                    case nameof(this.TextAlignment):
                        SyncColumnProperty(this, textBlock, TextAlignmentProperty, TextBlock.TextAlignmentProperty);
                        break;
                    default:
                        // StringFormat and Culture reach the text through their own bindings.
                        break;
                }
            }

            var numericUpDown = cell?.Content as NumericUpDownBase;
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
                        SyncColumnProperty(this, numericUpDown, TextAlignmentProperty, NumericUpDownBase.TextAlignmentProperty);
                        break;
                    case nameof(this.StringFormat):
                        SyncColumnProperty(this, numericUpDown, StringFormatProperty, NumericUpDownBase.StringFormatProperty);
                        break;
                    case nameof(this.Culture):
                        SyncColumnProperty(this, numericUpDown, CultureProperty, NumericUpDownBase.CultureProperty);
                        break;
                    case nameof(this.NumericInputMode):
                        SyncColumnProperty(this, numericUpDown, NumericInputModeProperty, NumericUpDownBase.NumericInputModeProperty);
                        break;
                    case nameof(this.DecimalPointCorrection):
                        SyncColumnProperty(this, numericUpDown, DecimalPointCorrectionProperty, NumericUpDownBase.DecimalPointCorrectionProperty);
                        break;
                    case nameof(this.Delay):
                        SyncColumnProperty(this, numericUpDown, DelayProperty, NumericUpDownBase.DelayProperty);
                        break;
                    case nameof(this.Speedup):
                        SyncColumnProperty(this, numericUpDown, SpeedupProperty, NumericUpDownBase.SpeedupProperty);
                        break;
                    case nameof(this.SnapToMultipleOfInterval):
                        SyncColumnProperty(this, numericUpDown, SnapToMultipleOfIntervalProperty, NumericUpDownBase.SnapToMultipleOfIntervalProperty);
                        break;
                    case nameof(this.InterceptArrowKeys):
                        SyncColumnProperty(this, numericUpDown, InterceptArrowKeysProperty, NumericUpDownBase.InterceptArrowKeysProperty);
                        break;
                    case nameof(this.InterceptManualEnter):
                        SyncColumnProperty(this, numericUpDown, InterceptManualEnterProperty, NumericUpDownBase.InterceptManualEnterProperty);
                        break;
                    case nameof(this.InterceptMouseWheel):
                        SyncColumnProperty(this, numericUpDown, InterceptMouseWheelProperty, NumericUpDownBase.InterceptMouseWheelProperty);
                        break;
                    case nameof(this.TrackMouseWheelWhenMouseOver):
                        SyncColumnProperty(this, numericUpDown, TrackMouseWheelWhenMouseOverProperty, NumericUpDownBase.TrackMouseWheelWhenMouseOverProperty);
                        break;
                    case nameof(this.HideUpDownButtons):
                        SyncColumnProperty(this, numericUpDown, HideUpDownButtonsProperty, NumericUpDownBase.HideUpDownButtonsProperty);
                        break;
                    case nameof(this.SwitchUpDownButtons):
                        SyncColumnProperty(this, numericUpDown, SwitchUpDownButtonsProperty, NumericUpDownBase.SwitchUpDownButtonsProperty);
                        break;
                    case nameof(this.ButtonsAlignment):
                        SyncColumnProperty(this, numericUpDown, ButtonsAlignmentProperty, NumericUpDownBase.ButtonsAlignmentProperty);
                        break;
                    case nameof(this.UpDownButtonsWidth):
                        SyncColumnProperty(this, numericUpDown, UpDownButtonsWidthProperty, NumericUpDownBase.UpDownButtonsWidthProperty);
                        break;
                    default:
                        this.SyncTypedProperty(numericUpDown, propertyName);
                        break;
                }
            }

            base.RefreshCellContent(element, propertyName);
        }
    }
}
