using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    public class DialogCoordinator : IDialogCoordinator
    {
        /// <summary>
        /// Gets the default instance if the dialog coordinator, which can be injected into a view model.
        /// </summary>
        public static readonly IDialogCoordinator Instance = new DialogCoordinator();

        public Task<string> ShowInputAsync(object context, string title, string message, MetroDialogSettings metroDialogSettings = null)
        {            
            return ExecuteShowMessageDialog(context, window => window.ShowInputAsync(title, message, metroDialogSettings));
        }

        public Task<LoginDialogData> ShowLoginAsync(object context, string title, string message, LoginDialogSettings settings = null)
        {
            return ExecuteShowMessageDialog(context, window => window.ShowLoginAsync(title, message, settings));
        }

        public Task<MessageDialogResult> ShowMessageAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return ExecuteShowMessageDialog(context, window => window.ShowMessageAsync(title, message, style, settings));
        }

        public Task<ProgressDialogController> ShowProgressAsync(object context, string title, string message,
            bool isCancelable = false, MetroDialogSettings settings = null)
        {
            return ExecuteShowMessageDialog(context, window => window.ShowProgressAsync(title, message, isCancelable, settings));
        }

        public Task ShowMetroDialogAsync(object context, BaseMetroDialog dialog,
            MetroDialogSettings settings = null)
        {
            return ExecuteShowMessageDialog(context, window => window.ShowMetroDialogAsync(dialog, settings));
        }

        public Task HideMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return ExecuteShowMessageDialog(context, window => window.HideMetroDialogAsync(dialog, settings));
        }

        public Task<TDialog> GetCurrentDialogAsync<TDialog>(object context) where TDialog : BaseMetroDialog
        {
            return ExecuteShowMessageDialog(context, window => window.GetCurrentDialogAsync<TDialog>());
        }

        private static Task ExecuteShowMessageDialog(object context, Func<MetroWindow, Task> messageDialogFunc)
        {
            var metroWindow = GetMetroWindow(context);
            if (metroWindow.CheckAccess())
            {
                return messageDialogFunc(metroWindow);
            }
            return metroWindow.Dispatcher.Invoke(new Func<Task>(() => messageDialogFunc(metroWindow))) as Task;
        }

        private static Task<TResult> ExecuteShowMessageDialog<TResult>(object context, Func<MetroWindow, Task<TResult>> messageDialogFunc)
        {
            var metroWindow = GetMetroWindow(context);
            if (metroWindow.CheckAccess())
            {
                return messageDialogFunc(metroWindow);
            }
            return metroWindow.Dispatcher.Invoke(new Func<Task<TResult>>(() => messageDialogFunc(metroWindow))) as Task<TResult>;
        }

        private static MetroWindow GetMetroWindow(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!DialogParticipation.IsRegistered(context))
            {
                throw new InvalidOperationException("Context is not registered. Consider using DialogParticipation.Register in XAML to bind in the DataContext.");
            }

            var association = DialogParticipation.GetAssociation(context);

            MetroWindow metroWindow;

            if (Thread.CurrentThread.IsBackground)
            {
                metroWindow = association.Dispatcher.Invoke(new Func<Window>(() => Window.GetWindow(association))) as MetroWindow;
            }
            else
            {
                metroWindow = Window.GetWindow(association) as MetroWindow;
            }

            if (metroWindow == null)
            {
                throw new InvalidOperationException("Control is not inside a MetroWindow.");
            }
            return metroWindow;
        }
    }
}
