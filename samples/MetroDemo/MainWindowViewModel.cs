using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MetroDemo.Models;
using System.Windows.Input;

namespace MetroDemo
{
    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => ChangeAccent(x) }); }
        }

        private void ChangeAccent(object sender)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            var accent = ThemeManager.DefaultAccents.First(x => x.Name == this.Name);
            ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        readonly PanoramaGroup _albums;
        readonly PanoramaGroup _artists;
        int? _integerGreater10Property;

        public MainWindowViewModel()
        {
            SampleData.Seed();

            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.DefaultAccents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();
            
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
        public List<AccentColorMenuData> AccentColors { get; set; }

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

        DateTime? _datePickerDate;
        public DateTime? DatePickerDate
        {
            get { return this._datePickerDate; }
            set
            {
                if (Equals(value, _datePickerDate))
                {
                    return;
                }

                _datePickerDate = value;
                RaisePropertyChanged("DatePickerDate");
            }
        }

        private ICommand textBoxButtonCmd;

        public ICommand TextBoxButtonCmd
        {
            get
            {
                return this.textBoxButtonCmd ?? (this.textBoxButtonCmd = new TextBoxButtonCommand());
            }
        }

        public class TextBoxButtonCommand : ICommand
        {
            public bool CanExecute(object parameter) {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (parameter is TextBox)
                {
                    MessageBox.Show("TextBox Button was clicked!" + Environment.NewLine + "Text: " + ((TextBox)parameter).Text);
                }
                else if (parameter is PasswordBox)
                {
                    MessageBox.Show("PasswordBox Button was clicked!" + Environment.NewLine + "Text: " + ((PasswordBox)parameter).Password);
                }
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

                if (columnName == "DatePickerDate" && this.DatePickerDate == null)
                {
                    return "No date given!";
                }

                return null;
            }
        }

        public string Error { get { return string.Empty; } }

        public ICommand SingleCloseTabCommand { get { return new ExampleSingleTabCloseCommand(); } }

        public class ExampleSingleTabCloseCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                System.Windows.MessageBox.Show("You are now closing the '" + parameter + "' tab!");
            }
        }

        public ICommand NeverCloseTabCommand { get { return new AlwaysInvalidCloseCommand(); } }

        public class AlwaysInvalidCloseCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return false;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {

            }
        }
    }
}