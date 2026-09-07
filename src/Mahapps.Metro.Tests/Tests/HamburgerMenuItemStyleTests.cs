// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The item of a hamburger menu draws its background, its frame and the clip for its content in one
    /// border. These tests hold the template to what its triggers say, and to a clip that matches the
    /// element carrying it.
    /// </summary>
    [TestFixture]
    public class HamburgerMenuItemStyleTests
    {
        private static readonly SolidColorBrush Selected = new(Colors.Red);
        private static readonly SolidColorBrush Disabled = new(Colors.Lime);
        private static readonly SolidColorBrush DisabledSelected = new(Colors.Magenta);

        private static ListBoxItem CreateItem()
        {
            var dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Themes/HamburgerMenuTemplate.xaml", UriKind.Absolute) };

            var item = new ListBoxItem
                       {
                           Width = 200,
                           Height = 48,
                           Style = (Style)dictionary["MahApps.Styles.ListBoxItem.HamburgerMenuItem"],
                           Content = "Item"
                       };
            ControlsHelper.SetCornerRadius(item, new CornerRadius(12));
            ItemHelper.SetSelectedBackgroundBrush(item, Selected);
            ItemHelper.SetDisabledBackgroundBrush(item, Disabled);
            ItemHelper.SetDisabledSelectedBackgroundBrush(item, DisabledSelected);

            return item;
        }

        private static async Task<ListBoxItem> ShowItemAsync(TestWindow window)
        {
            var item = CreateItem();
            var listBox = new ListBox { Width = 220 };
            listBox.Items.Add(item);

            window.Content = listBox;
            window.UpdateLayout();
            listBox.UpdateLayout();
            item.UpdateLayout();

            await Task.Yield();

            return item;
        }

        private static Border GetBorder(ListBoxItem item)
        {
            var border = item.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the template should carry the border");

            return border!;
        }

        [Test]
        public async Task TheClipShouldCoverTheGridItSitsOn()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var item = await ShowItemAsync(window);
                var grid = ClipAssert.ContentGrid(item);

                ClipAssert.CoversElement(grid, "item");
                ClipAssert.CutsEveryCorner(grid, "item");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ASelectedItemShouldPaintTheBorderWithTheSelectedBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var item = await ShowItemAsync(window);

                item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
                item.UpdateLayout();

                Assert.That(Selector.GetIsSelectionActive(item), Is.False, "an invisible window has no focus, so the plain selected trigger is the one that applies here");
                Assert.That(GetBorder(item).Background, Is.SameAs(Selected), "the selected background belongs on the border that draws the item");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ADisabledItemShouldPaintTheBorderWithTheDisabledBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var item = await ShowItemAsync(window);

                item.SetCurrentValue(UIElement.IsEnabledProperty, false);
                item.UpdateLayout();

                Assert.That(GetBorder(item).Background, Is.SameAs(Disabled), "the disabled background belongs on the same border");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ADisabledAndSelectedItemShouldPaintTheBorderWithItsOwnBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var item = await ShowItemAsync(window);

                item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
                item.SetCurrentValue(UIElement.IsEnabledProperty, false);
                item.UpdateLayout();

                Assert.That(GetBorder(item).Background, Is.SameAs(DisabledSelected), "disabled and selected has a brush of its own, and it wins over the plain disabled one");
            }
            finally
            {
                window.Close();
            }
        }
    }
}
