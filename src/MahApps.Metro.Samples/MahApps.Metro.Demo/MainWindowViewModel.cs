// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
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
using MetroDemo.Models;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.Core;
using MetroDemo.ExampleViews;
using NHotkey;
using NHotkey.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Data;
using ControlzEx.Theming;

namespace MetroDemo
{
    public class AccentColorMenuData
    {
        public string? Name { get; set; }

        public Brush? BorderColorBrush { get; set; }

        public Brush? ColorBrush { get; set; }

        public AccentColorMenuData()
        {
            this.ChangeAccentCommand = new SimpleCommand<string?>(o => true, this.DoChangeTheme);
        }

        public ICommand ChangeAccentCommand { get; }

        protected virtual void DoChangeTheme(string? name)
        {
            if (name is not null)
            {
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, name);
            }
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(string? name)
        {
            if (name is not null)
            {
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, name);
            }
        }
    }

    public class MainWindowViewModel : ViewModelBase, IDataErrorInfo, IDisposable
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        int? _integerGreater10Property = 2;
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
                                         .Select(a => new AppThemeMenuData { Name = a.BaseColorScheme, BorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, ColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();

            this.Albums = new ObservableCollection<Album>(SampleData.Albums!);
            var cvs = CollectionViewSource.GetDefaultView(this.Albums);
            cvs.GroupDescriptions.Add(new PropertyGroupDescription("Artist"));

            this.Artists = SampleData.Artists;

            this.FlipViewImages = new Uri[]
                                  {
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Home.jpg", UriKind.RelativeOrAbsolute),
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Privat.jpg", UriKind.RelativeOrAbsolute),
                                      new Uri("pack://application:,,,/MahApps.Metro.Demo;component/Assets/Photos/Settings.jpg", UriKind.RelativeOrAbsolute)
                                  };

            this.ThemeResources = new ObservableCollection<ThemeResource>();
            var view = CollectionViewSource.GetDefaultView(this.ThemeResources);
            view.SortDescriptions.Add(new SortDescription(nameof(ThemeResource.Key), ListSortDirection.Ascending));
            this.UpdateThemeResources();

            this.CultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.DisplayName).ToList();

            try
            {
                if (this.HotKey is not null)
                {
                    HotkeyManager.Current.AddOrReplace("demo", this.HotKey.Key, this.HotKey.ModifierKeys, async (sender, e) => await this.OnHotKey(sender, e));
                }
            }
            catch (HotkeyAlreadyRegisteredException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Uups, the hotkey {0} is already registered!", exception.Name);
            }

            this.EndOfScrollReachedCmdWithParameter = new SimpleCommand<object>(o => true, async x => { await this._dialogCoordinator.ShowMessageAsync(this, "End of scroll reached!", $"Parameter: {x}"); });

            this.CloseCmd = new SimpleCommand<Flyout>(f => f is not null && this.CanCloseFlyout, f => f!.IsOpen = false);

            this.TextBoxButtonCmd = new SimpleCommand<object>(
                o => true,
                async x =>
                    {
                        if (x is string s)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this, "Wow, you typed Return and got", s).ConfigureAwait(false);
                        }
                        else if (x is RichTextBox richTextBox)
                        {
                            var text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
                            await this._dialogCoordinator.ShowMessageAsync(this, "RichTextBox Button was clicked!", text).ConfigureAwait(false);
                        }
                        else if (x is TextBox textBox)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this, "TextBox Button was clicked!", textBox.Text).ConfigureAwait(false);
                        }
                        else if (x is PasswordBox passwordBox)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this, "PasswordBox Button was clicked!", passwordBox.Password).ConfigureAwait(false);
                        }
                        else if (x is DatePicker datePicker)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this, "DatePicker Button was clicked!", datePicker.Text).ConfigureAwait(false);
                        }
                    }
            );

            this.TextBoxButtonCmdWithParameter = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowMessageAsync(this, "TextBox Button with parameter was clicked!", $"Parameter: {x}"); }
            );

            this.SingleCloseTabCommand = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowMessageAsync(this, "Closing tab!", $"You are now closing the '{x}' tab"); }
            );

            this.NeverCloseTabCommand = new SimpleCommand<object>(o => false);

            this.ShowInputDialogCommand = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowInputAsync(this, "From a VM", "This dialog was shown from a VM, without knowledge of Window").ContinueWith(t => Console.WriteLine(t.Result)); }
            );

            this.ShowLoginDialogCommand = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowLoginAsync(this, "Login from a VM", "This login dialog was shown from a VM, so you can be all MVVM.").ContinueWith(t => Console.WriteLine(t.Result)); }
            );

            this.ShowMessageDialogCommand = new SimpleCommand<string>(
                x => !string.IsNullOrEmpty(x),
                x => PerformDialogCoordinatorAction(this.ShowMessage(x!), x == "DISPATCHER_THREAD")
            );

            this.ShowProgressDialogCommand = new SimpleCommand<object>(o => true, x => this.RunProgressFromVm());

            this.ShowCustomDialogCommand = new SimpleCommand<object>(o => true, x => this.RunCustomFromVm());

            this.ToggleIconScalingCommand = new SimpleCommand<MultiFrameImageMode?>(m => m is not null, this.ToggleIconScaling);

            this.OpenFirstFlyoutCommand = new SimpleCommand<Flyout>(f => f is not null, f => f!.SetCurrentValue(Flyout.IsOpenProperty, !f.IsOpen));

            this.ArtistsDropDownCommand = new SimpleCommand<object>(o => false);

            this.GenreDropDownMenuItemCommand = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowMessageAsync(this, "DropDownButton Menu", $"You are clicked the '{x}' menu item."); }
            );

            this.GenreSplitButtonItemCommand = new SimpleCommand<object>(
                o => true,
                async x => { await this._dialogCoordinator.ShowMessageAsync(this, "Split Button", $"The selected item is '{x}'."); }
            );

            this.ShowHamburgerAboutCommand = ShowAboutCommand.Command;

            this.ToggleSwitchCommand = new SimpleCommand<ToggleSwitch?>(x => x is not null && this.CanUseToggleSwitch,
                                                                        async x => { await this._dialogCoordinator.ShowMessageAsync(this, "ToggleSwitch", $"The ToggleSwitch is now {x!.IsOn}."); });

            this.ToggleSwitchOnCommand = new SimpleCommand<MainWindowViewModel?>(x => x is not null && x.CanUseToggleSwitch,
                                                                                 async x => { await this._dialogCoordinator.ShowMessageAsync(this, "ToggleSwitch", "The ToggleSwitch is now On."); });

            this.ToggleSwitchOffCommand = new SimpleCommand<MainWindowViewModel?>(x => x is not null && x.CanUseToggleSwitch,
                                                                                  async x => { await this._dialogCoordinator.ShowMessageAsync(this, "ToggleSwitch", "The ToggleSwitch is now Off."); });
        }

        public ICommand ArtistsDropDownCommand { get; }

        public ICommand GenreDropDownMenuItemCommand { get; }

        public ICommand GenreSplitButtonItemCommand { get; }

        public ICommand ShowHamburgerAboutCommand { get; }

        public ICommand OpenFirstFlyoutCommand { get; }

        public ICommand ChangeSyncModeCommand { get; } = new SimpleCommand<ThemeSyncMode?>(
            x => x is not null,
            x =>
                {
                    ThemeManager.Current.ThemeSyncMode = x!.Value;
                    ThemeManager.Current.SyncTheme();
                });

        public ICommand SyncThemeNowCommand { get; } = new SimpleCommand<object>(execute: x => ThemeManager.Current.SyncTheme());

        public ICommand ToggleSwitchCommand { get; }

        private bool canUseToggleSwitch = true;

        public bool CanUseToggleSwitch
        {
            get => this.canUseToggleSwitch;
            set => this.Set(ref this.canUseToggleSwitch, value);
        }

        public ICommand ToggleSwitchOnCommand { get; }

        public ICommand ToggleSwitchOffCommand { get; }

        public void Dispose()
        {
            HotkeyManager.Current.Remove("demo");
        }

        public string Title { get; set; }

        public int SelectedIndex { get; set; }

        public ICollection<Album> Albums { get; set; }

        public List<Artist>? Artists { get; set; }

        private ObservableCollection<Artist>? _selectedArtists = new ObservableCollection<Artist>();

        public ObservableCollection<Artist>? SelectedArtists
        {
            get => this._selectedArtists;
            set => this.Set(ref this._selectedArtists, value);
        }

        public List<AccentColorMenuData> AccentColors { get; set; }

        public List<AppThemeMenuData> AppThemes { get; set; }

        public List<CultureInfo> CultureInfos { get; set; }

        private CultureInfo? currentCulture = CultureInfo.CurrentCulture;

        public CultureInfo? CurrentCulture
        {
            get => this.currentCulture;
            set => this.Set(ref this.currentCulture, value);
        }

        private double numericUpDownValue = default;

        public double NumericUpDownValue
        {
            get => this.numericUpDownValue;
            set => this.Set(ref this.numericUpDownValue, value);
        }

        private double? nullableNumericUpDownValue = null;

        public double? NullableNumericUpDownValue
        {
            get => this.nullableNumericUpDownValue;
            set => this.Set(ref this.nullableNumericUpDownValue, value);
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

        public string? this[string columnName]
        {
            get
            {
                if (columnName == nameof(this.IntegerGreater10Property) && this.IntegerGreater10Property < 10)
                {
                    return "Number is not greater than 10!";
                }

                if (columnName == nameof(this.DatePickerDate) && this.DatePickerDate == null)
                {
                    return "No date given!";
                }

                if (columnName == nameof(this.HotKey) && this.HotKey != null && this.HotKey.Key == Key.D && this.HotKey.ModifierKeys == ModifierKeys.Shift)
                {
                    return "SHIFT-D is not allowed";
                }

                if (columnName == nameof(this.TimePickerDate) && this.TimePickerDate == null)
                {
                    return "No time given!";
                }

                if (columnName == nameof(this.IsToggleSwitchVisible) && !this.IsToggleSwitchVisible)
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
            var customDialog = new CustomDialog { Title = "Custom Dialog" };

            var dataContext = new CustomDialogExampleContent(instance =>
                {
                    this._dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                    System.Diagnostics.Debug.WriteLine(instance.FirstName);
                });
            customDialog.Content = new CustomDialogExample { DataContext = dataContext };

            await this._dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ObservableCollection<ThemeResource> ThemeResources { get; }

        public bool AnimateOnPositionChange
        {
            get => this._animateOnPositionChange;
            set => this.Set(ref this._animateOnPositionChange, value);
        }

        public void UpdateThemeResources()
        {
            this.ThemeResources.Clear();

            if (Application.Current.MainWindow != null)
            {
                var theme = ThemeManager.Current.DetectTheme(Application.Current.MainWindow);
                if (theme is not null)
                {
                    var libraryTheme = theme.LibraryThemes.FirstOrDefault(x => x.Origin == "MahApps.Metro");
                    var resourceDictionary = libraryTheme?.Resources.MergedDictionaries.FirstOrDefault();

                    if (resourceDictionary != null)
                    {
                        foreach (var dictionaryEntry in resourceDictionary.OfType<DictionaryEntry>())
                        {
                            this.ThemeResources.Add(new ThemeResource(theme, libraryTheme!, resourceDictionary, dictionaryEntry));
                        }
                    }
                }
            }
        }

        public Uri[] FlipViewImages { get; set; }

        public class RandomDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate? TemplateOne { get; set; }

            public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            {
                return this.TemplateOne;
            }
        }

        private HotKey? _hotKey = new HotKey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift);

        public HotKey? HotKey
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

        private async Task OnHotKey(object? sender, HotkeyEventArgs e)
        {
            await this._dialogCoordinator.ShowMessageAsync(this,
                                                           "Hotkey pressed",
                                                           "You pressed the hotkey '" + this.HotKey + "' registered with the name '" + e.Name + "'");
        }

        public ICommand ToggleIconScalingCommand { get; }

        private void ToggleIconScaling(MultiFrameImageMode? multiFrameImageMode)
        {
            ((MetroWindow)Application.Current.MainWindow).IconScalingMode = multiFrameImageMode!.Value;
            this.OnPropertyChanged(nameof(this.IsScaleDownLargerFrame));
            this.OnPropertyChanged(nameof(this.IsNoScaleSmallerFrame));
        }

        public bool IsScaleDownLargerFrame => ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.ScaleDownLargerFrame;

        public bool IsNoScaleSmallerFrame => ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.NoScaleSmallerFrame;

        public bool IsToggleSwitchVisible { get; set; }
    }
}