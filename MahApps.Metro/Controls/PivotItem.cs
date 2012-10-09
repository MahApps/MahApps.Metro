using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class PivotItem : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(PivotItem), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        static PivotItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotItem), new FrameworkPropertyMetadata(typeof(PivotItem)));
        }

        public PivotItem()
        {
            RequestBringIntoView += (s, e) => { e.Handled = true; };
        }
    }
}