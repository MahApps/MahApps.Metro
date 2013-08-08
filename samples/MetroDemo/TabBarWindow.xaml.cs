using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for TabBarWindow.xaml
    /// </summary>
    public partial class TabBarWindow : MetroWindow
    {
        public TabBarWindow()
        {
            InitializeComponent();

            //Ideally, you would do this via MVVM but for the sake of simplicity, I'm faking MVVM.
            Albums = new ObservableCollection<Album>(SampleData.Albums.Take(30));
            Artists = new ObservableCollection<Artist>(SampleData.Artists.Take(30));

            artistTab.SetBinding(TabBarItem.DataContextProperty, new Binding() { Source = Artists });
            albumTab.SetBinding(TabBarItem.DataContextProperty, new Binding() { Source = Albums });
        }

        public ObservableCollection<Models.Album> Albums
        {
            get;
            set;
        }
        public ObservableCollection<Models.Artist> Artists
        {
            get;
            set;
        }
    }

    public class SampleTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
                return ItemTemplate;

            return DefaultTemplate;
        }
    }

    public class SampleDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ArtistTemplate { get; set; }
        public DataTemplate AlbumTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
                if (item is Models.Artist)
                    return ArtistTemplate;
                else if (item is Models.Album)
                    return AlbumTemplate;

            return null;
        }
    }
}
