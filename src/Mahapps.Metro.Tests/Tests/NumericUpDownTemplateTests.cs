// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Tests around the template lifecycle of the <see cref="NumericUpDown"/>.
    /// These live in their own fixture, and thus on their own window, because re-applying a
    /// template leaves event handlers on the control that no [SetUp] can clear again.
    /// </summary>
    [TestFixture]
    public class NumericUpDownTemplateTests
    {
        private NumericUpDownWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        /// <summary>
        /// GH-4565: OnApplyTemplate attached the repeat button handlers without detaching the
        /// previous ones, so every further template pass added another handler to the same button
        /// and a single click changed the value once per pass.
        /// </summary>
        [Test]
        public void ShouldNotAttachRepeatButtonClickHandlerTwiceOnReappliedTemplate()
        {
            Assert.That(this.window, Is.Not.Null);

            var numUp = this.window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            Assert.That(numUp, Is.Not.Null);

            this.window.TheNUD.SetCurrentValue(NumericUpDown.SpeedupProperty, false);
            this.window.TheNUD.SetCurrentValue(NumericUpDown.IntervalProperty, 1d);
            this.window.TheNUD.SetCurrentValue(NumericUpDown.ValueProperty, 0d);

            this.window.TheNUD.OnApplyTemplate();

            numUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            Assert.That(this.window.TheNUD.Value, Is.EqualTo(1d), "one click changed the value more than once");
        }
    }
}
