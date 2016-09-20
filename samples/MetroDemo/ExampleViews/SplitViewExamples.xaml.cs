namespace MetroDemo.ExampleViews
{
    using System.Windows;
    using System.Windows.Controls;
    using MahApps.Metro.Controls;

    /// <summary>
    ///     Interaction logic for SplitViewExamples.xaml
    /// </summary>
    public partial class SplitViewExamples
    {
        public SplitViewExamples()
        {
            this.InitializeComponent();

            this.SimpleSplitview.Tag = false;
            this.ShellSplitView.Tag = false;
            this.HomeRadioButton.IsChecked = true;
        }

        private void AboutRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.ShellSplitViewContentText != null)
                this.ShellSplitViewContentText.Text = "About page";
        }

        private void HomeRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.ShellSplitViewContentText != null)
                this.ShellSplitViewContentText.Text = "Home page";
        }

        private void SettingsRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.ShellSplitViewContentText != null)
                this.ShellSplitViewContentText.Text = "Settings page";
        }

        private void DontCheck(object sender, RoutedEventArgs e)
        {
            // don't let the radiobutton check
            var radioButton = sender as RadioButton;
            if (radioButton != null)
                radioButton.IsChecked = false;
        }

        private void Splitview_PaneClosing(object sender, SplitViewPaneClosingEventArgs e)
        {
            var splitView = sender as SplitView;

            if (splitView == null)
                return;

            e.Cancel = (bool)splitView.Tag;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.ShellSplitView == null)
                return;

            this.ShellSplitView.IsPaneOpen = !this.ShellSplitView.IsPaneOpen;
            DontCheck(sender, e);
        }
    }
}