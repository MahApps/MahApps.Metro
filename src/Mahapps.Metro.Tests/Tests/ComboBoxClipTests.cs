// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The combo box rounds the box, the drop down around the list and every item in it. A clip lives in
    /// the coordinates of the element carrying it, so each geometry has to match that element rather
    /// than the border around it.
    /// </summary>
    [TestFixture]
    public class ComboBoxClipTests
    {
        private TestWindow? window;
        private ResourceDictionary? dictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.ComboBox.xaml", UriKind.Absolute) };
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private ComboBox ShowComboBox()
        {
            Assert.That(this.window, Is.Not.Null);

            var comboBox = new ComboBox { Width = 200, Height = 32 };
            comboBox.SetValue(FrameworkElement.StyleProperty, this.dictionary!["MahApps.Styles.ComboBox"]);
            comboBox.Items.Add("Beam me up...");
            comboBox.Items.Add("Warp nine");
            ControlsHelper.SetCornerRadius(comboBox, new CornerRadius(12));

            this.window!.Content = comboBox;
            this.window.UpdateLayout();
            comboBox.UpdateLayout();

            // Let the box finish loading before a test reaches into its template.
            ClipAssert.Pump();
            Assert.That(comboBox.IsLoaded, Is.True, "the box should be loaded before a test looks at it");

            return comboBox;
        }

        private ComboBoxItem ShowItem()
        {
            Assert.That(this.window, Is.Not.Null);

            var item = new ComboBoxItem { Width = 200, Height = 32, Content = "Beam me up..." };
            item.SetValue(FrameworkElement.StyleProperty, this.dictionary!["MahApps.Styles.ComboBoxItem"]);
            ControlsHelper.SetCornerRadius(item, new CornerRadius(8));

            this.window!.Content = item;
            this.window.UpdateLayout();
            item.UpdateLayout();

            return item;
        }

        /// <summary>
        /// Takes the content of the drop down out of its popup and lays it out in the window instead.
        /// Opening the popup for real is not something a test can rely on: the box only opens while it
        /// holds the mouse capture, and a popup closes again when its window loses activation, which
        /// happens all the time while four test hosts share one desktop. The geometry inside is the same
        /// either way, and the element names the clip binds to travel with it.
        /// </summary>
        private Border ShowDropDownContent(ComboBox comboBox)
        {
            var popup = comboBox.FindChild<Popup>("PART_Popup");
            Assert.That(popup, Is.Not.Null, "the template should carry the popup");

            var content = popup!.Child as FrameworkElement;
            Assert.That(content, Is.Not.Null, "the popup should carry its content");

            popup.Child = null;

            var host = new Grid { Width = 200 };
            host.Children.Add(content);

            this.window!.Content = host;
            this.window.UpdateLayout();
            host.UpdateLayout();
            ClipAssert.Pump();

            var border = content!.FindChild<Border>("PopupBorder") ?? content as Border;
            Assert.That(border, Is.Not.Null, "the drop down should sit in a border");

            return border!;
        }

        [Test]
        public void TheBoxShouldRoundItsFrameAndInsetItsContent()
        {
            var comboBox = this.ShowComboBox();

            var border = comboBox.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the template should carry the border");
            Assert.That(border!.CornerRadius, Is.EqualTo(new CornerRadius(12)), "the frame should take the corner radius of the box");

            // The box itself does not clip: its frame is a border of its own and the text and the buttons
            // sit next to it in a grid inset by the border thickness. Only the drop down and the items
            // carry a clip.
            var textBox = comboBox.FindChild<TextBox>("PART_EditableTextBox");
            var content = (textBox as FrameworkElement) ?? comboBox.FindChild<ContentPresenter>(string.Empty);
            Assert.That(content, Is.Not.Null, "the box should carry its content");
            Assert.That(content!.Clip, Is.Null, "the content of the box is inset rather than clipped");
        }

        [Test]
        public void TheDropDownShouldClipItsContentToItsBorder()
        {
            var comboBox = this.ShowComboBox();
            var popupBorder = this.ShowDropDownContent(comboBox);

            var grid = ClipAssert.ContentGrid(popupBorder);

            ClipAssert.CoversElement(grid, "drop down");
        }

        [Test]
        public void TheDropDownShouldTakeItsBackgroundFromItsOwnThemeKey()
        {
            var comboBox = this.ShowComboBox();
            var popupBorder = this.ShowDropDownContent(comboBox);

            var expected = comboBox.TryFindResource("MahApps.Brushes.ComboBox.PopupBackground") as Brush;
            Assert.That(expected, Is.Not.Null, "the theme should carry a background of its own for this drop down");
            Assert.That(popupBorder.Background, Is.SameAs(expected), "the drop down should be recolourable without touching the theme background of everything else");
        }

        [Test]
        public void AnItemShouldClipItsContentToItsBorder()
        {
            var item = this.ShowItem();

            var grid = ClipAssert.ContentGrid(item);

            ClipAssert.CoversElement(grid, "item");
            ClipAssert.CutsEveryCorner(grid, "item");
        }

        [Test]
        public void AnItemWithoutACornerRadiusShouldKeepTheWholeRectangle()
        {
            var item = this.ShowItem();

            ControlsHelper.SetCornerRadius(item, new CornerRadius(0));
            item.UpdateLayout();

            ClipAssert.KeepsEveryCorner(ClipAssert.ContentGrid(item), "item");
        }
    }
}
