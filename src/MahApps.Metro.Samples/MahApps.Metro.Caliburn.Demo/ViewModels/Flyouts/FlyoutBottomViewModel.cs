// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;

namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    public class FlyoutBottomViewModel : FlyoutBaseViewModel
    {
        public FlyoutBottomViewModel()
        {
            this.Header = "Bottom";
            this.Position = Position.Bottom;
            this.Theme = FlyoutTheme.Light;
        }
    }
}