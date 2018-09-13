using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
        /// <summary>
        /// Identifies the <see cref="HamburgerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerWidthProperty = DependencyProperty.Register(nameof(HamburgerWidth), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(48.0));

        /// <summary>
        /// Identifies the <see cref="HamburgerHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerHeightProperty = DependencyProperty.Register(nameof(HamburgerHeight), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(48.0));

        /// <summary>
        /// Identifies the <see cref="HamburgerMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerMarginProperty = DependencyProperty.Register(nameof(HamburgerMargin), typeof(Thickness), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HamburgerVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerVisibilityProperty = DependencyProperty.Register(nameof(HamburgerVisibility), typeof(Visibility), typeof(HamburgerMenu), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="HamburgerButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerButtonStyleProperty = DependencyProperty.Register(nameof(HamburgerButtonStyle), typeof(Style), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HamburgerButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerButtonTemplateProperty = DependencyProperty.Register(nameof(HamburgerButtonTemplate), typeof(DataTemplate), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Style used for the hamburger button.
        /// </summary>
        public Style HamburgerButtonStyle
        {
            get { return (Style)GetValue(HamburgerButtonStyleProperty); }
            set { SetValue(HamburgerButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a template for the hamburger button.
        /// </summary>
        public DataTemplate HamburgerButtonTemplate
        {
            get { return (DataTemplate)GetValue(HamburgerButtonTemplateProperty); }
            set { SetValue(HamburgerButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HamburgerMenuHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerMenuHeaderTemplateProperty = DependencyProperty.Register(nameof(HamburgerMenuHeaderTemplate), typeof(DataTemplate), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a template for the hamburger pane header.
        /// </summary>
        public DataTemplate HamburgerMenuHeaderTemplate
        {
            get { return (DataTemplate)GetValue(HamburgerMenuHeaderTemplateProperty); }
            set { SetValue(HamburgerMenuHeaderTemplateProperty, value); }
        }
        /// <summary>
        /// Gets or sets main button's width.
        /// </summary>
        public double HamburgerWidth
        {
            get { return (double)GetValue(HamburgerWidthProperty); }
            set { SetValue(HamburgerWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets main button's height.
        /// </summary>
        public double HamburgerHeight
        {
            get { return (double)GetValue(HamburgerHeightProperty); }
            set { SetValue(HamburgerHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets main button's margin.
        /// </summary>
        public Thickness HamburgerMargin
        {
            get { return (Thickness)GetValue(HamburgerMarginProperty); }
            set { SetValue(HamburgerMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets main button's visibility.
        /// </summary>
        public Visibility HamburgerVisibility
        {
            get { return (Visibility)GetValue(HamburgerVisibilityProperty); }
            set { SetValue(HamburgerVisibilityProperty, value); }
        }
    }
}
