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
        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        /// <summary>Identifies the <see cref="IsCancelable"/> dependency property.</summary>
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register(nameof(IsCancelable), typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), (s, e) => { ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden; }));

        public bool IsCancelable
        {
            get { return (bool)this.GetValue(IsCancelableProperty); }
            set { this.SetValue(IsCancelableProperty, value); }
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register(nameof(NegativeButtonText), typeof(string), typeof(ProgressDialog), new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        /// <summary>Identifies the <see cref="ProgressBarForeground"/> dependency property.</summary>
        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register(nameof(ProgressBarForeground), typeof(Brush), typeof(ProgressDialog), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ProgressBarForeground
        {
            get { return (Brush)this.GetValue(ProgressBarForegroundProperty); }
            set { this.SetValue(ProgressBarForegroundProperty, value); }
        }

        internal ProgressDialog()
            : this(null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
        }

        protected override void OnLoaded()
        {
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;
            this.SetResourceReference(ProgressBarForegroundProperty, this.DialogSettings.ColorScheme == MetroDialogColorScheme.Theme ? "MahApps.Brushes.Accent" : "MahApps.Brushes.ThemeForeground");
        }

        internal CancellationToken CancellationToken => this.DialogSettings.CancellationToken;

        internal double Minimum
        {
            get { return this.PART_ProgressBar.Minimum; }
            set { this.PART_ProgressBar.Minimum = value; }
        }

        internal double Maximum
        {
            get { return this.PART_ProgressBar.Maximum; }
            set { this.PART_ProgressBar.Maximum = value; }
        }

        internal double ProgressValue
        {
            get { return this.PART_ProgressBar.Value; }
            set
            {
                this.PART_ProgressBar.IsIndeterminate = false;
                this.PART_ProgressBar.Value = value;
                this.PART_ProgressBar.ApplyTemplate();
            }
        }

        internal void SetIndeterminate()
        {
            this.PART_ProgressBar.IsIndeterminate = true;
        }
    }
}