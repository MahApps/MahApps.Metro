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
    public class TextBoxHelperTestsFixture : IAsyncLifetime
    {
        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxHelperTestWindow>().ConfigureAwait(false);
        }

        public TextBoxHelperTestWindow? Window { get; private set; }

        public async Task PrepareForTestAsync(IList<string>? properties = null)
        {
            await TestHost.SwitchToAppThread();

            this.Window?.TestTextBox.ClearDependencyProperties(properties);
            this.Window?.TestButtonTextBox.ClearDependencyProperties(properties);
            this.Window?.TestPasswordBox.ClearDependencyProperties(properties);
            this.Window?.TestButtonRevealedPasswordBox.ClearDependencyProperties(properties);
            this.Window?.TestComboBox.ClearDependencyProperties(properties);
            this.Window?.TestEditableComboBox.ClearDependencyProperties(properties);
            this.Window?.TestNumericUpDown.ClearDependencyProperties(properties);
            this.Window?.TestHotKeyBox.ClearDependencyProperties(properties);
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