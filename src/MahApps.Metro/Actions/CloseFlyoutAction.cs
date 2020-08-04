// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using MahApps.Metro.ValueBoxes;

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
                this.AssociatedFlyout?.SetCurrentValue(Flyout.IsOpenProperty, BooleanBoxes.FalseBox);
            }
        }

        protected override object GetCommandParameter()
        {
            return this.CommandParameter ?? this.AssociatedFlyout;
        }
    }
}