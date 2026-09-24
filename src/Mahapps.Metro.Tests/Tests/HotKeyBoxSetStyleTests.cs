// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// The shortcut box is one text box filling the control, so what a set says about it is what
    /// that set says about its text box: the same fill, the same frame, the same padding and the
    /// same delete button, and the box inside is the one of that set.
    /// </summary>
    [TestFixture]
    public class HotKeyBoxSetStyleTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCase("MahApps.Styles.HotKeyBox", "MahApps.Styles.TextBox")]
        [TestCase("MahApps.Styles.HotKeyBox.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.HotKeyBox.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("A shortcut box wears the chrome of the text box of its own set, so the two stand next to each other in a form the way Windows draws them.")]
        public void TheBoxWearsTheChromeOfItsTextBox(string key, string textBoxKey)
        {
            var box = this.Show(key);
            var reference = new TextBox { Style = (Style)Application.Current.FindResource(textBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(box.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(box.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(box.Padding, Is.EqualTo(reference.Padding), "where the text stands in it");
                    Assert.That(box.MinHeight, Is.EqualTo(reference.MinHeight), "how tall it is");
                    Assert.That(box.FontSize, Is.EqualTo(reference.FontSize), "and how big the text is");
                    Assert.That(ControlsHelper.GetCornerRadius(box), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "its corners");
                    Assert.That(ControlsHelper.GetFocusBorderBrush(box), Is.SameAs(ControlsHelper.GetFocusBorderBrush(reference)), "the frame with the caret in it");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(box), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "the frame under the pointer");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(box), Is.SameAs(ControlsHelper.GetBottomBorderBrush(reference)), "the edge along its bottom");
                    Assert.That(ControlsHelper.GetDisabledBorderBrush(box), Is.SameAs(ControlsHelper.GetDisabledBorderBrush(reference)), "the frame of a box that is off");
                    Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "and the delete button is the one that box draws");
                });
        }

        [TestCase("MahApps.Styles.HotKeyBox.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.HotKeyBox.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("The shortcut lands in the box of that set, and a single control wearing the style brings it along rather than waiting for the set to be merged.")]
        public void TheShortcutLandsInTheBoxOfItsSet(string key, string textBoxKey)
        {
            var box = this.Show(key);

            Assert.That(Inside(box).Style?.BasedOn, Is.SameAs(Application.Current.FindResource(textBoxKey)));
        }

        [TestCase("MahApps.Styles.HotKeyBox")]
        [TestCase("MahApps.Styles.HotKeyBox.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var box = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(Part(box, "BottomEdge"), Is.Null, "and there is no edge along the bottom to say it with");
                });
        }

        [Test]
        [Description("The WinUI box says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.HotKeyBox.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the bottom edge should take the focus brush");
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(box.BorderBrush), "and the frame should be left alone");
                });
        }

        [Test]
        [Description("That edge is two units either way, so a box with the caret in it is as tall as the one beside it and nothing under it moves.")]
        public void TheWinUIEdgeIsTheSameTwoUnitsEitherWay()
        {
            var idle = this.Show("MahApps.Styles.HotKeyBox.WinUI");
            var idleEdge = Edge(idle).BorderThickness;

            var focused = this.Focused("MahApps.Styles.HotKeyBox.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(idleEdge, Is.EqualTo(new Thickness(0, 0, 0, 2)), "at rest");
                    Assert.That(Edge(focused).BorderThickness, Is.EqualTo(idleEdge), "and with the caret in it");
                });
        }

        [TestCase("MahApps.Styles.HotKeyBox.Win10", "MahApps.Brushes.TextControl.BackgroundFocused")]
        [TestCase("MahApps.Styles.HotKeyBox.WinUI", "MahApps.Brushes.TextControl.WinUI.BackgroundFocused")]
        [Description("A box of these sets fills itself differently while it has the caret. The control around the box hands it that fill, so it is the control that has to change it.")]
        public void TheWindowsBoxChangesItsFillWithTheCaret(string key, string fill)
        {
            var box = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(Application.Current.FindResource(fill)), "the control");
                    Assert.That(Inside(box).Background, Is.SameAs(box.Background), "and the box it hands it to");
                });
        }

        [TestCase("MahApps.Styles.HotKeyBox.Win10", "MahApps.Brushes.TextControl.BackgroundDisabled")]
        [TestCase("MahApps.Styles.HotKeyBox.WinUI", "MahApps.Brushes.TextControl.WinUI.BackgroundDisabled")]
        [Description("A box of these sets that is switched off says so with its colours rather than with a veil over it.")]
        public void ASwitchedOffBoxSaysSoWithItsColours(string key, string fill)
        {
            var box = this.Show(key);
            box.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(Application.Current.FindResource(fill)), "the fill goes grey");
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "and so does the frame");
                });
        }

        [Test]
        [Description("The Metro box keeps the veil it has always drawn over itself.")]
        public void TheMetroBoxKeepsItsVeil()
        {
            var box = this.Show("MahApps.Styles.HotKeyBox");
            box.IsEnabled = false;
            this.Settle();

            var veil = box.FindChild<Border>("DisabledVisualElement");

            Assert.That(veil, Is.Not.Null, "the template should carry the veil");
            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(box), Is.EqualTo(Visibility.Visible));
                    Assert.That(veil!.Opacity, Is.EqualTo(0.6).Within(0.001), "and it should be drawn over the box");
                    Assert.That(veil.Background, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "in the colour the control was given for it");
                });
        }

        private HotKeyBox Focused(string key)
        {
            var box = this.Show(key);

            var inside = Inside(box);
            inside.Focus();
            Keyboard.Focus(inside);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            return box;
        }

        private HotKeyBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new HotKeyBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          HotKey = new HotKey(Key.F, ModifierKeys.Control),
                          Width = 280,
                          VerticalAlignment = VerticalAlignment.Top
                      };

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        /// <summary>
        /// The box the shortcut is shown in, which is the whole of this control's template.
        /// </summary>
        private static TextBox Inside(HotKeyBox box)
        {
            var inside = box.Template?.FindName("PART_TextBox", box) as TextBox;
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            return inside!;
        }

        private static Border Frame(HotKeyBox box)
        {
            var frame = Part(box, "BorderElement");
            Assert.That(frame, Is.Not.Null, "the box should carry its frame");

            return (Border)frame!;
        }

        private static Border Edge(HotKeyBox box)
        {
            var edge = Part(box, "BottomEdge");
            Assert.That(edge, Is.Not.Null, "the box should carry the edge along its bottom");

            return (Border)edge!;
        }

        /// <summary>
        /// A named part of the template of the box inside, since the frame of this control is the
        /// frame of that box.
        /// </summary>
        private static FrameworkElement? Part(HotKeyBox box, string name)
        {
            var inside = Inside(box);

            return inside.Template?.FindName(name, inside) as FrameworkElement;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
