// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    public class CustomDialogExampleContent : ViewModelBase
    {
        private string _firstName;
        private string _lastName;

        public CustomDialogExampleContent(Action<CustomDialogExampleContent> closeHandler)
        {
            this.CloseCommand = new SimpleCommand(o => true, o => closeHandler(this));
        }

        public string FirstName
        {
            get => this._firstName;
            set
            {
                this._firstName = value;
                this.OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => this._lastName;
            set
            {
                this._lastName = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; }
    }
}