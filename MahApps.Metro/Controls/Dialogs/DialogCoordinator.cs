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

        public Task<string> ShowInput(object context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (!DialogParticipation.IsRegistered(context))
                throw new InvalidOperationException(
                    "Context is not registered. Consider using DialogParticipation.Register in XAML to bind in the DataContext.");

            var association = DialogParticipation.GetAssociation(context);
            var metroWindow = Window.GetWindow(association) as MetroWindow;

            if (metroWindow == null)
                throw new InvalidOperationException("Control is not inside a MetroWindow.");

            return metroWindow.ShowInputAsync("From a VM", "This dialog was shown from a VM, without knoweldge of Window");
        }
    }

    /// <summary>
    /// Use the dialog coordinator to help you interfact with dialogs from a view model.
    /// </summary>
    public interface IDialogCoordinator
    {
        /// <summary>
        /// Shows the input dialog.
        /// </summary>
        /// <param name="context">Typically this should be the view model, which you register in XAML using <see cref="DialogParticipation.Register"/>.</param>
        /// <returns></returns>
        Task<string> ShowInput(object context);
    }
}