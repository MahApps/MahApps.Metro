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
    /// The suggestion box is the combo box of its set with the drop-down button taken off it, so
    /// each of the three sets hands the one template it shares the outfit of its own combo box:
    /// the same fill, the same frame, the same delete button and the same rows in the list that
    /// comes up while typing.
    /// </summary>
    [TestFixture]
    public class AutoSuggestBoxSetStyleTests
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

        [TestCase("MahApps.Styles.AutoSuggestBox", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.AutoSuggestBox.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.AutoSuggestBox.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("A suggestion box wears the chrome of the combo box of its own set, so the two stand next to each other in a form the way Windows draws them.")]
        public void TheBoxWearsTheChromeOfItsComboBox(string key, string comboBoxKey)
        {
            var box = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(box.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(box.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(box.Padding, Is.EqualTo(reference.Padding), "where the text stands in it");
                    Assert.That(box.MinHeight, Is.EqualTo(reference.MinHeight), "and how tall it is");
                    Assert.That(ControlsHelper.GetCornerRadius(box), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "its corners");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(box), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "the frame under the pointer");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(box), Is.SameAs(ControlsHelper.GetBottomBorderBrush(reference)), "the edge along its bottom");
                    Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "and the delete button is the one that box draws");
                });
        }

        [TestCase("MahApps.Styles.AutoSuggestBox", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.AutoSuggestBox.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.AutoSuggestBox.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("The list of suggestions is the drop-down of that set as well, down to the corners a WinUI flyout is rounded by and the rows standing in it.")]
        public void TheSuggestionsComeUpInTheListOfItsSet(string key, string comboBoxKey)
        {
            var box = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(ComboBoxHelper.GetDropDownBackground(box), Is.SameAs(ComboBoxHelper.GetDropDownBackground(reference)), "what the list is filled with");
                    Assert.That(ComboBoxHelper.GetDropDownBorderBrush(box), Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(reference)), "the frame round it");
                    Assert.That(ComboBoxHelper.GetDropDownCornerRadius(box), Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(reference)), "how it is rounded");
                    Assert.That(box.ItemContainerStyle, Is.SameAs(reference.ItemContainerStyle), "and a row in it");
                });
        }

        [TestCase("MahApps.Styles.AutoSuggestBox.Win10", "MahApps.Styles.TextBox.ComboBox.Editable.Win10")]
        [TestCase("MahApps.Styles.AutoSuggestBox.WinUI", "MahApps.Styles.TextBox.ComboBox.Editable.WinUI")]
        [Description("The text is typed into the box of that set, caret and selection and all, and a single control wearing the style brings it along rather than waiting for the set to be merged.")]
        public void TheTextIsTypedIntoTheBoxOfItsSet(string key, string editableKey)
        {
            var box = this.Show(key);

            var inside = box.FindChild<TextBox>("PART_EditableTextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            Assert.That(inside!.Style?.BasedOn, Is.SameAs(Application.Current.FindResource(editableKey)));
        }

        [TestCase("MahApps.Styles.AutoSuggestBox")]
        [TestCase("MahApps.Styles.AutoSuggestBox.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var box = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(Edge(box).BorderBrush, Is.Null, "while nothing is drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI box says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.AutoSuggestBox.WinUI");

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
            var idle = this.Show("MahApps.Styles.AutoSuggestBox.WinUI");
            var idleEdge = Edge(idle).BorderThickness;

            var focused = this.Focused("MahApps.Styles.AutoSuggestBox.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(idleEdge, Is.EqualTo(new Thickness(0, 0, 0, 2)), "at rest");
                    Assert.That(Edge(focused).BorderThickness, Is.EqualTo(idleEdge), "and with the caret in it");
                });
        }

        [TestCase("MahApps.Styles.AutoSuggestBox.Win10")]
        [TestCase("MahApps.Styles.AutoSuggestBox.WinUI")]
        [Description("A box of these sets that is switched off says so with its colours rather than with a veil over it.")]
        public void ASwitchedOffBoxSaysSoWithItsColours(string key)
        {
            var box = this.Show(key);
            box.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(box), Is.EqualTo(Visibility.Collapsed), "no veil");
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "the frame goes grey");
                    Assert.That(Edge(box).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "and so does the edge along the bottom");
                });
        }

        [Test]
        [Description("The Metro box keeps the veil it has always drawn over itself.")]
        public void TheMetroBoxKeepsItsVeil()
        {
            var box = this.Show("MahApps.Styles.AutoSuggestBox");
            box.IsEnabled = false;
            this.Settle();

            var veil = box.FindChild<Border>("DisabledVisualElement");

            Assert.That(veil, Is.Not.Null, "the template should carry the veil");
            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(box), Is.EqualTo(Visibility.Visible));
                    Assert.That(veil!.Opacity, Is.EqualTo(0.6).Within(0.001), "and it should be drawn over the box");
                });
        }

        private AutoSuggestBox Focused(string key)
        {
            var box = this.Show(key);

            var inside = box.FindChild<TextBox>("PART_EditableTextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            inside!.Focus();
            Keyboard.Focus(inside);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            return box;
        }

        private AutoSuggestBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new AutoSuggestBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 280,
                          VerticalAlignment = VerticalAlignment.Top
                      };

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private static Border Frame(Control box)
        {
            return (Border)PartOf(box, "Border");
        }

        private static Border Edge(Control box)
        {
            return (Border)PartOf(box, "BottomEdge");
        }

        /// <summary>
        /// A named part of the control's own template. The text box inside it carries parts of the
        /// same names, and a walk down the tree reaches those first.
        /// </summary>
        private static FrameworkElement PartOf(Control box, string name)
        {
            var part = box.Template?.FindName(name, box) as FrameworkElement;
            Assert.That(part, Is.Not.Null, $"the template should carry {name}");

            return part!;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
