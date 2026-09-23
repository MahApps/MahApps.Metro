// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public class ComboBoxSetStyleTests
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

        [TestCase("MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.ComboBox.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var box = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(Edge(box).BorderBrush, Is.Null, "and nothing should be drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI box says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.ComboBox.WinUI");

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
            var box = this.Show(key);
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

        private ComboBox Focused(string key)
        {
            var box = this.Show(key);

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

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
