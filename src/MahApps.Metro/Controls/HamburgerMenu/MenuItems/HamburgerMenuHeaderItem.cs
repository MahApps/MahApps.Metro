using System.Windows;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuHeaderItem : HamburgerMenuItemBase, IHamburgerMenuHeaderItem
    {
        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(HamburgerMenuHeaderItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies label to display.
        /// </summary>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuHeaderItem();
        }
    }
}
