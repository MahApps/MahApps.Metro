namespace MetroDemo
{
    using System.Windows;

    public partial class PanoramaDemo
    {
        #region Constructors and Destructors

        public PanoramaDemo()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
        }

        #endregion

        #region Methods

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainWindowViewModel(this.Dispatcher);
        }

        #endregion
    }
}