namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class FlyoutTopViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public FlyoutTopViewModel()
        {
            this.Header = "Top";
            this.Position = Position.Top;
        }

        #endregion
    }
}