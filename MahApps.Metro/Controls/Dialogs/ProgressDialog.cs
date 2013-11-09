using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class ProgressDialog : BaseMetroDialog
    {
        //private const string PART_AffirmativeButton = "PART_AffirmativeButton";
        //private const string PART_NegativeButton = "PART_NegativeButton";

        //private Button AffirmativeButton = null;
        //private Button NegativeButton = null;

        //static MessageDialog()
        //{
        //    //DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        //}
        internal ProgressDialog(MetroWindow parentWindow)
            : base(parentWindow)
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

    }

    public class ProgressDialogController
    {
        //No spiritdead, you can't change this.
        private ProgressDialog WrappedDialog { get; set; }
        private Action CloseCallback { get; set; }

        public bool IsOpen { get; private set; }

        internal ProgressDialogController(ProgressDialog dialog, Action closeCallBack)
        {
            WrappedDialog = dialog;
            CloseCallback = closeCallBack;

            IsOpen = dialog.IsVisible;
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
        }

        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0) throw new ArgumentOutOfRangeException("value");

            WrappedDialog.PART_ProgressBar.IsIndeterminate = false;

            WrappedDialog.PART_ProgressBar.Value = value;

            WrappedDialog.PART_ProgressBar.Maximum = 1.0;

            WrappedDialog.PART_ProgressBar.ApplyTemplate();
        }

        public void SetMessage(string message)
        {
            WrappedDialog.Message = message;
        }

        public void Close()
        {
            if (!IsOpen) throw new InvalidOperationException();

            WrappedDialog.Dispatcher.VerifyAccess();

            CloseCallback();

            IsOpen = false;
        }
    }
}
