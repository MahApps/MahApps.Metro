namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class DataGridNumericUpDownColumn : DataGridBoundColumn
    {
        #region Static Fields

        private static Style _defaultEditingElementStyle;
        private static Style _defaultElementStyle;

        #endregion

        #region Constructors and Destructors

        static DataGridNumericUpDownColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #endregion

        #region Public Properties

        public static Style DefaultEditingElementStyle
        {
            get
            {
                if (_defaultEditingElementStyle == null)
                {
                    Style style = new Style(typeof(NumericUpDown));

                    style.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));

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

                    style.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
                    style.Setters.Add(new Setter(UIElement.FocusableProperty, false));
                    style.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                    style.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top));
                    style.Setters.Add(new Setter(NumericUpDown.HideUpDownButtonsProperty, true));

                    style.Seal();
                    _defaultElementStyle = style;
                }

                return _defaultElementStyle;
            }
        }

        #endregion

        #region Methods

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
            return GenerateNumericUpDown(false, cell);
        }

        private NumericUpDown GenerateNumericUpDown(bool isEditing, DataGridCell cell)
        {
            NumericUpDown numericUpDown = (cell != null) ? (cell.Content as NumericUpDown) : null;
            if (numericUpDown == null)
            {
                numericUpDown = new NumericUpDown();
            }

            ApplyStyle(isEditing, true, numericUpDown);
            ApplyBinding(numericUpDown, NumericUpDown.ValueProperty);

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

        #endregion
    }
}