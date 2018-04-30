using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class CloseFlyoutAction : CommandTriggerAction
    {
        private Flyout associatedFlyout;

        private Flyout AssociatedFlyout => this.associatedFlyout ?? (this.associatedFlyout = this.AssociatedObject.TryFindParent<Flyout>());

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null || (this.AssociatedObject != null && !this.AssociatedObject.IsEnabled))
            {
                return;
            }

            var command = this.Command;
            if (command != null)
            {
                var commandParameter = this.GetCommandParameter();
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
            else
            {
                this.AssociatedFlyout?.SetCurrentValue(Flyout.IsOpenProperty, false);
            }
        }

        protected override object GetCommandParameter()
        {
            return this.CommandParameter ?? this.AssociatedFlyout;
        }
    }
}