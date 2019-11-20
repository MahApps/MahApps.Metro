using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for TextExamples.xaml
    /// </summary>
    public partial class TextExamples : UserControl
    {
        public static readonly string[] NUD_StringFormats =
        {
            "C",
            "P",
            "N0",
            "{}you have {0:N0} pieces",
            "X"
        };

        public TextExamples()
        {
            this.InitializeComponent();
        }
    }
}