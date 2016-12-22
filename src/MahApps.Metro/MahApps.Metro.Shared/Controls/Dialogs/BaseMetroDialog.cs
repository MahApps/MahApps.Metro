using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// The base class for dialogs.
    ///
    /// You probably don't want to use this class, if you want to add arbitrary content to your dialog,
    /// use the <see cref="CustomDialog"/> class.
    /// </summary>
    public abstract class BaseMetroDialog : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BaseMetroDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DialogTopProperty = DependencyProperty.Register("DialogTop", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty DialogBottomProperty = DependencyProperty.Register("DialogBottom", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty DialogTitleFontSizeProperty = DependencyProperty.Register("DialogTitleFontSize", typeof(double), typeof(BaseMetroDialog), new PropertyMetadata(26D));
        public static readonly DependencyProperty DialogMessageFontSizeProperty = DependencyProperty.Register("DialogMessageFontSize", typeof(double), typeof(BaseMetroDialog), new PropertyMetadata(15D));

        public MetroDialogSettings DialogSettings { get; private set; }

        /// <summary>
        /// Gets/sets the dialog's title.
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content on top of the dialog.
        /// </summary>
        public object DialogTop
        {
            get { return this.GetValue(DialogTopProperty); }
            set { this.SetValue(DialogTopProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content below the dialog.
        /// </summary>
        public object DialogBottom
        {
            get { return this.GetValue(DialogBottomProperty); }
            set { this.SetValue(DialogBottomProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the dialog title font.
        /// </summary>
        /// <value>
        /// The size of the dialog title font.
        /// </value>
        public double DialogTitleFontSize
        {
            get { return (double)this.GetValue(DialogTitleFontSizeProperty); }
            set { this.SetValue(DialogTitleFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the dialog message font.
        /// </summary>
        /// <value>
        /// The size of the dialog message font.
        /// </value>
        public double DialogMessageFontSize
        {
            get { return (double)this.GetValue(DialogMessageFontSizeProperty); }
            set { this.SetValue(DialogMessageFontSizeProperty, value); }
        }

        internal SizeChangedEventHandler SizeChangedHandler { get; set; }

        static BaseMetroDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseMetroDialog), new FrameworkPropertyMetadata(typeof(BaseMetroDialog)));
        }

        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        /// <param name="owningWindow">The window that is the parent of the dialog.</param>
        /// <param name="settings">The settings for the message dialog.</param>
        protected BaseMetroDialog(MetroWindow owningWindow, MetroDialogSettings settings)
        {
            this.Initialize(owningWindow, settings);
        }

        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        protected BaseMetroDialog()
            : this(null, new MetroDialogSettings())
        {
        }

        /// <summary>
        /// With this method it's possible to return your own settings in a custom dialog.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected virtual MetroDialogSettings ConfigureSettings(MetroDialogSettings settings)
        {
            return settings;
        }

        private void Initialize([CanBeNull] MetroWindow owningWindow, [CanBeNull] MetroDialogSettings settings)
        {
            this.OwningWindow = owningWindow;
            this.DialogSettings = this.ConfigureSettings(settings ?? (owningWindow?.MetroDialogOptions ?? new MetroDialogSettings()));

            if (this.DialogSettings != null && !this.DialogSettings.SuppressDefaultResources)
            {
                this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });
            }

            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Themes/Dialogs/BaseMetroDialog.xaml") });
            if (this.DialogSettings?.CustomResourceDictionary != null)
            {
                this.Resources.MergedDictionaries.Add(this.DialogSettings.CustomResourceDictionary);
            }

            this.DialogTitleFontSize = (this.TryFindResource("DialogTitleFontSize") as double?).GetValueOrDefault(this.DialogTitleFontSize);
            this.DialogMessageFontSize = (this.TryFindResource("DialogMessageFontSize") as double?).GetValueOrDefault(this.DialogMessageFontSize);

            this.HandleTheme();
            this.Loaded += (sender, args) => { this.OnLoaded(); };
            ThemeManager.IsThemeChanged += this.ThemeManagerIsThemeChanged;
            this.Unloaded += this.BaseMetroDialogUnloaded;
        }

        private void BaseMetroDialogUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.IsThemeChanged -= this.ThemeManagerIsThemeChanged;
            this.Unloaded -= this.BaseMetroDialogUnloaded;
        }

        private void ThemeManagerIsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            this.HandleTheme();
        }

        private void HandleTheme()
        {
            if (this.DialogSettings != null)
            {
                var windowTheme = DetectTheme(this);

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) || windowTheme == null)
                {
                    return;
                }

                var theme = windowTheme.Item1;
                var windowAccent = windowTheme.Item2;

                switch (this.DialogSettings.ColorScheme)
                {
                    case MetroDialogColorScheme.Theme:
                        ThemeManager.ChangeAppStyle(this.Resources, windowAccent, theme);
                        this.SetValue(BackgroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "WhiteColorBrush"));
                        this.SetValue(ForegroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "BlackBrush"));
                        break;
                    case MetroDialogColorScheme.Inverted:
                        var inverseTheme = ThemeManager.GetInverseAppTheme(theme);
                        if (inverseTheme == null)
                        {
                            throw new InvalidOperationException("The inverse dialog theme only works if the window theme abides the naming convention. " +
                                                                "See ThemeManager.GetInverseAppTheme for more infos");
                        }

                        ThemeManager.ChangeAppStyle(this.Resources, windowAccent, inverseTheme);
                        this.SetValue(BackgroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "BlackColorBrush"));
                        this.SetValue(ForegroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "WhiteColorBrush"));
                        break;
                    case MetroDialogColorScheme.Accented:
                        ThemeManager.ChangeAppStyle(this.Resources, windowAccent, theme);
                        this.SetValue(BackgroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "HighlightBrush"));
                        this.SetValue(ForegroundProperty, ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "IdealForegroundColorBrush"));
                        break;
                }
            }

            if (this.ParentDialogWindow != null)
            {
                this.ParentDialogWindow.SetValue(BackgroundProperty, this.Background);
                var glowBrush = ThemeManager.GetResourceFromAppStyle(this.OwningWindow ?? Application.Current.MainWindow, "AccentColorBrush");
                if (glowBrush != null)
                {
                    this.ParentDialogWindow.SetValue(MetroWindow.GlowBrushProperty, glowBrush);
                }
            }
        }

        /// <summary>
        /// This is called in the loaded event.
        /// </summary>
        protected virtual void OnLoaded()
        {
            // nothing here
        }

        private static Tuple<AppTheme, Accent> DetectTheme(DependencyObject dialog)
        {
            if (dialog == null)
            {
                return null;
            }

            // first look for owner
            var window = dialog.TryFindParent<MetroWindow>();
            var theme = window != null ? ThemeManager.DetectAppStyle(window) : null;
            if (theme?.Item2 != null)
            {
                return theme;
            }

            // second try, look for main window
            if (Application.Current != null)
            {
                var mainWindow = Application.Current.MainWindow as MetroWindow;
                theme = mainWindow != null ? ThemeManager.DetectAppStyle(mainWindow) : null;
                if (theme?.Item2 != null)
                {
                    return theme;
                }

                // oh no, now look at application resource
                theme = ThemeManager.DetectAppStyle(Application.Current);
                if (theme?.Item2 != null)
                {
                    return theme;
                }
            }
            return null;
        }

        /// <summary>
        /// Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public Task WaitForLoadAsync()
        {
            this.Dispatcher.VerifyAccess();

            if (this.IsLoaded) return new Task(() => { });

            if (!this.DialogSettings.AnimateShow)
            {
                this.Opacity = 1.0; //skip the animation
            }

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            RoutedEventHandler handler = null;
            handler = (sender, args) =>
                {
                    this.Loaded -= handler;

                    this.Focus();

                    tcs.TrySetResult(null);
                };

            this.Loaded += handler;

            return tcs.Task;
        }

        /// <summary>
        /// Requests an externally shown Dialog to close. Will throw an exception if the Dialog is inside of a MetroWindow.
        /// </summary>
        public Task RequestCloseAsync()
        {
            if (this.OnRequestClose())
            {
                //Technically, the Dialog is /always/ inside of a MetroWindow.
                //If the dialog is inside of a user-created MetroWindow, not one created by the external dialog APIs.
                if (this.ParentDialogWindow == null)
                {
                    //This is from a user-created MetroWindow
                    return DialogManager.HideMetroDialogAsync(this.OwningWindow, this);
                }

                //This is from a MetroWindow created by the external dialog APIs.
                return this._WaitForCloseAsync().ContinueWith(x => { this.ParentDialogWindow.Dispatcher.Invoke(new Action(() => { this.ParentDialogWindow.Close(); })); });
            }
            return Task.Factory.StartNew(() => { });
        }

        protected internal virtual void OnShown()
        {
        }

        protected internal virtual void OnClose()
        {
            // this is only set when a dialog is shown (externally) in it's OWN window.
            this.ParentDialogWindow?.Close();
        }

        /// <summary>
        /// A last chance virtual method for stopping an external dialog from closing.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool OnRequestClose()
        {
            return true; //allow the dialog to close.
        }

        /// <summary>
        /// Gets the window that owns the current Dialog IF AND ONLY IF the dialog is shown externally.
        /// </summary>
        protected internal Window ParentDialogWindow { get; internal set; }

        /// <summary>
        /// Gets the window that owns the current Dialog IF AND ONLY IF the dialog is shown inside of a window.
        /// </summary>
        protected internal MetroWindow OwningWindow { get; internal set; }

        /// <summary>
        /// Waits until this dialog gets unloaded.
        /// </summary>
        /// <returns></returns>
        public Task WaitUntilUnloadedAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            this.Unloaded += (s, e) => { tcs.TrySetResult(null); };

            return tcs.Task;
        }

        public Task _WaitForCloseAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (this.DialogSettings.AnimateHide)
            {
                Storyboard closingStoryboard = this.Resources["DialogCloseStoryboard"] as Storyboard;

                if (closingStoryboard == null)
                {
                    throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add BaseMetroDialog.xaml to your merged dictionaries?");
                }

                EventHandler handler = null;
                handler = (sender, args) =>
                    {
                        closingStoryboard.Completed -= handler;

                        tcs.TrySetResult(null);
                    };

                closingStoryboard = closingStoryboard.Clone();

                closingStoryboard.Completed += handler;

                closingStoryboard.Begin(this);
            }
            else
            {
                this.Opacity = 0.0;
                tcs.TrySetResult(null); //skip the animation
            }

            return tcs.Task;
        }
    }

    /// <summary>
    /// A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        public MetroDialogSettings()
        {
            this.AffirmativeButtonText = "OK";
            this.NegativeButtonText = "Cancel";

            this.ColorScheme = MetroDialogColorScheme.Theme;
            this.AnimateShow = this.AnimateHide = true;

            this.MaximumBodyHeight = Double.NaN;

            this.DefaultText = "";
            this.DefaultButtonFocus = MessageDialogResult.Negative;
            this.CancellationToken = CancellationToken.None;
            this.DialogTitleFontSize = Double.NaN;
            this.DialogMessageFontSize = Double.NaN;
        }

        /// <summary>
        /// Gets/sets the text used for the Affirmative button. For example: "OK" or "Yes".
        /// </summary>
        public string AffirmativeButtonText { get; set; }

        /// <summary>
        /// Gets/sets the text used for the Negative button. For example: "Cancel" or "No".
        /// </summary>
        public string NegativeButtonText { get; set; }

        public string FirstAuxiliaryButtonText { get; set; }

        public string SecondAuxiliaryButtonText { get; set; }

        /// <summary>
        /// Gets/sets whether the metro dialog should use the default black/white appearance (theme) or try to use the current accent.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; }

        /// <summary>
        /// Enable/disable dialog showing animation.
        /// "True" - play showing animation.
        /// "False" - skip showing animation.
        /// </summary>
        public bool AnimateShow { get; set; }

        /// <summary>
        /// Enable/disable dialog hiding animation
        /// "True" - play hiding animation.
        /// "False" - skip hiding animation.
        /// </summary>
        public bool AnimateHide { get; set; }

        /// <summary>
        /// Gets/sets the default text( just the inputdialog needed)
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        /// Gets/sets the maximum height. (Default is unlimited height, <a href="http://msdn.microsoft.com/de-de/library/system.double.nan">Double.NaN</a>)
        /// </summary>
        public double MaximumBodyHeight { get; set; }

        /// <summary>
        /// Gets or sets which button should be focused by default
        /// </summary>
        public MessageDialogResult DefaultButtonFocus { get; set; }

        /// <summary>
        /// Gets/sets the token to cancel the dialog.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets/sets a custom resource dictionary which can contains custom styles, brushes or something else.
        /// </summary>
        public ResourceDictionary CustomResourceDictionary { get; set; }

        /// <summary>
        /// If set, stops standard resource dictionaries being applied to the dialog.
        /// </summary>
        public bool SuppressDefaultResources { get; set; }

        /// <summary>
        /// Gets or sets the size of the dialog title font.
        /// </summary>
        /// <value>
        /// The size of the dialog title font.
        /// </value>
        public double DialogTitleFontSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the dialog message font.
        /// </summary>
        /// <value>
        /// The size of the dialog message font.
        /// </value>
        public double DialogMessageFontSize { get; set; }
    }

    /// <summary>
    /// An enum representing the different choices for a color scheme in a Metro Dialog.
    /// </summary>
    public enum MetroDialogColorScheme
    {
        Theme = 0,
        Accented = 1,
        Inverted = 2
    }
}