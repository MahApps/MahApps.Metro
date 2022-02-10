// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class HeaderedControlHelperTestsFixture : IAsyncLifetime
    {
        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window = await WindowHelpers.CreateInvisibleWindowAsync<HeaderedControlHelperTestWindow>().ConfigureAwait(false);
        }

        public HeaderedControlHelperTestWindow? Window { get; private set; }

        public async Task PrepareForTestAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window?.TestGroupBox.ClearDependencyProperties();
            this.Window?.TestGroupBoxClean.ClearDependencyProperties();
            this.Window?.TestGroupBoxVS.ClearDependencyProperties();
            this.Window?.TestMetroHeader.ClearDependencyProperties();
            this.Window?.TestColorPalette.ClearDependencyProperties();
            this.Window?.TestToggleSwitch.ClearDependencyProperties();
            this.Window?.TestExpander.ClearDependencyProperties();
            this.Window?.TestExpanderVS.ClearDependencyProperties();
            this.Window?.TestTabControl.ClearDependencyProperties();
            this.Window?.TestTabItem.ClearDependencyProperties();
            this.Window?.TestTabControlVS.ClearDependencyProperties();
            this.Window?.TestTabItemVS.ClearDependencyProperties();
            this.Window?.TestMetroTabControl.ClearDependencyProperties();
            this.Window?.TestMetroTabItem.ClearDependencyProperties();
            this.Window?.TestFlyout.ClearDependencyProperties();
        }

        /// <summary>
        /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
        /// if the class also implements that.
        /// </summary>
        public Task DisposeAsync()
        {
            this.Window = null;

            return Task.CompletedTask;
        }
    }
}