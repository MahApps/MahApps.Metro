// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls.Dialogs
{
    [TemplatePart(Name = nameof(PART_AffirmativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_NegativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_TextBox), Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(PART_PasswordBox), Type = typeof(PasswordBox))]
    public class LoginDialog : BaseMetroDialog
    {
        private readonly TaskCompletionSource<LoginDialogData?> tcs = new();
        private CancellationTokenRegistration? cancellationTokenRegistration;

        #region Controls

        private Button? PART_AffirmativeButton;
        private Button? PART_NegativeButton;
        private TextBox? PART_TextBox;
        private PasswordBox? PART_PasswordBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_AffirmativeButton = this.GetTemplateChild(nameof(this.PART_AffirmativeButton)) as Button;
            this.PART_NegativeButton = this.GetTemplateChild(nameof(this.PART_NegativeButton)) as Button;
            this.PART_TextBox = this.GetTemplateChild(nameof(this.PART_TextBox)) as TextBox;
            this.PART_PasswordBox = this.GetTemplateChild(nameof(this.PART_PasswordBox)) as PasswordBox;
        }

        #endregion Controls

        #region DependencyProperties

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(default(string)));

        public string? Message
        {
            get => (string?)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="Username"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameProperty
            = DependencyProperty.Register(nameof(Username),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string? Username
        {
            get => (string?)this.GetValue(UsernameProperty);
            set => this.SetValue(UsernameProperty, value);
        }

        /// <summary>Identifies the <see cref="UsernameWatermark"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameWatermarkProperty
            = DependencyProperty.Register(nameof(UsernameWatermark),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(default(string)));

        public string? UsernameWatermark
        {
            get => (string?)this.GetValue(UsernameWatermarkProperty);
            set => this.SetValue(UsernameWatermarkProperty, value);
        }

        /// <summary>Identifies the <see cref="UsernameCharacterCasing"/> dependency property.</summary>
        public static readonly DependencyProperty UsernameCharacterCasingProperty
            = DependencyProperty.Register(nameof(UsernameCharacterCasing),
                                          typeof(CharacterCasing),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(default(CharacterCasing)));

        public CharacterCasing UsernameCharacterCasing
        {
            get => (CharacterCasing)this.GetValue(UsernameCharacterCasingProperty);
            set => this.SetValue(UsernameCharacterCasingProperty, value);
        }

        /// <summary>Identifies the <see cref="Password"/> dependency property.</summary>
        public static readonly DependencyProperty PasswordProperty
            = DependencyProperty.Register(nameof(Password),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(default(string)));

        public string? Password
        {
            get => (string?)this.GetValue(PasswordProperty);
            set => this.SetValue(PasswordProperty, value);
        }

        /// <summary>Identifies the <see cref="PasswordWatermark"/> dependency property.</summary>
        public static readonly DependencyProperty PasswordWatermarkProperty
            = DependencyProperty.Register(nameof(PasswordWatermark),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(default(string)));

        public string? PasswordWatermark
        {
            get => (string?)this.GetValue(PasswordWatermarkProperty);
            set => this.SetValue(PasswordWatermarkProperty, value);
        }

        /// <summary>Identifies the <see cref="EnablePasswordPreview"/> dependency property.</summary>
        public static readonly DependencyProperty EnablePasswordPreviewProperty
            = DependencyProperty.Register(nameof(EnablePasswordPreview),
                                          typeof(bool),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool EnablePasswordPreview
        {
            get => (bool)this.GetValue(EnablePasswordPreviewProperty);
            set => this.SetValue(EnablePasswordPreviewProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty
            = DependencyProperty.Register(nameof(AffirmativeButtonText),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata("OK"));

        public string AffirmativeButtonText
        {
            get => (string)this.GetValue(AffirmativeButtonTextProperty);
            set => this.SetValue(AffirmativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty
            = DependencyProperty.Register(nameof(NegativeButtonText),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)this.GetValue(NegativeButtonTextProperty);
            set => this.SetValue(NegativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeButtonButtonVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty
            = DependencyProperty.Register(nameof(NegativeButtonButtonVisibility),
                                          typeof(Visibility),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(Visibility.Collapsed));

        public Visibility NegativeButtonButtonVisibility
        {
            get => (Visibility)this.GetValue(NegativeButtonButtonVisibilityProperty);
            set => this.SetValue(NegativeButtonButtonVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="ShouldHideUsername"/> dependency property.</summary>
        public static readonly DependencyProperty ShouldHideUsernameProperty
            = DependencyProperty.Register(nameof(ShouldHideUsername),
                                          typeof(bool),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool ShouldHideUsername
        {
            get => (bool)this.GetValue(ShouldHideUsernameProperty);
            set => this.SetValue(ShouldHideUsernameProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxVisibilityProperty
            = DependencyProperty.Register(nameof(RememberCheckBoxVisibility),
                                          typeof(Visibility),
                                          typeof(LoginDialog),
                                          new PropertyMetadata(Visibility.Collapsed));

        public Visibility RememberCheckBoxVisibility
        {
            get => (Visibility)this.GetValue(RememberCheckBoxVisibilityProperty);
            set => this.SetValue(RememberCheckBoxVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxText"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxTextProperty
            = DependencyProperty.Register(nameof(RememberCheckBoxText),
                                          typeof(string),
                                          typeof(LoginDialog),
                                          new PropertyMetadata("Remember"));

        public string RememberCheckBoxText
        {
            get => (string)this.GetValue(RememberCheckBoxTextProperty);
            set => this.SetValue(RememberCheckBoxTextProperty, value);
        }

        /// <summary>Identifies the <see cref="RememberCheckBoxChecked"/> dependency property.</summary>
        public static readonly DependencyProperty RememberCheckBoxCheckedProperty
            = DependencyProperty.Register(nameof(RememberCheckBoxChecked),
                                          typeof(bool),
                                          typeof(LoginDialog),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool RememberCheckBoxChecked
        {
            get => (bool)this.GetValue(RememberCheckBoxCheckedProperty);
            set => this.SetValue(RememberCheckBoxCheckedProperty, BooleanBoxes.Box(value));
        }

        #endregion DependencyProperties

        #region Constructor

        internal LoginDialog()
            : this(null)
        {
        }

        internal LoginDialog(MetroWindow? parentWindow)
            : this(parentWindow, null)
        {
        }

        internal LoginDialog(MetroWindow? parentWindow, LoginDialogSettings? settings)
            : base(parentWindow, settings ?? new LoginDialogSettings())
        {
            this.SetCurrentValue(AffirmativeButtonTextProperty, this.DialogSettings.AffirmativeButtonText);
            this.SetCurrentValue(NegativeButtonTextProperty, this.DialogSettings.NegativeButtonText);

            if (this.DialogSettings is LoginDialogSettings loginDialogSettings)
            {
                this.SetCurrentValue(EnablePasswordPreviewProperty, loginDialogSettings.EnablePasswordPreview);
                this.SetCurrentValue(UsernameProperty, loginDialogSettings.InitialUsername);
                this.SetCurrentValue(PasswordProperty, loginDialogSettings.InitialPassword);
                this.SetCurrentValue(UsernameCharacterCasingProperty, loginDialogSettings.UsernameCharacterCasing);
                this.SetCurrentValue(UsernameWatermarkProperty, loginDialogSettings.UsernameWatermark);
                this.SetCurrentValue(PasswordWatermarkProperty, loginDialogSettings.PasswordWatermark);
                this.SetCurrentValue(NegativeButtonButtonVisibilityProperty, loginDialogSettings.NegativeButtonVisibility);
                this.SetCurrentValue(ShouldHideUsernameProperty, loginDialogSettings.ShouldHideUsername);
                this.SetCurrentValue(RememberCheckBoxVisibilityProperty, loginDialogSettings.RememberCheckBoxVisibility);
                this.SetCurrentValue(RememberCheckBoxTextProperty, loginDialogSettings.RememberCheckBoxText);
                this.SetCurrentValue(RememberCheckBoxCheckedProperty, loginDialogSettings.RememberCheckBoxChecked);
            }
        }

        static LoginDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoginDialog), new FrameworkPropertyMetadata(typeof(LoginDialog)));
        }

        #endregion Constructor

        internal async Task<LoginDialogData?> WaitForButtonPressAsync()
        {
            await this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();
                    if (this.PART_TextBox is not null && string.IsNullOrEmpty(this.PART_TextBox.Text) && !this.ShouldHideUsername)
                    {
                        this.PART_TextBox.Focus();
                    }
                    else
                    {
                        this.PART_PasswordBox?.Focus();
                    }
                }));

            this.SetUpHandlers();

            return await this.tcs.Task.ConfigureAwait(false);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
            {
                this.CleanUpHandlers();

                this.tcs.TrySetResult(null!);

                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                this.CleanUpHandlers();

                this.tcs.TrySetResult(!ReferenceEquals(sender, this.PART_NegativeButton)
                                          ? new LoginDialogData
                                            {
                                                Username = this.Username,
                                                SecurePassword = this.PART_PasswordBox?.SecurePassword,
                                                ShouldRemember = this.RememberCheckBoxChecked
                                            }
                                          : null!);

                e.Handled = true;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this.CleanUpHandlers();

            this.tcs.TrySetResult(ReferenceEquals(sender, this.PART_AffirmativeButton)
                                      ? new LoginDialogData
                                        {
                                            Username = this.Username,
                                            SecurePassword = this.PART_PasswordBox?.SecurePassword,
                                            ShouldRemember = this.RememberCheckBoxChecked
                                        }
                                      : null!);

            e.Handled = true;
        }

        private void SetUpHandlers()
        {
            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.Click += this.OnButtonClick;
                this.PART_NegativeButton.KeyDown += this.OnKeyDownHandler;
            }

            if (this.PART_AffirmativeButton is not null)
            {
                this.PART_AffirmativeButton.Click += this.OnButtonClick;
                this.PART_AffirmativeButton.KeyDown += this.OnKeyDownHandler;
            }

            if (this.PART_TextBox is not null)
            {
                this.PART_TextBox.KeyDown += this.OnKeyDownHandler;
            }

            if (this.PART_PasswordBox is not null)
            {
                this.PART_PasswordBox.KeyDown += this.OnKeyDownHandler;
            }

            this.KeyDown += this.OnKeyDownHandler;

            this.cancellationTokenRegistration = this.DialogSettings
                                                     .CancellationToken
                                                     .Register(() =>
                                                         {
                                                             this.BeginInvoke(() =>
                                                                 {
                                                                     this.CleanUpHandlers();
                                                                     this.tcs.TrySetResult(null!);
                                                                 });
                                                         });
        }

        private void CleanUpHandlers()
        {
            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.Click -= this.OnButtonClick;
                this.PART_NegativeButton.KeyDown -= this.OnKeyDownHandler;
            }

            if (this.PART_AffirmativeButton is not null)
            {
                this.PART_AffirmativeButton.Click -= this.OnButtonClick;
                this.PART_AffirmativeButton.KeyDown -= this.OnKeyDownHandler;
            }

            if (this.PART_TextBox is not null)
            {
                this.PART_TextBox.KeyDown -= this.OnKeyDownHandler;
            }

            if (this.PART_PasswordBox is not null)
            {
                this.PART_PasswordBox.KeyDown -= this.OnKeyDownHandler;
            }

            this.KeyDown -= this.OnKeyDownHandler;

            this.cancellationTokenRegistration?.Dispose();
        }
    }
}