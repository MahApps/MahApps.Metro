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
        public bool IsOpen { get; private set;}

        internal ProgressDialogController(ProgressDialog dialog, Func<Task> closeCallBack)
        {
            WrappedDialog = dialog;
            CloseCallback = closeCallBack;

            IsOpen = dialog.IsVisible;

            InvokeAction(() => {
                WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click;
            });

            dialog.CancellationToken.Register(() =>
            {
                PART_NegativeButton_Click(null, new RoutedEventArgs());
            });
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () => {
                IsCanceled = true;
                var handler = Canceled;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
                WrappedDialog.PART_NegativeButton.IsEnabled = false;
            };

            InvokeAction(action);
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            InvokeAction(() => WrappedDialog.SetIndeterminate());
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            InvokeAction(() => WrappedDialog.IsCancelable = value);
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            Action action = () => {
                if (value < WrappedDialog.Minimum || value > WrappedDialog.Maximum)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                WrappedDialog.ProgressValue = value;
            };

            InvokeAction(action);
        }

        /// <summary>
        ///  Gets/Sets the minimum restriction of the progress Value property
        /// </summary>
        public double Minimum
        {
            get { return InvokeFunc(() => WrappedDialog.Minimum); }
            set { InvokeAction(() => WrappedDialog.Minimum = value); }
        }

        /// <summary>
        ///  Gets/Sets the maximum restriction of the progress Value property
        /// </summary>
        public double Maximum
        {
            get { return InvokeFunc(() => WrappedDialog.Maximum); }
            set { InvokeAction(() => WrappedDialog.Maximum = value); }
        }

        /// <summary>
        /// Sets the dialog's message content.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            InvokeAction(() => WrappedDialog.Message = message);
        }

        /// <summary>
        /// Sets the dialog's title.
        /// </summary>
        /// <param name="title">The title to be set.</param>
        public void SetTitle(string title)
        {
            InvokeAction(() => WrappedDialog.Title = title);
        }

       

        /// <summary>
        /// Begins an operation to close the ProgressDialog.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public Task CloseAsync()
        {
            Action action = () => {
                if (!WrappedDialog.IsVisible)
                {
                    throw new InvalidOperationException("Dialog isn't visible to close");
                }
                WrappedDialog.Dispatcher.VerifyAccess();
                WrappedDialog.PART_NegativeButton.Click -= PART_NegativeButton_Click;
            };

            InvokeAction(action);

            return CloseCallback().ContinueWith(_ => InvokeAction(new Action(() => {
                IsOpen = false;

                var handler = Closed;
                if (handler != null) {
                    handler(this, EventArgs.Empty);
                }
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
                return (double)WrappedDialog.Dispatcher.Invoke(new Func<double>(getValueFunc));
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
                WrappedDialog.Dispatcher.Invoke(setValueAction);
            }
        }
    }
}