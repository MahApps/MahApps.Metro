using Caliburn.Micro;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    public class Flyout3ViewModel : FlyoutBaseViewModel
    {
        private readonly IObservableCollection<Artist> artists =
            new BindableCollection<Artist>();

        public IObservableCollection<Artist> Artists
        {
            get
            {
                return this.artists;
            }
        }

        public Flyout3ViewModel()
        {
            SampleData.Seed();
            this.Artists.AddRange(SampleData.Artists);
            this.Header = "third";
            this.Position = Position.Right;
        }
    }
}