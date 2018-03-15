#if NET4
using System;
#endif
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    internal static class CommandHelpers
    {
#if NET4
        internal static readonly DependencyProperty CanExecuteChangedHandlerProperty
            = DependencyProperty.RegisterAttached(
                "CanExecuteChangedHandler",
                typeof(EventHandler),
                typeof(CommandHelpers), new FrameworkPropertyMetadata(null));

        internal static EventHandler GetCanExecuteChangedHandler(DependencyObject element)
        {
            return (EventHandler)element.GetValue(CanExecuteChangedHandlerProperty);
        }

        internal static void SetCanExecuteChangedHandler(DependencyObject element, EventHandler value)
        {
            element.SetValue(CanExecuteChangedHandlerProperty, value);
        }
#endif

        internal static bool CanExecuteCommandSource(ICommandSource commandSource)
        {
            var command = commandSource.Command;
            if (command == null)
            {
                return false;
            }
            var commandParameter = commandSource.CommandParameter ?? commandSource;
            var routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                return routedCommand.CanExecute(commandParameter, target);
            }
            return command.CanExecute(commandParameter);
        }

        [SecurityCritical]
        [SecuritySafeCritical]
        internal static void ExecuteCommandSource(ICommandSource commandSource)
        {
            CriticalExecuteCommandSource(commandSource);
        }

        [SecurityCritical]
        internal static void CriticalExecuteCommandSource(ICommandSource commandSource)
        {
            var command = commandSource.Command;
            if (command == null)
            {
                return;
            }
            var commandParameter = commandSource.CommandParameter ?? commandSource;
            var routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                if (routedCommand.CanExecute(commandParameter, target))
                {
                    routedCommand.Execute(commandParameter, target);
                }
            }
            else
            {
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }
    }
}