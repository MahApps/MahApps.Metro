using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TabControlExamples.xaml
    /// </summary>
    public partial class TabControlExamples : UserControl
    {
        public TabControlExamples()
        {
            InitializeComponent();
        }

        private void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            if (e.ClosingTabItem.Header.ToString().StartsWith("sizes"))
                e.Cancel = true;
        }

        private void TextBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;

            textBlock.SetCurrentValue(TextBlock.TextProperty, (int.Parse(textBlock.Text) + 1).ToString());
        }
    }
}
