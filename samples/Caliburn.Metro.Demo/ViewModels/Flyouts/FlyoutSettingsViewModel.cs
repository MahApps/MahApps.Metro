namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    using MahApps.Metro.Controls;

    public class FlyoutSettingsViewModel : FlyoutBaseViewModel
    {
        #region Constructors and Destructors

        public FlyoutSettingsViewModel()
        {
            this.Header = "settings";
            this.Position = Position.Right;
        }

        #endregion
    }
}