// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4514: the drop-down of a picker was a piece of markup in the middle of a template, so
    /// changing anything about it meant copying the template and everything else in it. The clock is
    /// a control of its own now, and the clock, its size and the drop-down each have a property that
    /// reaches them.
    /// </summary>
    [TestFixture]
    public class TimePickerCustomisationTests
    {
        private const string DefaultSet = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
        private const string Win10Set = "pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml";

        private static readonly DateTime TenPastTen = new DateTime(2016, 8, 31, 10, 10, 42);

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
        [Description("The clock in the drop-down is a control, and it stands at the time the picker stands at.")]
        public void TheDropDownCarriesAClockThatShowsTheTime()
        {
            var picker = this.window.Show(new TimePicker { SelectedDateTime = TenPastTen });

            Assert.That(picker.Clock().Time, Is.EqualTo(TenPastTen), "the clock should follow the picker");
        }

        [Test]
        [Description("A clock is 120 wide unless somebody says otherwise, and then it is what they said.")]
        public void TheClockIsAsLargeAsItIsToldToBe()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.window.Show(new TimePicker()).Clock().Width, Is.EqualTo(120d), "the size it always had");
                    Assert.That(this.window.Show(new TimePicker { ClockSize = 240 }).Clock().Width, Is.EqualTo(240d), "and the one it was given");
                });
        }

        [Test]
        [Description("A style of one's own reaches the clock, so its look can be changed without the template around it.")]
        public void AStyleOfItsOwnReachesTheClock()
        {
            var clockStyle = new Style(typeof(AnalogClock));
            clockStyle.Setters.Add(new Setter(Control.BorderBrushProperty, Brushes.Red));

            var picker = this.window.Show(new TimePicker { ClockStyle = clockStyle });

            Assert.That(picker.Clock().BorderBrush, Is.SameAs(Brushes.Red));
        }

        [Test]
        [Description("So does one for the drop-down, which is where its placement comes from.")]
        public void AStyleOfItsOwnReachesTheDropDown()
        {
            var popupStyle = new Style(typeof(Popup));
            popupStyle.Setters.Add(new Setter(Popup.PlacementProperty, PlacementMode.Top));

            var picker = this.window.Show(new TimePicker { PopupStyle = popupStyle });

            Assert.That(picker.DropDown().Placement, Is.EqualTo(PlacementMode.Top));
        }

        [Test]
        [Description("Nobody has to ask for a drop-down that behaves like one: that is what it wears out of the box.")]
        public void TheDropDownBehavesLikeOneWithoutBeingAskedTo()
        {
            var dropDown = this.window.Show(new TimePicker()).DropDown();

            Assert.Multiple(() =>
                {
                    Assert.That(dropDown.Placement, Is.EqualTo(PlacementMode.Bottom), "it hangs under the picker");
                    Assert.That(dropDown.StaysOpen, Is.False, "and closes when somebody clicks past it");
                    Assert.That(dropDown.AllowsTransparency, Is.True);
                });
        }

        [Test]
        [Description("What the picker says about the hands is what the clock draws.")]
        public void TheClockShowsTheHandsThePickerAsksFor()
        {
            var picker = this.window.Show(new TimePicker { HandVisibility = TimePartVisibility.Hour });

            var clock = picker.Clock();
            clock.ApplyTemplate();

            Assert.Multiple(() =>
                {
                    Assert.That(clock.HandVisibility, Is.EqualTo(TimePartVisibility.Hour), "the clock should have been told");
                    Assert.That(Hand(clock, "PART_HourHand").Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(Hand(clock, "PART_MinuteHand").Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(Hand(clock, "PART_SecondHand").Visibility, Is.EqualTo(Visibility.Collapsed));
                });
        }

        [Test]
        [Description("A style of one's own can stand on the base style, which an application can reach now that the set it merges carries it.")]
        public void TheBaseStyleIsThereToStandOn()
        {
            var set = new ResourceDictionary { Source = new Uri(DefaultSet, UriKind.Absolute) };

            var baseStyle = set["MahApps.Styles.TimePickerBase"] as Style;
            Assert.That(baseStyle, Is.Not.Null, "the default set should carry the base style");

            var ownStyle = new Style(typeof(TimePicker), baseStyle);
            ownStyle.Setters.Add(new Setter(TimePickerBase.ClockSizeProperty, 240d));

            var picker = this.window.Show(new TimePicker { Style = ownStyle });

            Assert.Multiple(() =>
                {
                    Assert.That(picker.Template, Is.Not.Null, "the template should come down from the base style");
                    Assert.That(picker.Clock().Width, Is.EqualTo(240d), "and what stands on it should win");
                });
        }

        [Test]
        [Description("Neither Windows 10 nor Fluent draws a clock face, so the drop-down of those two is the columns, and the columns wear the look of the set they belong to.")]
        public void TheWindows10DropDownIsColumnsAndNoClock()
        {
            var set = new ResourceDictionary { Source = new Uri(Win10Set, UriKind.Absolute) };

            var picker = this.window.Show(new TimePicker { Style = (Style)set["MahApps.Styles.TimePicker.Win10"] });

            var hours = picker.Template?.FindName("PART_HourPicker", picker) as ComboBox;
            Assert.That(hours, Is.Not.Null, "the template should carry its hour column");

            Assert.Multiple(() =>
                {
                    Assert.That(picker.IsClockVisible, Is.False, "the face is off in that set");
                    Assert.That(hours!.Style?.BasedOn, Is.SameAs(set["MahApps.Styles.ComboBox.TimePickerBase.Win10"]), "the column should have been reached by the style of the picker");
                    Assert.That(hours.BorderThickness, Is.EqualTo(new Thickness(0)), "and wear it: a column carries no border of its own");
                    Assert.That(TextBoxHelper.GetButtonWidth(hours), Is.EqualTo(0d), "nor a chevron, since the whole column is the toggle");
                });
        }

        [Test]
        [Description("And one for the button under it, which is a command in a flyout rather than a slab across it.")]
        public void AStyleOfItsOwnReachesTheNowButton()
        {
            var buttonStyle = new Style(typeof(ButtonBase));
            buttonStyle.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.Red));

            var picker = this.window.Show(new TimePicker { NowButtonStyle = buttonStyle });

            Assert.That(picker.NowButton().Foreground, Is.SameAs(Brushes.Red));
        }

        [Test]
        [Description("The frame around the drop-down is a brush the picker carries, so a style can match it to the field rather than leaving it on the one every control shares.")]
        public void TheFrameAroundTheDropDownIsThePickersOwn()
        {
            var picker = this.window.Show(new TimePicker());

            DatePickerHelper.SetDropDownBorderBrush(picker, Brushes.Red);
            this.window.Settle();

            var frame = picker.Template?.FindName("PART_PopupBorder", picker) as Border;

            Assert.That(frame, Is.Not.Null, "the template should carry the frame around the drop-down");
            Assert.That(frame!.BorderBrush, Is.SameAs(Brushes.Red));
        }

        private static UIElement Hand(AnalogClock clock, string part)
        {
            var hand = clock.Template?.FindName(part, clock) as UIElement;

            Assert.That(hand, Is.Not.Null, $"the face should carry {part}");

            return hand!;
        }
    }
}
