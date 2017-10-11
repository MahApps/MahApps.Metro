using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroHeaderControl : HeaderedContentControl
    {
        static MetroHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroHeaderControl), new FrameworkPropertyMetadata(typeof(MetroHeaderControl)));
        }
    }
}