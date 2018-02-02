using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public partial class LoginDialog : BaseMetroDialog
    {
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
                    if (e.Key == Key.Escape)
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
                var win8MetroPasswordStyle = this.FindResource("Win8MetroPasswordBox") as Style;
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
                    this.PART_NegativeButton.SetResourceReference(StyleProperty, "AccentedDialogHighlightedSquareButton");
                    this.PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    this.PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameCharacterCasingProperty = DependencyProperty.Register("UsernameCharacterCasing", typeof(CharacterCasing), typeof(LoginDialog), new PropertyMetadata(default(CharacterCasing)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty = DependencyProperty.Register("NegativeButtonButtonVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ShouldHideUsernameProperty = DependencyProperty.Register("ShouldHideUsername", typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));
        public static readonly DependencyProperty RememberCheckBoxVisibilityProperty = DependencyProperty.Register("RememberCheckBoxVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty RememberCheckBoxTextProperty = DependencyProperty.Register("RememberCheckBoxText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Remember"));
        public static readonly DependencyProperty RememberCheckBoxCheckedProperty = DependencyProperty.Register("RememberCheckBoxChecked", typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public string Username
        {
            get { return (string)this.GetValue(UsernameProperty); }
            set { this.SetValue(UsernameProperty, value); }
        }

        public string Password
        {
            get { return (string)this.GetValue(PasswordProperty); }
            set { this.SetValue(PasswordProperty, value); }
        }

        public string UsernameWatermark
        {
            get { return (string)this.GetValue(UsernameWatermarkProperty); }
            set { this.SetValue(UsernameWatermarkProperty, value); }
        }

        public CharacterCasing UsernameCharacterCasing
        {
            get { return (CharacterCasing)this.GetValue(UsernameCharacterCasingProperty); }
            set { this.SetValue(UsernameCharacterCasingProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)this.GetValue(PasswordWatermarkProperty); }
            set { this.SetValue(PasswordWatermarkProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { this.SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        public Visibility NegativeButtonButtonVisibility
        {
            get { return (Visibility)this.GetValue(NegativeButtonButtonVisibilityProperty); }
            set { this.SetValue(NegativeButtonButtonVisibilityProperty, value); }
        }

        public bool ShouldHideUsername
        {
            get { return (bool)this.GetValue(ShouldHideUsernameProperty); }
            set { this.SetValue(ShouldHideUsernameProperty, value); }
        }

        public Visibility RememberCheckBoxVisibility
        {
            get { return (Visibility)this.GetValue(RememberCheckBoxVisibilityProperty); }
            set { this.SetValue(RememberCheckBoxVisibilityProperty, value); }
        }

        public string RememberCheckBoxText
        {
            get { return (string)this.GetValue(RememberCheckBoxTextProperty); }
            set { this.SetValue(RememberCheckBoxTextProperty, value); }
        }

        public bool RememberCheckBoxChecked
        {
            get { return (bool)this.GetValue(RememberCheckBoxCheckedProperty); }
            set { this.SetValue(RememberCheckBoxCheckedProperty, value); }
        }
    }
}