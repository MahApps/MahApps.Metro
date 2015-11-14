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
        private double minimum = (double)NumericUpDown.MinimumProperty.DefaultMetadata.DefaultValue;
        private double maximum = (double)NumericUpDown.MaximumProperty.DefaultMetadata.DefaultValue;
        private double interval = (double)NumericUpDown.IntervalProperty.DefaultMetadata.DefaultValue;
        private string stringFormat = (string)NumericUpDown.StringFormatProperty.DefaultMetadata.DefaultValue;
        private bool hideUpDownButtons = (bool)NumericUpDown.HideUpDownButtonsProperty.DefaultMetadata.DefaultValue;
        private double upDownButtonsWidth = (double)NumericUpDown.UpDownButtonsWidthProperty.DefaultMetadata.DefaultValue;
        private Binding foregroundBinding;

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
                // create binding to cell foreground to get changed brush from selection
                foregroundBinding = new Binding("Foreground") { Source = cell, Mode = BindingMode.OneWay };
            }

            SyncProperties(numericUpDown);

            ApplyStyle(isEditing, true, numericUpDown);
            ApplyBinding(Binding, numericUpDown, NumericUpDown.ValueProperty);

            if (!isEditing && Equals(this.Foreground, SystemColors.ControlTextBrush))
            {
                // bind to cell foreground to get changed brush from selection
                ApplyBinding(foregroundBinding, numericUpDown, Control.ForegroundProperty);
            }
            else
            {
                // no foreground change for editing
                BindingOperations.ClearBinding(numericUpDown, Control.ForegroundProperty);
            }

            numericUpDown.Minimum = Minimum;
            numericUpDown.Maximum = Maximum;
            numericUpDown.StringFormat = StringFormat;
            numericUpDown.Interval = Interval;
            numericUpDown.InterceptArrowKeys = true;
            numericUpDown.InterceptMouseWheel = true;
            numericUpDown.Speedup = true;
            numericUpDown.HideUpDownButtons = HideUpDownButtons;
            numericUpDown.UpDownButtonsWidth = UpDownButtonsWidth;

            return numericUpDown;
        }

        private void SyncProperties(FrameworkElement e)
        {
            SyncColumnProperty(this, e, TextElement.FontFamilyProperty, FontFamilyProperty);
            SyncColumnProperty(this, e, TextElement.FontSizeProperty, FontSizeProperty);
            SyncColumnProperty(this, e, TextElement.FontStyleProperty, FontStyleProperty);
            SyncColumnProperty(this, e, TextElement.FontWeightProperty, FontWeightProperty);
            SyncColumnProperty(this, e, TextElement.ForegroundProperty, ForegroundProperty);
        }

        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            var cell = element as DataGridCell;
            if (cell != null)
            {
                var numericUpDown = cell.Content as FrameworkElement;
                if (numericUpDown != null)
                {
                    switch (propertyName)
                    {
                        case "FontFamily":
                            SyncColumnProperty(this, numericUpDown, TextElement.FontFamilyProperty, FontFamilyProperty);
                            break;
                        case "FontSize":
                            SyncColumnProperty(this, numericUpDown, TextElement.FontSizeProperty, FontSizeProperty);
                            break;
                        case "FontStyle":
                            SyncColumnProperty(this, numericUpDown, TextElement.FontStyleProperty, FontStyleProperty);
                            break;
                        case "FontWeight":
                            SyncColumnProperty(this, numericUpDown, TextElement.FontWeightProperty, FontWeightProperty);
                            break;
                        case "Foreground":
                            SyncColumnProperty(this, numericUpDown, TextElement.ForegroundProperty, ForegroundProperty);
                            break;
                    }
                }
            }
            base.RefreshCellContent(element, propertyName);
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
        private static void SyncColumnProperty(DependencyObject column, DependencyObject content, DependencyProperty contentProperty, DependencyProperty columnProperty)
        {
            if (IsDefaultValue(column, columnProperty))
            {
                content.ClearValue(contentProperty);
            }
            else
            {
                content.SetValue(contentProperty, column.GetValue(columnProperty));
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

        public double Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        public double Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public string StringFormat
        {
            get { return stringFormat; }
            set { stringFormat = value; }
        }

        public bool HideUpDownButtons
        {
            get { return hideUpDownButtons; }
            set { hideUpDownButtons = value; }
        }

        public double UpDownButtonsWidth
        {
            get { return upDownButtonsWidth; }
            set { upDownButtonsWidth = value; }
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
    }
}