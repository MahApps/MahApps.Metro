// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The drop-down of a picker already has a frame around it and corners of its own, so the
    /// calendar inside it carries no frame, only the one edge that divides it from the clock. Which
    /// edge that is follows the orientation of the picker, and it is the picker that decides it: a
    /// binding that has to find the picker from inside the popup is not always there when the
    /// calendar asks, which is why this is a trigger on the picker and a test of its own.
    /// </summary>
    [TestFixture]
    public class DateTimePickerCalendarTests
    {
        private const string Win10Set = "pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml";
        private const string WinUISet = "pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml";

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

        [TestCase(Win10Set, "MahApps.Styles.DateTimePicker.Win10")]
        [TestCase(WinUISet, "MahApps.Styles.DateTimePicker.WinUI")]
        [Description("Stacked, which is what a drop-down with no clock in it is, the line runs along the bottom of the calendar.")]
        public void StackedTheCalendarIsDividedAlongItsBottom(string set, string key)
        {
            var picker = this.window.Show(Picker(set, key));

            Assert.Multiple(() =>
                {
                    Assert.That(picker.Orientation, Is.EqualTo(Orientation.Vertical), "no clock means the two halves are stacked");
                    Assert.That(Calendar(picker).BorderThickness, Is.EqualTo(new Thickness(0, 0, 0, 1)));
                });
        }

        [TestCase(Win10Set, "MahApps.Styles.DateTimePicker.Win10")]
        [TestCase(WinUISet, "MahApps.Styles.DateTimePicker.WinUI")]
        [Description("Side by side, the line stands between the two instead.")]
        public void SideBySideTheCalendarIsDividedOnItsRight(string set, string key)
        {
            var picker = this.window.Show(Picker(set, key));

            picker.IsClockVisible = true;
            picker.Orientation = Orientation.Horizontal;
            this.window.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(picker.Orientation, Is.EqualTo(Orientation.Horizontal));
                    Assert.That(Calendar(picker).BorderThickness, Is.EqualTo(new Thickness(0, 0, 1, 0)));
                });
        }

        [TestCase(Win10Set, "MahApps.Styles.DateTimePicker.Win10")]
        [TestCase(WinUISet, "MahApps.Styles.DateTimePicker.WinUI")]
        [Description("And nothing of the calendar's own frame is left to sit inside the one the drop-down draws.")]
        public void TheCalendarBringsNoFrameOfItsOwn(string set, string key)
        {
            var calendar = Calendar(this.window.Show(Picker(set, key)));

            var thickness = calendar.BorderThickness;

            Assert.That(thickness.Left + thickness.Top + thickness.Right + thickness.Bottom, Is.EqualTo(1d), "one edge and no more");
        }

        [TestCase(Win10Set, "MahApps.Styles.DateTimePicker.Win10")]
        [TestCase(WinUISet, "MahApps.Styles.DateTimePicker.WinUI")]
        [Description("One stroke in a drop-down: the line that divides the two halves is the one the frame around them is drawn in, and neither is the light grey every control shares.")]
        public void TheDividerAndTheFrameAreTheSameStroke(string set, string key)
        {
            var picker = this.window.Show(Picker(set, key));

            var frame = picker.Template?.FindName("PART_PopupBorder", picker) as Border;
            Assert.That(frame, Is.Not.Null, "the template should carry the frame around the drop-down");

            Assert.Multiple(() =>
                {
                    Assert.That(Calendar(picker).BorderBrush, Is.SameAs(frame!.BorderBrush), "the divider and the frame");
                    Assert.That(frame!.BorderBrush, Is.Not.SameAs(Application.Current.TryFindResource("MahApps.Brushes.Control.Border")), "and not the one every control shares");
                });
        }

        private static DateTimePicker Picker(string set, string key)
        {
            var dictionary = new ResourceDictionary { Source = new Uri(set, UriKind.Absolute) };

            return new DateTimePicker { Style = (Style)dictionary[key] };
        }

        private static Calendar Calendar(DateTimePicker picker)
        {
            var host = picker.Template?.FindName("PART_Calendar", picker) as ContentPresenter;

            Assert.That(host, Is.Not.Null, "the template should carry the place the calendar goes");
            Assert.That(host!.Content, Is.InstanceOf<Calendar>(), "and the picker should have put one there");

            return (Calendar)host.Content;
        }
    }
}
