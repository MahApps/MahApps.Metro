namespace MahApps.Metro.Controls.Dialogs
{
    using System.Windows;

    public class LoginDialogSettings : MetroDialogSettings
    {
        private const string DefaultUsernameWatermark = "Username...";
        private const string DefaultPasswordWatermark = "Password...";
        private const Visibility DefaultNegativeButtonVisibility = Visibility.Collapsed;
        private const bool DefaultEnablePasswordPreview = false;

        public LoginDialogSettings()
        {
            UsernameWatermark = DefaultUsernameWatermark;
            PasswordWatermark = DefaultPasswordWatermark;
            NegativeButtonVisibility = DefaultNegativeButtonVisibility;
            AffirmativeButtonText = "Login";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string InitialUsername { get; set; }

        public string InitialPassword { get; set; }

        public string UsernameWatermark { get; set; }

        public string PasswordWatermark { get; set; }

        public Visibility NegativeButtonVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }
}