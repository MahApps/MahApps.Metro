using System.Windows.Controls;

namespace MahAppsMetroNavApp.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void OnOpenDetails(object sender, System.Windows.RoutedEventArgs e)
        {
            // a page reached from another page rather than from the pane. The frame keeps it in its
            // history all the same, which is what the arrow in the title bar walks.
            this.NavigationService?.Navigate(new DetailsPage());
        }
    }
}
