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
    public class ButtonTestsFixture : IAsyncLifetime
    {
        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window = await WindowHelpers.CreateInvisibleWindowAsync<ButtonWindow>().ConfigureAwait(false);
        }

        public ButtonWindow? Window { get; private set; }

        public async Task PrepareForTestAsync(IList<string>? properties = null)
        {
            await TestHost.SwitchToAppThread();

            this.Window?.DefaultButton.ClearDependencyProperties(properties);
            this.Window?.SquareButton.ClearDependencyProperties(properties);
            this.Window?.TheDropDownButton.ClearDependencyProperties(properties);
            this.Window?.TheSplitButton.ClearDependencyProperties(properties);
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