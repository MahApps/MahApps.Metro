namespace MetroDemo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows.Threading;

    using MetroDemo.Models;

    using Newtonsoft.Json;

    public class MainWindowViewModel
    {
        #region Constants and Fields

        private readonly Dispatcher _dispatcher;

        #endregion

        #region Constructors and Destructors

        public MainWindowViewModel(Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
            this.Artists = new ObservableCollection<Artist>();

            var wc = new WebClient();
            wc.DownloadStringCompleted += this.WcDownloadStringCompleted;
            wc.DownloadStringAsync(
                new Uri(
                    "http://ws.audioscrobbler.com/2.0/?method=chart.gethypedartists&api_key=b25b959554ed76058ac220b7b2e0a026&format=json"));
        }

        #endregion

        #region Public Properties

        public ObservableCollection<Artist> Artists { get; private set; }

        #endregion

        #region Methods

        private void WcDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<Wrapper>(e.Result);
                this._dispatcher.BeginInvoke(
                    new Action(
                        () =>
                            {
                                foreach (var artist in x.Artists.artist)
                                {
                                    this.Artists.Add(artist);
                                }
                            }));
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}