using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using MetroDemo.Models;
using System.Windows.Input;

namespace MetroDemo
{
    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        int? _integerGreater10Property;
        private bool _animateOnPositionChange = true;

        public MainWindowViewModel()
        {
            SampleData.Seed();

            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();
            

            Albums = SampleData.Albums;
            Artists = SampleData.Artists;

            FlipViewTemplateSelector = new RandomDataTemplateSelector();

            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(Image));
            spFactory.SetBinding(Image.SourceProperty, new System.Windows.Data.Binding("."));
            spFactory.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            spFactory.SetValue(Image.StretchProperty, Stretch.Fill);
            FlipViewTemplateSelector.TemplateOne = new DataTemplate()
            {
                VisualTree = spFactory
            };
            FlipViewImages = new string[] { "http://trinities.org/blog/wp-content/uploads/red-ball.jpg", "http://savingwithsisters.files.wordpress.com/2012/05/ball.gif" };

            RaisePropertyChanged("FlipViewTemplateSelector");


            BrushResources = FindBrushResources();
        }

        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }

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

        bool _magicToggleButtonIsChecked = true;
        public bool MagicToggleButtonIsChecked
        {
            get { return this._magicToggleButtonIsChecked; }
            set
            {
                if (Equals(value, _magicToggleButtonIsChecked))
                {
                    return;
                }

                _magicToggleButtonIsChecked = value;
                RaisePropertyChanged("MagicToggleButtonIsChecked");
            }
        }

        private bool _quitConfirmationEnabled;
        public bool QuitConfirmationEnabled
        {
            get { return _quitConfirmationEnabled; }
            set
            {
                if (value.Equals(_quitConfirmationEnabled)) return;
                _quitConfirmationEnabled = value;
                RaisePropertyChanged("QuitConfirmationEnabled");
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
            public bool CanExecute(object parameter)
            {
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

        private ICommand textBoxButtonCmdWithParameter;

        public ICommand TextBoxButtonCmdWithParameter
        {
            get
            {
                return this.textBoxButtonCmdWithParameter ?? (this.textBoxButtonCmdWithParameter = new TextBoxButtonCommandWithIntParameter());
            }
        }

        public class TextBoxButtonCommandWithIntParameter : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (parameter is String)
                {
                    MessageBox.Show("TextBox Button was clicked with parameter!" + Environment.NewLine + "Text: " + parameter);
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

        public IEnumerable<string> BrushResources { get; private set; }

        public bool AnimateOnPositionChange
        {
            get
            {
                return _animateOnPositionChange;
            }
            set
            {
                if (Equals(_animateOnPositionChange, value)) return;
                _animateOnPositionChange = value;
                RaisePropertyChanged("AnimateOnPositionChange");
            }
        }

        private IEnumerable<string> FindBrushResources()
        {
            var rd = new ResourceDictionary
                {
                    Source = new Uri(@"/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute)
                };

            var resources = rd.Keys.Cast<object>()
                    .Where(key => rd[key] is Brush)
                    .Select(key => key.ToString())
                    .OrderBy(s => s)
                    .ToList();

            return resources;
        }

        public RandomDataTemplateSelector FlipViewTemplateSelector
        {
            get;
            set;
        }

        public string[] FlipViewImages
        {
            get;
            set;
        }


        public class RandomDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate TemplateOne { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return TemplateOne;
            }
        }
    }
}