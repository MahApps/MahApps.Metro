using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MetroDemo.Models;
using System.Windows.Input;

namespace MetroDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        readonly PanoramaGroup _albums;
        readonly PanoramaGroup _artists;
        int? _integerGreater10Property;

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

            Busy = false;
        }

        public ObservableCollection<PanoramaGroup> Groups { get; set; }
        public bool Busy { get; set; }
        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }

        public int? IntegerGreater10Property
        {
            get { return this._integerGreater10Property; }
            set
            {
                if (Equals(value, _integerGreater10Property))
                {
                    return;
                }

                _integerGreater10Property = value;
                RaisePropertyChanged("IntegerGreater10Property");
            }
        }

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

        public string this[string columnName]
        {
            get
            {
                if (columnName == "IntegerGreater10Property" && this.IntegerGreater10Property < 10)
                {
                    return "Number is not greater than 10!";
                }

                return null;
            }
        }

        public string Error { get { return string.Empty; } }

        public ICommand SingleCloseTabCommand { get { return new ExampleSingleTabCloseCommand(); } }

        public class ExampleCloseCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                System.Windows.MessageBox.Show("You tried to close the '" + ((MetroTabItem)parameter).Header + "' tab!");
            }
        }
        public class ExampleSingleTabCloseCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                System.Windows.MessageBox.Show("You tried to close the '" + ((MetroTabItem)parameter).Header + "' tab!");
            }
        }
    }
}