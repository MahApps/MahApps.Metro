// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    internal static class CommandHelpers
    {
        internal static bool CanExecuteCommandSource(ICommandSource commandSource)
        {
            var command = commandSource.Command;
            if (command == null)
            {
                return false;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
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
            if (command is RoutedCommand routedCommand)
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

        internal static bool CanExecuteCommandSource(ICommandSource commandSource, ICommand theCommand)
        {
            var command = theCommand;
            if (command == null)
            {
                return false;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                return routedCommand.CanExecute(commandParameter, target);
            }

            return command.CanExecute(commandParameter);
        }

        [SecurityCritical]
        [SecuritySafeCritical]
        internal static void ExecuteCommandSource(ICommandSource commandSource, ICommand theCommand)
        {
            CriticalExecuteCommandSource(commandSource, theCommand);
        }

        [SecurityCritical]
        internal static void CriticalExecuteCommandSource(ICommandSource commandSource, ICommand theCommand)
        {
            var command = theCommand;
            if (command == null)
            {
                return;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
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