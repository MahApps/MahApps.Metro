using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// The base class for dialogs.
    ///
    /// You probably don't want to use this class, if you want to add arbitrary content to your dialog, 
    /// use the <see cref="SimpleDialog"/> class.
    /// </summary>
    [System.Windows.Markup.ContentProperty("DialogBody")]
    public abstract class BaseMetroDialog : Control
    {
        private const string PART_DialogBody_ContentPresenter = "PART_DialogBody_ContentPresenter";
        protected ContentPresenter DialogBody_ContentPresenter = null;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BaseMetroDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DialogBodyProperty = DependencyProperty.Register("DialogBody", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null, (o, e) =>
        {
            BaseMetroDialog dialog = (o as BaseMetroDialog);
            if (dialog != null)
            {
                if (e.OldValue != null)
                {
                    dialog.RemoveLogicalChild(e.OldValue);
                }
                if (e.NewValue != null)
                {
                    dialog.AddLogicalChild(e.NewValue);
                }
            }
        }));
        public static readonly DependencyProperty DialogTopProperty = DependencyProperty.Register("DialogTop", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty DialogBottomProperty = DependencyProperty.Register("DialogBottom", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));

        protected MetroDialogSettings DialogSettings { get; private set; }

        /// <summary>
        /// Gets/sets the dialog's title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content in the "message" area in the dialog. 
        /// </summary>
        public object DialogBody
        {
            get { return GetValue(DialogBodyProperty); }
            set { SetValue(DialogBodyProperty, value); }
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

        public override void OnApplyTemplate()
        {
            DialogBody_ContentPresenter = GetTemplateChild(PART_DialogBody_ContentPresenter) as ContentPresenter;

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        /// <param name="owningWindow">The window that is the parent of the dialog.</param>
        protected BaseMetroDialog(MetroWindow owningWindow, MetroDialogSettings settings)
        {
            DialogSettings = settings == null ? owningWindow.MetroDialogOptions : settings;

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
            ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
            this.Unloaded += BaseMetroDialog_Unloaded;

            HandleTheme();

            this.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Themes/Dialogs/BaseMetroDialog.xaml") });

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
                switch (DialogSettings.ColorScheme)
                {
                    case MetroDialogColorScheme.Theme:
                        this.SetValue(BackgroundProperty, ThemeManager.GetResourceFromAppStyle(OwningWindow ?? Application.Current.MainWindow, "WhiteColorBrush"));
                        this.SetValue(ForegroundProperty, ThemeManager.GetResourceFromAppStyle(OwningWindow ?? Application.Current.MainWindow, "BlackBrush"));
                        break;
                    case MetroDialogColorScheme.Accented:
                        this.SetValue(BackgroundProperty, ThemeManager.GetResourceFromAppStyle(OwningWindow ?? Application.Current.MainWindow, "HighlightBrush"));
                        this.SetValue(ForegroundProperty, ThemeManager.GetResourceFromAppStyle(OwningWindow ?? Application.Current.MainWindow, "IdealForegroundColorBrush"));
                        break;
                }
            }
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
            handler = new RoutedEventHandler((sender, args) =>
                {
                    this.Loaded -= handler;

                    tcs.TrySetResult(null);
                });

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
                else
                {
                    //This is from a MetroWindow created by the external dialog APIs.
                    return _WaitForCloseAsync().ContinueWith(x =>
                        {
                            ParentDialogWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                ParentDialogWindow.Close();
                            }));
                        });
                }
            }
            return Task.Factory.StartNew(() => { });
        }

        internal protected virtual void OnShown() { }
        internal protected virtual void OnClose()
        {
            if (ParentDialogWindow != null) //this is only set when a dialog is shown (externally) in it's OWN window.
                ParentDialogWindow.Close();
        }

        /// <summary>
        /// A last chance virtual method for stopping an external dialog from closing.
        /// </summary>
        /// <returns></returns>
        internal protected virtual bool OnRequestClose()
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

        public Task _WaitForCloseAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (DialogSettings.AnimateHide)
            {
                Storyboard closingStoryboard = this.Resources["DialogCloseStoryboard"] as Storyboard;

                if (closingStoryboard == null)
                    throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add BaseMetroDialog.xaml to your merged dictionaries?");

                EventHandler handler = null;
                handler = new EventHandler((sender, args) =>
                {
                    closingStoryboard.Completed -= handler;

                    tcs.TrySetResult(null);
                });

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
            AffirmativeButtonText = "OK";
            NegativeButtonText = "Cancel";

            ColorScheme = MetroDialogColorScheme.Theme;
            AnimateShow = AnimateHide = true;

            DefaultText = "";
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
        /// Enable/disable dialog showing/hiding animations.
        /// Setting this value will also set both AnimateShow and AnimateHide properties to the same value.
        /// Getting "True" - at least AnimateHide or AnimateHide is "True".
        /// Getting "False" - both AnimateHide and AnimateHide are "False".
        /// </summary>
        [Obsolete("This property is obsolete and will be deleted in next release, use AnimateShow and AnimateHide instead.")]
        public bool UseAnimations
        {
            get { return AnimateShow || AnimateHide; }
            set { AnimateShow = AnimateHide = value; }
        }

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
    }

    /// <summary>
    /// An enum representing the different choices for a color scheme in a Metro Dialog.
    /// </summary>
    public enum MetroDialogColorScheme
    {
        Theme = 0,
        Accented = 1
    }
}
