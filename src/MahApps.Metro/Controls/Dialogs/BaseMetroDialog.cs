// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ControlzEx.Theming;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Controls.Helper;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// The base class for dialogs.
    ///
    /// You probably don't want to use this class, if you want to add arbitrary content to your dialog,
    /// use the <see cref="CustomDialog"/> class.
    /// </summary>
    [TemplatePart(Name = PART_Top, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Content, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Bottom, Type = typeof(ContentPresenter))]
    public abstract class BaseMetroDialog : ContentControl
    {
        private const string PART_Top = "PART_Top";
        private const string PART_Content = "PART_Content";
        private const string PART_Bottom = "PART_Bottom";

        #region DependencyProperties

        /// <summary>Identifies the <see cref="ColorScheme"/> dependency property.</summary>
        public static readonly DependencyProperty ColorSchemeProperty
            = DependencyProperty.Register(nameof(ColorScheme),
                                          typeof(MetroDialogColorScheme),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(MetroDialogColorScheme.Theme));

        public MetroDialogColorScheme ColorScheme
        {
            get => (MetroDialogColorScheme)this.GetValue(ColorSchemeProperty);
            set => this.SetValue(ColorSchemeProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogContentMargin"/> dependency property.</summary>
        public static readonly DependencyProperty DialogContentMarginProperty
            = DependencyProperty.Register(nameof(DialogContentMargin),
                                          typeof(GridLength),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(new GridLength(25, GridUnitType.Star)));

        /// <summary>
        /// Gets or sets the left and right margin for the dialog content.
        /// </summary>
        public GridLength DialogContentMargin
        {
            get => (GridLength)this.GetValue(DialogContentMarginProperty);
            set => this.SetValue(DialogContentMarginProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogContentWidth"/> dependency property.</summary>
        public static readonly DependencyProperty DialogContentWidthProperty
            = DependencyProperty.Register(nameof(DialogContentWidth),
                                          typeof(GridLength),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(new GridLength(50, GridUnitType.Star)));

        /// <summary>
        /// Gets or sets the width for the dialog content.
        /// </summary>
        public GridLength DialogContentWidth
        {
            get => (GridLength)this.GetValue(DialogContentWidthProperty);
            set => this.SetValue(DialogContentWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title),
                                          typeof(object),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(default(object)));

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        public object? Title
        {
            get => (object?)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogTop"/> dependency property.</summary>
        public static readonly DependencyProperty DialogTopProperty
            = DependencyProperty.Register(nameof(DialogTop),
                                          typeof(object),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(null, UpdateLogicalChild));

        /// <summary>
        /// Gets or sets the content above the dialog.
        /// </summary>
        public object? DialogTop
        {
            get => this.GetValue(DialogTopProperty);
            set => this.SetValue(DialogTopProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogBottom"/> dependency property.</summary>
        public static readonly DependencyProperty DialogBottomProperty
            = DependencyProperty.Register(nameof(DialogBottom),
                                          typeof(object),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(null, UpdateLogicalChild));

        /// <summary>
        /// Gets or sets the content below the dialog.
        /// </summary>
        public object? DialogBottom
        {
            get => this.GetValue(DialogBottomProperty);
            set => this.SetValue(DialogBottomProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogTitleFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty DialogTitleFontSizeProperty
            = DependencyProperty.Register(nameof(DialogTitleFontSize),
                                          typeof(double),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(26D));

        /// <summary>
        /// Gets or sets the font size of the dialog title.
        /// </summary>
        public double DialogTitleFontSize
        {
            get => (double)this.GetValue(DialogTitleFontSizeProperty);
            set => this.SetValue(DialogTitleFontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogMessageFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty DialogMessageFontSizeProperty
            = DependencyProperty.Register(nameof(DialogMessageFontSize),
                                          typeof(double),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(15D));

        /// <summary>
        /// Gets or sets the font size of the dialog message text.
        /// </summary>
        public double DialogMessageFontSize
        {
            get => (double)this.GetValue(DialogMessageFontSizeProperty);
            set => this.SetValue(DialogMessageFontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="DialogButtonFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty DialogButtonFontSizeProperty
            = DependencyProperty.Register(nameof(DialogButtonFontSize),
                                          typeof(double),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata(SystemFonts.MessageFontSize));

        /// <summary>
        /// Gets or sets the font size of any dialog buttons.
        /// </summary>
        public double DialogButtonFontSize
        {
            get => (double)this.GetValue(DialogButtonFontSizeProperty);
            set => this.SetValue(DialogButtonFontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(BaseMetroDialog),
                                          new PropertyMetadata());

        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(BaseMetroDialog));

        public DataTemplate? IconTemplate
        {
            get => (DataTemplate?)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        #endregion DependencyProperties

        public MetroDialogSettings DialogSettings { get; private set; } = null!;

        internal SizeChangedEventHandler? SizeChangedHandler { get; set; }

        #region Constructor

        static BaseMetroDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseMetroDialog), new FrameworkPropertyMetadata(typeof(BaseMetroDialog)));
        }

        /// <summary>
        /// Initializes a new <see cref="BaseMetroDialog"/>.
        /// </summary>
        /// <param name="owningWindow">The window that is the parent of the dialog.</param>
        /// <param name="settings">The settings for the message dialog.</param>
        protected BaseMetroDialog(MetroWindow? owningWindow, MetroDialogSettings? settings)
        {
            this.Initialize(owningWindow, settings);
        }

        /// <summary>
        /// Initializes a new <see cref="BaseMetroDialog"/>.
        /// </summary>
        protected BaseMetroDialog()
            : this(null, new MetroDialogSettings())
        {
        }

        #endregion Constructor

        private static void UpdateLogicalChild(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not BaseMetroDialog dialog)
            {
                return;
            }

            if (e.OldValue is FrameworkElement oldChild)
            {
                dialog.RemoveLogicalChild(oldChild);
            }

            if (e.NewValue is FrameworkElement newChild)
            {
                dialog.AddLogicalChild(newChild);
                newChild.DataContext = dialog.DataContext;
            }
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // cheat, make a list with all logical content and return the enumerator
                ArrayList children = new ArrayList();
                if (this.DialogTop != null)
                {
                    children.Add(this.DialogTop);
                }

                if (this.Content != null)
                {
                    children.Add(this.Content);
                }

                if (this.DialogBottom != null)
                {
                    children.Add(this.DialogBottom);
                }

                return children.GetEnumerator();
            }
        }

        /// <summary>
        /// With this method it's possible to return your own settings in a custom dialog.
        /// </summary>
        /// <param name="settings">
        /// Settings from the <see cref="MetroWindow.MetroDialogOptions"/> or from constructor.
        /// The default is a new created settings.
        /// </param>
        /// <returns></returns>
        protected virtual MetroDialogSettings ConfigureSettings(MetroDialogSettings settings)
        {
            return settings;
        }

        private void Initialize(MetroWindow? owningWindow, MetroDialogSettings? settings)
        {
            AccessKeyHelper.SetIsAccessKeyScope(this, true);

            this.OwningWindow = owningWindow;
            this.DialogSettings = this.ConfigureSettings(settings ?? owningWindow?.MetroDialogOptions ?? new MetroDialogSettings());

            if (this.DialogSettings.CustomResourceDictionary is not null)
            {
                this.Resources.MergedDictionaries.Add(this.DialogSettings.CustomResourceDictionary);
            }

            this.SetCurrentValue(ColorSchemeProperty, this.DialogSettings.ColorScheme);

            this.SetCurrentValue(IconProperty, this.DialogSettings.Icon);
            this.SetCurrentValue(IconTemplateProperty, this.DialogSettings.IconTemplate);

            this.HandleThemeChange();

            this.DataContextChanged += this.BaseMetroDialogDataContextChanged;
            this.Loaded += this.BaseMetroDialogLoaded;
            this.Unloaded += this.BaseMetroDialogUnloaded;
        }

        private void BaseMetroDialogDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            // MahApps add these content presenter to the dialog with AddLogicalChild method.
            // This has the side effect that the DataContext doesn't update, so do this now here.
            if (this.DialogTop is FrameworkElement elementTop)
            {
                elementTop.DataContext = this.DataContext;
            }

            if (this.DialogBottom is FrameworkElement elementBottom)
            {
                elementBottom.DataContext = this.DataContext;
            }
        }

        private void BaseMetroDialogLoaded(object? sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.HandleThemeManagerThemeChanged;
            ThemeManager.Current.ThemeChanged += this.HandleThemeManagerThemeChanged;
            this.OnLoaded();
        }

        private void BaseMetroDialogUnloaded(object? sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.HandleThemeManagerThemeChanged;
        }

        private void HandleThemeManagerThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            this.Invoke(this.HandleThemeChange);
        }

        private static object? TryGetResource(Theme? theme, string key)
        {
            return theme?.Resources[key];
        }

        internal void HandleThemeChange()
        {
            var theme = DetectTheme(this);

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)
                || theme is null)
            {
                return;
            }

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Theme:
                    ThemeManager.Current.ChangeTheme(this, this.Resources, theme);
                    this.SetCurrentValue(BackgroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Background"));
                    this.SetCurrentValue(ForegroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Foreground"));
                    break;

                case MetroDialogColorScheme.Inverted:
                    theme = ThemeManager.Current.GetInverseTheme(theme);
                    if (theme is null)
                    {
                        throw new InvalidOperationException("The inverse dialog theme only works if the window theme abides the naming convention. " +
                                                            "See ThemeManager.GetInverseAppTheme for more infos");
                    }

                    ThemeManager.Current.ChangeTheme(this, this.Resources, theme);
                    this.SetCurrentValue(BackgroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Background"));
                    this.SetCurrentValue(ForegroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Foreground"));
                    break;

                case MetroDialogColorScheme.Accented:
                    ThemeManager.Current.ChangeTheme(this, this.Resources, theme);
                    this.SetCurrentValue(BackgroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Background.Accent"));
                    this.SetCurrentValue(ForegroundProperty, TryGetResource(theme, "MahApps.Brushes.Dialog.Foreground.Accent"));
                    break;
            }
        }

        /// <summary>
        /// This is called in the loaded event.
        /// </summary>
        protected virtual void OnLoaded()
        {
            // nothing here
        }

        private static Theme? DetectTheme(BaseMetroDialog? dialog)
        {
            if (dialog is null)
            {
                return null;
            }

            // first look for owner
            var window = dialog.OwningWindow ?? dialog.TryFindParent<MetroWindow>();
            var theme = window != null ? ThemeManager.Current.DetectTheme(window) : null;
            if (theme != null)
            {
                return theme;
            }

            // second try, look for main window and then for current application
            if (Application.Current != null)
            {
                theme = Application.Current.MainWindow is null
                    ? ThemeManager.Current.DetectTheme(Application.Current)
                    : ThemeManager.Current.DetectTheme(Application.Current.MainWindow);
                if (theme != null)
                {
                    return theme;
                }
            }

            return null;
        }

        private RoutedEventHandler? dialogOnLoaded;

        /// <summary>
        /// Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public Task WaitForLoadAsync()
        {
            this.Dispatcher.VerifyAccess();

            if (this.IsLoaded)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<object>();

            if (this.DialogSettings.AnimateShow != true)
            {
                this.SetCurrentValue(OpacityProperty, 1.0); // skip the animation
            }

            this.dialogOnLoaded = (_, _) =>
                {
                    this.Loaded -= this.dialogOnLoaded;

                    this.Focus();

                    tcs.TrySetResult(null!);
                };

            this.Loaded += this.dialogOnLoaded;

            return tcs.Task;
        }

        internal void FireOnShown()
        {
            this.OnShown();
        }

        protected virtual void OnShown()
        {
        }

        internal void FireOnClose()
        {
            this.OnClose();
        }

        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// Gets the window that owns the current Dialog IF AND ONLY IF the dialog is shown inside of a window.
        /// </summary>
        protected MetroWindow? OwningWindow { get; private set; }

        /// <summary>
        /// Waits until this dialog gets unloaded.
        /// </summary>
        /// <returns></returns>
        public Task WaitUntilUnloadedAsync()
        {
            var tcs = new TaskCompletionSource<object>();

            this.Unloaded += (_, _) => { tcs.TrySetResult(null!); };

            return tcs.Task;
        }

        private EventHandler? closingStoryboardOnCompleted;

        public Task WaitForCloseAsync()
        {
            var tcs = new TaskCompletionSource<object>();

            if (this.DialogSettings.AnimateHide)
            {
                if (this.TryFindResource("MahApps.Storyboard.Dialogs.Close") is not Storyboard closingStoryboard)
                {
                    throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add BaseMetroDialog.xaml to your merged dictionaries?");
                }

                closingStoryboard = closingStoryboard.Clone();

                this.closingStoryboardOnCompleted = (_, _) =>
                    {
                        closingStoryboard.Completed -= this.closingStoryboardOnCompleted;

                        tcs.TrySetResult(null!);
                    };

                closingStoryboard.Completed += this.closingStoryboardOnCompleted;

                closingStoryboard.Begin(this);
            }
            else
            {
                this.SetCurrentValue(OpacityProperty, 0.0);
                tcs.TrySetResult(null!); //skip the animation
            }

            return tcs.Task;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroDialogAutomationPeer(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey is Key.LeftAlt or Key.RightAlt or Key.F10)
            {
                if (ReferenceEquals(e.Source, this))
                {
                    // Try to look if there is a main menu inside the dialog.
                    // If no main menu exists then handle the Alt-Key and F10-Key
                    // to prevent focusing the first menu item at the main menu (window).
                    var menu = this.FindChildren<Menu>(true).FirstOrDefault(m => m.IsMainMenu);
                    if (menu is null)
                    {
                        e.Handled = true;
                    }
                }
            }

            base.OnKeyDown(e);
        }
    }
}