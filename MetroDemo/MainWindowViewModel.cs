using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace MetroDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<PanoramaGroup> Groups { get; set; }
        readonly PanoramaGroup _albums;
        readonly PanoramaGroup _artists;

        public bool Busy { get; set; }

        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public MainWindowViewModel(Dispatcher dispatcher)
        {

            SampleData.Seed();
            Albums = SampleData.Albums;
            Artists = SampleData.Artists;
            Busy = true;
            _albums = new PanoramaGroup("trending tracks");
            _artists = new PanoramaGroup("trending artists");
            Groups = new ObservableCollection<PanoramaGroup> {_albums, _artists};

            _artists.SetSource(SampleData.Artists.Take(25));
            _albums.SetSource(SampleData.Albums.Take(25));
            Busy = false;
        }
    }
}