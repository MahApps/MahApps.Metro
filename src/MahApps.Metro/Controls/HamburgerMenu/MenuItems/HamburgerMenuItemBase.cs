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
        /// Identifies the <see cref="Visibility" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(HamburgerMenuItemBase), new PropertyMetadata(Visibility.Visible));

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
        /// Gets or sets the user interface (UI) visibility of this element. This is a dependency property.
        /// </summary>
        /// <returns>
        /// A value of the enumeration. The default value is System.Windows.Visibility.Visible.
        /// </returns>
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItemBase();
        }
    }
}