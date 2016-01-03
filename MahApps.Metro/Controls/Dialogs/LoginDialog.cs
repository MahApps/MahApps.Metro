using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public class LoginDialogSettings : MetroDialogSettings
    {
        private const string DefaultUsernameWatermark = "Username...";
        private const string DefaultPasswordWatermark = "Password...";
        private const Visibility DefaultNegativeButtonVisibility = Visibility.Collapsed;
        private const bool DefaultShouldHideUsername = false;
        private const bool DefaultEnablePasswordPreview = false;

        public LoginDialogSettings()
        {
            UsernameWatermark = DefaultUsernameWatermark;
            PasswordWatermark = DefaultPasswordWatermark;
            NegativeButtonVisibility = DefaultNegativeButtonVisibility;
            ShouldHideUsername = DefaultShouldHideUsername;
            AffirmativeButtonText = "Login";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string InitialUsername { get; set; }

        public string InitialPassword { get; set; }

        public string UsernameWatermark { get; set; }

        public bool ShouldHideUsername { get; set; }

        public string PasswordWatermark { get; set; }

        public Visibility NegativeButtonVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class LoginDialogData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public partial class LoginDialog : BaseMetroDialog
    {
        internal LoginDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal LoginDialog(MetroWindow parentWindow, LoginDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
            Username = settings.InitialUsername;
            Password = settings.InitialPassword;
            UsernameWatermark = settings.UsernameWatermark;
            PasswordWatermark = settings.PasswordWatermark;
            NegativeButtonButtonVisibility = settings.NegativeButtonVisibility;
            ShouldHideUsername = settings.ShouldHideUsername;
        }

        internal Task<LoginDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                if (string.IsNullOrEmpty(PART_TextBox.Text) && !ShouldHideUsername)
                {
                    PART_TextBox.Focus();
                }
                else
                {
                    PART_TextBox2.Focus();
                }
            }));

            TaskCompletionSource<LoginDialogData> tcs = new TaskCompletionSource<LoginDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                PART_TextBox.KeyDown -= affirmativeKeyHandler;
                PART_TextBox2.KeyDown -= affirmativeKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;

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
                    tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password });
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

                tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password });

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;
            PART_TextBox2.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

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
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                }
            }

            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty = DependencyProperty.Register("NegativeButtonButtonVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ShouldHideUsernameProperty = DependencyProperty.Register("ShouldHideUsername", typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string UsernameWatermark
        {
            get { return (string)GetValue(UsernameWatermarkProperty); }
            set { SetValue(UsernameWatermarkProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)GetValue(PasswordWatermarkProperty); }
            set { SetValue(PasswordWatermarkProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public Visibility NegativeButtonButtonVisibility
        {
            get { return (Visibility)GetValue(NegativeButtonButtonVisibilityProperty); }
            set { SetValue(NegativeButtonButtonVisibilityProperty, value); }
        }

        public bool ShouldHideUsername
        {
            get { return (bool)GetValue(ShouldHideUsernameProperty); }
            set { SetValue(ShouldHideUsernameProperty, value); }
        }
    }
}
