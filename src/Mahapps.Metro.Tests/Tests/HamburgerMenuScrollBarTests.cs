// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4549: the ScrollBar of a hamburger menu was nailed to Auto in the template and faded away
    /// with the pointer, so nothing told the reader that there is more of the menu below.
    /// </summary>
    [TestFixture]
    public class HamburgerMenuScrollBarTests
    {
        private TestWindow window = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        private HamburgerMenu ShowMenu()
        {
            var items = new HamburgerMenuItemCollection();
            foreach (var item in Enumerable.Range(1, 42).Select(number => new HamburgerMenuGlyphItem { Label = $"Item {number}" }))
            {
                items.Add(item);
            }

            var menu = new HamburgerMenu
                       {
                           Width = 400,
                           Height = 300,
                           IsPaneOpen = true,
                           ItemsSource = items,
                           OptionsItemsSource = new HamburgerMenuItemCollection { new HamburgerMenuGlyphItem { Label = "Settings" } }
                       };

            this.window.Content = menu;
            this.window.UpdateLayout();
            menu.UpdateLayout();
            ClipAssert.Pump();

            return menu;
        }

        private static ListBox ListOf(HamburgerMenu menu, string name)
        {
            var list = menu.FindChild<ListBox>(name);
            Assert.That(list, Is.Not.Null, $"the template should carry {name}");

            return list!;
        }

        private static ScrollBar BarOf(HamburgerMenu menu)
        {
            var bar = ListOf(menu, "ButtonsListView").FindChild<ScrollBar>("PART_VerticalScrollBar");
            Assert.That(bar, Is.Not.Null, "the menu items should be in a ScrollViewer with a vertical ScrollBar");

            return bar!;
        }

        [Test]
        [Description("Nobody said anything, so the menu behaves the way every menu from before does.")]
        public void TheScrollBarWaitsForThePointerByItself()
        {
            var menu = this.ShowMenu();

            Assert.That(menu.VerticalScrollBarVisibility, Is.EqualTo(ScrollBarVisibility.Auto));
            Assert.That(ScrollViewer.GetVerticalScrollBarVisibility(ListOf(menu, "ButtonsListView")), Is.EqualTo(ScrollBarVisibility.Auto));
            Assert.That(BarOf(menu).Opacity, Is.Zero, "and it is out of sight until the pointer is over the pane");
        }

        [Test]
        [Description("Asked for a ScrollBar, the reader gets one without having to go looking for it.")]
        public void AVisibleScrollBarStays()
        {
            var menu = this.ShowMenu();

            menu.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(ScrollViewer.GetVerticalScrollBarVisibility(ListOf(menu, "ButtonsListView")), Is.EqualTo(ScrollBarVisibility.Visible));
            Assert.That(BarOf(menu).Opacity, Is.EqualTo(1d), "the pointer has nothing to do with it");
        }

        [Test]
        [Description("A menu whose pane is closed is a strip of icons, and a ScrollBar over it would cover them.")]
        public void AClosedPaneKeepsItsIcons()
        {
            var menu = this.ShowMenu();

            menu.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            menu.IsPaneOpen = false;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(BarOf(menu).Opacity, Is.Zero);
        }

        [Test]
        [Description("Switched off, the ScrollBar takes no room at all.")]
        public void ADisabledScrollBarIsGone()
        {
            var menu = this.ShowMenu();

            menu.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(BarOf(menu).Visibility, Is.EqualTo(System.Windows.Visibility.Collapsed));
        }

        [Test]
        [Description("The options at the bottom have a ScrollBar of their own, off unless asked for.")]
        public void TheOptionsHaveTheirOwnScrollBar()
        {
            var menu = this.ShowMenu();

            Assert.That(menu.OptionsVerticalScrollBarVisibility, Is.EqualTo(ScrollBarVisibility.Disabled));
            Assert.That(ScrollViewer.GetVerticalScrollBarVisibility(ListOf(menu, "OptionsListView")), Is.EqualTo(ScrollBarVisibility.Disabled));

            menu.OptionsVerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(ScrollViewer.GetVerticalScrollBarVisibility(ListOf(menu, "OptionsListView")), Is.EqualTo(ScrollBarVisibility.Auto));
        }
    }
}
