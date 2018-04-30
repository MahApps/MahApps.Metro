namespace MetroDemo.ExampleViews
{
    using MahApps.Metro.Controls;

    /// <summary>
    ///     Interaction logic for SplitViewExamples.xaml
    /// </summary>
    public partial class SplitViewExamples
    {
        public SplitViewExamples()
        {
            this.InitializeComponent();

            // The Tag is used to handle closing
            this.SimpleSplitview.Tag = false;
        }

        private void Splitview_PaneClosing(object sender, SplitViewPaneClosingEventArgs e)
        {
            var splitView = sender as SplitView;

            if (splitView == null)
                return;

            e.Cancel = (bool)splitView.Tag;
        }
    }
}