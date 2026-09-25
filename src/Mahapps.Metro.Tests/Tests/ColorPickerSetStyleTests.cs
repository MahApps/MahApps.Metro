// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A box a colour is picked in is the combo box of its set with a swatch and a name where the
    /// one pick would stand, so each of the three sets hands it the outfit of its own combo box and
    /// fills the drop-down behind it with the palettes and the canvas of that set.
    /// </summary>
    [TestFixture]
    public class ColorPickerSetStyleTests
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

        [TestCase("MahApps.Styles.ColorPicker.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("The picker wears the chrome of the combo box of its own set, so the two stand next to each other in a form the way Windows draws them.")]
        public void ThePickerWearsTheChromeOfItsComboBox(string key, string comboBoxKey)
        {
            var picker = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(picker.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(picker.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(picker.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(picker.MinHeight, Is.EqualTo(reference.MinHeight), "and how tall it is");
                    Assert.That(picker.Padding, Is.EqualTo(reference.Padding), "where what it shows stands in it");
                    Assert.That(ControlsHelper.GetCornerRadius(picker), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "its corners");
                    Assert.That(ControlsHelper.GetFocusBorderBrush(picker), Is.SameAs(ControlsHelper.GetFocusBorderBrush(reference)), "the frame with the caret in it");
                    Assert.That(ControlsHelper.GetMouseOverBorderBrush(picker), Is.SameAs(ControlsHelper.GetMouseOverBorderBrush(reference)), "the frame under the pointer");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(picker), Is.SameAs(ControlsHelper.GetBottomBorderBrush(reference)), "the edge along its bottom");
                    Assert.That(ControlsHelper.GetDisabledBorderBrush(picker), Is.SameAs(ControlsHelper.GetDisabledBorderBrush(reference)), "the frame of a picker that is off");
                    Assert.That(TextBoxHelper.GetButtonTemplate(picker), Is.SameAs(TextBoxHelper.GetButtonTemplate(reference)), "and the delete button is the one that box draws");
                });
        }

        [TestCase("MahApps.Styles.ColorPicker", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.ColorPicker.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("A picker is as tall as the combo box beside it, so a form holding both keeps its rows in line.")]
        public void ThePickerIsAsTallAsItsComboBox(string key, string comboBoxKey)
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = new ColorPicker
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             SelectedColor = Colors.SteelBlue,
                             Width = 280,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            var reference = new ComboBox
                            {
                                Style = (Style)Application.Current.FindResource(comboBoxKey),
                                ItemsSource = new[] { "Steel blue", "Sea green" },
                                SelectedIndex = 0,
                                Width = 280,
                                VerticalAlignment = VerticalAlignment.Top
                            };

            var page = new StackPanel();
            page.Children.Add(picker);
            page.Children.Add(reference);

            this.window!.Content = page;
            this.Settle();

            Assert.That(picker.ActualHeight, Is.EqualTo(reference.ActualHeight).Within(0.01));
        }

        [TestCase("MahApps.Styles.ColorPicker", "MahApps.Styles.ComboBox")]
        [TestCase("MahApps.Styles.ColorPicker.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("The palettes come up in the drop-down of that set, down to the corners a WinUI flyout is rounded by.")]
        public void ThePalettesComeUpInTheDropDownOfItsSet(string key, string comboBoxKey)
        {
            var picker = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(ComboBoxHelper.GetDropDownBackground(picker), Is.SameAs(ComboBoxHelper.GetDropDownBackground(reference)), "what the drop-down is filled with");
                    Assert.That(ComboBoxHelper.GetDropDownBorderBrush(picker), Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(reference)), "the frame round it");
                    Assert.That(ComboBoxHelper.GetDropDownCornerRadius(picker), Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(reference)), "and how it is rounded");
                });
        }

        [TestCase("MahApps.Styles.ColorPicker")]
        [TestCase("MahApps.Styles.ColorPicker.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI")]
        [Description("The frame of the drop-down reads the control rather than the dictionary, which is what lets a set say anything about it at all.")]
        public void TheDropDownIsFilledByTheControl(string key)
        {
            var picker = this.Show(key);

            var popupBorder = (Border)Part(picker, "PopupBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(popupBorder.Background, Is.SameAs(ComboBoxHelper.GetDropDownBackground(picker)), "the fill");
                    Assert.That(popupBorder.BorderBrush, Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(picker)), "the frame");
                    Assert.That(popupBorder.CornerRadius, Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(picker)), "and the corners");
                });
        }

        [TestCase("MahApps.Styles.ColorPicker", "MahApps.Styles.ColorPalette.ColorPickerDropDown")]
        [TestCase("MahApps.Styles.ColorPicker.Win10", "MahApps.Styles.ColorPalette.ColorPickerDropDown.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", "MahApps.Styles.ColorPalette.ColorPickerDropDown.WinUI")]
        [Description("All five palettes in the drop-down are the palette of that set, the custom one that used to be left out with them.")]
        public void EveryPaletteIsThePaletteOfItsSet(string key, string paletteKey)
        {
            var picker = this.Show(key);
            var palette = (Style)Application.Current.FindResource(paletteKey);

            Assert.Multiple(() =>
                {
                    Assert.That(picker.StandardColorPaletteStyle, Is.SameAs(palette), "the standard colours");
                    Assert.That(picker.AvailableColorPaletteStyle, Is.SameAs(palette), "the ones WPF knows by name");
                    Assert.That(picker.CustomColorPalette01Style, Is.SameAs(palette), "the first custom one");
                    Assert.That(picker.CustomColorPalette02Style, Is.SameAs(palette), "the second");
                    Assert.That(picker.RecentColorPaletteStyle, Is.SameAs(palette), "and the ones picked lately");
                });
        }

        [TestCase("MahApps.Styles.ColorPalette.Win10")]
        [TestCase("MahApps.Styles.ColorPalette.WinUI")]
        [Description("Neither Windows set heads a palette with a filled bar and a frame round the swatches: the name stands over them in plain text.")]
        public void ThePaletteOfAWindowsSetIsHeadedByPlainText(string key)
        {
            var palette = new ColorPalette { Style = (Style)Application.Current.FindResource(key) };

            Assert.Multiple(() =>
                {
                    Assert.That(palette.BorderThickness, Is.EqualTo(new Thickness(0)), "no frame round the swatches");
                    Assert.That(((SolidColorBrush)HeaderedControlHelper.GetHeaderBackground(palette)).Color, Is.EqualTo(Colors.Transparent), "nothing filled behind the name");
                    Assert.That(ControlsHelper.GetContentCharacterCasing(palette), Is.EqualTo(CharacterCasing.Normal), "and that name is not shouted");
                });
        }

        [Test]
        [Description("WinUI rounds what shows a colour by the same number it rounds a control by, so the swatches and the square match the boxes beside them.")]
        public void TheWinUISwatchesAreRoundedByTheSameNumber()
        {
            var radius = Application.Current.FindResource("MahApps.CornerRadius.WinUI.Control");

            var palette = new ColorPalette { Style = (Style)Application.Current.FindResource("MahApps.Styles.ColorPalette.WinUI") };
            var canvas = new ColorCanvas { Style = (Style)Application.Current.FindResource("MahApps.Styles.ColorCanvas.WinUI") };

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetCornerRadius(palette), Is.EqualTo(radius), "the swatches in a palette");
                    Assert.That(ControlsHelper.GetCornerRadius(canvas), Is.EqualTo(radius), "and the square, the preview and the bars on the canvas");
                });
        }

        [TestCase("MahApps.Styles.ColorPicker.Win10", "MahApps.Styles.ColorCanvas.Win10", "MahApps.Styles.TextBox.Win10", "MahApps.Styles.NumericUpDown.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", "MahApps.Styles.ColorCanvas.WinUI", "MahApps.Styles.TextBox.WinUI", "MahApps.Styles.NumericUpDown.WinUI")]
        [Description("The canvas in the drop-down is the canvas of that set, and the boxes on it are that set's boxes, so a picker wearing the style brings them along rather than waiting for the set to be merged.")]
        public void TheCanvasAndItsBoxesAreTheOnesOfItsSet(string key, string canvasKey, string textBoxKey, string upDownKey)
        {
            var picker = (Style)Application.Current.FindResource(key);
            var canvas = (Style)Application.Current.FindResource(canvasKey);

            Assert.Multiple(() =>
                {
                    Assert.That(((Style?)picker.Resources[typeof(ColorCanvas)])?.BasedOn, Is.SameAs(canvas), "the canvas the drop-down shows");
                    Assert.That(((Style?)canvas.Resources[typeof(TextBox)])?.BasedOn, Is.SameAs(Application.Current.FindResource(textBoxKey)), "the box the name is typed into");
                    Assert.That(((Style?)canvas.Resources[typeof(NumericUpDown)])?.BasedOn, Is.SameAs(Application.Current.FindResource(upDownKey)), "and the box a channel is typed into");
                });
        }

        [TestCase("MahApps.Styles.ColorCanvas")]
        [TestCase("MahApps.Styles.ColorCanvas.Win10")]
        [TestCase("MahApps.Styles.ColorCanvas.WinUI")]
        [Description("The boxes a channel is typed into stand in one column whatever is written in them, because the widest of them says how wide that column is for all three groups rather than for its own.")]
        public void TheBoxesOnTheCanvasStandInOneColumn(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var canvas = new ColorCanvas
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             SelectedColor = Colors.SteelBlue,
                             Width = 460,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            this.window!.Content = canvas;
            this.Settle();

            var boxes = canvas.FindChildren<NumericUpDown>(true).ToList();
            Assert.That(boxes, Has.Count.GreaterThanOrEqualTo(7), "the canvas should carry a box per channel");

            var left = boxes.Select(box => Math.Round(box.TranslatePoint(new Point(0, 0), canvas).X, 2)).Distinct().ToList();

            Assert.That(left, Has.Count.EqualTo(1), $"the boxes start at {string.Join(", ", left)}");
        }

        [TestCase("MahApps.Styles.ColorPicker.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI")]
        [Description("The chevron is the one the combo box of that set draws, rather than the filled triangle the Metro picker has.")]
        public void TheChevronIsTheOneOfItsSet(string key)
        {
            var picker = this.Show(key);

            var toggle = (ToggleButton)Part(picker, "PART_DropDownToggle");

            Assert.That(toggle.Style?.BasedOn, Is.SameAs(Application.Current.FindResource("MahApps.Styles.ToggleButton.ComboBoxDropDown.Win10")));
        }

        [TestCase("MahApps.Styles.ColorPicker")]
        [TestCase("MahApps.Styles.ColorPicker.Win10")]
        [Description("A set that draws no edge of its own says the focus with the frame, the way the picker always has.")]
        public void WithoutAnEdgeTheFrameSaysTheFocus(string key)
        {
            var picker = this.Focused(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(picker)), "the frame should take the focus brush");
                    Assert.That(Edge(picker).BorderBrush, Is.Null, "while nothing is drawn along the bottom edge");
                });
        }

        [Test]
        [Description("The WinUI picker says it with that edge instead, and the frame behind it stays the colour it was.")]
        public void TheWinUIEdgeSaysTheFocus()
        {
            var picker = this.Focused("MahApps.Styles.ColorPicker.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(picker).BorderBrush, Is.SameAs(ControlsHelper.GetFocusBorderBrush(picker)), "the bottom edge should take the focus brush");
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(picker.BorderBrush), "and the frame should be left alone");
                });
        }

        [Test]
        [Description("That edge is two units either way, so a picker with the caret in it is as tall as the one beside it and nothing under it moves.")]
        public void TheWinUIEdgeIsTheSameTwoUnitsEitherWay()
        {
            var idle = this.Show("MahApps.Styles.ColorPicker.WinUI");
            var idleEdge = Edge(idle).BorderThickness;

            var focused = this.Focused("MahApps.Styles.ColorPicker.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(idleEdge, Is.EqualTo(new Thickness(0, 0, 0, 2)), "at rest");
                    Assert.That(Edge(focused).BorderThickness, Is.EqualTo(idleEdge), "and with the caret in it");
                });
        }

        [TestCase("MahApps.Styles.ColorPicker.Win10")]
        [TestCase("MahApps.Styles.ColorPicker.WinUI")]
        [Description("A picker of these sets that is switched off says so with its colours rather than with a veil over it.")]
        public void ASwitchedOffPickerSaysSoWithItsColours(string key)
        {
            var picker = this.Show(key);
            picker.IsEnabled = false;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(picker), Is.EqualTo(Visibility.Collapsed), "no veil");
                    Assert.That(Frame(picker).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(picker)), "the frame goes grey");
                    Assert.That(Edge(picker).BorderBrush, Is.SameAs(ControlsHelper.GetDisabledBorderBrush(picker)), "and so does the edge along the bottom");
                });
        }

        [Test]
        [Description("The Metro picker keeps the veil it has always drawn over itself.")]
        public void TheMetroPickerKeepsItsVeil()
        {
            var picker = this.Show("MahApps.Styles.ColorPicker");
            picker.IsEnabled = false;
            this.Settle();

            var veil = (Border)Part(picker, "DisabledVisualElement");

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(picker), Is.EqualTo(Visibility.Visible));
                    Assert.That(veil.Opacity, Is.EqualTo(0.6).Within(0.001), "and it should be drawn over the picker");
                });
        }

        private ColorPicker Focused(string key)
        {
            var picker = this.Show(key);

            picker.Focus();
            Keyboard.Focus(picker);
            this.Settle();

            // a build agent hands the keyboard to one window at a time
            Assume.That(picker.IsKeyboardFocusWithin, Is.True);

            return picker;
        }

        private ColorPicker Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var picker = new ColorPicker
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             SelectedColor = Colors.SteelBlue,
                             Width = 280,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            this.window!.Content = picker;
            this.Settle();

            return picker;
        }

        private static Border Frame(ColorPicker picker)
        {
            return (Border)Part(picker, "Border");
        }

        private static Border Edge(ColorPicker picker)
        {
            return (Border)Part(picker, "BottomEdge");
        }

        private static FrameworkElement Part(ColorPicker picker, string name)
        {
            var part = picker.Template?.FindName(name, picker) as FrameworkElement;
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
