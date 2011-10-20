using System;
using System.Net;
using System.Windows;
using Newtonsoft.Json;

namespace MetroDemo
{
    public partial class PanoramaDemo
    {
        public PanoramaDemo()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://ws.audioscrobbler.com/2.0/?method=chart.gethypedartists&api_key=b25b959554ed76058ac220b7b2e0a026&format=json"));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<Wrapper>(e.Result);
                lbArtists.Dispatcher.BeginInvoke(new Action(() =>
                                                                {
                                                                    lbArtists.ItemsSource = x.Artists.artist;
                                                                }));
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("foo");
        }
    }
}
