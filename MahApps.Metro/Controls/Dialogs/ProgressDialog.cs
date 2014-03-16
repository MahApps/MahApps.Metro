﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        internal ProgressDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();

            if (parentWindow.MetroDialogOptions.ColorScheme == MetroDialogColorScheme.Theme)
            {
                try
                {
                    ProgressBarForeground = parentWindow.FindResource("AccentColorBrush") as Brush;
                }
                catch (Exception) { }
            }
            else
                ProgressBarForeground = Brushes.White;
        }
        internal ProgressDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
            
        }

        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(ProgressDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register("IsCancelable", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), new PropertyChangedCallback((s, e) =>
            {
                ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
            })));
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
    }

    /// <summary>
    /// A class for manipulating an open ProgressDialog.
    /// </summary>
    public class ProgressDialogController
    {
        //No spiritdead, you can't change this.
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

            WrappedDialog.PART_NegativeButton.Dispatcher.Invoke(new Action(() =>
                {
                    WrappedDialog.PART_NegativeButton.Click += PART_NegativeButton_Click;
                }));
        }

        void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                IsCanceled = true;

                WrappedDialog.PART_NegativeButton.IsEnabled = false;
            }
            WrappedDialog.Dispatcher.Invoke(new Action(() =>
            {
                IsCanceled = true;

                WrappedDialog.PART_NegativeButton.IsEnabled = false;
            }));

            //Close();
        }

        /// <summary>
        /// Sets the ProgressBar's IsIndeterminate to true. To set it to false, call SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
                WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    WrappedDialog.PART_ProgressBar.IsIndeterminate = true;
                                                }));
            }
        }

        /// <summary>
        /// Sets if the Cancel button is visible.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
                WrappedDialog.IsCancelable = value;
            else
                WrappedDialog.Dispatcher.Invoke(new Action(() =>
                    {
                        WrappedDialog.IsCancelable = value;
                    }));
        }

        /// <summary>
        /// Sets the dialog's progress bar value and sets IsIndeterminate to false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            if (value < 0.0 || value > 1.0) throw new ArgumentOutOfRangeException("value");

            Action action = () =>
                       {
                           WrappedDialog.PART_ProgressBar.IsIndeterminate = false;
                           WrappedDialog.PART_ProgressBar.Value = value;
                           WrappedDialog.PART_ProgressBar.Maximum = 1.0;
                           WrappedDialog.PART_ProgressBar.ApplyTemplate();
                       };

            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(action);
            }

      
        }

        /// <summary>
        /// Sets the dialog's message content.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                WrappedDialog.Message = message;
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(new Action(() => { WrappedDialog.Message = message; }));
            }
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
            Action action = () =>
                       {
                           if (!IsOpen) throw new InvalidOperationException();
                           WrappedDialog.Dispatcher.VerifyAccess();
                           WrappedDialog.PART_NegativeButton.Click -= PART_NegativeButton_Click;
                       };

            if (WrappedDialog.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                WrappedDialog.Dispatcher.Invoke(action);
              
            }

            return CloseCallback().ContinueWith(x => WrappedDialog.Dispatcher.Invoke(new Action(() =>
                                                                                     {
                                                                                         IsOpen = false;
                                                                                     })));
        }
    }
}
