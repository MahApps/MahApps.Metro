// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The progress of this dialog runs from 0.0 to 1.0 and not from 0 to 100, which is what the
    /// documentation of <see cref="ProgressDialogController.SetProgress"/> used to say. These tests
    /// hold it to the range it really has.
    /// </summary>
    [TestFixture]
    public class ProgressDialogTests
    {
        [Test]
        public async Task ProgressShouldRunFromZeroToOne()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Title", "Message");

                Assert.That(controller.Minimum, Is.EqualTo(0d), "the dialog should start at zero");
                Assert.That(controller.Maximum, Is.EqualTo(1d), "and it should be full at one, not at a hundred");

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task SetProgressShouldTakeAValueInThatRange()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Title", "Message");

                Assert.That(() => controller.SetProgress(0.25d), Throws.Nothing, "a quarter of the way is 0.25");

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task SetProgressShouldRefuseAPercentage()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Title", "Message");

                Assert.That(() => controller.SetProgress(25d), Throws.InstanceOf<ArgumentOutOfRangeException>(), "25 is past the maximum, which is what makes the old wording so misleading");

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task SetProgressShouldFollowAMaximumOfYourOwn()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Title", "Message");

                controller.Maximum = 100d;

                Assert.That(() => controller.SetProgress(25d), Throws.Nothing, "raising Maximum is how a caller gets percentages");

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }
    }
}
