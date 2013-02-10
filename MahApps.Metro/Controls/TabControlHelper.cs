using System.Windows;

namespace MahApps.Metro.Controls
{
    public class TabControlHelper : DependencyObject
    {
        public static DependencyProperty TabStripMarginProperty =
            DependencyProperty.RegisterAttached("TabStripMargin", 
            typeof(Thickness), 
            typeof(TabControlHelper), 
            new UIPropertyMetadata(new Thickness(0, 0, 0, 0)));       

        public static Thickness GetTabStripMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(TabStripMarginProperty);
        }

        public static void SetTabStripMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(TabStripMarginProperty, value);
        }
    }
}
