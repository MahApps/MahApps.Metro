// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public partial class InputDialog : BaseMetroDialog
    {
        private CancellationTokenRegistration cancellationTokenRegistration;

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message),
                                          typeof(string),
                                          typeof(InputDialog),
                                          new PropertyMetadata(default(string)));

        public string? Message
        {
            get => (string?)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty
            = DependencyProperty.Register(nameof(Input),
                                          typeof(string),
                                          typeof(InputDialog),
                                          new PropertyMetadata(default(string)));

        public string? Input
        {
            get => (string?)this.GetValue(InputProperty);
            set => this.SetValue(InputProperty, value);
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty
            = DependencyProperty.Register(nameof(AffirmativeButtonText),
                                          typeof(string),
                                          typeof(InputDialog),
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
                                          typeof(InputDialog),
                                          new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)this.GetValue(NegativeButtonTextProperty);
            set => this.SetValue(NegativeButtonTextProperty, value);
        }

        internal InputDialog()
            : this(null)
        {
        }

        internal InputDialog(MetroWindow? parentWindow)
            : this(parentWindow, null)
        {
        }

        internal InputDialog(MetroWindow? parentWindow, MetroDialogSettings? settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
        }

        private RoutedEventHandler? negativeHandler = null;
        private KeyEventHandler? negativeKeyHandler = null;
        private RoutedEventHandler? affirmativeHandler = null;
        private KeyEventHandler? affirmativeKeyHandler = null;
        private KeyEventHandler? escapeKeyHandler = null;

        internal Task<string?> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();
                    this.PART_TextBox.Focus();
                }));

            var tcs = new TaskCompletionSource<string?>();

            void CleanUpHandlers()
            {
                this.PART_TextBox.KeyDown -= this.affirmativeKeyHandler;

                this.KeyDown -= this.escapeKeyHandler;

                this.PART_NegativeButton.Click -= this.negativeHandler;
                this.PART_AffirmativeButton.Click -= this.affirmativeHandler;

                this.PART_NegativeButton.KeyDown -= this.negativeKeyHandler;
                this.PART_AffirmativeButton.KeyDown -= this.affirmativeKeyHandler;

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

                        tcs.TrySetResult(this.Input!);
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

                    tcs.TrySetResult(this.Input!);

                    e.Handled = true;
                };

            this.PART_NegativeButton.KeyDown += this.negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += this.affirmativeKeyHandler;

            this.PART_TextBox.KeyDown += this.affirmativeKeyHandler;

            this.KeyDown += this.escapeKeyHandler;

            this.PART_NegativeButton.Click += this.negativeHandler;
            this.PART_AffirmativeButton.Click += this.affirmativeHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                    this.PART_TextBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                    this.PART_TextBox.SetResourceReference(ControlsHelper.FocusBorderBrushProperty, "MahApps.Brushes.TextBox.Border.Focus");
                    break;
            }
        }
    }
}