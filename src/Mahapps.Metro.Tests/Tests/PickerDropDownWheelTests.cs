// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A drop-down stays where it was opened, so the page behind it must stay there as well. A
    /// routed event leaves a popup along the tree the popup hangs in, which means a wheel nobody in
    /// the drop-down wanted used to travel on to whatever the picker itself stood in and scroll it.
    /// </summary>
    [TestFixture]
    public class PickerDropDownWheelTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("The wheel over an open drop-down leaves the page it stands on where it is.")]
        public void AWheelOverTheDropDownDoesNotScrollThePageBehindIt()
        {
            var picker = new TimePicker();
            var scroller = Page(picker);

            picker.SetCurrentValue(TimePickerBase.IsDropDownOpenProperty, true);
            this.window.Settle();

            var wheel = Wheel();
            ((UIElement)picker.DropDown().Child).RaiseEvent(wheel);
            this.window.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(wheel.Handled, Is.True, "the drop-down should keep the wheel to itself");
                    Assert.That(scroller.VerticalOffset, Is.EqualTo(0d), "and the page should not have moved");
                });

            picker.SetCurrentValue(TimePickerBase.IsDropDownOpenProperty, false);
        }

        [Test]
        [Description("With the drop-down shut the wheel belongs to the page again, the way it does on any other control.")]
        public void AWheelOverAClosedPickerStillScrollsThePage()
        {
            var picker = new TimePicker();
            var scroller = Page(picker);

            picker.RaiseEvent(Wheel());
            this.window.Settle();

            Assert.That(scroller.VerticalOffset, Is.GreaterThan(0d), "the page should have scrolled");
        }

        /// <summary>
        /// The picker somewhere on a page that is taller than what is showing of it.
        /// </summary>
        private ScrollViewer Page(TimePicker picker)
        {
            var content = new StackPanel { Height = 2000 };
            content.Children.Add(picker);

            var scroller = new ScrollViewer
                           {
                               Height = 200,
                               Width = 400,
                               Content = content,
                               VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                           };

            this.window.Show(scroller);

            Assert.That(scroller.ScrollableHeight, Is.GreaterThan(0d), "the page should have somewhere to scroll to");

            return scroller;
        }

        private static MouseWheelEventArgs Wheel()
        {
            return new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, -3 * Mouse.MouseWheelDeltaForOneLine)
                   {
                       RoutedEvent = UIElement.MouseWheelEvent
                   };
        }
    }
}
