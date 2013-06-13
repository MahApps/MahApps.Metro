namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class Flyout2ViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public Flyout2ViewModel()
        {
            this.Header = "new goodness";
            this.Position = Position.Right;
        }

        #endregion
    }
}