// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The three sets share one template for the up-down box, so what tells them apart is the
    /// brushes, the glyphs on the two buttons and the text box the number is typed into. The WinUI
    /// one says the focus with the line along its bottom edge, the other two say it with the frame.
    /// </summary>
    [TestFixture]
    public class NumericUpDownSetStyleTests
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

        [TestCase("MahApps.Styles.NumericUpDown.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("A box of one of the two sets wears the chrome of the text box of that same set, down to the thickness of its frame: the two stand next to each other in a form and Windows draws them alike.")]
        public void TheBoxWearsTheChromeOfItsTextBox(string key, string textBoxKey)
        {
            var box = this.Show(key);
            var reference = new TextBox { Style = (Style)Application.Current.FindResource(textBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(box.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(box.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(box.Padding, Is.EqualTo(reference.Padding), "where the number stands in it");
                    Assert.That(box.MinHeight, Is.EqualTo(reference.MinHeight), "and how tall it is");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(box), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "the frame under the pointer");
                    Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "and the delete button is the one that box draws");
                });
        }

        [Test]
        [Description("Its corners are the ones the other WinUI boxes with something attached to them have, the date picker and the combo box, rather than the slightly tighter ones of a plain text box.")]
        public void TheWinUIBoxIsRoundedLikeTheOthersWithAButtonInThem()
        {
            var box = this.Show("MahApps.Styles.NumericUpDown.WinUI");
            var reference = new DatePicker { Style = (Style)Application.Current.FindResource("MahApps.Styles.DatePicker.WinUI") };

            Assert.That(ControlsHelper.GetCornerRadius(box), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)));
        }

        [TestCase("MahApps.Styles.NumericUpDown.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("The number is typed into the text box of that set as well, and a single control wearing the style brings that box with it rather than waiting for the set to be merged.")]
        public void TheNumberIsTypedIntoTheTextBoxOfItsSet(string key, string textBoxKey)
        {
            var box = this.Show(key);

            var inside = box.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            Assert.That(inside!.Style?.BasedOn, Is.SameAs(Application.Current.FindResource(textBoxKey)));
        }

        [TestCase("MahApps.Styles.NumericUpDown")]
        [TestCase("MahApps.Styles.NumericUpDown.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var box = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(FocusFrame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(FocusFrame(box).BorderThickness, Is.Not.EqualTo(default(Thickness)), "and be drawn at all");
                    Assert.That(Edge(box).BorderThickness, Is.EqualTo(default(Thickness)), "while nothing is drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI box says it with that edge instead, and draws no frame of its own round the whole box.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.NumericUpDown.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the bottom edge should take the focus brush");
                    Assert.That(FocusFrame(box).BorderThickness, Is.EqualTo(default(Thickness)), "and the frame the caret used to draw should be gone");
                });
        }

        [TestCase("MahApps.Styles.NumericUpDown.Win10")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI")]
        [Description("The box inside draws neither a frame nor an edge of its own, with the caret in it or without. It ends where the number ends and the buttons begin, so an edge of its own would stop there while the one belonging to the control runs the whole width.")]
        public void TheBoxInsideDrawsNoEdgeOfItsOwn(string key)
        {
            var box = this.Focused(key);

            var inside = box.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            Assert.That(((Border)PartOf(inside!, "BorderElement")).BorderThickness, Is.EqualTo(default(Thickness)), "no frame");
        }

        [Test]
        [Description("The WinUI box inside carries that edge in its template and is told to draw none of it, which is what keeps the accent from stopping where the number stops.")]
        public void TheWinUIBoxInsideIsToldToDrawNoEdge()
        {
            var box = this.Focused("MahApps.Styles.NumericUpDown.WinUI");

            var inside = box.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            var edge = (Border)PartOf(inside!, "BottomEdge");

            Assert.Multiple(() =>
                {
                    Assert.That(edge.BorderThickness, Is.EqualTo(default(Thickness)), "the box inside draws none of it");
                    Assert.That(Edge(box).BorderThickness, Is.EqualTo(new Thickness(0, 0, 0, 2)), "while the one belonging to the control is there");
                });
        }

        [Test]
        [Description("That edge is the same two units whether the caret is in the box or not, so the box stays as tall as it was and nothing under it moves.")]
        public void TheWinUIEdgeIsTheSameTwoUnitsEitherWay()
        {
            var box = this.Show("MahApps.Styles.NumericUpDown.WinUI");
            var idle = Edge(box).BorderThickness;

            box = this.Focused("MahApps.Styles.NumericUpDown.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(idle, Is.EqualTo(new Thickness(0, 0, 0, 2)));
                    Assert.That(Edge(box).BorderThickness, Is.EqualTo(idle));
                });
        }

        [Test]
        [Description("Idle, that edge is the stroke WinUI draws a little stronger than the rest of the frame.")]
        public void AnIdleWinUIBoxCarriesTheStrongerStroke()
        {
            var box = this.Show("MahApps.Styles.NumericUpDown.WinUI");

            Assert.That(ColourOf(Edge(box).BorderBrush), Is.EqualTo((Color)box.FindResource("MahApps.Colors.WinUI.ControlStrokeSecondary")));
        }

        [TestCase("MahApps.Styles.NumericUpDown.Win10")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI")]
        [Description("Both Windows sets step the number with the chevrons a UWP box carries, where the Metro one draws a plus and a minus.")]
        public void TheWindowsSetsStepWithChevrons(string key)
        {
            var box = this.Show(key);

            Assert.Multiple(() =>
                {
                    Assert.That(GlyphOn(box, "PART_NumericUp"), Is.EqualTo(""), "the button that counts up");
                    Assert.That(GlyphOn(box, "PART_NumericDown"), Is.EqualTo(""), "and the one that counts down");
                });
        }

        [Test]
        [Description("The Metro box keeps the plus and the minus it has always had.")]
        public void TheMetroBoxKeepsItsPlusAndMinus()
        {
            var box = this.Show("MahApps.Styles.NumericUpDown");

            Assert.Multiple(() =>
                {
                    Assert.That(Button(box, "PART_NumericUp").FindChild<PathIcon>(null), Is.Not.Null, "the button should carry a drawn glyph");
                    Assert.That(Button(box, "PART_NumericUp").FindChild<FontIcon>(null), Is.Null, "and no glyph out of a font in its place");
                });
        }

        [TestCase("MahApps.Styles.NumericUpDown.Win10", "MahApps.Styles.Button.NumericUpDown.Spin.Win10")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", "MahApps.Styles.Button.NumericUpDown.Spin.WinUI")]
        [Description("A box of one of the two sets hands its buttons the style of that same set, whatever the application merged.")]
        public void EachSetHandsOverItsOwnButtons(string key, string spinKey)
        {
            var box = this.Show(key);

            Assert.That(Button(box, "PART_NumericUp").Style, Is.SameAs(Application.Current.FindResource(spinKey)));
        }

        [TestCase("MahApps.Styles.NumericUpDown", "MahApps.Brushes.SystemControlForegroundChromeDisabledLow")]
        [TestCase("MahApps.Styles.NumericUpDown.Win10", "MahApps.Brushes.NumericUpDown.Win10.SpinButtonForegroundDisabled")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", "MahApps.Brushes.NumericUpDown.WinUI.SpinButtonForegroundDisabled")]
        [Description("A button that can count no further is grey rather than gone. The value has reached its minimum, and a chevron that disappears there leaves a gap where the reader expects a pair.")]
        public void AButtonThatCanCountNoFurtherGreysItsGlyph(string key, string brushKey)
        {
            var box = this.Show(key);
            box.Minimum = 0;
            box.Value = 0;
            this.Settle();

            var down = Button(box, "PART_NumericDown");
            Assume.That(down.IsEnabled, Is.False, "the button should be off at the minimum, otherwise this test proves nothing");

            var glyph = down.FindChild<FrameworkElement>(null);
            Assert.That(glyph, Is.Not.Null, "the button should still carry its glyph");

            Assert.Multiple(() =>
                {
                    Assert.That(down.IsVisible, Is.True, "the button is still there");
                    Assert.That(NothingIsInvisible(down), Is.True, "and nothing over it has been taken out of sight");
                    Assert.That(TextElement.GetForeground(PartOf(down, "PART_ContentPresenter")), Is.SameAs(box.FindResource(brushKey)), "the glyph goes grey instead");
                });
        }

        [TestCase("MahApps.Styles.NumericUpDown.Win10", "MahApps.Brushes.NumericUpDown.Win10.BackgroundDisabled", "MahApps.Brushes.NumericUpDown.Win10.ForegroundDisabled")]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", "MahApps.Brushes.NumericUpDown.WinUI.BackgroundDisabled", "MahApps.Brushes.NumericUpDown.WinUI.ForegroundDisabled")]
        [Description("A box of these sets that is switched off says so with its colours rather than with a veil over it.")]
        public void ASwitchedOffBoxSaysSoWithItsColours(string key, string backgroundKey, string foregroundKey)
        {
            var box = this.Show(key);
            box.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(box), Is.EqualTo(Visibility.Collapsed), "no veil");
                    Assert.That(box.Background, Is.SameAs(box.FindResource(backgroundKey)), "the fill");
                    Assert.That(box.Foreground, Is.SameAs(box.FindResource(foregroundKey)), "and the number");
                    Assert.That(box.BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "and the frame");
                });
        }

        [Test]
        [Description("The Metro box keeps the veil it has always drawn over itself.")]
        public void TheMetroBoxKeepsItsVeil()
        {
            var box = this.Show("MahApps.Styles.NumericUpDown");
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

        private NumericUpDown Focused(string key)
        {
            var box = this.Show(key);

            var inside = box.FindChild<TextBox>("PART_TextBox");
            Assert.That(inside, Is.Not.Null, "the template should carry the text box");

            inside!.Focus();
            Keyboard.Focus(inside);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            return box;
        }

        private NumericUpDown Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new NumericUpDown
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 220,
                          Value = 42
                      };

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private static RepeatButton Button(NumericUpDown box, string name)
        {
            var button = box.FindChild<RepeatButton>(name);
            Assert.That(button, Is.Not.Null, $"the template should carry {name}");

            return button!;
        }

        private static string? GlyphOn(NumericUpDown box, string name)
        {
            var glyph = Button(box, name).FindChild<FontIcon>(null);
            Assert.That(glyph, Is.Not.Null, $"{name} should carry a glyph");

            return glyph!.Glyph;
        }

        private static Border FocusFrame(Control box)
        {
            return (Border)PartOf(box, "FocusBorder");
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

        /// <summary>
        /// Whether anything under the button has been taken out of sight altogether. A template that
        /// says a button is off by putting the opacity of its chrome at nought takes the glyph in it
        /// along, which is what the Metro one turning its glyph down to half does not do.
        /// </summary>
        private static bool NothingIsInvisible(DependencyObject from)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(from); i++)
            {
                var child = VisualTreeHelper.GetChild(from, i);

                if (child is UIElement element && element.Opacity <= 0)
                {
                    return false;
                }

                if (!NothingIsInvisible(child))
                {
                    return false;
                }
            }

            return true;
        }

        private static Color ColourOf(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush!).Color;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
