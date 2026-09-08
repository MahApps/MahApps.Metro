// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

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
        private ResourceDictionary? colorEyeDropperDictionary;
        private ResourceDictionary? numericUpDownDictionary;
        private ResourceDictionary? multiSelectionComboBoxDictionary;
        private ResourceDictionary? metroWindowDictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.colorEyeDropperDictionary = Load("Themes/ColorPicker/ColorEyeDropper.xaml");
            this.numericUpDownDictionary = Load("Themes/NumericUpDown.xaml");
            this.multiSelectionComboBoxDictionary = Load("Themes/MultiSelectionComboBox.xaml");
            this.metroWindowDictionary = Load("Themes/MetroWindow.xaml");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private static ResourceDictionary Load(string path)
        {
            return new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/{path}", UriKind.Absolute) };
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
                new object[] { "MultiSelectionComboBoxItem", "ContentGrid" }
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
                    eyeDropper.SetValue(FrameworkElement.StyleProperty, this.colorEyeDropperDictionary!["MahApps.Styles.ColorEyeDropper"]);
                    return eyeDropper;

                case "NumericUpDown spin button":
                    var spinButton = new Button { Content = "+" };
                    spinButton.SetValue(FrameworkElement.StyleProperty, this.numericUpDownDictionary!["MahApps.Styles.Button.NumericUpDown.Spin"]);
                    return spinButton;

                case "MultiSelectionComboBoxItem":
                    var item = new ListBoxItem { Content = "Beam me up..." };
                    item.SetValue(FrameworkElement.StyleProperty, this.multiSelectionComboBoxDictionary!["MahApps.Styles.MultiSelectionComboBoxItem.CheckBox"]);
                    return item;

                default:
                    throw new ArgumentOutOfRangeException(nameof(what), what, "no such template in this fixture");
            }
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
                metroWindow.SetValue(Control.TemplateProperty, this.metroWindowDictionary![templateKey]);
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
