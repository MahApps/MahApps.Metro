﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Caliburn.Metro.Demo.ViewModels
{
    public abstract class FlyoutBaseViewModel : PropertyChangedBase
    {
        private string? header;
        private bool isOpen;
        private Position position;
        private FlyoutTheme theme = FlyoutTheme.Dark;

        public string? Header
        {
            get => this.header;
            set
            {
                if (value == this.header)
                {
                    return;
                }

                this.header = value;
                this.NotifyOfPropertyChange(() => this.Header);
            }
        }

        public bool IsOpen
        {
            get => this.isOpen;
            set
            {
                if (value.Equals(this.isOpen))
                {
                    return;
                }

                this.isOpen = value;
                this.NotifyOfPropertyChange(() => this.IsOpen);
            }
        }

        public Position Position
        {
            get => this.position;
            set
            {
                if (value == this.position)
                {
                    return;
                }

                this.position = value;
                this.NotifyOfPropertyChange(() => this.Position);
            }
        }

        public FlyoutTheme Theme
        {
            get => this.theme;
            set
            {
                if (value == this.theme)
                {
                    return;
                }

                this.theme = value;
                this.NotifyOfPropertyChange(() => this.Theme);
            }
        }
    }
}