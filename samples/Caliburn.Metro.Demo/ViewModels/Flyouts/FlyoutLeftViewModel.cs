using MahApps.Metro.Controls;

namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    public class FlyoutLeftViewModel : FlyoutBaseViewModel
    {
        public FlyoutLeftViewModel()
        {
            this.Header = "left";
            this.Position = Position.Left;
        }
    }
}