using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for CleanWindowDemo.xaml
    /// </summary>
    public partial class CleanWindowDemo
    {
        public CleanWindowDemo()
        {
            this.InitializeComponent();

            var selector = new SuperDataTemplateSelector();
            selector.FirstTemplate = this.Resources["Temp1"] as DataTemplate;
            selector.SecondTemplate = this.Resources["Temp2"] as DataTemplate;
            selector.NullTemplate = this.Resources["Temp0"] as DataTemplate;

            this.Closing += this.CleanWindowClosing;
        }

        private bool closeMe;

        private async void CleanWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;

            // we want manage the closing itself!
            e.Cancel = !this.closeMe;
            // yes we want now really close the window
            if (this.closeMe) return;

            var mySettings = new MetroDialogSettings()
                             {
                                 AffirmativeButtonText = "Quit",
                                 NegativeButtonText = "Cancel",
                                 AnimateShow = true,
                                 AnimateHide = false
                             };
            var result = await this.ShowMessageAsync(
                "Quit application?",
                "Sure you want to quit application?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            this.closeMe = result == MessageDialogResult.Affirmative;

            if (this.closeMe) this.Close();
        }

        internal class SuperDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate FirstTemplate { get; set; }

            public DataTemplate SecondTemplate { get; set; }

            public DataTemplate NullTemplate { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item == null)
                {
                    return this.NullTemplate;
                }

                var text = (string)((ListBoxItem)item).Content;
                if (text == "Item 1")
                {
                    return this.FirstTemplate;
                }
                return this.SecondTemplate;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.settingsFlyout.IsOpen = !this.settingsFlyout.IsOpen;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Something",
                                  "Something should be displayed here.",
                                  MessageDialogStyle.Affirmative,
                                  new MetroDialogSettings()
                                  {
                                      ColorScheme = MetroDialogColorScheme.Inverted
                                  });
        }
    }
}