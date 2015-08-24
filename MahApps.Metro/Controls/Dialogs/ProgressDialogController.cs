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
        private ProgressDialog WrappedDialog { get; set; }
        private Func<Task> CloseCallback { get; set; }

        /// <summary>
        /// Gets if the wrapped ProgressDialog is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        internal ProgressDialogController(ProgressDialog dialog, Func<Task> closeCallBack)
        {
            WrappedDialog = dialog;
            CloseCallback = closeCallBack;

            IsOpen = dialog.IsVisible;

            WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(new Action(() => {
                WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click;
            }));

            dialog.CancellationToken.Register(() =>
            {
                PART_NegativeButton_Click(null, new RoutedEventArgs());
            });
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () => {
                IsCanceled = true;
                WrappedDialog.PART_NegativeButton.IsEnabled = false;
            };

            this.InvokeAction(action);
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            this.InvokeAction(() => WrappedDialog.PART_ProgressBar.IsIndeterminate = true);
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            this.InvokeAction(() => WrappedDialog.IsCancelable = value);
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            Action action = () => {
                if (value < WrappedDialog.PART_ProgressBar.Minimum || value > WrappedDialog.PART_ProgressBar.Maximum)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                WrappedDialog.PART_ProgressBar.IsIndeterminate = false;
                WrappedDialog.PART_ProgressBar.Value = value;
                WrappedDialog.PART_ProgressBar.ApplyTemplate();
            };

            this.InvokeAction(action);
        }

        /// <summary>
        ///  Gets/Sets the minimum restriction of the progress Value property
        /// </summary>
        public double Minimum
        {
            get { return this.InvokeFunc(() => WrappedDialog.PART_ProgressBar.Minimum); }
            set { this.InvokeAction(() => WrappedDialog.PART_ProgressBar.Minimum = value); }
        }

        /// <summary>
        ///  Gets/Sets the maximum restriction of the progress Value property
        /// </summary>
        public double Maximum
        {
            get { return this.InvokeFunc(() => WrappedDialog.PART_ProgressBar.Maximum); }
            set { this.InvokeAction(() => WrappedDialog.PART_ProgressBar.Maximum = value); }
        }

        /// <summary>
        /// Sets the dialog's message content.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            this.InvokeAction(() => WrappedDialog.Message = message);
        }

        /// <summary>
        /// Sets the dialog's title.
        /// </summary>
        /// <param name="title">The title to be set.</param>
        public void SetTitle(string title)
        {
            this.InvokeAction(() => WrappedDialog.Title = title);
        }

        /// <summary>
        /// Gets if the Cancel button has been pressed.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public Task CloseAsync()
        {
            Action action = () => {
                if (!IsOpen)
                {
                    throw new InvalidOperationException();
                }
                WrappedDialog.Dispatcher.VerifyAccess();
                WrappedDialog.PART_NegativeButton.Click -= PART_NegativeButton_Click;
            };

            this.InvokeAction(action);

            return CloseCallback().ContinueWith(x => WrappedDialog.Dispatcher.Invoke(new Action(() => {
                IsOpen = false;
            })));
        }

        private double InvokeFunc(Func<double> getValueFunc)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                return getValueFunc();
            }
            else
            {
                return (double)this.WrappedDialog.Dispatcher.Invoke(new Func<double>(getValueFunc));
            }
        }

        private void InvokeAction(Action setValueAction)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                setValueAction();
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(setValueAction));
            }
        }
    }
}