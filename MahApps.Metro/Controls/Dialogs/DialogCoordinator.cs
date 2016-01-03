using System;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    public class DialogCoordinator : IDialogCoordinator
    {
        /// <summary>
        /// Gets the default instance if the dialog coordinator, which can be injected into a view model.
        /// </summary>
        public static readonly DialogCoordinator Instance = new DialogCoordinator();

        public Task<string> ShowInputAsync(object context, string title, string message, MetroDialogSettings metroDialogSettings = null)
        {            
            var metroWindow = GetMetroWindow(context);

            return metroWindow.ShowInputAsync(title, message, metroDialogSettings);
        }

        public Task<LoginDialogData> ShowLoginAsync(object context, string title, string message, LoginDialogSettings settings = null)
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.ShowLoginAsync(title, message, settings);
        }

        public Task<MessageDialogResult> ShowMessageAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.ShowMessageAsync(title, message, style, settings);
        }

        public Task<ProgressDialogController> ShowProgressAsync(object context, string title, string message,
            bool isCancelable = false, MetroDialogSettings settings = null)
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.ShowProgressAsync(title, message, isCancelable, settings);
        }

        public Task ShowMetroDialogAsync(object context, BaseMetroDialog dialog,
            MetroDialogSettings settings = null)
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.ShowMetroDialogAsync(dialog, settings);
        }

        public Task HideMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.HideMetroDialogAsync(dialog, settings);
        }

        public Task<TDialog> GetCurrentDialogAsync<TDialog>(object context) where TDialog : BaseMetroDialog
        {
            var metroWindow = GetMetroWindow(context);

            return metroWindow.GetCurrentDialogAsync<TDialog>();
        }

        private static MetroWindow GetMetroWindow(object context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!DialogParticipation.IsRegistered(context))
                throw new InvalidOperationException(
                    "Context is not registered. Consider using DialogParticipation.Register in XAML to bind in the DataContext.");

            var association = DialogParticipation.GetAssociation(context);
            var metroWindow = Window.GetWindow(association) as MetroWindow;

            if (metroWindow == null)
                throw new InvalidOperationException("Control is not inside a MetroWindow.");
            return metroWindow;
        }
    }
}
