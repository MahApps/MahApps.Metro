// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
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
    /// GH-4549: the ScrollBar of a hamburger menu was nailed to Auto in the template and faded away
    /// with the pointer, so nothing told the reader that there is more of the menu below.
    ///
    /// GH-3568: and a pointer that is somewhere else leaves the reader with nothing at all. The
    /// menu now shows the panning indicator while it waits, two units at the edge of the items,
    /// which is narrow enough for a closed pane as well.
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

        private static Track IndicatorOf(HamburgerMenu menu)
        {
            var indicator = ListOf(menu, "ButtonsListView").FindChild<Track>("PanningIndicator");
            Assert.That(indicator, Is.Not.Null, "the menu items should be in a ScrollViewer with a panning indicator");

            return indicator!;
        }

        private static ScrollViewer ViewerOf(HamburgerMenu menu)
        {
            var viewer = ListOf(menu, "ButtonsListView").FindChild<ScrollViewer>();
            Assert.That(viewer, Is.Not.Null, "the menu items should be in a ScrollViewer");

            return viewer!;
        }

        /// <summary>
        /// A test has no pointer to move, so it writes the state a pointer would leave behind where
        /// WPF keeps it. A trigger reading IsMouseOver cannot tell the difference, and the storyboard
        /// it starts wants a clock rather than an empty queue.
        /// </summary>
        private static void PointAt(ScrollViewer viewer)
        {
            var key = typeof(UIElement).GetField("IsMouseOverPropertyKey", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as DependencyPropertyKey;
            Assert.That(key, Is.Not.Null, "WPF should keep IsMouseOver behind a read-only property key");

            viewer.SetValue(key!, true);
            ClipAssert.Pump();
            ClipAssert.Pump(400);
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
        [Description("What the reader sees instead: a line as wide as two units, as long as the thumb of the bar it stands for.")]
        public void ThePanningIndicatorTakesTheBarsPlace()
        {
            var menu = this.ShowMenu();

            var indicator = IndicatorOf(menu);

            Assert.That(indicator.Visibility, Is.EqualTo(System.Windows.Visibility.Visible));
            Assert.That(indicator.Opacity, Is.EqualTo(1d));
            Assert.That(indicator.Thumb!.ActualWidth, Is.EqualTo(2d).Within(0.5), "two units, or what a display that is not at 100 percent rounds them to");
            Assert.That(indicator.Thumb.ActualHeight, Is.GreaterThan(0d).And.LessThan(indicator.ActualHeight));
        }

        [Test]
        [Description("The line and the air beside it have to fit into the Track, or the thumb is pushed out of it and drawn nowhere.")]
        public void ThePanningIndicatorHasRoomForItsLine()
        {
            var menu = this.ShowMenu();

            var indicator = IndicatorOf(menu);
            var offset = indicator.Thumb!.TransformToAncestor(indicator).Transform(new Point(0, 0)).X;

            Assert.That(offset, Is.GreaterThanOrEqualTo(0d));
            Assert.That(offset + indicator.Thumb.ActualWidth, Is.LessThanOrEqualTo(indicator.ActualWidth));
        }

        [Test]
        [Description("And it stands where the thumb of the bar stands, so that nothing moves when the one gives way to the other.")]
        public void ThePanningIndicatorStandsWhereTheBarDoes()
        {
            var menu = this.ShowMenu();

            var line = IndicatorOf(menu).Thumb!;
            var thumb = BarOf(menu).FindChild<Thumb>();
            Assert.That(thumb, Is.Not.Null, "the bar should carry a thumb");

            var lineTop = line.TransformToAncestor(menu).Transform(new Point(0, 0)).Y;
            var thumbTop = thumb!.TransformToAncestor(menu).Transform(new Point(0, 0)).Y;

            Assert.That(lineTop, Is.EqualTo(thumbTop).Within(1), "a bar keeps the rows of its chevrons, and the line has to keep them as well");
        }

        [Test]
        [Description("And it says where in the menu the reader is, so it moves along with the items.")]
        public void ThePanningIndicatorFollowsTheItems()
        {
            var menu = this.ShowMenu();

            var viewer = ListOf(menu, "ButtonsListView").FindChild<ScrollViewer>();
            Assert.That(viewer, Is.Not.Null, "the menu items should be in a ScrollViewer");

            viewer!.ScrollToVerticalOffset(120);
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(IndicatorOf(menu).Value, Is.EqualTo(viewer.VerticalOffset));
            Assert.That(IndicatorOf(menu).Value, Is.GreaterThan(0d));
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
            Assert.That(IndicatorOf(menu).Opacity, Is.Zero, "and the indicator has nothing left to say");
        }

        [Test]
        [Description("A menu whose pane is closed is a strip of icons, and two units beside them still fit.")]
        public void AClosedPaneKeepsItsIconsAndSaysThereIsMore()
        {
            var menu = this.ShowMenu();

            menu.IsPaneOpen = false;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(BarOf(menu).Opacity, Is.Zero, "the bar would lie on the icons");
            Assert.That(IndicatorOf(menu).Opacity, Is.EqualTo(1d));
            Assert.That(IndicatorOf(menu).Thumb!.ActualWidth, Is.EqualTo(2d).Within(0.5));

            var right = IndicatorOf(menu).TransformToAncestor(menu).Transform(new Point(IndicatorOf(menu).ActualWidth, 0)).X;
            Assert.That(right, Is.LessThanOrEqualTo(menu.CompactPaneLength), "and the strip is all there is left of the pane to draw on");
        }

        [Test]
        [Description("An open pane has room for the bar, so the line gives way to it once the pointer is there.")]
        public void AnOpenPaneTradesItsLineForTheBar()
        {
            var menu = this.ShowMenu();

            PointAt(ViewerOf(menu));

            Assert.That(BarOf(menu).Opacity, Is.EqualTo(1d).Within(0.01));
            Assert.That(IndicatorOf(menu).Opacity, Is.EqualTo(0d).Within(0.01), "the two say the same thing, and only one of them at a time");
        }

        [Test]
        [Description("A closed pane trades them as well, and the bar it opens stands in the strip rather than at the edge of the items the clip takes away.")]
        public void AClosedPaneOpensItsBarInTheStrip()
        {
            var menu = this.ShowMenu();

            menu.IsPaneOpen = false;
            menu.UpdateLayout();
            ClipAssert.Pump();

            PointAt(ViewerOf(menu));

            var bar = BarOf(menu);
            Assert.That(bar.Opacity, Is.EqualTo(1d).Within(0.01), "the pointer opens the bar here too");
            Assert.That(IndicatorOf(menu).Opacity, Is.EqualTo(0d).Within(0.01), "and the line it stands for steps aside");

            var right = bar.TransformToAncestor(menu).Transform(new Point(bar.ActualWidth, 0)).X;
            Assert.That(right, Is.LessThanOrEqualTo(menu.CompactPaneLength), "the strip is all there is left of the pane to draw on");
        }

        [Test]
        [Description("Switched off, neither of the two takes any room.")]
        public void ADisabledScrollBarIsGone()
        {
            var menu = this.ShowMenu();

            menu.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(BarOf(menu).Visibility, Is.EqualTo(System.Windows.Visibility.Collapsed));
            Assert.That(IndicatorOf(menu).Visibility, Is.Not.EqualTo(System.Windows.Visibility.Visible), "a Track keeps out of the way by itself once there is nothing left to scroll");

            menu.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            menu.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(IndicatorOf(menu).Visibility, Is.EqualTo(System.Windows.Visibility.Visible), "and comes back with the scrolling");
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
