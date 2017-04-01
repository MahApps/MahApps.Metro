using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for ButtonsExample.xaml
    /// </summary>
    public partial class ButtonsExample : UserControl
    {
        public ButtonsExample()
        {
            this.InitializeComponent();
        }

        private void CountingButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.CountingBadge.Badge == null || Equals(this.CountingBadge.Badge, ""))
            {
                this.CountingBadge.Badge = 0;
            }
            var next = int.Parse(this.CountingBadge.Badge.ToString()) + 1;
            this.CountingBadge.Badge = next < 43 ? (object)next : null;
        }

        private void SplitButton_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ((Selector)sender).SelectedIndex;
            var item = ((Selector)sender).SelectedItem;
            var value = ((Selector)sender).SelectedValue;
        }
    }
}
