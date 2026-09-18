// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// A command that hands its parameter to a delegate, for the buttons in the templates here.
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Func<object?, bool>? canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <inheritdoc />
        public bool CanExecute(object? parameter)
        {
            return this.canExecute is null || this.canExecute(parameter);
        }

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }
    }
}
