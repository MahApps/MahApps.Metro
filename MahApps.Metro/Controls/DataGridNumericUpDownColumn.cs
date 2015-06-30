using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));
                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0d)));
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

                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
                    style.Setters.Add(new Setter(UIElement.FocusableProperty, false));
                    style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, true));
                    style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0d)));
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

        internal void ApplyBinding(DependencyObject target, DependencyProperty property)
        {
            BindingBase binding = Binding;
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
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

        internal void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
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

            ApplyStyle(isEditing, true, numericUpDown);
            ApplyBinding(numericUpDown, NumericUpDown.ValueProperty);

            if (!isEditing)
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
    }
}