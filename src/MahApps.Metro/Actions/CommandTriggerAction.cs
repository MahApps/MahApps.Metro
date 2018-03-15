using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

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
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
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
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
        }

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
        }

        private static void OnCommandChanged(CommandTriggerAction action, DependencyPropertyChangedEventArgs e)
        {
            if (action == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= action.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += action.OnCommandCanExecuteChanged;
            }

            action.EnableDisableElement();
        }

        protected virtual object GetCommandParameter()
        {
            return this.CommandParameter ?? this.AssociatedObject;
        }

        private void EnableDisableElement()
        {
            if (this.AssociatedObject == null)
            {
                return;
            }

            var command = this.Command;
            this.AssociatedObject.SetCurrentValue(UIElement.IsEnabledProperty, command == null || command.CanExecute(this.GetCommandParameter()));
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }
    }
}