using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using MetroDemo.Models;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.ExampleViews;

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
        private readonly IDialogCoordinator _dialogCoordinator;
        int? _integerGreater10Property;
        private bool _animateOnPositionChange = true;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
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

            CultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).ToList();
        }

        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }
        public List<CultureInfo> CultureInfos { get; set; }

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
                return this.textBoxButtonCmd ?? (this.textBoxButtonCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        if (x is TextBox)
                        {
                            await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("TextBox Button was clicked!",
                                                                                                    string.Format("Text: {0}", ((TextBox) x).Text));
                        }
                        else if (x is PasswordBox)
                        {
                            await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("PasswordBox Button was clicked!",
                                                                                                    string.Format("Password: {0}", ((PasswordBox) x).Password));
                        }
                    }
                });
            }
        }

        private ICommand textBoxButtonCmdWithParameter;

        public ICommand TextBoxButtonCmdWithParameter
        {
            get
            {
                return this.textBoxButtonCmdWithParameter ?? (this.textBoxButtonCmdWithParameter = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        if (x is String)
                        {
                            await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("TextBox Button with parameter was clicked!",
                                                                                                  string.Format("Parameter: {0}", x));
                        }
                    }
                });
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

        private ICommand singleCloseTabCommand;

        public ICommand SingleCloseTabCommand
        {
            get
            {
                return this.singleCloseTabCommand ?? (this.singleCloseTabCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("Closing tab!", string.Format("You are now closing the '{0}' tab", x));
                    }
                });
            }
        }

        private ICommand neverCloseTabCommand;

        public ICommand NeverCloseTabCommand
        {
            get { return this.neverCloseTabCommand ?? (this.neverCloseTabCommand = new SimpleCommand { CanExecuteDelegate = x => false }); }
        }


        private ICommand showInputDialogCommand;

        public ICommand ShowInputDialogCommand
        {
            get
            {
                return this.showInputDialogCommand ?? (this.showInputDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x =>
                    {
                        _dialogCoordinator.ShowInputAsync(this, "From a VM", "This dialog was shown from a VM, without knowledge of Window").ContinueWith(t => Console.WriteLine(t.Result));
                    }
                });
            }
        }

        private ICommand showLoginDialogCommand;

        public ICommand ShowLoginDialogCommand
        {
            get
            {
                return this.showLoginDialogCommand ?? (this.showLoginDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x =>
                    {
                        _dialogCoordinator.ShowLoginAsync(this, "Login from a VM", "This login dialog was shown from a VM, so you can be all MVVM.").ContinueWith(t => Console.WriteLine(t.Result));
                    }
                });
            }
        }

        private ICommand showMessageDialogCommand;

        public ICommand ShowMessageDialogCommand
        {
            get
            {
                return this.showMessageDialogCommand ?? (this.showMessageDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x =>
                    {
                        _dialogCoordinator.ShowMessageAsync(this, "Message from VM", "MVVM based messages!").ContinueWith(t => Console.WriteLine(t.Result));
                    }
                });
            }
        }

        private ICommand showProgressDialogCommand;

        public ICommand ShowProgressDialogCommand
        {
            get
            {
                return this.showProgressDialogCommand ?? (this.showProgressDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunProgressFromVm()
                });
            }
        }

        private async void RunProgressFromVm()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Progress from VM", "Progressing all the things, wait 3 seconds");
            controller.SetIndeterminate();

            await TaskEx.Delay(3000);

            await controller.CloseAsync();
        }
        

        private ICommand showCustomDialogCommand;

        public ICommand ShowCustomDialogCommand
        {
            get
            {
                return this.showCustomDialogCommand ?? (this.showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()                    
                });
            }
        }

        private async void RunCustomFromVm()
        {
            var customDialog = new CustomDialog() { Title = "Custom Dialog" };

            var customDialogExampleContent = new CustomDialogExampleContent(instance =>
            {
                _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                System.Diagnostics.Debug.WriteLine(instance.FirstName);
            });
            customDialog.Content = new CustomDialogExample { DataContext = customDialogExampleContent};            

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);            
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