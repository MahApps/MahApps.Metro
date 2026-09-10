// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class HwndHostTests
    {
        private HwndHostWindow? window;
        private TestHwndHost? host;
        private CustomDialog? dialog;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<HwndHostWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            Assert.That(this.window, Is.Not.Null);

            this.dialog = new CustomDialog { Title = "a dialog" };
            this.host = new TestHwndHost { Width = 20, Height = 20 };
            this.window.TheContent.Children.Add(this.host);
            this.window.SetCurrentValue(MetroWindow.CollapseHwndHostsProperty, false);
            this.window.UpdateLayout();
            ClipAssert.Pump();
        }

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null && this.host is not null)
            {
                this.window.TheContent.Children.Remove(this.host);
                this.host.Dispose();
                this.host = null;
            }
        }

        [Test]
        [Description("Nobody who did not ask for it should see a hosted handle disappear.")]
        public async Task AHostIsLeftAloneWhileTheSwitchIsOff()
        {
            await this.window!.ShowMetroDialogAsync(this.dialog).ConfigureAwait(true);
            ClipAssert.Pump();

            Assert.That(this.host!.Visibility, Is.EqualTo(Visibility.Visible));

            await this.CloseTheDialogAsync().ConfigureAwait(true);
        }

        [Test]
        [Description("A dialog is drawn over the window, so the handle has to be gone for it to be seen whole.")]
        public async Task AHostGoesOutOfSightWhileADialogIsOpen()
        {
            this.window!.SetCurrentValue(MetroWindow.CollapseHwndHostsProperty, true);

            await this.window.ShowMetroDialogAsync(this.dialog).ConfigureAwait(true);
            ClipAssert.Pump();

            Assert.That(this.host!.Visibility, Is.EqualTo(Visibility.Collapsed), "out of sight while the dialog is up");

            await this.CloseTheDialogAsync().ConfigureAwait(true);

            Assert.That(this.host.Visibility, Is.EqualTo(Visibility.Visible), "and back once it is gone");
        }

        [Test]
        [Description("A Flyout is drawn over the window in the same way.")]
        public void AHostGoesOutOfSightWhileAFlyoutIsOpen()
        {
            this.window!.SetCurrentValue(MetroWindow.CollapseHwndHostsProperty, true);

            this.window.TheFlyout.SetCurrentValue(Flyout.IsOpenProperty, true);
            ClipAssert.Pump();

            Assert.That(this.host!.Visibility, Is.EqualTo(Visibility.Collapsed), "out of sight while the flyout is open");

            this.window.TheFlyout.SetCurrentValue(Flyout.IsOpenProperty, false);
            ClipAssert.Pump();

            Assert.That(this.host.Visibility, Is.EqualTo(Visibility.Visible), "and back once it has closed");
        }

        [Test]
        [Description("The switch is what decides, so turning it on with a dialog already up still hides the host.")]
        public async Task TurningTheSwitchOnWhileADialogIsUpTakesTheHostOutOfSight()
        {
            await this.window!.ShowMetroDialogAsync(this.dialog).ConfigureAwait(true);
            ClipAssert.Pump();
            Assert.That(this.host!.Visibility, Is.EqualTo(Visibility.Visible));

            this.window.SetCurrentValue(MetroWindow.CollapseHwndHostsProperty, true);

            Assert.That(this.host.Visibility, Is.EqualTo(Visibility.Collapsed));

            await this.CloseTheDialogAsync().ConfigureAwait(true);
        }

        [Test]
        [Description("What comes back is what the host had, not a blanket Visible.")]
        public async Task AHostThatWasHiddenIsHiddenAgainAfterwards()
        {
            this.host!.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
            this.window!.SetCurrentValue(MetroWindow.CollapseHwndHostsProperty, true);

            await this.window.ShowMetroDialogAsync(this.dialog).ConfigureAwait(true);
            ClipAssert.Pump();
            Assert.That(this.host.Visibility, Is.EqualTo(Visibility.Collapsed));

            await this.CloseTheDialogAsync().ConfigureAwait(true);

            Assert.That(this.host.Visibility, Is.EqualTo(Visibility.Hidden), "hidden is what it was, so hidden is what it gets back");
        }

        private async Task CloseTheDialogAsync()
        {
            await this.window!.HideMetroDialogAsync(this.dialog).ConfigureAwait(true);

            ClipAssert.Pump();
        }
    }
}
