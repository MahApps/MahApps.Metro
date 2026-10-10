// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A box that holds several picks is the combo box of its set with a row of what has been
    /// picked where the one pick would stand, so each of the three sets hands it the outfit of its
    /// own combo box: the same fill, the same frame, the same chevron and the same rows in the list.
    /// </summary>
    [TestFixture]
    public class MultiSelectionComboBoxSetStyleTests : WindowTestFixture<TestWindow>
    {
        [TestCase("MahApps.Styles.MultiSelectionComboBox", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("The box wears the chrome of the combo box of its own set, so the two stand next to each other in a form the way Windows draws them.")]
        public void TheBoxWearsTheChromeOfItsComboBox(string key, string comboBoxKey)
        {
            var box = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(box.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(box.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(box.MinHeight, Is.EqualTo(reference.MinHeight), "and how tall it is");
                    Assert.That(ControlsHelper.GetCornerRadius(box), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "its corners");
                    Assert.That(ControlsHelper.GetFocusBorderBrush(box), Is.SameAs(ControlsHelper.GetFocusBorderBrush(reference)), "the frame with the caret in it");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(box), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "the frame under the pointer");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(box), Is.SameAs(ControlsHelper.GetBottomBorderBrush(reference)), "the edge along its bottom");
                    Assert.That(ControlsHelper.GetDisabledBorderBrush(box), Is.SameAs(ControlsHelper.GetDisabledBorderBrush(reference)), "the frame of a box that is off");
                    Assert.That(TextBoxHelper.GetButtonTemplate(box), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "and the delete button is the one that box draws");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("A box holding several picks is as tall as the one holding one, because it leaves less above and below itself by exactly what a nugget brings along.")]
        public void TheBoxIsAsTallAsItsComboBox(string key, string comboBoxKey)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new MultiSelectionComboBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          ItemsSource = new[] { "Beam me up...", "Warp nine" },
                          Width = 280,
                          VerticalAlignment = VerticalAlignment.Top
                      };
            box.SelectedItems!.Add("Beam me up...");

            var reference = new ComboBox
                            {
                                Style = (Style)Application.Current.FindResource(comboBoxKey),
                                ItemsSource = new[] { "Beam me up...", "Warp nine" },
                                SelectedIndex = 0,
                                Width = 280,
                                VerticalAlignment = VerticalAlignment.Top
                            };

            var page = new StackPanel();
            page.Children.Add(box);
            page.Children.Add(reference);

            this.window!.Content = page;
            this.Settle();

            Assert.That(box.ActualHeight, Is.EqualTo(reference.ActualHeight).Within(0.01));
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("The list comes up as the drop-down of that set, down to the corners a WinUI flyout is rounded by.")]
        public void TheListComesUpInTheDropDownOfItsSet(string key, string comboBoxKey)
        {
            var box = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(ComboBoxHelper.GetDropDownBackground(box), Is.SameAs(ComboBoxHelper.GetDropDownBackground(reference)), "what the list is filled with");
                    Assert.That(ComboBoxHelper.GetDropDownBorderBrush(box), Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(reference)), "the frame round it");
                    Assert.That(ComboBoxHelper.GetDropDownCornerRadius(box), Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(reference)), "and how it is rounded");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI")]
        [Description("The frame of the list reads the control rather than the dictionary, which is what lets a set say anything about it at all.")]
        public void TheDropDownIsFilledByTheControl(string key)
        {
            var box = this.Show(key);

            var popupBorder = (Border)Part(box, "PopupBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(popupBorder.Background, Is.SameAs(ComboBoxHelper.GetDropDownBackground(box)), "the fill");
                    Assert.That(popupBorder.BorderBrush, Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(box)), "the frame");
                    Assert.That(popupBorder.CornerRadius, Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(box)), "and the corners");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBoxItem.Win10", "MahApps.Styles.ComboBoxItem.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBoxItem.WinUI", "MahApps.Styles.ComboBoxItem.WinUI")]
        [Description("A row in that list is the row the combo box of the same set draws, with the colours it takes when the pointer or the pick reaches it.")]
        public void TheRowsAreTheRowsOfItsSet(string key, string comboBoxItemKey)
        {
            var row = new ListBoxItem { Style = (Style)Application.Current.FindResource(key) };
            var reference = new ComboBoxItem { Style = (Style)Application.Current.FindResource(comboBoxItemKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(row.Foreground, Is.SameAs(reference.Foreground), "the text");
                    Assert.That(row.MinHeight, Is.EqualTo(reference.MinHeight), "how tall a row is");
                    Assert.That(row.Padding, Is.EqualTo(reference.Padding), "where the text stands in it");
                    Assert.That(row.Margin, Is.EqualTo(reference.Margin), "the air around it");
                    Assert.That(ControlsHelper.GetCornerRadius(row), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "its corners");
                    Assert.That(ItemHelper.GetHoverBackgroundBrush(row), Is.SameAs(ItemHelper.GetHoverBackgroundBrush(reference)), "the fill under the pointer");
                    Assert.That(ItemHelper.GetSelectedBackgroundBrush(row), Is.SameAs(ItemHelper.GetSelectedBackgroundBrush(reference)), "the fill of the one that is picked");
                    Assert.That(ItemHelper.GetSelectedForegroundBrush(row), Is.SameAs(ItemHelper.GetSelectedForegroundBrush(reference)), "and the text on it");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBoxItem.CheckBox", "MahApps.Styles.MultiSelectionComboBoxItem")]
        [TestCase("MahApps.Styles.MultiSelectionComboBoxItem.CheckBox.Win10", "MahApps.Styles.MultiSelectionComboBoxItem.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBoxItem.CheckBox.WinUI", "MahApps.Styles.MultiSelectionComboBoxItem.WinUI")]
        [Description("A row with a check box in front of it is that same row of its set, and all three share the one template rather than carrying a copy of it.")]
        public void TheCheckBoxRowIsThatRowOfItsSet(string key, string rowKey)
        {
            var row = new ListBoxItem { Style = (Style)Application.Current.FindResource(key) };
            var reference = new ListBoxItem { Style = (Style)Application.Current.FindResource(rowKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(row.Template, Is.SameAs(Application.Current.FindResource("MahApps.Templates.MultiSelectionComboBoxItem.CheckBox")), "the one template");
                    Assert.That(row.Foreground, Is.SameAs(reference.Foreground), "the text of that set");
                    Assert.That(ItemHelper.GetSelectedBackgroundBrush(row), Is.SameAs(ItemHelper.GetSelectedBackgroundBrush(reference)), "and the fill of the one that is picked");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", "MahApps.Styles.MultiSelectionComboBoxSelectedItem.Win10", "MahApps.Brushes.MultiSelectionComboBox.Win10.ItemBackground")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", "MahApps.Styles.MultiSelectionComboBoxSelectedItem.WinUI", "MahApps.Brushes.MultiSelectionComboBox.WinUI.ItemBackground")]
        [Description("A nugget of either Windows set is a layer over the box rather than a fixed grey, because the box changes its fill under the pointer and again while the list is down, and one of those colours is the grey the nugget used to be.")]
        public void TheNuggetsAreALayerOverTheBox(string key, string nuggetKey, string fill)
        {
            var box = this.Show(key);
            var nugget = (Style)Application.Current.FindResource(nuggetKey);

            Assert.That(box.SelectedItemContainerStyle, Is.SameAs(nugget), "the set should hand the control its own nugget");

            var item = new ListBoxItem { Style = nugget };

            Assert.Multiple(() =>
                {
                    Assert.That(item.Background, Is.SameAs(Application.Current.FindResource(fill)), "and that nugget is a layer");
                    Assert.That(item.Background, Is.Not.SameAs(box.Background), "rather than the colour the box is filled with");
                });
        }

        [Test]
        [Description("A row of nuggets that outgrows the line leaves air above and below it, and the panel takes that air back off its own ends, so a box with one row of them is as tall as it was.")]
        public void TheNuggetsLeaveAirBetweenTheRows()
        {
            var nugget = new ListBoxItem { Style = (Style)Application.Current.FindResource("MahApps.Styles.MultiSelectionComboBoxSelectedItem") };

            var box = this.Show("MahApps.Styles.MultiSelectionComboBox");
            var panel = box.FindChild<WrapPanel>(null);

            Assert.Multiple(() =>
                {
                    Assert.That(nugget.Margin, Is.EqualTo(new Thickness(0, 2, 4, 2)), "the nugget");
                    Assert.That(panel?.Margin, Is.EqualTo(new Thickness(0, -2, 0, -2)), "and the panel that lays them out");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", "MahApps.Styles.TextBox.ComboBox.Editable.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", "MahApps.Styles.TextBox.ComboBox.Editable.WinUI")]
        [Description("The text is typed into the box of that set, caret and selection and all, and a single control wearing the style brings it along rather than waiting for the set to be merged.")]
        public void TheTextIsTypedIntoTheBoxOfItsSet(string key, string editableKey)
        {
            var box = this.Show(key);

            var inside = (TextBox)Part(box, "PART_EditableTextBox");

            Assert.That(inside.Style?.BasedOn, Is.SameAs(Application.Current.FindResource(editableKey)));
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox", false)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox", true)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", true)]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the box always has. The Windows 10 one does so only where the box can be typed into, the way its combo box does.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key, bool editable)
        {
            var box = this.Focused(key, editable);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(box).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(box)), "the frame should take the focus brush");
                    Assert.That(Edge(box).BorderBrush, Is.Null, "while nothing is drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI box that can be typed into says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var box = this.Focused("MahApps.Styles.MultiSelectionComboBox.WinUI", true);

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
            var idle = this.Show("MahApps.Styles.MultiSelectionComboBox.WinUI");
            var idleEdge = Edge(idle).BorderThickness;

            var focused = this.Focused("MahApps.Styles.MultiSelectionComboBox.WinUI", true);

            Assert.Multiple(() =>
                {
                    Assert.That(idleEdge, Is.EqualTo(new Thickness(0, 0, 0, 2)), "at rest");
                    Assert.That(Edge(focused).BorderThickness, Is.EqualTo(idleEdge), "and with the caret in it");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI")]
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
            var box = this.Show("MahApps.Styles.MultiSelectionComboBox");
            box.IsEnabled = false;
            this.Settle();

            var veil = (Border)Part(box, "DisabledVisualElement");

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(box), Is.EqualTo(Visibility.Visible));
                    Assert.That(veil.Opacity, Is.EqualTo(0.6).Within(0.001), "and it should be drawn over the box");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10")]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI")]
        [Description("A box of the two Windows sets that cannot be typed into keeps its frame and its edge with the focus, the way its combo box does: the accent there belongs to the caret.")]
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
        [Description("Brought there with the keyboard, the Windows 10 box fills with the accent the way its combo box does.")]
        public void TheKeyboardFillsTheWindows10BoxWithTheAccent()
        {
            var box = this.Show("MahApps.Styles.MultiSelectionComboBox.Win10");
            this.FocusWithTheKeyboard(box);

            Assert.Multiple(() =>
                {
                    Assert.That(box.Background, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.Win10.BackgroundFocused")), "the fill");
                    Assert.That(box.Foreground, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.Win10.ForegroundFocused")), "the text");
                });
        }

        [Test]
        [Description("Brought there with the keyboard, the WinUI box draws the ring and the bar of its combo box.")]
        public void TheKeyboardPutsTheRingOfItsComboBoxOnTheWinUIBox()
        {
            var box = this.Show("MahApps.Styles.MultiSelectionComboBox.WinUI");
            this.FocusWithTheKeyboard(box);

            Assert.Multiple(() =>
                {
                    Assert.That(FocusVisualHelper.GetFocusVisualStyle(box), Is.SameAs(box.FindResource("MahApps.Styles.FocusVisualStyle.ComboBox.WinUI")), "the ring of the combo box");
                    Assert.That(AdornerLayer.GetAdornerLayer(box)?.GetAdorners(box), Is.Not.Null.And.Not.Empty, "and it is drawn");
                    Assert.That(box.Background, Is.SameAs(box.FindResource("MahApps.Brushes.ComboBox.WinUI.Background")), "while the fill stays");
                });
        }

        [TestCase("MahApps.Styles.MultiSelectionComboBox", false)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox", true)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", false)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", true)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", false)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", true)]
        [Description("One press of the tab key takes the focus into the box and the next one out of it again, whatever has been picked: what is picked is shown in a list of its own, and that list is no stop of its own.")]
        public void OneTabPassesTheBox(string key, bool editable)
        {
            Assert.That(this.window, Is.Not.Null);

            var before = new Button { Content = "Before" };
            var after = new Button { Content = "After" };
            var box = new MultiSelectionComboBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          ItemsSource = new[] { "Beam me up...", "Warp nine" },
                          IsEditable = editable,
                          Width = 280
                      };
            var panel = new StackPanel();
            panel.Children.Add(before);
            panel.Children.Add(box);
            panel.Children.Add(after);
            this.window!.Content = panel;
            this.Settle();
            box.SelectedItems!.Add("Warp nine");
            this.Settle();

            Keyboard.Focus(before);
            this.Settle();
            Assume.That(before.IsKeyboardFocused, Is.True);

            var stops = new System.Collections.Generic.List<string>();
            for (var i = 0; i < 4 && !after.IsKeyboardFocused; i++)
            {
                (Keyboard.FocusedElement as UIElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                this.Settle();
                stops.Add(Keyboard.FocusedElement?.GetType().Name ?? "nothing");
            }

            Assert.That(stops, Has.Count.EqualTo(2), "the stops on the way were " + string.Join(", ", stops));
        }

        /// <summary>
        /// Puts the keyboard focus on the box the way a tab would, which is what the helper drawing
        /// the ring tells apart from a click: the last input before the focus came from the keyboard.
        /// </summary>
        private void FocusWithTheKeyboard(MultiSelectionComboBox box)
        {
            // a test has no keys to press, so it tells the input manager what a press would have told it
            typeof(InputManager).GetProperty(nameof(InputManager.MostRecentInputDevice))!.GetSetMethod(true)!.Invoke(InputManager.Current, new object[] { Keyboard.PrimaryDevice });
            Keyboard.Focus(box);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(box.IsKeyboardFocusWithin, Is.True);
        }

        private MultiSelectionComboBox Focused(string key, bool editable)
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

        private MultiSelectionComboBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var box = new MultiSelectionComboBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          ItemsSource = new[] { "Beam me up...", "Warp nine" },
                          Width = 280,
                          VerticalAlignment = VerticalAlignment.Top
                      };

            this.window!.Content = box;
            this.Settle();

            return box;
        }

        private static Border Frame(MultiSelectionComboBox box)
        {
            return (Border)Part(box, "Border");
        }

        private static Border Edge(MultiSelectionComboBox box)
        {
            return (Border)Part(box, "BottomEdge");
        }

        /// <summary>
        /// A named part of the control's own template. The boxes and lists inside it carry parts of
        /// the same names, and a walk down the tree reaches those first.
        /// </summary>
        private static FrameworkElement Part(MultiSelectionComboBox box, string name)
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
