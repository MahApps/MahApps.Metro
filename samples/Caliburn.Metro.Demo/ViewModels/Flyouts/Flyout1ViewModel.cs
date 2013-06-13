namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class Flyout1ViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public Flyout1ViewModel()
        {
            this.Header = "settings";
            this.Position = Position.Right;
        }

        #endregion
    }
}