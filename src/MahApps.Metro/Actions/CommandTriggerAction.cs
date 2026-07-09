// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Actions
{
    /// <summary>
    /// This CommandTriggerAction can be used to bind any event on any FrameworkElement to an <see cref="ICommand" />.
    /// This trigger can only be attached to a FrameworkElement or a class deriving from FrameworkElement.
    /// 
    /// This class is inspired from Laurent Bugnion and his EventToCommand.
    /// <web>http://www.mvvmlight.net</web>
    /// <license> See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt </license>
    /// </summary>
    public class CommandTriggerAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command),
                                          typeof(ICommand),
                                          typeof(CommandTriggerAction),
                                          new PropertyMetadata(null, (s, e) => OnCommandChanged(s as CommandTriggerAction, e)));

        /// <summary>
        /// Gets or sets the command that this trigger is bound to.
        /// </summary>
        public ICommand? Command
        {
            get => (ICommand?)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CommandParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(nameof(CommandParameter),
                                          typeof(object),
                                          typeof(CommandTriggerAction),
                                          new PropertyMetadata(null,
                                                               (s, e) =>
                                                                   {
                                                                       var sender = s as CommandTriggerAction;
                                                                       if (sender?.AssociatedObject != null)
                                                                       {
                                                                           sender.EnableDisableElement();
                                                                       }
                                                                   }));

        /// <summary>
        /// Gets or sets an object that will be passed to the <see cref="Command" /> attached to this trigger.
        /// </summary>
        public object? CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Specifies whether the AssociatedObject should be passed to the bound RelayCommand.
        /// This happens only if the <see cref="CommandParameter"/> is not set.
        /// </summary>
        public bool PassAssociatedObjectToCommand { get; set; }

        public CommandTriggerAction()
        {
            this.PassAssociatedObjectToCommand = true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject is null || (this.AssociatedObject != null && !this.AssociatedObject.IsEnabled))
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
        }

        private static void OnCommandChanged(CommandTriggerAction? action, DependencyPropertyChangedEventArgs e)
        {
            if (action is null)
            {
                return;
            }

            if (e.OldValue is ICommand oldCommand)
            {
                oldCommand.CanExecuteChanged -= action.OnCommandCanExecuteChanged;
            }

            if (e.NewValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += action.OnCommandCanExecuteChanged;
            }

            action.EnableDisableElement();
        }

        protected virtual object? GetCommandParameter()
        {
            var parameter = this.CommandParameter;
            if (parameter is null && this.PassAssociatedObjectToCommand)
            {
                parameter = this.AssociatedObject;
            }

            return parameter;
        }

        private void EnableDisableElement()
        {
            if (this.AssociatedObject is null)
            {
                return;
            }

            var command = this.Command;
            this.AssociatedObject.SetCurrentValue(UIElement.IsEnabledProperty, BooleanBoxes.Box(command is null || command.CanExecute(this.GetCommandParameter())));
        }

        private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            this.EnableDisableElement();
        }
    }
}