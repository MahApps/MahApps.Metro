using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MetroDemo.Models;
using Newtonsoft.Json;

namespace MetroDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<PanoramaGroup> Groups { get; set; }
        private readonly Dispatcher _dispatcher;
        readonly PanoramaGroup tracks;
        readonly PanoramaGroup artists;

        public ObservableCollection<Track> Tracks { get; set; }

        public MainWindowViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            tracks = new PanoramaGroup("trending tracks");
            artists = new PanoramaGroup("trending artists");
            Groups = new ObservableCollection<PanoramaGroup> { tracks, artists };

            var wc = new WebClient();
            wc.DownloadStringCompleted += WcDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://ws.audioscrobbler.com/2.0/?method=chart.gethypedartists&api_key=b25b959554ed76058ac220b7b2e0a026&format=json"));

            var wc2 = new WebClient();
            wc2.DownloadStringCompleted += WcDownloadStringCompleted2;
            wc2.DownloadStringAsync(new Uri("http://ws.audioscrobbler.com/2.0/?method=chart.gethypedtracks&api_key=b25b959554ed76058ac220b7b2e0a026&format=json"));
        }

        private void WcDownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<TrackWrapper>(e.Result);
                _dispatcher.BeginInvoke(new Action(() =>
                                                       {
                                                           tracks.SetSource(x.Tracks.track.Take(25));
                                                           Tracks = new ObservableCollection<Track>(x.Tracks.track.Take(25));
                                                       }));

            }
            catch (Exception ex)
            {

            }
        }

        private void WcDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<Wrapper>(e.Result);
                _dispatcher.BeginInvoke(new Action(() => artists.SetSource(x.Artists.artist.Take(25))));
                
            }
            catch (Exception ex)
            {

            }
        }

    }
}