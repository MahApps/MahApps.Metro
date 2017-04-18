namespace MahApps.Metro.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// A class for manipulating an open ProgressDialog.
    /// </summary>
    public class ProgressDialogController
    {
        private ProgressDialog WrappedDialog { get; }

        private Func<Task> CloseCallback { get; }

        /// <summary>
        /// This event is raised when the associated <see cref="ProgressDialog"/> was closed programmatically.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// This event is raised when the associated <see cref="ProgressDialog"/> was cancelled by the user.
        /// </summary>
        public event EventHandler Canceled;

        /// <summary>
        /// Gets if the Cancel button has been pressed.
        /// </summary>        
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Gets if the wrapped ProgressDialog is open.
        /// </summary>        
        public bool IsOpen { get; private set; }

        internal ProgressDialogController(ProgressDialog dialog, Func<Task> closeCallBack)
        {
            this.WrappedDialog = dialog;
            this.CloseCallback = closeCallBack;

            this.IsOpen = dialog.IsVisible;

            this.WrappedDialog.Invoke(() => { this.WrappedDialog.PART_NegativeButton.Click += this.PART_NegativeButton_Click; });

            dialog.CancellationToken.Register(() => { this.PART_NegativeButton_Click(null, new RoutedEventArgs()); });
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
                {
                    this.IsCanceled = true;
                    this.Canceled?.Invoke(this, EventArgs.Empty);
                    this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
                };
            this.WrappedDialog.Invoke(action);
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.SetIndeterminate());
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.IsCancelable = value);
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            Action action = () =>
                {
                    if (value < this.WrappedDialog.Minimum || value > this.WrappedDialog.Maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
                    this.WrappedDialog.ProgressValue = value;
                };
            this.WrappedDialog.Invoke(action);
        }

        /// <summary>
        ///  Gets/Sets the minimum restriction of the progress Value property
        /// </summary>
        public double Minimum
        {
            get { return this.WrappedDialog.Invoke(() => this.WrappedDialog.Minimum); }
            set { this.WrappedDialog.Invoke(() => this.WrappedDialog.Minimum = value); }
        }

        /// <summary>
        ///  Gets/Sets the maximum restriction of the progress Value property
        /// </summary>
        public double Maximum
        {
            get { return this.WrappedDialog.Invoke(() => this.WrappedDialog.Maximum); }
            set { this.WrappedDialog.Invoke(() => this.WrappedDialog.Maximum = value); }
        }

        /// <summary>
        /// Sets the dialog's message content.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.Message = message);
        }

        /// <summary>
        /// Sets the dialog's title.
        /// </summary>
        /// <param name="title">The title to be set.</param>
        public void SetTitle(string title)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.Title = title);
        }

        /// <summary>
        /// Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public Task CloseAsync()
        {
            Action action = () =>
                {
                    if (!this.WrappedDialog.IsVisible)
                    {
                        throw new InvalidOperationException("Dialog isn't visible to close");
                    }
                    this.WrappedDialog.Dispatcher.VerifyAccess();
                    this.WrappedDialog.PART_NegativeButton.Click -= this.PART_NegativeButton_Click;
                };

            this.WrappedDialog.Invoke(action);

            return this.CloseCallback()
                       .ContinueWith(_ => this.WrappedDialog.Invoke(() =>
                                                                        {
                                                                            this.IsOpen = false;
                                                                            this.Closed?.Invoke(this, EventArgs.Empty);
                                                                        }));
        }
    }
}