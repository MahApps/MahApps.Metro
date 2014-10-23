using MahApps.Metro.Controls;

namespace MetroDemo.Navigation
{
    public partial class NavigationFrameWindow : MetroWindow
    {
        public NavigationFrameWindow()
        {
            InitializeComponent();
        }

        public void NavigateTo(object o)
        {
            Frame1.Navigate(o);
        }
    }
}