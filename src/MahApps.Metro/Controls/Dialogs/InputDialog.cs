// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    [TemplatePart(Name = nameof(PART_AffirmativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_NegativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_TextBox), Type = typeof(TextBox))]
    public class InputDialog : BaseMetroDialog, INotifyDataErrorInfo
    {
        private readonly TaskCompletionSource<string?> tcs = new();
        private CancellationTokenRegistration? cancellationTokenRegistration;

        #region Controls

        // ReSharper disable InconsistentNaming
        private Button? PART_AffirmativeButton;
        private Button? PART_NegativeButton;
        private TextBox? PART_TextBox;
        // ReSharper restore InconsistentNaming

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_AffirmativeButton = this.GetTemplateChild(nameof(this.PART_AffirmativeButton)) as Button;
            this.PART_NegativeButton = this.GetTemplateChild(nameof(this.PART_NegativeButton)) as Button;
            this.PART_TextBox = this.GetTemplateChild(nameof(this.PART_TextBox)) as TextBox;
        }

        #endregion Controls

        #region DependencyProperties

        /// <summary>Identifies the <see cref="DefaultButtonFocus"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultButtonFocusProperty
            = DependencyProperty.Register(nameof(DefaultButtonFocus),
                                          typeof(MessageDialogResult),
                                          typeof(InputDialog),
                                          new PropertyMetadata(MessageDialogResult.Affirmative));

        /// <summary>
        /// Gets or sets which button is the one a press of return stands for, and which is marked as
        /// such. The caret waits in the field either way, since that is what there is to fill in.
        /// </summary>
        public MessageDialogResult DefaultButtonFocus
        {
            get => (MessageDialogResult)this.GetValue(DefaultButtonFocusProperty);
            set => this.SetValue(DefaultButtonFocusProperty, value);
        }

        /// <summary>
        /// Takes the button out of the settings, falling back to the one that carries on where the
        /// settings name a button this dialog does not have.
        /// </summary>
        private void ApplyDefaultButtonFocus()
        {
            // told nothing, it is the one that carries on, which is what this dialog has always marked
            var defaultButtonFocus = this.DialogSettings.DefaultButtonFocus ?? MessageDialogResult.Affirmative;

            if (defaultButtonFocus != MessageDialogResult.Affirmative && defaultButtonFocus != MessageDialogResult.Negative)
            {
                defaultButtonFocus = MessageDialogResult.Affirmative;
            }

            this.SetCurrentValue(DefaultButtonFocusProperty, defaultButtonFocus);
        }

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
                                          new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnInputChanged));

        private static void OnInputChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // what was said about a line is about that line, so it goes as soon as it is typed over
            (dependencyObject as InputDialog)?.Complain(null);
        }

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

        #endregion DependencyProperties

        #region Constructor

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
            this.SetCurrentValue(AffirmativeButtonTextProperty, this.DialogSettings.AffirmativeButtonText);
            this.SetCurrentValue(NegativeButtonTextProperty, this.DialogSettings.NegativeButtonText);

            this.ApplyDefaultButtonFocus();
        }

        static InputDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputDialog), new FrameworkPropertyMetadata(typeof(InputDialog)));
        }

        #endregion Constructor

        internal async Task<string?> WaitForButtonPressAsync()
        {
            await this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();
                    this.PART_TextBox?.Focus();
                }));

            this.SetUpHandlers();

            return await this.tcs.Task.ConfigureAwait(false);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
            {
                this.Finish(null);

                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                if (ReferenceEquals(sender, this.PART_NegativeButton))
                {
                    this.Finish(null);
                }
                else
                {
                    this.FinishWithTheInput();
                }

                e.Handled = true;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, this.PART_AffirmativeButton))
            {
                this.FinishWithTheInput();
            }
            else
            {
                this.Finish(null);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Leaves the dialog with what was typed, unless the settings carry a check and the check has
        /// something to say about it. What it says goes on the field and the dialog stays where it is.
        /// </summary>
        private void FinishWithTheInput()
        {
            var whatIsWrong = (this.DialogSettings as InputDialogSettings)?.ValidateInput?.Invoke(this.Input);

            if (string.IsNullOrEmpty(whatIsWrong))
            {
                this.Finish(this.Input);
                return;
            }

            this.Complain(whatIsWrong);

            // the theme shows what is wrong with a field while the caret is in it, and the caret is
            // on the button that was just pressed
            this.PART_TextBox?.Focus();
        }

        private void Finish(string? result)
        {
            this.Complain(null);
            this.CleanUpHandlers();

            this.tcs.TrySetResult(result!);
        }

        #region What the check had to say

        private string? complaint;

        /// <summary>
        /// Puts what the check said on <see cref="Input"/>, or takes it off again with
        /// <see langword="null"/>. The field reads it from here, the way it reads any other error a
        /// binding source reports.
        /// </summary>
        private void Complain(string? about)
        {
            if (this.complaint == about)
            {
                return;
            }

            this.complaint = about;
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(this.Input)));
        }

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <inheritdoc />
        public bool HasErrors => this.complaint is not null;

        /// <inheritdoc />
        public IEnumerable GetErrors(string? propertyName)
        {
            return this.complaint is not null && propertyName == nameof(this.Input)
                ? new[] { this.complaint }
                : Array.Empty<string>();
        }

        #endregion What the check had to say

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

            this.KeyDown -= this.OnKeyDownHandler;

            this.cancellationTokenRegistration?.Dispose();
        }
    }
}