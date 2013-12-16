using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;
namespace MetroDemo
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
            tc.ContentTemplateSelector = selector;
        }

        internal class SuperDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public DataTemplate FirstTemplate { get; set; }
            public DataTemplate SecondTemplate { get; set; }
            public DataTemplate NullTemplate { get; set; }

            public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
            {
                if (item == null)
                    return NullTemplate;
                else
                {
                    var text = (string)((ListBoxItem)item).Content;
                    if (text == "Item 1")
                        return FirstTemplate;
                    else
                        return SecondTemplate;
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowMessageAsync("Something", "Something should be displayed here.");
        }
    }
}
