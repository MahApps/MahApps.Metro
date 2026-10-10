// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The three sets share one template for the combo box, so what tells them apart is the brushes,
    /// the chevron and the list each of them hands over. The WinUI one says the focus with the line
    /// along its bottom edge, the other two say it with the frame.
    /// </summary>
    [TestFixture]
    public class ComboBoxSetStyleTests : WindowTestFixture<TestWindow>
    {
        [TestCase("MahApps.Styles.ComboBox", false)]
        [TestCase("MahApps.Styles.ComboBox", true)]
        [TestCase("MahApps.Styles.ComboBox.Win10", true)]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has. The Windows 10 one does so only where the box can be typed into, since there it is the text box of that set.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key, bool editable)
        {
            var box = this.Focused(key, editable);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(Edge(box).BorderBrush, Is.Null, "and nothing should be drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI box that can be typed into says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.ComboBox.WinUI", true);

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the bottom edge should take the focus brush");
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(box.BorderBrush), "while the frame stays what it was");
                });
        }

        [Test]
        [Description("Idle, that edge is the stroke WinUI draws a little stronger than the rest of the frame.")]
        public void AnIdleWinUIBoxCarriesTheStrongerStroke()
        {
            var box = this.Show("MahApps.Styles.ComboBox.WinUI");

            Assert.That(ColourOf(Edge(box).BorderBrush), Is.EqualTo((Color)box.FindResource("MahApps.Colors.WinUI.ControlStrokeSecondary")));
        }

        [TestCase("MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI")]
        [Description("Both Windows sets draw the chevron of a UWP box where the Metro one draws a filled triangle.")]
        public void TheWindowsSetsDrawAChevron(string key)
        {
            var box = this.Show(key);

            var chevron = box.FindChild<FontIcon>("Arrow");

            Assert.That(chevron, Is.Not.Null, "the drop-down button should carry a glyph");
            Assert.That(chevron!.Glyph, Is.EqualTo("\uE70D"), "and that glyph should be the chevron");
        }

        [Test]
        [Description("The Metro box keeps the triangle it has always had.")]
        public void TheMetroBoxKeepsItsTriangle()
        {
            var box = this.Show("MahApps.Styles.ComboBox");

            Assert.Multiple(() =>
                {
                    Assert.That(box.FindChild<Path>("Arrow"), Is.Not.Null, "the drop-down button should carry the triangle");
                    Assert.That(box.FindChild<FontIcon>("Arrow"), Is.Null, "and no glyph in its place");
                });
        }

        [TestCase("MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI")]
        [Description("The chevron of an editable box opens the list. An editable box hands the toggle no fill of its own, so the cell the glyph stands in has to be the thing that takes the click.")]
        public void TheArrowOfAnEditableBoxTakesTheClick(string key)
        {
            var box = this.Show(key);
            box.IsEditable = true;
            this.Settle();

            var toggle = box.FindChild<ToggleButton>("PART_DropDownToggle");
            Assert.That(toggle, Is.Not.Null, "the template should carry the drop-down toggle");

            var arrow = box.FindChild<FrameworkElement>("Arrow");
            Assert.That(arrow, Is.Not.Null, "the drop-down button should carry the arrow");
            Assume.That(arrow!.ActualWidth, Is.GreaterThan(0), "the arrow should be laid out, otherwise this test proves nothing");

            var middle = arrow.TransformToAncestor(box).Transform(new Point(arrow.ActualWidth / 2, arrow.ActualHeight / 2));
            var hit = box.InputHitTest(middle) as DependencyObject;

            Assert.That(hit, Is.Not.Null, "a click on the arrow should reach something");
            Assert.That(Inside(hit!, toggle!), Is.True, $"a click on the arrow of an editable box lands on {hit!.GetType().Name} instead of the toggle");
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("A box of one of the two sets wears the chrome of the text box of that same set, down to the thickness of its frame: the two stand next to each other in a form and UWP draws them alike.")]
        public void TheBoxWearsTheChromeOfItsTextBox(string key, string textBoxKey)
        {
            // the frame takes the accent for the caret, so the box to hold against the text box is one that can be typed into
            var box = this.Show(key);
            box.IsEditable = true;
            this.Settle();
            var reference = new TextBox { Style = (Style)Application.Current.FindResource(textBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(box.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(box.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(box.Padding, Is.EqualTo(reference.Padding), "where the text stands in it");
                    Assert.That(box.MinHeight, Is.EqualTo(reference.MinHeight), "and how tall it is");
                    Assert.That(ControlsHelper.GetFocusBorderBrush(box), Is.SameAs(ControlsHelper.GetFocusBorderBrush(reference)), "the frame while it has the caret");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(box), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "and under the pointer");
                    Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "the delete button is the one that box draws");
                });
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Styles.TextBox.ComboBox.Editable.Win10", "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Styles.TextBox.ComboBox.Editable.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("What an editable box of one of the two sets types into is the text box of that set as well, caret and selection included.")]
        public void AnEditableBoxTypesIntoTheTextBoxOfItsSet(string key, string editableKey, string textBoxKey)
        {
            var box = this.Show(key);
            box.IsEditable = true;
            this.Settle();

            var editable = box.FindChild<TextBox>("PART_EditableTextBox");
            Assert.That(editable, Is.Not.Null, "the template should carry the text box");

            var reference = new TextBox { Style = (Style)Application.Current.FindResource(textBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(editable!.Style?.BasedOn, Is.SameAs(Application.Current.FindResource(editableKey)), "the style behind it");
                    Assert.That(editable.CaretBrush, Is.SameAs(reference.CaretBrush), "the caret");
                    Assert.That(editable.SelectionBrush, Is.SameAs(reference.SelectionBrush), "and what a selection is painted with");
                });
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Styles.ComboBoxItem.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Styles.ComboBoxItem.WinUI")]
        [Description("A box of one of the two sets hands its rows the style of that same set, whatever the application merged.")]
        public void EachSetHandsOverItsOwnRows(string key, string itemKey)
        {
            var box = this.Show(key);

            Assert.That(box.ItemContainerStyle, Is.SameAs(Application.Current.FindResource(itemKey)));
        }

        [TestCase("MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI")]
        [Description("The list hangs over what is behind it, so it carries a fill and a frame of its own, and both come from the box rather than from a key everything else reads too.")]
        public void TheListTakesItsChromeFromTheBox(string key)
        {
            var box = this.Show(key);

            var popupBorder = DropDown(box);

            Assert.Multiple(() =>
                {
                    Assert.That(popupBorder.Background, Is.SameAs(ComboBoxHelper.GetDropDownBackground(box)), "the fill of the list");
                    Assert.That(popupBorder.BorderBrush, Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(box)), "and the frame around it");
                });
        }

        [Test]
        [Description("A WinUI flyout is rounded more than the box it hangs under, so the two radii are knobs of their own.")]
        public void TheWinUIListIsRoundedMoreThanTheBox()
        {
            var box = this.Show("MahApps.Styles.ComboBox.WinUI");

            Assert.That(ComboBoxHelper.GetDropDownCornerRadius(box), Is.EqualTo(new CornerRadius(8)));
            Assert.That(ControlsHelper.GetCornerRadius(box), Is.EqualTo(new CornerRadius(4)));
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Brushes.ComboBox.Win10.BackgroundDisabled", "MahApps.Brushes.ComboBox.Win10.ForegroundDisabled")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Brushes.ComboBox.WinUI.BackgroundDisabled", "MahApps.Brushes.ComboBox.WinUI.ForegroundDisabled")]
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
                    Assert.That(box.Foreground, Is.SameAs(box.FindResource(foregroundKey)), "and the text");
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(box)), "and the frame");
                });
        }

        [Test]
        [Description("The Metro box keeps the veil it has always drawn over itself.")]
        public void TheMetroBoxKeepsItsVeil()
        {
            var box = this.Show("MahApps.Styles.ComboBox");
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

        [Test]
        [Description("The row a WinUI list has picked carries the accent as a short bar along its left edge, and no other row does.")]
        public void OnlyThePickedWinUIRowCarriesTheBar()
        {
            var item = this.ShowItem("MahApps.Styles.ComboBoxItem.WinUI");

            var pill = item.FindChild<Rectangle>("Pill");
            Assert.That(pill, Is.Not.Null, "the template should carry the bar");
            Assume.That(pill!.Visibility, Is.EqualTo(Visibility.Collapsed), "a row nobody picked should not carry it");

            item.IsSelected = true;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(pill.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(pill.Fill, Is.SameAs(item.FindResource("MahApps.Brushes.ComboBox.WinUI.ItemPillFill")), "and it should be the accent");
                });
        }

        [Test]
        [Description("The Windows 10 row has no bar: it says which one it is with the accent turned right down behind the whole row.")]
        public void TheWindows10RowSaysItWithItsFill()
        {
            var item = this.ShowItem("MahApps.Styles.ComboBoxItem.Win10");

            Assert.Multiple(() =>
                {
                    Assert.That(item.FindChild<Rectangle>("Pill"), Is.Null);
                    Assert.That(ItemHelper.GetSelectedBackgroundBrush(item), Is.SameAs(item.FindResource("MahApps.Brushes.ComboBox.Win10.ItemBackgroundSelected")));
                });
        }

        [TestCase("MahApps.Styles.ComboBox.WinUI", true, "MahApps.Brushes.TextControl.WinUI.PlaceholderForeground")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", false, "MahApps.Brushes.TextControl.WinUI.PlaceholderForeground")]
        [TestCase("MahApps.Styles.ComboBox.Win10", true, "MahApps.Brushes.TextControl.PlaceholderForeground")]
        [TestCase("MahApps.Styles.ComboBox.Win10", false, "MahApps.Brushes.ComboBox.Win10.Foreground")]
        [Description("The watermark writes in the placeholder colour of its set, at full strength. WinUI takes TextFillColorSecondary whether the box can be typed into or not; UWP writes the placeholder of a box that cannot in the colour of its text, and that of one that can in the placeholder colour of the text box inside it.")]
        public void TheWatermarkTakesThePlaceholderColourOfItsSet(string key, bool editable, string brushKey)
        {
            var message = Watermark(this.ShowEmpty(key, editable));

            Assert.Multiple(() =>
                {
                    Assert.That(Colour(message.Foreground), Is.EqualTo(Colour(message.FindResource(brushKey))));
                    Assert.That(message.Opacity, Is.EqualTo(1), "the colour says it, nothing is taken away on top");
                });
        }

        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Brushes.TextControl.WinUI.PlaceholderForegroundFocused")]
        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Brushes.TextControl.PlaceholderForegroundFocused")]
        [Description("With the caret in it the box is a text box, and the watermark takes the colour the text box of its set gives it then. In the Windows 10 look that is the darker one the white fill needs.")]
        public void WithTheCaretTheWatermarkIsThatOfTheTextBox(string key, string brushKey)
        {
            var box = this.ShowEmpty(key, true);
            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            Assert.That(Colour(Watermark(box).Foreground), Is.EqualTo(Colour(box.FindResource(brushKey))));
        }

        [TestCase("MahApps.Styles.ComboBox.WinUI", true, "MahApps.Brushes.TextControl.WinUI.PlaceholderForegroundDisabled")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", false, "MahApps.Brushes.TextControl.WinUI.PlaceholderForegroundDisabled")]
        [TestCase("MahApps.Styles.ComboBox.Win10", true, "MahApps.Brushes.TextControl.PlaceholderForegroundDisabled")]
        [TestCase("MahApps.Styles.ComboBox.Win10", false, "MahApps.Brushes.ComboBox.Win10.ForegroundDisabled")]
        [Description("A box that is switched off writes its watermark in the colour its set switches it off with.")]
        public void ASwitchedOffBoxDimsItsWatermark(string key, bool editable, string brushKey)
        {
            var box = this.ShowEmpty(key, editable);
            box.IsEnabled = false;
            this.Settle();

            Assert.That(Colour(Watermark(box).Foreground), Is.EqualTo(Colour(box.FindResource(brushKey))));
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("The Metro box keeps writing its watermark in its own foreground with some of it taken away.")]
        public void TheMetroWatermarkIsTheForegroundTakenDown(bool editable)
        {
            var box = this.ShowEmpty("MahApps.Styles.ComboBox", editable);
            var message = Watermark(box);

            Assert.Multiple(() =>
                {
                    Assert.That(Colour(message.Foreground), Is.EqualTo(Colour(box.Foreground)));
                    Assert.That(message.Opacity, Is.EqualTo(0.6).Within(0.001));
                });
        }

        [TestCase("MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ComboBox.WinUI")]
        [Description("A box of the two Windows sets that cannot be typed into keeps its frame and its edge with the focus: the accent there belongs to the caret, and such a box has none.")]
        public void ABoxThatCannotBeTypedIntoKeepsItsFrame(string key)
        {
            var idle = this.Show(key);
            var frame = Frame(idle).BorderBrush;
            var edge = Edge(idle).BorderBrush;

            var box = this.Focused(key, false);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(frame), "the frame");
                    Assert.That(Edge(box).BorderBrush, Is.SameAs(edge), "the edge");
                });
        }

        [Test]
        [Description("Brought there with the keyboard, the Windows 10 box fills with the accent and writes over it in the colour UWP gives the text there, ComboBoxBackgroundUnfocused and ComboBoxForegroundFocused. It draws no ring around it.")]
        public void TheKeyboardFillsTheWindows10BoxWithTheAccent()
        {
            var box = this.Show("MahApps.Styles.ComboBox.Win10");
            this.FocusWithTheKeyboard(box);

            Assert.Multiple(() =>
                {
                    Assert.That(FocusVisualHelper.GetIsFocusVisualShown(box), Is.True, "the helper should know the keyboard brought the focus");
                    Assert.That(box.Background, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.Win10.BackgroundFocused")), "the fill");
                    Assert.That(box.Foreground, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.Win10.ForegroundFocused")), "the text");
                    Assert.That(Ring(box)?.FindChildren<Border>(true).Any() ?? false, Is.False, "and no ring");
                });
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", "MahApps.Brushes.ComboBox.Win10.Background")]
        [TestCase("MahApps.Styles.ComboBox.WinUI", "MahApps.Brushes.ComboBox.WinUI.Background")]
        [Description("A press of the pointer is a focus of its own kind, PointerFocused, and the box shows nothing for it: the fill of the Windows 10 box and the ring of the WinUI one go.")]
        public void APressTakesTheKeyboardFocusLookAway(string key, string background)
        {
            var box = this.Show(key);
            this.FocusWithTheKeyboard(box);

            box.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.PreviewMouseDownEvent });
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(FocusVisualHelper.GetIsFocusVisualShown(box), Is.False, "the helper");
                    Assert.That(box.Background, Is.SameAs(box.FindResource(background)), "the fill");
                    Assert.That(Ring(box), Is.Null, "the ring");
                });
        }

        [Test]
        [Description("The Windows 10 box that can be typed into is the text box of its set with the keyboard in it, white with dark text, and not the accent.")]
        public void TheKeyboardLeavesAnEditableWindows10BoxWhite()
        {
            var box = this.ShowEmpty("MahApps.Styles.ComboBox.Win10", true);
            this.FocusWithTheKeyboard(box);

            Assert.That(box.Background, Is.SameAs(box.FindResource("MahApps.Brushes.TextControl.BackgroundFocused")));
        }

        [Test]
        [Description("Brought there with the keyboard, the WinUI box keeps its fill and draws the ring of its HighlightBackground, two units of FocusStrokeColorOuter four units out with corners of seven, and the short bar of the accent at its left edge, three by sixteen, one unit in.")]
        public void TheKeyboardPutsARingAndTheBarOnTheWinUIBox()
        {
            var box = this.Show("MahApps.Styles.ComboBox.WinUI");
            this.FocusWithTheKeyboard(box);

            var ring = Ring(box);
            Assert.That(ring, Is.Not.Null, "the box should carry a ring");

            var frame = ring!.FindChildren<Border>(true).FirstOrDefault();
            var pill = ring.FindChildren<Rectangle>(true).FirstOrDefault();

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.WinUI.Background")), "the fill stays");
                    Assert.That(frame, Is.Not.Null, "the ring");
                    Assert.That(frame!.Margin, Is.EqualTo(new Thickness(-4)), "four units out");
                    Assert.That(frame.BorderThickness, Is.EqualTo(new Thickness(2)), "two units thick");
                    Assert.That(frame.CornerRadius, Is.EqualTo(new CornerRadius(7)), "with corners of seven");
                    Assert.That(ColourOf(frame.BorderBrush), Is.EqualTo(ColourOf((Brush)box.FindResource("MahApps.Brushes.FocusVisual.WinUI.Outer"))), "in the outer focus stroke");
                    Assert.That(pill, Is.Not.Null, "the bar");
                    Assert.That(pill!.Width, Is.EqualTo(3), "three wide");
                    Assert.That(pill.Height, Is.EqualTo(16), "sixteen high");
                    Assert.That(pill.Margin, Is.EqualTo(new Thickness(1, 0, 0, 0)), "one unit in");
                    Assert.That(pill.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Left), "at the left edge");
                    Assert.That(pill.Fill, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.WinUI.ItemPillFill")), "in the accent");
                });
        }

        [Test]
        [Description("The WinUI box that can be typed into says the caret with the accent along its bottom edge, and draws neither the ring nor the bar.")]
        public void AnEditableWinUIBoxDrawsNoRing()
        {
            var box = this.ShowEmpty("MahApps.Styles.ComboBox.WinUI", true);
            this.FocusWithTheKeyboard(box);

            Assert.That(Ring(box)?.FindChildren<Border>(true).Any(b => b.IsVisible) ?? false, Is.False);
        }

        private ComboBox Focused(string key, bool editable)
        {
            var box = this.Show(key);
            box.IsEditable = editable;
            this.Settle();

            box.Focus();
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);

            return box;
        }

        private ComboBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new ComboBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 280
                      };
            box.Items.Add("Beam me up...");
            box.Items.Add("Warp nine");
            box.SelectedIndex = 0;

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private ComboBox ShowEmpty(string key, bool editable)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new ComboBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          IsEditable = editable,
                          Width = 280
                      };
            TextBoxHelper.SetWatermark(box, "Where to");
            box.Items.Add("Beam me up...");

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        // the watermark that is on the screen, which for a box that can be typed into is the one of the text box inside it
        private static TextBlock Watermark(ComboBox box)
        {
            var shown = box.FindChildren<TextBlock>(true).Where(t => t.Text == "Where to" && t.IsVisible).ToList();
            Assert.That(shown, Has.Count.EqualTo(1), "the box should show its watermark once");
            return shown[0];
        }

        private static Color Colour(object brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>());
            return ((SolidColorBrush)brush).Color;
        }

        private ComboBoxItem ShowItem(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var item = new ComboBoxItem
                       {
                           Style = (Style)Application.Current.FindResource(key),
                           Content = "Beam me up...",
                           Width = 280
                       };

            this.window!.Content = item;
            this.Settle();

            return item;
        }

        /// <summary>
        /// The frame around the list. Opening the popup for real is not something a test can rely on,
        /// since the box holds it open with the mouse capture and a popup closes again when its window
        /// loses activation, so this reaches the border where it stands instead.
        /// </summary>
        private static Border DropDown(ComboBox box)
        {
            var popup = box.FindChild<System.Windows.Controls.Primitives.Popup>("PART_Popup");
            Assert.That(popup, Is.Not.Null, "the template should carry the popup");

            var border = (popup!.Child as FrameworkElement)?.FindChild<Border>("PopupBorder");
            Assert.That(border, Is.Not.Null, "the list should sit in a border");

            return border!;
        }

        /// <summary>
        /// Whether the element a click landed on belongs to the given part of the template.
        /// </summary>
        private static bool Inside(DependencyObject hit, DependencyObject part)
        {
            for (var walk = hit; walk is not null; walk = VisualTreeHelper.GetParent(walk))
            {
                if (ReferenceEquals(walk, part))
                {
                    return true;
                }
            }

            return false;
        }

        private static Border Frame(Control box)
        {
            var frame = box.FindChild<Border>("Border");
            Assert.That(frame, Is.Not.Null, "the template should carry the frame");

            return frame!;
        }

        private static Border Edge(Control box)
        {
            var edge = box.FindChild<Border>("BottomEdge");
            Assert.That(edge, Is.Not.Null, "the template should carry the bottom edge");

            return edge!;
        }

        private static Color ColourOf(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush!).Color;
        }

        /// <summary>
        /// Puts the keyboard focus on the box the way a tab would, which is what the helper drawing
        /// the ring tells apart from a click: the last input before the focus came from the keyboard.
        /// </summary>
        private void FocusWithTheKeyboard(ComboBox box)
        {
            // a test has no keys to press, so it tells the input manager what a press would have told it
            typeof(InputManager).GetProperty(nameof(InputManager.MostRecentInputDevice))!.GetSetMethod(true)!.Invoke(InputManager.Current, new object[] { Keyboard.PrimaryDevice });
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);
        }

        /// <summary>
        /// What the helper draws over the box while the keyboard holds it, or nothing.
        /// </summary>
        private static FrameworkElement? Ring(ComboBox box)
        {
            var adorner = AdornerLayer.GetAdornerLayer(box)?.GetAdorners(box)?.FirstOrDefault();
            return adorner is null ? null : VisualTreeHelper.GetChild(adorner, 0) as FrameworkElement;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
