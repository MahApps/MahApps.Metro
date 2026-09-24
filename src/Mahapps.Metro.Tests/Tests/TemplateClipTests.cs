// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The templates that round their corners and clip what they show to them, gathered in one place.
    /// A clip lives in the coordinates of the element carrying it, so each geometry has to match that
    /// element rather than the border around it.
    /// </summary>
    [TestFixture]
    public class TemplateClipTests
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

        /// <summary>
        /// The name of the element each template puts its clip on, and how to build a control that has it.
        /// </summary>
        private static object[] ClippedTemplates =>
            new object[]
            {
                new object[] { "DropDownButton", "ContentGrid" },
                new object[] { "SplitButton", "PART_Container" },
                new object[] { "SplitButton vertical", "PART_Container" },
                new object[] { "Underline", "ContentGrid" },
                new object[] { "ColorEyeDropper", "ContentGrid" },
                new object[] { "NumericUpDown spin button", "ContentGrid" },
                new object[] { "MultiSelectionComboBoxItem", "ContentGrid" },
                new object[] { "TextBox", "PART_InnerGrid" },
                new object[] { "TextBox Win10", "PART_InnerGrid" },
                new object[] { "TextBox WinUI", "PART_InnerGrid" },
                new object[] { "PasswordBox", "PART_InnerGrid" },
                new object[] { "PasswordBox revealed", "PART_InnerGrid" },
                new object[] { "PasswordBox Win10", "PART_InnerGrid" },
                new object[] { "PasswordBox WinUI", "PART_InnerGrid" },
                new object[] { "RichTextBox", "PART_InnerGrid" },
                new object[] { "RichTextBox Win10", "PART_InnerGrid" },
                new object[] { "RichTextBox WinUI", "PART_InnerGrid" },
                new object[] { "DatePicker", "PART_InnerGrid" },
                new object[] { "DatePicker Win10", "PART_InnerGrid" },
                new object[] { "DatePicker WinUI", "PART_InnerGrid" },
                new object[] { "ComboBox", "PART_InnerGrid" },
                new object[] { "ComboBox Win10", "PART_InnerGrid" },
                new object[] { "ComboBox WinUI", "PART_InnerGrid" },
                new object[] { "NumericUpDown", "PART_InnerGrid" },
                new object[] { "NumericUpDown Win10", "PART_InnerGrid" },
                new object[] { "NumericUpDown WinUI", "PART_InnerGrid" },
                new object[] { "AutoSuggestBox", "PART_InnerGrid" },
                new object[] { "AutoSuggestBox Win10", "PART_InnerGrid" },
                new object[] { "AutoSuggestBox WinUI", "PART_InnerGrid" },
                new object[] { "HotKeyBox", "PART_InnerGrid" },
                new object[] { "HotKeyBox Win10", "PART_InnerGrid" },
                new object[] { "HotKeyBox WinUI", "PART_InnerGrid" },
                new object[] { "MultiSelectionComboBox", "PART_InnerGrid" },
                new object[] { "MultiSelectionComboBox Win10", "PART_InnerGrid" },
                new object[] { "MultiSelectionComboBox WinUI", "PART_InnerGrid" },
                new object[] { "Chromeless button", "ContentGrid" }
            };

        private FrameworkElement Build(string what)
        {
            switch (what)
            {
                case "DropDownButton":
                    return new DropDownButton { Content = "Beam me up..." };

                case "SplitButton":
                    return new SplitButton();

                case "SplitButton vertical":
                    return new SplitButton { Orientation = Orientation.Vertical };

                case "Underline":
                    return new Underline();

                case "ColorEyeDropper":
                    var eyeDropper = new ColorEyeDropper();
                    eyeDropper.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource("MahApps.Styles.ColorEyeDropper"));
                    return eyeDropper;

                case "NumericUpDown spin button":
                    var spinButton = new Button { Content = "+" };
                    spinButton.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource("MahApps.Styles.Button.NumericUpDown.Spin"));
                    return spinButton;

                case "MultiSelectionComboBoxItem":
                    var item = new ListBoxItem { Content = "Beam me up..." };
                    item.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource("MahApps.Styles.MultiSelectionComboBoxItem.CheckBox"));
                    return item;

                case "TextBox":
                    return new TextBox { Text = "Beam me up..." };

                case "TextBox Win10":
                    return Styled(new TextBox { Text = "Beam me up..." }, "MahApps.Styles.TextBox.Win10");

                case "TextBox WinUI":
                    return Styled(new TextBox { Text = "Beam me up..." }, "MahApps.Styles.TextBox.WinUI");

                case "PasswordBox":
                    return new PasswordBox { Password = "Beam me up..." };

                case "PasswordBox revealed":
                    return Styled(new PasswordBox { Password = "Beam me up..." }, "MahApps.Styles.PasswordBox.Revealed");

                case "PasswordBox Win10":
                    return Styled(new PasswordBox { Password = "Beam me up..." }, "MahApps.Styles.PasswordBox.Win10");

                case "PasswordBox WinUI":
                    return Styled(new PasswordBox { Password = "Beam me up..." }, "MahApps.Styles.PasswordBox.WinUI");

                case "RichTextBox":
                    return Document(new RichTextBox());

                case "RichTextBox Win10":
                    return Document((RichTextBox)Styled(new RichTextBox(), "MahApps.Styles.RichTextBox.Win10"));

                case "RichTextBox WinUI":
                    return Document((RichTextBox)Styled(new RichTextBox(), "MahApps.Styles.RichTextBox.WinUI"));

                case "DatePicker":
                    return new DatePicker { SelectedDate = new DateTime(2026, 9, 23) };

                case "DatePicker Win10":
                    return Styled(new DatePicker { SelectedDate = new DateTime(2026, 9, 23) }, "MahApps.Styles.DatePicker.Win10");

                case "DatePicker WinUI":
                    return Styled(new DatePicker { SelectedDate = new DateTime(2026, 9, 23) }, "MahApps.Styles.DatePicker.WinUI");

                case "ComboBox":
                    return Filled(new ComboBox());

                case "ComboBox Win10":
                    return Filled((ComboBox)Styled(new ComboBox(), "MahApps.Styles.ComboBox.Win10"));

                case "ComboBox WinUI":
                    return Filled((ComboBox)Styled(new ComboBox(), "MahApps.Styles.ComboBox.WinUI"));

                case "NumericUpDown":
                    return new NumericUpDown { Value = 42 };

                case "NumericUpDown Win10":
                    return Styled(new NumericUpDown { Value = 42 }, "MahApps.Styles.NumericUpDown.Win10");

                case "NumericUpDown WinUI":
                    return Styled(new NumericUpDown { Value = 42 }, "MahApps.Styles.NumericUpDown.WinUI");

                case "AutoSuggestBox":
                    return Suggesting(new AutoSuggestBox());

                case "AutoSuggestBox Win10":
                    return Suggesting((AutoSuggestBox)Styled(new AutoSuggestBox(), "MahApps.Styles.AutoSuggestBox.Win10"));

                case "AutoSuggestBox WinUI":
                    return Suggesting((AutoSuggestBox)Styled(new AutoSuggestBox(), "MahApps.Styles.AutoSuggestBox.WinUI"));

                case "HotKeyBox":
                    return Shortcut(new HotKeyBox());

                case "HotKeyBox Win10":
                    return Shortcut((HotKeyBox)Styled(new HotKeyBox(), "MahApps.Styles.HotKeyBox.Win10"));

                case "HotKeyBox WinUI":
                    return Shortcut((HotKeyBox)Styled(new HotKeyBox(), "MahApps.Styles.HotKeyBox.WinUI"));

                case "MultiSelectionComboBox":
                    return Picking(new MultiSelectionComboBox());

                case "MultiSelectionComboBox Win10":
                    return Picking((MultiSelectionComboBox)Styled(new MultiSelectionComboBox(), "MahApps.Styles.MultiSelectionComboBox.Win10"));

                case "MultiSelectionComboBox WinUI":
                    return Picking((MultiSelectionComboBox)Styled(new MultiSelectionComboBox(), "MahApps.Styles.MultiSelectionComboBox.WinUI"));

                case "Chromeless button":
                    return Styled(new Button { Content = "Beam me up..." }, "MahApps.Styles.Button.Chromeless");

                default:
                    throw new ArgumentOutOfRangeException(nameof(what), what, "no such template in this fixture");
            }
        }

        /// <summary>
        /// A combo box with something to show, so that the row the clip has to cover is the one a box
        /// with a selection in it really has.
        /// </summary>
        private static ComboBox Filled(ComboBox box)
        {
            box.Items.Add("Beam me up...");
            box.Items.Add("Warp nine");
            box.SelectedIndex = 0;

            return box;
        }

        /// <summary>
        /// A suggestion box with something to suggest and something typed into it, so that the row
        /// the clip has to cover is the one a box being used really has.
        /// </summary>
        private static AutoSuggestBox Suggesting(AutoSuggestBox box)
        {
            box.ItemsSource = new[] { "Beam me up...", "Warp nine" };
            box.Text = "Beam me up...";

            return box;
        }

        /// <summary>
        /// A shortcut box with a shortcut in it and a button to take it away again, so that the row
        /// the clip has to cover is the one a box being used really has.
        /// </summary>
        private static HotKeyBox Shortcut(HotKeyBox box)
        {
            box.HotKey = new HotKey(Key.F, ModifierKeys.Control);
            TextBoxHelper.SetClearTextButton(box, true);

            return box;
        }

        /// <summary>
        /// A box with something picked in it, so that the row the clip has to cover is the one a
        /// box being used really has.
        /// </summary>
        private static MultiSelectionComboBox Picking(MultiSelectionComboBox box)
        {
            box.ItemsSource = new[] { "Beam me up...", "Warp nine" };
            box.SelectedItem = "Beam me up...";

            return box;
        }

        /// <summary>
        /// A rich text box takes its text as a document rather than as a string.
        /// </summary>
        private static RichTextBox Document(RichTextBox box)
        {
            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(new Paragraph(new Run("Beam me up...")));

            return box;
        }

        /// <summary>
        /// One of the styles the library itself merges, rather than a theme dictionary of its own.
        /// </summary>
        private static FrameworkElement Styled(FrameworkElement element, string key)
        {
            element.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource(key));

            return element;
        }

        private FrameworkElement Show(string what)
        {
            Assert.That(this.window, Is.Not.Null);

            var element = this.Build(what);
            element.Width = 200;
            element.Height = 48;
            ControlsHelper.SetCornerRadius(element, new CornerRadius(12));

            this.window!.Content = element;
            this.window.UpdateLayout();
            element.UpdateLayout();
            ClipAssert.Pump();

            return element;
        }

        [TestCaseSource(nameof(ClippedTemplates))]
        public void TheClipShouldCoverTheElementItSitsOn(string what, string gridName)
        {
            var element = this.Show(what);
            var grid = ClipAssert.ContentGrid(element, gridName);

            ClipAssert.CoversElement(grid, what);
        }

        [TestCaseSource(nameof(ClippedTemplates))]
        public void TheClipShouldCutEveryCorner(string what, string gridName)
        {
            var element = this.Show(what);
            var grid = ClipAssert.ContentGrid(element, gridName);

            ClipAssert.CutsEveryCorner(grid, what);
        }

        [TestCaseSource(nameof(ClippedTemplates))]
        public void WithoutACornerRadiusTheClipShouldKeepTheWholeRectangle(string what, string gridName)
        {
            var element = this.Show(what);

            // Some of these templates carry a corner radius of their own, so clearing the local value is
            // not the same as asking for square corners.
            ControlsHelper.SetCornerRadius(element, new CornerRadius(0));
            element.UpdateLayout();
            ClipAssert.Pump();

            ClipAssert.KeepsEveryCorner(ClipAssert.ContentGrid(element, gridName), what);
        }

        [TestCase("MahApps.Templates.MetroWindow")]
        [TestCase("MahApps.Templates.MetroWindow.Center")]
        public async Task AMetroWindowShouldClipItsContentToItsBorder(string templateKey)
        {
            var metroWindow = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);

            try
            {
                metroWindow.SetValue(Control.TemplateProperty, Application.Current.FindResource(templateKey));
                ControlsHelper.SetCornerRadius(metroWindow, new CornerRadius(12));
                metroWindow.UpdateLayout();
                ClipAssert.Pump();

                var grid = ClipAssert.ContentGrid(metroWindow);

                ClipAssert.CoversElement(grid, "window");
                ClipAssert.CutsEveryCorner(grid, "window");
            }
            finally
            {
                metroWindow.Close();
            }
        }

        [Test]
        public void TheDropDownOfAMultiSelectionComboBoxShouldClipItsContentToItsBorder()
        {
            Assert.That(this.window, Is.Not.Null);

            var comboBox = new MultiSelectionComboBox { Width = 200, Height = 32 };
            comboBox.Items.Add("Beam me up...");
            comboBox.Items.Add("Warp nine");

            this.window!.Content = comboBox;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            var popup = comboBox.FindChild<Popup>("PART_Popup");
            Assert.That(popup, Is.Not.Null, "the template should carry the popup");

            // The drop down is measured outside of its popup: a popup closes again when its window loses
            // activation, which is nothing a test run with several hosts on one desktop can rely on. The
            // geometry and the element names the clip binds to are the same either way.
            var content = popup!.Child as FrameworkElement;
            Assert.That(content, Is.Not.Null, "the popup should carry its content");
            popup.Child = null;

            var host = new Grid { Width = 220, Height = 120 };
            host.Children.Add(content);
            this.window.Content = host;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            var popupBorder = content!.FindChild<Border>("PopupBorder") ?? content as Border;
            Assert.That(popupBorder, Is.Not.Null, "the drop down should sit in a border");

            ClipAssert.CoversElement(ClipAssert.ContentGrid(popupBorder!), "drop down");
        }
    }
}
