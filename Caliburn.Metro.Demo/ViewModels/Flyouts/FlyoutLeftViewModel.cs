namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class FlyoutLeftViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public FlyoutLeftViewModel()
        {
            this.Header = "left";
            this.Position = Position.Left;
        }

        #endregion
    }
}