using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class ProgressDialog : BaseMetroDialog
    {
        internal ProgressDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();

            if (parentWindow.MetroDialogOptions.ColorScheme == MetroDialogColorScheme.Theme)
            {
                try
                {
                    ProgressBarForeground = ThemeManager.GetResourceFromAppStyle(parentWindow, "AccentColorBrush") as Brush;
                }
                catch (Exception) { }
            }

            else
            {
                ProgressBarForeground = Brushes.White;
            }
        }

        internal ProgressDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        { }

        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(ProgressDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register("IsCancelable", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), (s, e) =>
        {
            ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
        }));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ProgressDialog), new PropertyMetadata("Cancel"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public bool IsCancelable
        {
            get { return (bool)GetValue(IsCancelableProperty); }
            set { SetValue(IsCancelableProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public Brush ProgressBarForeground
        {
            get { return (Brush)GetValue(ProgressBarForegroundProperty); }
            set { SetValue(ProgressBarForegroundProperty, value); }
        }

        internal CancellationToken CancellationToken
        {
            get { return DialogSettings.CancellationToken; }
        }

        internal double Minimum
        {
            get { return PART_ProgressBar.Minimum; }
            set { PART_ProgressBar.Minimum = value; }
        }

        internal double Maximum
        {
            get { return PART_ProgressBar.Maximum; }
            set { PART_ProgressBar.Maximum = value; }
        }

        internal double ProgressValue
        {
            get { return PART_ProgressBar.Value; }
            set
            {
                PART_ProgressBar.IsIndeterminate = false;
                PART_ProgressBar.Value = value;
                PART_ProgressBar.ApplyTemplate();
            }
        }

        internal void SetIndeterminate()
        {
            PART_ProgressBar.IsIndeterminate = true;
        }
    }
}
