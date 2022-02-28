// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        public async Task PrepareForTestAsync(IList<string>? properties = null)
        {
            await TestHost.SwitchToAppThread();

            this.Window?.TestGroupBox.ClearDependencyProperties(properties);
            this.Window?.TestGroupBoxClean.ClearDependencyProperties(properties);
            this.Window?.TestGroupBoxVS.ClearDependencyProperties(properties);
            this.Window?.TestMetroHeader.ClearDependencyProperties(properties);
            this.Window?.TestColorPalette.ClearDependencyProperties(properties);
            this.Window?.TestToggleSwitch.ClearDependencyProperties(properties);
            this.Window?.TestExpander.ClearDependencyProperties(properties);
            this.Window?.TestExpanderVS.ClearDependencyProperties(properties);
            this.Window?.TestTabControl.ClearDependencyProperties(properties);
            this.Window?.TestTabItem.ClearDependencyProperties(properties);
            this.Window?.TestTabItemUnselected.ClearDependencyProperties(properties);
            this.Window?.TestTabControlVS.ClearDependencyProperties(properties);
            this.Window?.TestTabItemVS.ClearDependencyProperties(properties);
            this.Window?.TestTabItemVSUnselected.ClearDependencyProperties(properties);
            this.Window?.TestMetroTabControl.ClearDependencyProperties(properties);
            this.Window?.TestMetroTabItem.ClearDependencyProperties(properties);
            this.Window?.TestMetroTabItemUnselected.ClearDependencyProperties(properties);
            this.Window?.TestFlyout.ClearDependencyProperties(properties);
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