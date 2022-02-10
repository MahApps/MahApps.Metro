// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class NumericUpDownTestsFixture : IAsyncLifetime
    {
        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);

            this.TextBox = this.Window.TheNUD.FindChild<TextBox>();
            this.NumUp = this.Window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            this.NumDown = this.Window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");
        }

        public NumericUpDownWindow? Window { get; private set; }

        public TextBox? TextBox { get; private set; }

        public RepeatButton? NumUp { get; private set; }

        public RepeatButton? NumDown { get; private set; }

        public async Task PrepareForTestAsync()

        {
            await TestHost.SwitchToAppThread();

            this.Window?.TheNUD.ClearDependencyProperties();
        }

        /// <summary>
        /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
        /// if the class also implements that.
        /// </summary>
        public Task DisposeAsync()
        {
            this.TextBox = null;
            this.NumUp = null;
            this.NumDown = null;
            this.Window = null;

            return Task.CompletedTask;
        }
    }
}