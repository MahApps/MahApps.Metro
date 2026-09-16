// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Tests.Views
{
    public partial class DropDownButtonView : UserControl
    {
        public DropDownButtonView()
        {
            this.InitializeComponent();

            this.DataContext = new DropDownButtonViewModel();
        }
    }

    public class DropDownButtonViewModel
    {
        public IEnumerable<DropDownButtonEntry> Entries { get; } = new[]
                                                                   {
                                                                       new DropDownButtonEntry("the first one"),
                                                                       new DropDownButtonEntry("the second one")
                                                                   };

        public ICommand AddCommand { get; } = new DropDownButtonCommand();
    }

    public class DropDownButtonEntry
    {
        public DropDownButtonEntry(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    public class DropDownButtonCommand : ICommand
    {
        public object? LastParameter { get; private set; }

        public event System.EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            this.LastParameter = parameter;
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
        }
    }
}
