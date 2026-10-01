using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace MahAppsMetroNavApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // where the window opens, and where the history of the frame starts
            this.Menu.SelectedIndex = 0;
        }

        private void OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.Navigate((e.InvokedItem as HamburgerMenuItem)?.TargetPageType);
        }

        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            if (this.ContentFrame.CanGoBack)
            {
                this.ContentFrame.GoBack();
            }
        }

        /// <summary>
        /// Shows the page of that type, where it is not the page being shown already.
        /// </summary>
        /// <remarks>
        /// Asking for the page that is up is not a step of its own, and that is also what keeps the
        /// pane and the frame from pushing each other around: going back marks the entry of the page
        /// the history brought back, and marking an entry asks for the page the frame is now on.
        /// </remarks>
        private void Navigate(Type? pageType)
        {
            if (pageType is null || this.ContentFrame.Content?.GetType() == pageType)
            {
                return;
            }

            this.ContentFrame.Navigate(Activator.CreateInstance(pageType));
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // the pane follows the frame, so whatever the history brings back is marked there, and a
            // page that belongs to no entry, as the details page does, leaves the pane unmarked
            var pageType = e.Content?.GetType();

            this.Menu.SelectedItem = EntryFor(this.Menu.ItemsSource, pageType);
            this.Menu.SelectedOptionsItem = EntryFor(this.Menu.OptionsItemsSource, pageType);
        }

        private static object? EntryFor(object? entries, Type? pageType)
        {
            return (entries as IEnumerable)?
                   .OfType<HamburgerMenuItem>()
                   .FirstOrDefault(entry => entry.TargetPageType == pageType);
        }
    }
}
