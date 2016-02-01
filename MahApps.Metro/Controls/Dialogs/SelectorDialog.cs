using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public class SelectorDialogSettings : MetroDialogSettings
    {
        public List<string> ItemList { get; set; }

        public SelectorDialogSettings()
        {
            ItemList = new List<string>();
        }
    }

    public partial class SelectorDialog : BaseMetroDialog
    {
        internal SelectorDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }
        internal SelectorDialog(MetroWindow parentWindow, SelectorDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
            Input = settings.ItemList;
        }

        internal Task<string> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                PART_TextBox.Focus();
            }));

            var tcs = new TaskCompletionSource<string>();

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

            cleanUpHandlers = () =>
            {
                PART_TextBox.KeyDown -= affirmativeKeyHandler;

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

                    tcs.TrySetResult(SelectedItem);
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

                tcs.TrySetResult(SelectedItem);

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox.SetResourceReference(ControlsHelper.FocusBorderBrushProperty, "TextBoxFocusBorderBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SelectorDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(List<string>), typeof(SelectorDialog), new PropertyMetadata(new List<string>()));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(SelectorDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(SelectorDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(SelectorDialog), new PropertyMetadata("Cancel"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public List<string> Input
        {
            get { return (List<string>)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
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
    }
}
