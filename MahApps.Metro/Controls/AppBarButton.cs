namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class AppBarButton : Button
    {
        #region Constants and Fields

        public static readonly DependencyProperty MetroImageSourceProperty =
            DependencyProperty.Register(
                "MetroImageSource", typeof(Visual), typeof(AppBarButton), new PropertyMetadata(default(Visual)));

        #endregion

        #region Constructors and Destructors

        public AppBarButton()
        {
            this.DefaultStyleKey = typeof(AppBarButton);
        }

        #endregion

        #region Public Properties

        public Visual MetroImageSource
        {
            get
            {
                return (Visual)this.GetValue(MetroImageSourceProperty);
            }
            set
            {
                this.SetValue(MetroImageSourceProperty, value);
            }
        }

        #endregion
    }
}