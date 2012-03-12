namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Originally from http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
    /// </summary>
    public class MetroContentControl : ContentControl
    {
        #region Constructors and Destructors

        public MetroContentControl()
        {
            this.DefaultStyleKey = typeof(MetroContentControl);

            this.Loaded += this.MetroContentControlLoaded;
            this.Unloaded += this.MetroContentControlUnloaded;

            this.IsVisibleChanged += this.MetroContentControlIsVisibleChanged;
        }

        #endregion

        #region Public Methods and Operators

        public void Reload()
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", true);
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }

        #endregion

        #region Methods

        private void MetroContentControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                VisualStateManager.GoToState(this, "AfterUnLoaded", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }
        }

        private void MetroContentControlLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }

        private void MetroContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterUnLoaded", false);
        }

        #endregion
    }
}