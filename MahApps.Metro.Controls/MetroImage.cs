using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class MetroImage : Control
    {
        public MetroImage()
        {
            DefaultStyleKey = typeof(MetroImage);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Visual), typeof(MetroImage), new PropertyMetadata(default(Visual)));

        public Visual Source
        {
            get { return (Visual)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}