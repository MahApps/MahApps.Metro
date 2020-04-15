using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro;
using MetroDemo.Models;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.Core;
using MetroDemo.ExampleViews;
using NHotkey;
using NHotkey.Wpf;
using System.Collections.ObjectModel;
using ControlzEx.Theming;

namespace MetroDemo
{
    public class AccentColorMenuData
    {
        public string Name { get; set; }

        public Brush BorderColorBrush { get; set; }

        public Brush ColorBrush { get; set; }

        public AccentColorMenuData()
        {
            this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        }

        public ICommand ChangeAccentCommand { get; }

        protected virtual void DoChangeTheme(object sender)
        {
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, this.Name);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, this.Name);
        }
    }

    public class MainWindowViewModel : ViewModelBase, IDataErrorInfo, IDisposable
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        int? _integerGreater10Property;
        private bool _animateOnPositionChange = true;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.Title = "Flyout Binding Test";
            this._dialogCoordinator = dialogCoordinator;
            SampleData.Seed();

            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorMenuData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.Current.Themes
                                         .GroupBy(x => x.BaseColorScheme)
                                         .Select(x => x.First())
                                         .Select(a => new AppThemeMenuData() { Name = a.BaseColorScheme, BorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, ColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();

            this.Albums = SampleData.Albums;
            this.Artists = SampleData.Artists;

            this.FlipViewImages = new Uri[]
                                  {
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Home.jpg", UriKind.RelativeOrAbsolute),
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Privat.jpg", UriKind.RelativeOrAbsolute),
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Settings.jpg", UriKind.RelativeOrAbsolute)
                                  };

            this.BrushResources = this.FindBrushResources();

            this.CultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.DisplayName).ToList();

            try
            {
                HotkeyManager.Current.AddOrReplace("demo", this.HotKey.Key, this.HotKey.ModifierKeys, (sender, e) => this.OnHotKey(sender, e));
            }
            catch (HotkeyAlreadyRegisteredException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Uups, the hotkey {0} is already registered!", exception.Name);
            }

            this.EndOfScrollReachedCmdWithParameter = new SimpleCommand(o => true, async x => { await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("End of scroll reached!", $"Parameter: {x}"); });

            this.CloseCmd = new SimpleCommand(o => this.CanCloseFlyout, x => ((Flyout)x).IsOpen = false);

            this.TextBoxButtonCmd = new SimpleCommand(
                o => true,
                async x =>
                    {
                        if (x is string s)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Wow, you typed Return and got", s).ConfigureAwait(false);
                        }
                        else if (x is RichTextBox richTextBox)
                        {
                            var text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("RichTextBox Button was clicked!", text).ConfigureAwait(false);
                        }
                        else if (x is TextBox textBox)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("TextBox Button was clicked!", textBox.Text).ConfigureAwait(false);
                        }
                        else if (x is PasswordBox passwordBox)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("PasswordBox Button was clicked!", passwordBox.Password).ConfigureAwait(false);
                        }
                        else if (x is DatePicker datePicker)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("DatePicker Button was clicked!", datePicker.Text).ConfigureAwait(false);
                        }
                    }
            );

            this.TextBoxButtonCmdWithParameter = new SimpleCommand(
                o => true,
                async x => { await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("TextBox Button with parameter was clicked!", $"Parameter: {x}"); }
            );

            this.SingleCloseTabCommand = new SimpleCommand(
                o => true,
                async x => { await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Closing tab!", $"You are now closing the '{x}' tab"); }
            );

            this.NeverCloseTabCommand = new SimpleCommand(o => false);

            this.ShowInputDialogCommand = new SimpleCommand(
                o => true,
                async x => { await this._dialogCoordinator.ShowInputAsync(this, "From a VM", "This dialog was shown from a VM, without knowledge of Window").ContinueWith(t => Console.WriteLine(t.Result)); }
            );

            this.ShowLoginDialogCommand = new SimpleCommand(
                o => true,
                async x => { await this._dialogCoordinator.ShowLoginAsync(this, "Login from a VM", "This login dialog was shown from a VM, so you can be all MVVM.").ContinueWith(t => Console.WriteLine(t.Result)); }
            );

            this.ShowMessageDialogCommand = new SimpleCommand(
                o => true,
                x => PerformDialogCoordinatorAction(this.ShowMessage((string)x), (string)x == "DISPATCHER_THREAD")
            );

            this.ShowProgressDialogCommand = new SimpleCommand(o => true, x => this.RunProgressFromVm());

            this.ShowCustomDialogCommand = new SimpleCommand(o => true, x => this.RunCustomFromVm());

            this.ToggleIconScalingCommand = new SimpleCommand(o => true, this.ToggleIconScaling);

            this.OpenFirstFlyoutCommand = new SimpleCommand(o => true, o => (o as Flyout).IsOpen = !(o as Flyout).IsOpen);

            this.ArtistsDropDownCommand = new SimpleCommand(o => false);

            this.GenreDropDownMenuItemCommand = new SimpleCommand(
                o => true,
                async x => { await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("DropDownButton Menu", $"You are clicked the '{x}' menu item."); }
                );

            this.GenreSplitButtonItemCommand = new SimpleCommand(
                o => true,
                async x => { await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Split Button", $"The selected item is '{x}'."); }
                );

            this.ShowHamburgerAboutCommand = ShowAboutCommand.Command;
        }

        public ICommand ArtistsDropDownCommand { get; }

        public ICommand GenreDropDownMenuItemCommand { get; }

        public ICommand GenreSplitButtonItemCommand { get; }

        public ICommand ShowHamburgerAboutCommand { get; }

        public ICommand OpenFirstFlyoutCommand { get; }

        public ICommand ChangeSyncModeCommand { get; } = new SimpleCommand(execute: x =>
            {
                ThemeManager.Current.ThemeSyncMode = (ThemeSyncMode)x;
                ThemeManager.Current.SyncTheme();
            });

        public ICommand SyncThemeNowCommand { get; } = new SimpleCommand(execute: x => ThemeManager.Current.SyncTheme());

        public void Dispose()
        {
            HotkeyManager.Current.Remove("demo");
        }

        public string Title { get; set; }

        public int SelectedIndex { get; set; }

        public List<Album> Albums { get; set; }

        public List<Artist> Artists { get; set; }

        private ObservableCollection<Artist> _selectedArtists = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> SelectedArtists
        {
            get => _selectedArtists;
            set => Set(ref _selectedArtists, value);
        }

        public List<AccentColorMenuData> AccentColors { get; set; }

        public List<AppThemeMenuData> AppThemes { get; set; }

        public List<CultureInfo> CultureInfos { get; set; }

        private CultureInfo currentCulture = CultureInfo.CurrentCulture;

        public CultureInfo CurrentCulture
        {
            get => this.currentCulture;
            set => this.Set(ref this.currentCulture, value);
        }

        public ICommand EndOfScrollReachedCmdWithParameter { get; }

        public int? IntegerGreater10Property
        {
            get => this._integerGreater10Property;
            set => this.Set(ref this._integerGreater10Property, value);
        }

        private DateTime? _datePickerDate;

        [Display(Prompt = "Auto resolved Watermark")]
        public DateTime? DatePickerDate
        {
            get => this._datePickerDate;
            set => this.Set(ref this._datePickerDate, value);
        }

        private bool _quitConfirmationEnabled;

        public bool QuitConfirmationEnabled
        {
            get => this._quitConfirmationEnabled;
            set => this.Set(ref this._quitConfirmationEnabled, value);
        }

        private bool showMyTitleBar = true;

        public bool ShowMyTitleBar
        {
            get => this.showMyTitleBar;
            set => this.Set(ref this.showMyTitleBar, value);
        }

        private bool canCloseFlyout = true;

        public bool CanCloseFlyout
        {
            get => this.canCloseFlyout;
            set => this.Set(ref this.canCloseFlyout, value);
        }

        public ICommand CloseCmd { get; }

        private bool canShowHamburgerAboutCommand = true;

        public bool CanShowHamburgerAboutCommand
        {
            get => this.canShowHamburgerAboutCommand;
            set => this.Set(ref this.canShowHamburgerAboutCommand, value);
        }

        private bool isHamburgerMenuPaneOpen;

        public bool IsHamburgerMenuPaneOpen
        {
            get => this.isHamburgerMenuPaneOpen;
            set => this.Set(ref this.isHamburgerMenuPaneOpen, value);
        }

        public ICommand TextBoxButtonCmd { get; }

        public ICommand TextBoxButtonCmdWithParameter { get; }

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(IntegerGreater10Property) && this.IntegerGreater10Property < 10)
                {
                    return "Number is not greater than 10!";
                }

                if (columnName == nameof(DatePickerDate) && this.DatePickerDate == null)
                {
                    return "No date given!";
                }

                if (columnName == nameof(HotKey) && this.HotKey != null && this.HotKey.Key == Key.D && this.HotKey.ModifierKeys == ModifierKeys.Shift)
                {
                    return "SHIFT-D is not allowed";
                }

                if (columnName == nameof(TimePickerDate) && this.TimePickerDate == null)
                {
                    return "No time given!";
                }

                if (columnName == nameof(IsToggleSwitchVisible) && !IsToggleSwitchVisible)
                {
                    return "There is something hidden... \nActivate me to show it up.";
                }

                return null;
            }
        }

        [Description("Test-Property")]
        public string Error => string.Empty;

        private DateTime? _timePickerDate;

        [Display(Prompt = "Time needed...")]
        public DateTime? TimePickerDate
        {
            get => this._timePickerDate;
            set => this.Set(ref this._timePickerDate, value);
        }

        public ICommand SingleCloseTabCommand { get; }

        public ICommand NeverCloseTabCommand { get; }

        public ICommand ShowInputDialogCommand { get; }

        public ICommand ShowLoginDialogCommand { get; }

        public ICommand ShowMessageDialogCommand { get; }

        private Action ShowMessage(string startingThread)
        {
            return () =>
                {
                    var message = $"MVVM based messages!\n\nThis dialog was created by {startingThread} Thread with ID=\"{Thread.CurrentThread.ManagedThreadId}\"\n" +
                                  $"The current DISPATCHER_THREAD Thread has the ID=\"{Application.Current.Dispatcher.Thread.ManagedThreadId}\"";
                    this._dialogCoordinator.ShowMessageAsync(this, $"Message from VM created by {startingThread}", message).ContinueWith(t => Console.WriteLine(t.Result));
                };
        }

        public ICommand ShowProgressDialogCommand { get; }

        private async void RunProgressFromVm()
        {
            var controller = await this._dialogCoordinator.ShowProgressAsync(this, "Progress from VM", "Progressing all the things, wait 3 seconds");
            controller.SetIndeterminate();

            await Task.Delay(3000);

            await controller.CloseAsync();
        }

        private static void PerformDialogCoordinatorAction(Action action, bool runInMainThread)
        {
            if (!runInMainThread)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }

        public ICommand ShowCustomDialogCommand { get; }

        private async void RunCustomFromVm()
        {
            var customDialog = new CustomDialog() { Title = "Custom Dialog" };

            var dataContext = new CustomDialogExampleContent(instance =>
                {
                    this._dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                    System.Diagnostics.Debug.WriteLine(instance.FirstName);
                });
            customDialog.Content = new CustomDialogExample { DataContext = dataContext };

            await this._dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public IEnumerable<string> BrushResources { get; private set; }

        public bool AnimateOnPositionChange
        {
            get => this._animateOnPositionChange;
            set => this.Set(ref this._animateOnPositionChange, value);
        }

        private IEnumerable<string> FindBrushResources()
        {
            if (Application.Current.MainWindow != null)
            {
                var theme = ThemeManager.Current.DetectTheme(Application.Current.MainWindow);

                var resources = theme.LibraryThemes.First(x => x.Origin == "MahApps.Metro").Resources.MergedDictionaries.First();

                var brushResources = resources.Keys
                                     .Cast<object>()
                                     .Where(key => resources[key] is SolidColorBrush)
                                     .Select(key => key.ToString())
                                     .OrderBy(s => s)
                                     .ToList();

                return brushResources;
            }

            return Enumerable.Empty<string>();
        }

        public Uri[] FlipViewImages { get; set; }

        public class RandomDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate TemplateOne { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return this.TemplateOne;
            }
        }

        private HotKey _hotKey = new HotKey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift);

        public HotKey HotKey
        {
            get => this._hotKey;
            set
            {
                if (this.Set(ref this._hotKey, value))
                {
                    if (value != null && value.Key != Key.None)
                    {
                        HotkeyManager.Current.AddOrReplace("demo", value.Key, value.ModifierKeys, async (sender, e) => await this.OnHotKey(sender, e));
                    }
                    else
                    {
                        HotkeyManager.Current.Remove("demo");
                    }
                }
            }
        }

        private async Task OnHotKey(object sender, HotkeyEventArgs e)
        {
            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(
                "Hotkey pressed",
                "You pressed the hotkey '" + this.HotKey + "' registered with the name '" + e.Name + "'");
        }

        public ICommand ToggleIconScalingCommand { get; }

        private void ToggleIconScaling(object obj)
        {
            var multiFrameImageMode = (MultiFrameImageMode)obj;
            ((MetroWindow)Application.Current.MainWindow).IconScalingMode = multiFrameImageMode;
            this.OnPropertyChanged(nameof(IsScaleDownLargerFrame));
            this.OnPropertyChanged(nameof(IsNoScaleSmallerFrame));
        }

        public bool IsScaleDownLargerFrame => ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.ScaleDownLargerFrame;

        public bool IsNoScaleSmallerFrame => ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.NoScaleSmallerFrame;

        public bool IsToggleSwitchVisible { get; set; }
    }
}