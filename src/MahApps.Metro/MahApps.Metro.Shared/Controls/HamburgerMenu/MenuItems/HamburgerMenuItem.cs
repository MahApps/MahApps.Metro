namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;

    /// <summary>
    /// The HamburgerMenuItem provides an abstract implementation for HamburgerMenu entries.
    /// </summary>
    public abstract class HamburgerMenuItem : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TargetPageType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetPageTypeProperty = DependencyProperty.Register(nameof(TargetPageType), typeof(Type), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Tag"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(nameof(Tag), typeof(object), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets gets of sets a value that specifies label to display.
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

        /// <summary>
        /// Gets or sets gets of sets a value that specifies the page to navigate to (if you use the HamburgerMenu with a Frame content)
        /// </summary>
        public Type TargetPageType
        {
            get
            {
                return (Type)GetValue(TargetPageTypeProperty);
            }

            set
            {
                SetValue(TargetPageTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets gets of sets a value that specifies an user specific value.
        /// </summary>
        public object Tag
        {
            get
            {
                return GetValue(TagProperty);
            }

            set
            {
                SetValue(TagProperty, value);
            }
        }
    }
}
