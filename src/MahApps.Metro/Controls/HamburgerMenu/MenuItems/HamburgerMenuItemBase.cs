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

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItemBase();
        }
    }
}