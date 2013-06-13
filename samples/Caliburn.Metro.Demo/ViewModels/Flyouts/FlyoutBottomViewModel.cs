namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class FlyoutBottomViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public FlyoutBottomViewModel()
        {
            this.Header = "Bottom";
            this.Position = Position.Bottom;
        }

        #endregion
    }
}