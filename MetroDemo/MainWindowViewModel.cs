using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Threading;
using MetroDemo.Models;
using Newtonsoft.Json;

namespace MetroDemo
{
    public class MainWindowViewModel
    {
        private readonly Dispatcher _dispatcher;

        public MainWindowViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Artists = new ObservableCollection<Artist>();

            var wc = new WebClient();
            wc.DownloadStringCompleted += WcDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://ws.audioscrobbler.com/2.0/?method=chart.gethypedartists&api_key=b25b959554ed76058ac220b7b2e0a026&format=json"));
        }

        private void WcDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<Wrapper>(e.Result);
                _dispatcher.BeginInvoke(new Action(() =>
                                                       {
                                                           foreach (var artist in x.Artists.artist)
                                                           {
                                                               Artists.Add(artist);
                                                           }
                                                       }));
            }
            catch (Exception ex)
            {

            }
        }

        public ObservableCollection<Artist> Artists { get; private set; }
    }
}