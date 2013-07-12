using MahApps.Metro.Controls;

namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    public class FlyoutSettingsViewModel : FlyoutBaseViewModel
    {
        public FlyoutSettingsViewModel()
        {
            this.Header = "settings";
            this.Position = Position.Right;
        }
    }
}