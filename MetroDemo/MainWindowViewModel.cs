using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MetroDemo.Models;
using MetroDemo.ViewModels;

namespace MetroDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly PanoramaGroup _albums;
        readonly PanoramaGroup _artists;

        public MainWindowViewModel()
        {
            SampleData.Seed();
            
            Albums = SampleData.Albums;
            Artists = SampleData.Artists;
            
            Busy = true;

            _albums = new PanoramaGroup("trending tracks");
            _artists = new PanoramaGroup("trending artists");

            Groups = new ObservableCollection<PanoramaGroup> { _albums, _artists };

            _artists.SetSource(SampleData.Artists.Take(25));
            _albums.SetSource(SampleData.Albums.Take(25));

            ValidationExampleViewModel = new ValidationExampleViewModel();

            Busy = false;
        }

        public ObservableCollection<PanoramaGroup> Groups { get; set; }
        public bool Busy { get; set; }
        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public ValidationExampleViewModel ValidationExampleViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}