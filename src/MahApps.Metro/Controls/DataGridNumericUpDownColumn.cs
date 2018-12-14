using System.ComponentModel;
using System.Diagnostics;
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
                    Style style = new Style(typeof(NumericUpDown));

                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0.0)));
                    style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0.0)));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                    style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));

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
                    Style style = new Style(typeof(NumericUpDown));

                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0d)));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
                    style.Setters.Add(new Setter(UIElement.FocusableProperty, false));
                    style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, true));
                    style.Setters.Add(new Setter(Control.BackgroundProperty, Brushes.Transparent));
                    style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
                    style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0d));
                    style.Setters.Add(new Setter(ControlsHelper.DisabledVisualElementVisibilityProperty, Visibility.Collapsed));

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
            Style style = PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
            {
                element.Style = style;
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateNumericUpDown(true, cell);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            NumericUpDown generateNumericUpDown = GenerateNumericUpDown(false, cell);
            generateNumericUpDown.HideUpDownButtons = true;
            return generateNumericUpDown;
        }

        private NumericUpDown GenerateNumericUpDown(bool isEditing, DataGridCell cell)
        {
            NumericUpDown numericUpDown = (cell != null) ? (cell.Content as NumericUpDown) : null;
            if (numericUpDown == null)
            {
                numericUpDown = new NumericUpDown();
            }

            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontFamilyProperty, TextElement.FontFamilyProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontSizeProperty, TextElement.FontSizeProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontStyleProperty, TextElement.FontStyleProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontWeightProperty, TextElement.FontWeightProperty);

            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.StringFormatProperty, NumericUpDown.StringFormatProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.MinimumProperty, NumericUpDown.MinimumProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.MaximumProperty, NumericUpDown.MaximumProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.IntervalProperty, NumericUpDown.IntervalProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.HideUpDownButtonsProperty, NumericUpDown.HideUpDownButtonsProperty);
            SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.UpDownButtonsWidthProperty, NumericUpDown.UpDownButtonsWidthProperty);

            if (isEditing)
            {
                SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.ForegroundProperty, TextElement.ForegroundProperty);
            }
            else
            {
                if (!SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.ForegroundProperty, TextElement.ForegroundProperty))
                {
                    ApplyBinding(new Binding(Control.ForegroundProperty.Name) { Source = cell, Mode = BindingMode.OneWay }, numericUpDown, TextElement.ForegroundProperty);
                }
            }

            ApplyStyle(isEditing, true, numericUpDown);
            ApplyBinding(Binding, numericUpDown, NumericUpDown.ValueProperty);

            numericUpDown.InterceptArrowKeys = true;
            numericUpDown.InterceptMouseWheel = true;
            numericUpDown.Speedup = true;

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
            NumericUpDown numericUpDown = editingElement as NumericUpDown;
            if (numericUpDown != null)
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
            Style style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
            {
                style = ElementStyle;
            }
            return style;
        }

        /// <summary>
        /// The DependencyProperty for the StringFormat property. 
        /// </summary> 
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
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the Minimum property. 
        /// </summary> 
        public static readonly DependencyProperty MinimumProperty =
            NumericUpDown.MinimumProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.MinimumProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the Maximum property. 
        /// </summary> 
        public static readonly DependencyProperty MaximumProperty =
            NumericUpDown.MaximumProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.MaximumProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the Interval property. 
        /// </summary> 
        public static readonly DependencyProperty IntervalProperty =
            NumericUpDown.IntervalProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.IntervalProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the HideUpDownButtons property. 
        /// </summary> 
        public static readonly DependencyProperty HideUpDownButtonsProperty =
            NumericUpDown.HideUpDownButtonsProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((bool)NumericUpDown.HideUpDownButtonsProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public bool HideUpDownButtons
        {
            get { return (bool)GetValue(HideUpDownButtonsProperty); }
            set { SetValue(HideUpDownButtonsProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the UpDownButtonsWidth property. 
        /// </summary> 
        public static readonly DependencyProperty UpDownButtonsWidthProperty =
            NumericUpDown.UpDownButtonsWidthProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata((double)NumericUpDown.UpDownButtonsWidthProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        public double UpDownButtonsWidth
        {
            get { return (double)GetValue(UpDownButtonsWidthProperty); }
            set { SetValue(UpDownButtonsWidthProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the FontFamily property. 
        /// Default Value: SystemFonts.MessageFontFamily
        /// </summary> 
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary> 
        /// The font family of the desired font. 
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the FontSize property. 
        /// Default Value: SystemFonts.MessageFontSize
        /// </summary>
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
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary> 
        /// The DependencyProperty for the FontStyle property.
        /// Default Value: SystemFonts.MessageFontStyle 
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The style of the desired font. 
        /// </summary> 
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary> 
        /// The DependencyProperty for the FontWeight property.
        /// Default Value: SystemFonts.MessageFontWeight
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// The weight or thickness of the desired font. 
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the Foreground property.
        /// Default Value: SystemColors.ControlTextBrush 
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(DataGridNumericUpDownColumn),
                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// An brush that describes the foreground color. This overrides the cell foreground inherited color.
        /// </summary> 
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
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
                    case nameof(FontFamily):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontFamilyProperty, TextElement.FontFamilyProperty);
                        break;
                    case nameof(FontSize):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontSizeProperty, TextElement.FontSizeProperty);
                        break;
                    case nameof(FontStyle):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontStyleProperty, TextElement.FontStyleProperty);
                        break;
                    case nameof(FontWeight):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.FontWeightProperty, TextElement.FontWeightProperty);
                        break;
                    case nameof(StringFormat):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.StringFormatProperty, NumericUpDown.StringFormatProperty);
                        break;
                    case nameof(Minimum):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.MinimumProperty, NumericUpDown.MinimumProperty);
                        break;
                    case nameof(Maximum):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.MaximumProperty, NumericUpDown.MaximumProperty);
                        break;
                    case nameof(Interval):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.IntervalProperty, NumericUpDown.IntervalProperty);
                        break;
                    case nameof(HideUpDownButtons):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.HideUpDownButtonsProperty, NumericUpDown.HideUpDownButtonsProperty);
                        break;
                    case nameof(UpDownButtonsWidth):
                        SyncColumnProperty(this, numericUpDown, DataGridNumericUpDownColumn.UpDownButtonsWidthProperty, NumericUpDown.UpDownButtonsWidthProperty);
                        break;
                }
            }
            base.RefreshCellContent(element, propertyName);
        }
    }
}