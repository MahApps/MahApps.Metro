// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// A command that counts how often it ran, keeps the parameter it last saw and can be told to refuse.
    /// </summary>
    public sealed class CountingCommand : ICommand
    {
        private bool allowed = true;

        public bool Allowed
        {
            get => this.allowed;
            set
            {
                this.allowed = value;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Executed { get; private set; }

        public object? LastParameter { get; private set; }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            this.LastParameter = parameter;
            return this.allowed;
        }

        public void Execute(object? parameter)
        {
            this.LastParameter = parameter;
            this.Executed++;
        }
    }
}
