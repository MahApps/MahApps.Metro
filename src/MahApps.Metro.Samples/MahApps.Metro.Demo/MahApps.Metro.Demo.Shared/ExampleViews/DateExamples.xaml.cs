using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    using System.Windows;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for DateExamples.xaml
    /// </summary>
    public partial class DateExamples : UserControl
    {
        public DateExamples()
        {
            InitializeComponent();
#if NET4_5
            this.AutoWatermark.Visibility = Visibility.Visible;
            this.AutoWatermark.SetValue(TextBoxHelper.AutoWatermarkProperty, true);
#endif
        }
    }
}
