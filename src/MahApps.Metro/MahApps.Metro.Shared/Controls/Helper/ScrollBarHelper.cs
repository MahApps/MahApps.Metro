using System.Windows;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class ScrollBarHelper
    {
        /// <summary>
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide", typeof(bool), typeof(ScrollBarHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetVerticalScrollBarOnLeftSide(ScrollViewer obj)
        {
            return (bool)obj.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        public static void SetVerticalScrollBarOnLeftSide(ScrollViewer obj, bool value)
        {
            obj.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }

        /// <summary>
        /// This property can be used to trigger the call to a command when the user reach the end of the scrollable area.
        /// </summary>
        public static readonly DependencyProperty EndOfScrollReachedCommandProperty =
            DependencyProperty.RegisterAttached("EndOfScrollReachedCommand", typeof(ICommand), typeof(ScrollBarHelper),
                new FrameworkPropertyMetadata(null, EndOfScrollReachedCommandPropertyChanged));

        /// <summary>
        /// This property can be used to provide a command parameter to the command called when reaching the end of the scrollable area.
        /// </summary>
        public static readonly DependencyProperty EndOfScrollReachedCommandParameterProperty =
            DependencyProperty.RegisterAttached("EndOfScrollReachedCommandParameter", typeof(object), typeof(ScrollBarHelper),
                new FrameworkPropertyMetadata(null));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static ICommand GetEndOfScrollReachedCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(EndOfScrollReachedCommandProperty);
        }

        public static void SetEndOfScrollReachedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EndOfScrollReachedCommandProperty, value);
        }

        [Category(AppName.MahApps)]
        public static object GetEndOfScrollReachedCommandParameter(DependencyObject d)
        {
            return d.GetValue(EndOfScrollReachedCommandParameterProperty);
        }

        public static void SetEndOfScrollReachedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EndOfScrollReachedCommandParameterProperty, value);
        }

        private static void EndOfScrollReachedCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer == null)
                return;

            scrollViewer.Loaded += OnScrollViewerLoaded;
        }

        private static void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null)
                return;

            scrollViewer.Unloaded += OnScrollViewerUnloaded;
            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
        }

        private static void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (scrollViewer?.VerticalOffset < scrollViewer?.ScrollableHeight)
                return;

            var command = GetEndOfScrollReachedCommand(scrollViewer);
            var commandParameter = GetEndOfScrollReachedCommandParameter(scrollViewer);
            if (command == null || !command.CanExecute(commandParameter))
                return;

            command.Execute(commandParameter);
        }

        private static void OnScrollViewerUnloaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null)
                return;

            scrollViewer.Unloaded -= OnScrollViewerUnloaded;
            scrollViewer.ScrollChanged -= OnScrollViewerScrollChanged;
        }
    }
}
