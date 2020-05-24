using System.Windows;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuItemBase : Freezable
    {
        /// <summary>
        /// Identifies the <see cref="Tag"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(nameof(Tag), typeof(object), typeof(HamburgerMenuItemBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsVisible" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(HamburgerMenuItemBase), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that specifies an user specific value.
        /// </summary>
        public object Tag
        {
            get
            {
                return this.GetValue(TagProperty);
            }

            set
            {
                this.SetValue(TagProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether this element is visible in the user interface (UI). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the item is visible, otherwise false. The default value is true.
        /// </returns>
        public bool IsVisible
        {
            get
            {
                return (bool)this.GetValue(IsVisibleProperty);
            }

            set
            {
                this.SetValue(IsVisibleProperty, value);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItemBase();
        }
    }
}