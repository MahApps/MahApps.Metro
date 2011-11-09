using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class AppBarButton : Button
    {
        public AppBarButton()
        {
            DefaultStyleKey = typeof (AppBarButton);
        }

        public static readonly DependencyProperty MetroImageSourceProperty =
            DependencyProperty.Register("MetroImageSource", typeof(Visual), typeof(AppBarButton), new PropertyMetadata(default(Visual)));

        public Visual MetroImageSource
        {
            get { return (Visual)GetValue(MetroImageSourceProperty); }
            set { SetValue(MetroImageSourceProperty, value); }
        }
    }
}