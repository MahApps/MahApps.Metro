using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public partial class LoginDialog : BaseMetroDialog
    {
        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        /// <summary>Identifies the <see cref="Username"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(nameof(Username), typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));

        public string Username
        {
            get { return (string)this.GetValue(UsernameProperty); }
            set { this.SetValue(UsernameProperty, value); }
        }

        /// <summary>Identifies the <see cref="UsernameWatermark"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register(nameof(UsernameWatermark), typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));

        public string UsernameWatermark
        {
            get { return (string)this.GetValue(UsernameWatermarkProperty); }
            set { this.SetValue(UsernameWatermarkProperty, value); }
        }

        /// <summary>Identifies the <see cref="UsernameCharacterCasing"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameCharacterCasingProperty = DependencyProperty.Register(nameof(UsernameCharacterCasing), typeof(CharacterCasing), typeof(LoginDialog), new PropertyMetadata(default(CharacterCasing)));

        public CharacterCasing UsernameCharacterCasing
        {
            get { return (CharacterCasing)this.GetValue(UsernameCharacterCasingProperty); }
            set { this.SetValue(UsernameCharacterCasingProperty, value); }
        }

        /// <summary>Identifies the <see cref="Password"/> dependency property.</summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string)this.GetValue(PasswordProperty); }
            set { this.SetValue(PasswordProperty, value); }
        }

        /// <summary>Identifies the <see cref="PasswordWatermark"/> dependency property.</summary>
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register(nameof(PasswordWatermark), typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));

        public string PasswordWatermark
        {
            get { return (string)this.GetValue(PasswordWatermarkProperty); }
            set { this.SetValue(PasswordWatermarkProperty, value); }
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register(nameof(AffirmativeButtonText), typeof(string), typeof(LoginDialog), new PropertyMetadata("OK"));

        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { this.SetValue(AffirmativeButtonTextProperty, value); }
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register(nameof(NegativeButtonText), typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        /// <summary>Identifies the <see cref="NegativeButtonButtonVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty = DependencyProperty.Register(nameof(NegativeButtonButtonVisibility), typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));

        public Visibility NegativeButtonButtonVisibility
        {
            get { return (Visibility)this.GetValue(NegativeButtonButtonVisibilityProperty); }
            set { this.SetValue(NegativeButtonButtonVisibilityProperty, value); }
        }

        /// <summary>Identifies the <see cref="ShouldHideUsername"/> dependency property.</summary>
        public static readonly DependencyProperty ShouldHideUsernameProperty = DependencyProperty.Register(nameof(ShouldHideUsername), typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));

        public bool ShouldHideUsername
        {
            get { return (bool)this.GetValue(ShouldHideUsernameProperty); }
            set { this.SetValue(ShouldHideUsernameProperty, value); }
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxVisibilityProperty = DependencyProperty.Register(nameof(RememberCheckBoxVisibility), typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));

        public Visibility RememberCheckBoxVisibility
        {
            get { return (Visibility)this.GetValue(RememberCheckBoxVisibilityProperty); }
            set { this.SetValue(RememberCheckBoxVisibilityProperty, value); }
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxText"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxTextProperty = DependencyProperty.Register(nameof(RememberCheckBoxText), typeof(string), typeof(LoginDialog), new PropertyMetadata("Remember"));

        public string RememberCheckBoxText
        {
            get { return (string)this.GetValue(RememberCheckBoxTextProperty); }
            set { this.SetValue(RememberCheckBoxTextProperty, value); }
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxChecked"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxCheckedProperty = DependencyProperty.Register(nameof(RememberCheckBoxChecked), typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));

        public bool RememberCheckBoxChecked
        {
            get { return (bool)this.GetValue(RememberCheckBoxCheckedProperty); }
            set { this.SetValue(RememberCheckBoxCheckedProperty, value); }
        }

        internal LoginDialog()
            : this(null)
        {
        }

        internal LoginDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal LoginDialog(MetroWindow parentWindow, LoginDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
            this.Username = settings.InitialUsername;
            this.Password = settings.InitialPassword;
            this.UsernameCharacterCasing = settings.UsernameCharacterCasing;
            this.UsernameWatermark = settings.UsernameWatermark;
            this.PasswordWatermark = settings.PasswordWatermark;
            this.NegativeButtonButtonVisibility = settings.NegativeButtonVisibility;
            this.ShouldHideUsername = settings.ShouldHideUsername;
            this.RememberCheckBoxVisibility = settings.RememberCheckBoxVisibility;
            this.RememberCheckBoxText = settings.RememberCheckBoxText;
            this.RememberCheckBoxChecked = settings.RememberCheckBoxChecked;
        }

        internal Task<LoginDialogData> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();
                    if (string.IsNullOrEmpty(this.PART_TextBox.Text) && !this.ShouldHideUsername)
                    {
                        this.PART_TextBox.Focus();
                    }
                    else
                    {
                        this.PART_TextBox2.Focus();
                    }
                }));

            TaskCompletionSource<LoginDialogData> tcs = new TaskCompletionSource<LoginDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = this.DialogSettings.CancellationToken.Register(() =>
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(null);
                });

            cleanUpHandlers = () =>
                {
                    this.PART_TextBox.KeyDown -= affirmativeKeyHandler;
                    this.PART_TextBox2.KeyDown -= affirmativeKeyHandler;

                    this.KeyDown -= escapeKeyHandler;

                    this.PART_NegativeButton.Click -= negativeHandler;
                    this.PART_AffirmativeButton.Click -= affirmativeHandler;

                    this.PART_NegativeButton.KeyDown -= negativeKeyHandler;
                    this.PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;

                    cancellationTokenRegistration.Dispose();
                };

            escapeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(null);
                    }
                };

            negativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(null);
                    }
                };

            affirmativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();
                        tcs.TrySetResult(new LoginDialogData
                                         {
                                             Username = this.Username,
                                             SecurePassword = this.PART_TextBox2.SecurePassword,
                                             ShouldRemember = this.RememberCheckBoxChecked
                                         });
                    }
                };

            negativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);

                    e.Handled = true;
                };

            affirmativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new LoginDialogData
                                     {
                                         Username = this.Username,
                                         SecurePassword = this.PART_TextBox2.SecurePassword,
                                         ShouldRemember = this.RememberCheckBoxChecked
                                     });

                    e.Handled = true;
                };

            this.PART_NegativeButton.KeyDown += negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.PART_TextBox.KeyDown += affirmativeKeyHandler;
            this.PART_TextBox2.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            this.PART_NegativeButton.Click += negativeHandler;
            this.PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = this.DialogSettings as LoginDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = this.FindResource("MahApps.Styles.PasswordBox.Win8") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    this.PART_TextBox2.Style = win8MetroPasswordStyle;
                    // apply template again to fire the loaded event which is necessary for revealed password
                    this.PART_TextBox2.ApplyTemplate();
                }
            }

            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                    this.PART_TextBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                    this.PART_TextBox2.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                    break;
            }
        }
    }
}