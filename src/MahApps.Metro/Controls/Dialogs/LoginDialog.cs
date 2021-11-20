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
        private CancellationTokenRegistration cancellationTokenRegistration;

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
                                          new PropertyMetadata(default(string)));

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
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool RememberCheckBoxChecked
        {
            get => (bool)this.GetValue(RememberCheckBoxCheckedProperty);
            set => this.SetValue(RememberCheckBoxCheckedProperty, BooleanBoxes.Box(value));
        }

        #endregion DependencyProperties

        #region Constructor

        internal LoginDialog() : this(null)
        {
        }

        internal LoginDialog(MetroWindow? parentWindow) : this(parentWindow, null)
        {
        }

        internal LoginDialog(MetroWindow? parentWindow, LoginDialogSettings? settings) : base(parentWindow, settings ??= new LoginDialogSettings())
        {
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

        static LoginDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoginDialog), new FrameworkPropertyMetadata(typeof(LoginDialog)));
        }

        #endregion Constructor

        private RoutedEventHandler? negativeHandler = null;
        private KeyEventHandler? negativeKeyHandler = null;
        private RoutedEventHandler? affirmativeHandler = null;
        private KeyEventHandler? affirmativeKeyHandler = null;
        private KeyEventHandler? escapeKeyHandler = null;

        internal Task<LoginDialogData?> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                if (this.PART_TextBox is not null && string.IsNullOrEmpty(this.PART_TextBox.Text) && !this.ShouldHideUsername)
                {
                    this.PART_TextBox.Focus();
                }
                else
                {
                    if (this.PART_PasswordBox is not null)
                    {
                        this.PART_PasswordBox.Focus();
                    }

                }
            }));

            var tcs = new TaskCompletionSource<LoginDialogData?>();

            void CleanUpHandlers()
            {
                if (this.PART_TextBox is not null)
                {
                    this.PART_TextBox.KeyDown -= this.affirmativeKeyHandler;
                }

                if (this.PART_PasswordBox is not null)
                {
                    this.PART_PasswordBox.KeyDown -= this.affirmativeKeyHandler;
                }

                this.KeyDown -= this.escapeKeyHandler;

                if (this.PART_NegativeButton is not null)
                {
                    this.PART_NegativeButton.Click -= this.negativeHandler;
                }

                if (this.PART_AffirmativeButton is not null)
                {
                    this.PART_AffirmativeButton.Click -= this.affirmativeHandler;
                }

                if (this.PART_NegativeButton is not null)
                {
                    this.PART_NegativeButton.KeyDown -= this.negativeKeyHandler;
                }

                if (this.PART_AffirmativeButton is not null)
                {
                    this.PART_AffirmativeButton.KeyDown -= this.affirmativeKeyHandler;
                }

                this.cancellationTokenRegistration.Dispose();
            }

            this.cancellationTokenRegistration = this.DialogSettings
                                                     .CancellationToken
                                                     .Register(() =>
                                                     {
                                                         this.BeginInvoke(() =>
                                                         {
                                                             CleanUpHandlers();
                                                             tcs.TrySetResult(null!);
                                                         });
                                                     });

            this.escapeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(null!);
                }
            };

            this.negativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(null!);
                }
            };

            this.affirmativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();
                    tcs.TrySetResult(new LoginDialogData
                    {
                        Username = this.Username,
                        SecurePassword = this.PART_PasswordBox is not null ? this.PART_PasswordBox.SecurePassword : null,
                        ShouldRemember = this.RememberCheckBoxChecked
                    });
                }
            };

            this.negativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(null!);

                e.Handled = true;
            };

            this.affirmativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(new LoginDialogData
                {
                    Username = this.Username,
                    SecurePassword = this.PART_PasswordBox is not null ? this.PART_PasswordBox.SecurePassword : null,
                    ShouldRemember = this.RememberCheckBoxChecked
                });

                e.Handled = true;
            };
            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.KeyDown += this.negativeKeyHandler;
            }

            if (this.PART_AffirmativeButton is not null)
            {
                this.PART_AffirmativeButton.KeyDown += this.affirmativeKeyHandler;
            }

            if (this.PART_TextBox is not null)
            {
                this.PART_TextBox.KeyDown += this.affirmativeKeyHandler;
            }

            if (this.PART_PasswordBox is not null)
            {
                this.PART_PasswordBox.KeyDown += this.affirmativeKeyHandler;
            }

            this.KeyDown += this.escapeKeyHandler;

            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.Click += this.negativeHandler;
            }

            if (this.PART_AffirmativeButton is not null)
            {
                this.PART_AffirmativeButton.Click += this.affirmativeHandler;
            }

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            //base.OnLoaded();

            //this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            //this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            if (this.DialogSettings is LoginDialogSettings settings && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = this.FindResource("MahApps.Styles.PasswordBox.Win8") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    if (this.PART_PasswordBox is not null)
                    {
                        this.PART_PasswordBox.Style = win8MetroPasswordStyle;
                        // apply template again to fire the loaded event which is necessary for revealed password
                        this.PART_PasswordBox.ApplyTemplate();
                    }
                }
            }

            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            this.Icon = this.DialogSettings.Icon;
            this.IconTemplate = this.DialogSettings.IconTemplate;

            if (this.DialogSettings.ColorScheme == MetroDialogColorScheme.Accented)
            {
                if (this.PART_NegativeButton is not null)
                {
                    this.PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                }

                if (this.PART_TextBox is not null)
                {
                    this.PART_TextBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                }

                if (this.PART_PasswordBox is not null)
                {
                    this.PART_PasswordBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                }
            }
        }
    }
}