namespace MahApps.Metro.Behaviours
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using MahApps.Metro.Controls;

    public static class ReloadBehavior
    {
        #region Constants and Fields

        public static readonly DependencyProperty MetroContentControlProperty =
            DependencyProperty.RegisterAttached(
                "MetroContentControl",
                typeof(MetroContentControl),
                typeof(ReloadBehavior),
                new PropertyMetadata(default(MetroContentControl)));

        public static DependencyProperty OnDataContextChangedProperty =
            DependencyProperty.RegisterAttached(
                "OnDataContextChanged", typeof(bool), typeof(ReloadBehavior), new PropertyMetadata(OnDataContextChanged));

        public static DependencyProperty OnSelectedTabChangedProperty =
            DependencyProperty.RegisterAttached(
                "OnSelectedTabChanged", typeof(bool), typeof(ReloadBehavior), new PropertyMetadata(OnSelectedTabChanged));

        #endregion

        #region Public Methods and Operators

        public static MetroContentControl GetMetroContentControl(UIElement element)
        {
            return (MetroContentControl)element.GetValue(MetroContentControlProperty);
        }

        public static bool GetOnDataContextChanged(MetroContentControl element)
        {
            return (bool)element.GetValue(OnDataContextChangedProperty);
        }

        public static bool GetOnSelectedTabChanged(MetroContentControl element)
        {
            return (bool)element.GetValue(OnDataContextChangedProperty);
        }

        public static void SetMetroContentControl(UIElement element, MetroContentControl value)
        {
            element.SetValue(MetroContentControlProperty, value);
        }

        public static void SetOnDataContextChanged(MetroContentControl element, bool value)
        {
            element.SetValue(OnDataContextChangedProperty, value);
        }

        public static void SetOnSelectedTabChanged(MetroContentControl element, bool value)
        {
            element.SetValue(OnDataContextChangedProperty, value);
        }

        #endregion

        #region Methods

        private static IEnumerable<DependencyObject> Ancestors(DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                yield return parent;
                obj = parent;
                parent = VisualTreeHelper.GetParent(obj);
            }
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MetroContentControl)d).DataContextChanged += ReloadDataContextChanged;
        }

        private static void OnSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MetroContentControl)d).Loaded += ReloadLoaded;
        }

        private static void ReloadDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((MetroContentControl)sender).Reload();
        }

        private static void ReloadLoaded(object sender, RoutedEventArgs e)
        {
            var metroContentControl = ((MetroContentControl)sender);
            var tab = Ancestors(metroContentControl).OfType<TabControl>().FirstOrDefault();

            if (tab == null)
            {
                return;
            }

            SetMetroContentControl(tab, metroContentControl);
            tab.SelectionChanged += ReloadSelectionChanged;
        }

        private static void ReloadSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender)
            {
                return;
            }

            GetMetroContentControl((TabControl)sender).Reload();
        }

        #endregion
    }
}