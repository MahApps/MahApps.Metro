// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// A fixture that works on one window it opens once and closes at the end. Most of the style
    /// fixtures need nothing else from their window, and NUnit runs the one time setup of a base
    /// class before the one of the fixture under it, so deriving from this leaves a fixture with
    /// nothing to write for the window at all.
    /// </summary>
    /// <typeparam name="TWindow">The window the fixture wants.</typeparam>
    public abstract class WindowTestFixture<TWindow>
        where TWindow : Window, new()
    {
        /// <summary>
        /// The window of this fixture, up from the one time setup until the one time teardown.
        /// </summary>
        protected TWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TWindow>().ConfigureAwait(true);
        }

        /// <summary>
        /// Takes back what a test put in the window, so that the next one starts on an empty one.
        /// The window itself stays up, since opening one per test is what made this suite slow.
        /// </summary>
        [TearDown]
        public void ClearTheWindow()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }
    }
}
