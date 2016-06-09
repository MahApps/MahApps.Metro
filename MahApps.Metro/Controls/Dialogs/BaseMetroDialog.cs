using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.ComponentModel;

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
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BaseMetroDialog), new PropertyMetadata(default(DataTemplate)));

        public MetroDialogSettings DialogSettings { get; private set; }

        /// <summary>
        /// Gets/sets the dialog's title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the dialogs's icon.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets/sets the data template used to display the content of the <see cref="Icon"/>. 
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content on top of the dialog.
        /// </summary>
        public object DialogTop
        {
            get { return GetValue(DialogTopProperty); }
            set { SetValue(DialogTopProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content below the dialog.
        /// </summary>
        public object DialogBottom
        {
            get { return GetValue(DialogBottomProperty); }
            set { SetValue(DialogBottomProperty, value); }
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
            DialogSettings = settings ?? owningWindow.MetroDialogOptions;

            OwningWindow = owningWindow;

            Initialize();
        }


        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        protected BaseMetroDialog()
        {
            DialogSettings = new MetroDialogSettings();

            Initialize();
        }

        private void Initialize()
        {
            if (DialogSettings != null && !DialogSettings.SuppressDefaultResources)
            {
                this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });
            }

            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml") });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Themes/Dialogs/BaseMetroDialog.xaml") });
            if (DialogSettings != null && DialogSettings.CustomResourceDictionary != null)
            {
                this.Resources.MergedDictionaries.Add(DialogSettings.CustomResourceDictionary);
            }

            this.Loaded += (sender, args) =>
            {
                OnLoaded();
                HandleTheme();
            };
            ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
            this.Unloaded += BaseMetroDialog_Unloaded;
        }

        void BaseMetroDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.IsThemeChanged -= ThemeManager_IsThemeChanged;
            this.Unloaded -= BaseMetroDialog_Unloaded;
        }

        void ThemeManager_IsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            HandleTheme();
        }

        private void HandleTheme()
        {
            if (DialogSettings != null)
            {
                var windowTheme = DetectTheme(this);

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) || windowTheme == null)
                {
                    return;
                }

                var theme = windowTheme.Item1;
                var windowAccent = windowTheme.Item2;

                switch (DialogSettings.ColorScheme)
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

        private static Tuple<AppTheme, Accent> DetectTheme(BaseMetroDialog dialog)
        {
            if (dialog == null)
                return null;

            // first look for owner
            var window = dialog.TryFindParent<MetroWindow>();
            var theme = window != null ? ThemeManager.DetectAppStyle(window) : null;
            if (theme != null && theme.Item2 != null)
                return theme;

            // second try, look for main window
            if (Application.Current != null)
            {
                var mainWindow = Application.Current.MainWindow as MetroWindow;
                theme = mainWindow != null ? ThemeManager.DetectAppStyle(mainWindow) : null;
                if (theme != null && theme.Item2 != null)
                    return theme;

                // oh no, now look at application resource
                theme = ThemeManager.DetectAppStyle(Application.Current);
                if (theme != null && theme.Item2 != null)
                    return theme;
            }
            return null;
        }

        /// <summary>
        /// Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public Task WaitForLoadAsync()
        {
            Dispatcher.VerifyAccess();

            if (this.IsLoaded) return new Task(() => { });

            if (!DialogSettings.AnimateShow)
                this.Opacity = 1.0; //skip the animation

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
            if (OnRequestClose())
            {
                //Technically, the Dialog is /always/ inside of a MetroWindow.
                //If the dialog is inside of a user-created MetroWindow, not one created by the external dialog APIs.
                if (ParentDialogWindow == null)
                {
                    //This is from a user-created MetroWindow
                    return DialogManager.HideMetroDialogAsync(OwningWindow, this);
                }

                //This is from a MetroWindow created by the external dialog APIs.
                return _WaitForCloseAsync().ContinueWith(x =>
                {
                    ParentDialogWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        ParentDialogWindow.Close();
                    }));
                });
            }
            return Task.Factory.StartNew(() => { });
        }

        protected internal virtual void OnShown() { }
        protected internal virtual void OnClose()
        {
            if (ParentDialogWindow != null) //this is only set when a dialog is shown (externally) in it's OWN window.
                ParentDialogWindow.Close();
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

            Unloaded += (s, e) =>
            {
                tcs.TrySetResult(null);
            };

            return tcs.Task;
        }


        public Task _WaitForCloseAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (DialogSettings.AnimateHide)
            {
                Storyboard closingStoryboard = this.Resources["DialogCloseStoryboard"] as Storyboard;

                if (closingStoryboard == null)
                    throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add BaseMetroDialog.xaml to your merged dictionaries?");

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
}