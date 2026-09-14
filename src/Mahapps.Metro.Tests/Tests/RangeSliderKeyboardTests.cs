// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4621: a range slider did nothing at all when a key was pressed. It has two values, so the
    /// thumb that was last touched is the one the keyboard talks to.
    /// </summary>
    [TestFixture]
    public class RangeSliderKeyboardTests
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
        [Description("A thumb has to be able to hold the keyboard focus in the first place, which a WPF thumb does not do by itself.")]
        public void AThumbCanTakeTheFocus()
        {
            var slider = this.Show();

            var lower = PartOf(slider, "PART_LeftThumb");

            Assert.That(lower.Focusable, Is.True, "the lower thumb should be able to take the focus");
            Assert.That(PartOf(slider, "PART_RightThumb").Focusable, Is.True, "and so should the upper one");
            Assert.That(lower.Focus(), Is.True, "and focusing it should work");
        }

        [Test]
        [Description("Grabbing a thumb with the mouse is what hands the keyboard to it, so a drag has to leave the focus there.")]
        public void GrabbingAThumbGivesItTheFocus()
        {
            var slider = this.Show();

            var upper = PartOf(slider, "PART_RightThumb");
            upper.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent, Source = upper });
            this.Settle();

            Assert.That(upper.IsFocused, Is.True, Where(slider, "the thumb that was grabbed should hold the focus"));
        }

        [Test]
        [Description("Tab walks onto the thumbs, not onto the control first, so that tab and shift tab walk in and out again instead of stopping somewhere no key does anything.")]
        public void TabStopsAtTheThumbsAndNotAtTheControl()
        {
            var slider = this.Show();

            Assert.That(slider.IsTabStop, Is.False, "the control itself is not a stop");
            Assert.That(PartOf(slider, "PART_LeftThumb").IsTabStop, Is.True, "the lower thumb is");
            Assert.That(PartOf(slider, "PART_RightThumb").IsTabStop, Is.True, "and so is the upper one");
            Assert.That(PartOf(slider, "PART_MiddleThumb").IsTabStop, Is.False, "the band is not, there is no key that moves it");
        }

        [Test]
        [Description("Clicking anywhere on the control has to leave the keyboard on a thumb, otherwise an arrow key afterwards is left to the focus navigation and walks off to the next control.")]
        public void AClickLeavesTheKeyboardOnAThumb()
        {
            var slider = this.Show();

            slider.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                              {
                                  RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                                  Source = slider
                              });
            this.Settle();

            Assert.That(PartOf(slider, "PART_LeftThumb").IsFocused || PartOf(slider, "PART_RightThumb").IsFocused,
                        Is.True,
                        Where(slider, "the click should have handed the focus to one of the thumbs"));
        }

        [Test]
        [Description("Focusing the control itself has nothing for a key to move, so the lower thumb takes over.")]
        public void FocusingTheControlHandsOverToTheLowerThumb()
        {
            var slider = this.Show();

            // without the keyboard as well, which is the state a window that is not in front is in
            Keyboard.ClearFocus();
            slider.Focus();
            this.Settle();

            Assert.That(PartOf(slider, "PART_LeftThumb").IsFocused, Is.True, Where(slider, "the lower thumb should have taken over"));
        }

        [Test]
        [Description("After the mouse has been on a thumb, a key still has to move the value by SmallChange and by nothing else.")]
        public void AKeyAfterAClickStillMovesBySmallChange()
        {
            var slider = this.Show();

            var lower = PartOf(slider, "PART_LeftThumb");
            slider.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                              {
                                  RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                                  Source = slider
                              });
            lower.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent, Source = lower });
            lower.RaiseEvent(new DragCompletedEventArgs(0, 0, false) { RoutedEvent = Thumb.DragCompletedEvent, Source = lower });
            this.Settle();

            var before = slider.LowerValue;

            Press(slider, Key.Right);
            this.Settle();

            Assert.That(slider.LowerValue - before, Is.EqualTo(slider.SmallChange).Within(0.01), $"one press moved it from {before:0.00} to {slider.LowerValue:0.00}");
        }

        [TestCase(Key.Right, 51d, TestName = "RightMovesTheLowerValueUp")]
        [TestCase(Key.Left, 49d, TestName = "LeftMovesTheLowerValueDown")]
        [Description("With the lower thumb focused, left and right move the lower value by SmallChange.")]
        public void TheArrowKeysMoveTheFocusedLowerValue(Key key, double expected)
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, key);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(expected).Within(0.01));
            Assert.That(slider.UpperValue, Is.EqualTo(70d), "the value nobody is on should not move");
        }

        [TestCase(Key.Right, 71d, TestName = "RightMovesTheUpperValueUp")]
        [TestCase(Key.Left, 69d, TestName = "LeftMovesTheUpperValueDown")]
        [Description("And with the upper thumb focused, the same two keys move the upper value.")]
        public void TheArrowKeysMoveTheFocusedUpperValue(Key key, double expected)
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_RightThumb");

            Press(slider, key);
            this.Settle();

            Assert.That(slider.UpperValue, Is.EqualTo(expected).Within(0.01));
            Assert.That(slider.LowerValue, Is.EqualTo(50d), "the value nobody is on should not move");
        }

        [TestCase(Key.PageUp, 60d, TestName = "PageUpMovesByLargeChange")]
        [TestCase(Key.PageDown, 40d, TestName = "PageDownMovesByLargeChange")]
        [Description("The page keys move by LargeChange, the way they do on a plain slider.")]
        public void ThePageKeysMoveByLargeChange(Key key, double expected)
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, key);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        [Description("Home sends the focused thumb to the start of the track.")]
        public void HomeGoesToTheStart()
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, Key.Home);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(0d));
        }

        [Test]
        [Description("End sends it the other way, as far as the other value lets it.")]
        public void EndGoesAsFarAsItMay()
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, Key.End);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(70d), "End takes it up to the upper value and no further");
        }

        [Test]
        [Description("And one after the other, because a value set by a key must not fall back to what it was before.")]
        public void HomeAndThenEndBothCount()
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, Key.Home);
            this.Settle();
            Assume.That(slider.LowerValue, Is.EqualTo(0d));

            Press(slider, Key.End);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(70d));
        }

        [Test]
        [Description("A thumb pressed against the other one stops there instead of pushing it along.")]
        public void AThumbStopsAtTheOtherOne()
        {
            var slider = this.Show();
            slider.LowerValue = 69;
            this.Settle();

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, Key.Right);
            Press(slider, Key.Right);
            Press(slider, Key.Right);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(70d), "the lower value stops where the upper one is");
            Assert.That(slider.UpperValue, Is.EqualTo(70d), "and the upper one stays put");
        }

        [Test]
        [Description("MinRange keeps its distance for the keyboard as well.")]
        public void MinRangeHoldsAgainstTheKeyboard()
        {
            var slider = this.Show();
            slider.MinRange = 5;
            slider.LowerValue = 60;
            this.Settle();

            this.FocusOn(slider, "PART_LeftThumb");

            for (var press = 0; press < 10; press++)
            {
                Press(slider, Key.Right);
            }

            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(65d), "the lower value stops MinRange short of the upper one");
        }

        [TestCase(Key.Up, 51d, TestName = "UpMovesTheValueUpOnAnUprightSlider")]
        [TestCase(Key.Down, 49d, TestName = "DownMovesTheValueDownOnAnUprightSlider")]
        [Description("Standing upright, up and down do what right and left do lying down.")]
        public void TheUpAndDownKeysMoveAnUprightSlider(Key key, double expected)
        {
            var slider = this.Show(Orientation.Vertical);

            this.FocusOn(slider, "PART_LeftThumb");

            Press(slider, key);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        [Description("A key nobody asked for has to be left alone, so that it still reaches whatever else is listening.")]
        public void AKeyThatMeansNothingIsLeftAlone()
        {
            var slider = this.Show();

            this.FocusOn(slider, "PART_LeftThumb");

            var arguments = Press(slider, Key.A);
            this.Settle();

            Assert.That(arguments.Handled, Is.False, "the slider should not swallow a key it does nothing with");
            Assert.That(slider.LowerValue, Is.EqualTo(50d));
        }

        [Test]
        [Description("A window that is not the one the keyboard is pointed at still holds a focus of its own, and that is the one the key belongs to. Anything else makes the control depend on which window the desktop happens to be showing, which is how the same press does nothing on a build agent and works on a desk.")]
        public void AKeyStillMovesTheValueWithoutTheKeyboardFocus()
        {
            var slider = this.Show();
            var lower = this.FocusOn(slider, "PART_LeftThumb");

            // what losing the window to another one leaves behind: the focus stays, the keyboard goes
            Keyboard.ClearFocus();
            this.Settle();

            Assume.That(lower.IsKeyboardFocused, Is.False, "the keyboard should be gone for this test to prove anything");
            Assume.That(lower.IsFocused, Is.True, "and the focus should still be on the thumb");

            Press(slider, Key.Right);
            this.Settle();

            Assert.That(slider.LowerValue, Is.EqualTo(51d).Within(0.01), Where(slider, "the key should still have moved the value"));
        }

        /// <summary>
        /// Puts the focus on one of the thumbs and makes sure it arrived, so that a key press which
        /// does nothing afterwards is not read as the value having been left alone on purpose.
        /// </summary>
        private Thumb FocusOn(RangeSlider slider, string part)
        {
            var thumb = PartOf(slider, part);

            thumb.Focus();
            this.Settle();

            Assert.That(thumb.IsFocused, Is.True, Where(slider, $"{part} should hold the focus before a key is pressed"));

            return thumb;
        }

        /// <summary>Everything about the focus that would explain a key press going nowhere.</summary>
        private string Where(RangeSlider slider, string what)
        {
            var lower = PartOf(slider, "PART_LeftThumb");
            var upper = PartOf(slider, "PART_RightThumb");

            return $"{what}. The window is {(this.window!.IsActive ? "active" : "not active")}, "
                   + $"the lower thumb has focus {lower.IsFocused} and keyboard {lower.IsKeyboardFocused}, "
                   + $"the upper one has focus {upper.IsFocused} and keyboard {upper.IsKeyboardFocused}, "
                   + $"and the keyboard is on {Keyboard.FocusedElement?.GetType().Name ?? "nothing"}";
        }

        private static KeyEventArgs Press(UIElement target, Key key)
        {
            var arguments = new KeyEventArgs(Keyboard.PrimaryDevice,
                                             new HwndSource(0, 0, 0, 0, 0, string.Empty, IntPtr.Zero),
                                             0,
                                             key)
                            {
                                RoutedEvent = Keyboard.KeyDownEvent
                            };

            target.RaiseEvent(arguments);

            return arguments;
        }

        private static Thumb PartOf(RangeSlider slider, string part)
        {
            var thumb = slider.Template?.FindName(part, slider) as Thumb;

            Assert.That(thumb, Is.Not.Null, $"the template should carry {part}");

            return thumb!;
        }

        private RangeSlider Show(Orientation orientation = Orientation.Horizontal)
        {
            Assert.That(this.window, Is.Not.Null);

            var slider = new RangeSlider
                         {
                             Orientation = orientation,
                             Minimum = 0,
                             Maximum = 100,
                             LowerValue = 50,
                             UpperValue = 70,
                             SmallChange = 1,
                             LargeChange = 10
                         };

            if (orientation == Orientation.Horizontal)
            {
                slider.Width = 400;
            }
            else
            {
                slider.Height = 400;
            }

            this.window!.Content = slider;
            this.Settle();

            Assert.That(slider.IsLoaded, Is.True, "the slider should be up before a test looks at it");

            return slider;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
