using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    using System.ComponentModel;

    public static class ReloadBehavior
    {
        public static DependencyProperty OnDataContextChangedProperty =
            DependencyProperty.RegisterAttached("OnDataContextChanged", typeof(bool), typeof(ReloadBehavior), new PropertyMetadata(OnDataContextChanged));

        [Category(AppName.MahApps)]
        public static bool GetOnDataContextChanged(MetroContentControl element)
        {
            return (bool)element.GetValue(OnDataContextChangedProperty);
        }

        public static void SetOnDataContextChanged(MetroContentControl element, bool value)
        {
            element.SetValue(OnDataContextChangedProperty, value);
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MetroContentControl)d).DataContextChanged += ReloadDataContextChanged;
        }

        static void ReloadDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((MetroContentControl)sender).Reload();
        }

        public static DependencyProperty OnSelectedTabChangedProperty =
            DependencyProperty.RegisterAttached("OnSelectedTabChanged", typeof(bool), typeof(ReloadBehavior), new PropertyMetadata(OnSelectedTabChanged));

        [Category(AppName.MahApps)]
        public static bool GetOnSelectedTabChanged(ContentControl element)
        {
            return (bool)element.GetValue(OnDataContextChangedProperty);
        }

        public static void SetOnSelectedTabChanged(ContentControl element, bool value)
        {
            element.SetValue(OnDataContextChangedProperty, value);
        }

        private static void OnSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControl)d).Loaded += ReloadLoaded;
        }

        private static void ReloadLoaded(object sender, RoutedEventArgs e)
        {
            var metroContentControl = (ContentControl)sender;
            var tab = metroContentControl.TryFindParent<TabControl>();

            if (tab == null) return;

            SetMetroContentControl(tab, metroContentControl);
            tab.SelectionChanged -= ReloadSelectionChanged;
            tab.SelectionChanged += ReloadSelectionChanged;
        }
        
        private static void ReloadSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender)
                return;

            var contentControl = GetMetroContentControl((TabControl)sender);
            var metroContentControl = contentControl as MetroContentControl;
            if (metroContentControl != null)
            {
                metroContentControl.Reload();
            }
            
            var transitioningContentControl = contentControl as TransitioningContentControl;
            if (transitioningContentControl != null)
            {
                transitioningContentControl.ReloadTransition();
            }
        }

        public static readonly DependencyProperty MetroContentControlProperty =
            DependencyProperty.RegisterAttached("MetroContentControl", typeof(ContentControl), typeof(ReloadBehavior), new PropertyMetadata(default(ContentControl)));

        public static void SetMetroContentControl(UIElement element, ContentControl value)
        {
            element.SetValue(MetroContentControlProperty, value);
        }

        [Category(AppName.MahApps)]
        public static ContentControl GetMetroContentControl(UIElement element)
        {
            return (ContentControl)element.GetValue(MetroContentControlProperty);
        }
    }
}