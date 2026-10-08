// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages.ContentDialogExamples
{
    /// <summary>
    /// The user name and whether to remember it. The password stays in the password box, which
    /// gives it out only through code.
    /// </summary>
    public sealed class LoginViewModel : INotifyPropertyChanged
    {
        private string userName = "MahApps";
        private bool rememberMe;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string UserName
        {
            get => this.userName;
            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UserName)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CanSignIn)));
                }
            }
        }

        public bool RememberMe
        {
            get => this.rememberMe;
            set
            {
                if (this.rememberMe != value)
                {
                    this.rememberMe = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.RememberMe)));
                }
            }
        }

        public bool CanSignIn => !string.IsNullOrWhiteSpace(this.UserName);
    }

    /// <summary>
    /// A question and the answer typed in.
    /// </summary>
    public sealed class InputViewModel : INotifyPropertyChanged
    {
        private string text = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Question { get; set; } = string.Empty;

        public string Text
        {
            get => this.text;
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Text)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasText)));
                }
            }
        }

        public bool HasText => !string.IsNullOrWhiteSpace(this.Text);
    }

    /// <summary>
    /// How far the work is, from 0 to 1, and what it is doing right now.
    /// </summary>
    public sealed class ProgressViewModel : INotifyPropertyChanged
    {
        private double value;
        private string message = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }

        public string Message
        {
            get => this.message;
            set
            {
                this.message = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Message)));
            }
        }
    }

    /// <summary>
    /// A list to choose from and the one chosen.
    /// </summary>
    public sealed class SelectionViewModel : INotifyPropertyChanged
    {
        private Person? selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IReadOnlyList<Person> People { get; set; } = Array.Empty<Person>();

        public Person? Selected
        {
            get => this.selected;
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Selected)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasSelection)));
                }
            }
        }

        public bool HasSelection => this.Selected is not null;
    }
}
