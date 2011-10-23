using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Originally from http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
    /// </summary>
    public class MetroContentControl : ContentControl
    {
        public MetroContentControl()
        {
            DefaultStyleKey = typeof(MetroContentControl);

            Loaded += MetroContentControlLoaded;
            Unloaded += MetroContentControlUnloaded;

            IsVisibleChanged += MetroContentControlIsVisibleChanged;
        }

        void MetroContentControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                VisualStateManager.GoToState(this, "AfterUnLoaded", false);
            else
                VisualStateManager.GoToState(this, "AfterLoaded", true);
        }

        private void MetroContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterUnLoaded", false);
        }

        private void MetroContentControlLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }

        public void Reload()
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", true);
            VisualStateManager.GoToState(this, "AfterLoaded", true);            
        }
    }
}
