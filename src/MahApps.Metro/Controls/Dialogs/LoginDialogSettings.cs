// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls.Dialogs
{
    public class LoginDialogSettings : MetroDialogSettings
    {
        private const string DefaultAffirmativeButtonText = "Login";
        private const string DefaultUsernameWatermark = "Username...";
        private const string DefaultPasswordWatermark = "Password...";
        private const string DefaultRememberCheckBoxText = "Remember";

        public LoginDialogSettings()
        {
            this.AffirmativeButtonText = DefaultAffirmativeButtonText;
        }

        public LoginDialogSettings(MetroDialogSettings? source)
            : base(source)
        {
            this.AffirmativeButtonText = DefaultAffirmativeButtonText;

            if (source is LoginDialogSettings settings)
            {
                this.InitialUsername = settings.InitialUsername;
                this.InitialPassword = settings.InitialPassword;
                this.UsernameWatermark = settings.UsernameWatermark;
                this.UsernameCharacterCasing = settings.UsernameCharacterCasing;
                this.ShouldHideUsername = settings.ShouldHideUsername;
                this.PasswordWatermark = settings.PasswordWatermark;
                this.NegativeButtonVisibility = settings.NegativeButtonVisibility;
                this.EnablePasswordPreview = settings.EnablePasswordPreview;
                this.RememberCheckBoxVisibility = settings.RememberCheckBoxVisibility;
                this.RememberCheckBoxText = settings.RememberCheckBoxText;
                this.RememberCheckBoxChecked = settings.RememberCheckBoxChecked;
            }
        }

        public string? InitialUsername { get; set; }

        public string? InitialPassword { get; set; }

        public string UsernameWatermark { get; set; } = DefaultUsernameWatermark;

        public CharacterCasing UsernameCharacterCasing { get; set; } = CharacterCasing.Normal;

        public bool ShouldHideUsername { get; set; }

        public string PasswordWatermark { get; set; } = DefaultPasswordWatermark;

        public Visibility NegativeButtonVisibility { get; set; } = Visibility.Collapsed;

        public bool EnablePasswordPreview { get; set; }

        public Visibility RememberCheckBoxVisibility { get; set; } = Visibility.Collapsed;

        public string RememberCheckBoxText { get; set; } = DefaultRememberCheckBoxText;

        public bool RememberCheckBoxChecked { get; set; } = false;
    }
}