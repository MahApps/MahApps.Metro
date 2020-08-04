// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Composition;
using Caliburn.Metro.Demo.ViewModels.Flyouts;
using Caliburn.Micro;

namespace Caliburn.Metro.Demo.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        public IObservableCollection<FlyoutBaseViewModel> FlyoutViewModels { get; }

        public ShellViewModel()
        {
            FlyoutViewModels = new BindableCollection<FlyoutBaseViewModel>();
        }

        public void Close()
        {
            this.TryClose();
        }

        public void ToggleFlyout(int index)
        {
            var flyout = this.FlyoutViewModels[index];
            flyout.IsOpen = !flyout.IsOpen;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.DisplayName = "Caliburn Metro Demo";
            this.FlyoutViewModels.Add(new Flyout1ViewModel());
            this.FlyoutViewModels.Add(new Flyout2ViewModel());
            this.FlyoutViewModels.Add(new Flyout3ViewModel());
            this.FlyoutViewModels.Add(new FlyoutSettingsViewModel());
            this.FlyoutViewModels.Add(new FlyoutLeftViewModel());
            this.FlyoutViewModels.Add(new FlyoutTopViewModel());
            this.FlyoutViewModels.Add(new FlyoutBottomViewModel());
        }
    }
}