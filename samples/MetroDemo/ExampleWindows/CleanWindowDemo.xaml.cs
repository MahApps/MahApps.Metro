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
            InitializeComponent();

            var selector = new SuperDataTemplateSelector();
            selector.FirstTemplate = this.Resources["Temp1"] as DataTemplate;
            selector.SecondTemplate = this.Resources["Temp2"] as DataTemplate;
            selector.NullTemplate = this.Resources["Temp0"] as DataTemplate;
            //this.tc.ContentTemplateSelector = selector;
        }

        internal class SuperDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public DataTemplate FirstTemplate { get; set; }
            public DataTemplate SecondTemplate { get; set; }
            public DataTemplate NullTemplate { get; set; }

            public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
            {
                if (item == null)
                    return this.NullTemplate;
                else
                {
                    var text = (string)((ListBoxItem)item).Content;
                    if (text == "Item 1")
                        return this.FirstTemplate;
                    else
                        return this.SecondTemplate;
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.settingsFlyout.IsOpen = !this.settingsFlyout.IsOpen;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowMessageAsync("Something", "Something should be displayed here.", MessageDialogStyle.Affirmative, new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Inverted
            });
        }
    }
}
