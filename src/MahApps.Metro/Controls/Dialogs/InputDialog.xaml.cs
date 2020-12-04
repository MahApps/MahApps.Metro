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
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(InputDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Input), typeof(string), typeof(InputDialog), new PropertyMetadata(default(string)));

        public string Input
        {
            get { return (string)this.GetValue(InputProperty); }
            set { this.SetValue(InputProperty, value); }
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register(nameof(AffirmativeButtonText), typeof(string), typeof(InputDialog), new PropertyMetadata("OK"));

        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { this.SetValue(AffirmativeButtonTextProperty, value); }
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register(nameof(NegativeButtonText), typeof(string), typeof(InputDialog), new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        internal InputDialog()
            : this(null)
        {
        }

        internal InputDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal InputDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
        }

        internal Task<string> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();
                    this.PART_TextBox.Focus();
                }));

            var tcs = new TaskCompletionSource<string>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = () =>
                {
                    this.PART_TextBox.KeyDown -= affirmativeKeyHandler;

                    this.KeyDown -= escapeKeyHandler;

                    this.PART_NegativeButton.Click -= negativeHandler;
                    this.PART_AffirmativeButton.Click -= affirmativeHandler;

                    this.PART_NegativeButton.KeyDown -= negativeKeyHandler;
                    this.PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;

                    this.cancellationTokenRegistration.Dispose();
                };

            this.cancellationTokenRegistration = this.DialogSettings
                                                     .CancellationToken
                                                     .Register(() =>
                                                         {
                                                             this.BeginInvoke(() =>
                                                                 {
                                                                     cleanUpHandlers();
                                                                     tcs.TrySetResult(null);
                                                                 });
                                                         });

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

                        tcs.TrySetResult(this.Input);
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

                    tcs.TrySetResult(this.Input);

                    e.Handled = true;
                };

            this.PART_NegativeButton.KeyDown += negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.PART_TextBox.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            this.PART_NegativeButton.Click += negativeHandler;
            this.PART_AffirmativeButton.Click += affirmativeHandler;

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