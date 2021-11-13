using System;
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
    public partial class InputDialog : BaseMetroDialog
    {
        private CancellationTokenRegistration cancellationTokenRegistration;

        #region Controls

        private Button? PART_AffirmativeButton;
        private Button? PART_NegativeButton;
        private TextBox? PART_TextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_AffirmativeButton = this.GetTemplateChild(nameof(this.PART_AffirmativeButton)) as Button;
            this.PART_NegativeButton = this.GetTemplateChild(nameof(this.PART_NegativeButton)) as Button;
            this.PART_TextBox = this.GetTemplateChild(nameof(this.PART_TextBox)) as TextBox;
        }

        #endregion Controls

        #region DependencyProperties

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

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(InputDialog),
                                          new PropertyMetadata());

        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(InputDialog));

        public DataTemplate? IconTemplate
        {
            get => (DataTemplate?)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        #endregion DependencyProperties

        #region Constructor

        internal InputDialog() : this(null)
        { }

        internal InputDialog(MetroWindow? parentWindow) : this(parentWindow, null)
        { }

        internal InputDialog(MetroWindow? parentWindow, MetroDialogSettings? settings) : base(parentWindow, settings)
        { }

        static InputDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputDialog), new FrameworkPropertyMetadata(typeof(InputDialog)));
        }

        #endregion Constructor

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
                if (this.PART_TextBox is not null)
                {
                    this.PART_TextBox.Focus();
                }
            }));

            var tcs = new TaskCompletionSource<string?>();

            void CleanUpHandlers()
            {
                if (this.PART_TextBox is not null)
                {
                    this.PART_TextBox.KeyDown -= this.affirmativeKeyHandler;
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
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            this.Icon = this.DialogSettings.Icon;
            this.IconTemplate = this.DialogSettings.IconTemplate;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    if (this.PART_NegativeButton is not null)
                    {
                        this.PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                    }

                    if (this.PART_TextBox is not null)
                    {
                        this.PART_TextBox.SetResourceReference(ForegroundProperty, "MahApps.Brushes.ThemeForeground");
                        this.PART_TextBox.SetResourceReference(ControlsHelper.FocusBorderBrushProperty, "MahApps.Brushes.TextBox.Border.Focus");
                    }

                    break;
            }
        }
    }
}