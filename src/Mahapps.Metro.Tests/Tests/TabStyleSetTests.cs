// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two WinUI readings of a row of tabs. One is the TabView, where the tab showing is a card
    /// filled like the page under it; the other is the SelectorBar, where it is a short bar of the
    /// accent under the header. What is measured here is the thing that tells them apart: the card
    /// has no room for an underline and says nothing about the kind asked for, the bar is the
    /// underline and reads every one of them.
    /// </summary>
    [TestFixture]
    public class TabStyleSetTests : WindowTestFixture<TestWindow>
    {
        private static TabControl ARowOfTabs(string styleKey)
        {
            var tabs = new TabControl
                       {
                           Width = 400,
                           Height = 160,
                           Style = (Style)Application.Current.FindResource(styleKey)
                       };

            tabs.Items.Add(new TabItem { Header = "General", Content = "42" });
            tabs.Items.Add(new TabItem { Header = "Display", Content = "42" });

            return tabs;
        }

        private TabControl Show(TabControl tabs)
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = tabs;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return tabs;
        }

        private static Brush? FillOf(TabItem item, string partName)
        {
            return item.FindChild<Border>(partName)?.Background;
        }

        [Test]
        [Description("The tab showing is a card filled with the same colour as the page under it, which is what makes the two read as one surface.")]
        public void TheWinUICardIsFilledLikeThePageUnderIt()
        {
            var tabs = this.Show(ARowOfTabs("MahApps.Styles.TabControl.WinUI"));

            var showing = (TabItem)tabs.Items[0]!;
            var surface = Application.Current.FindResource("MahApps.Brushes.TabControl.WinUI.Background");

            Assert.Multiple(() =>
                {
                    Assert.That(FillOf(showing, "TabContainer"), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.TabItem.WinUI.HeaderBackgroundSelected")));
                    Assert.That(tabs.Background, Is.SameAs(surface));
                });
        }

        [TestCase(UnderlinedType.None)]
        [TestCase(UnderlinedType.SelectedTabItem)]
        [TestCase(UnderlinedType.TabItems)]
        [TestCase(UnderlinedType.TabPanel)]
        [Description("Whatever kind of underline is asked for, the card stays a card: that style has no room for one and leaves the question alone.")]
        public void TheCardSaysNothingAboutTheKindOfUnderline(UnderlinedType kind)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            TabControlHelper.SetUnderlined(tabs, kind);

            this.Show(tabs);

            var showing = (TabItem)tabs.Items[0]!;

            Assert.Multiple(() =>
                {
                    Assert.That(FillOf(showing, "TabContainer"), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.TabItem.WinUI.HeaderBackgroundSelected")));
                    Assert.That(showing.FindChild<Border>("Pill"), Is.Null, "the card carries no bar of its own");

                    // the line between the strip and the page is the edge of the page, so no answer
                    // to this question can take it away
                    Assert.That(tabs.BorderThickness, Is.EqualTo((Thickness)Application.Current.FindResource("MahApps.Thickness.TabControl.WinUI.PageBorder.Top")));
                    Assert.That(tabs.BorderBrush, Is.SameAs(Application.Current.FindResource("MahApps.Brushes.TabControl.WinUI.BorderBrush")));
                });
        }

        [TestCase(Dock.Top, "MahApps.Thickness.TabControl.WinUI.PageBorder.Top")]
        [TestCase(Dock.Bottom, "MahApps.Thickness.TabControl.WinUI.PageBorder.Bottom")]
        [TestCase(Dock.Left, "MahApps.Thickness.TabControl.WinUI.PageBorder.Left")]
        [TestCase(Dock.Right, "MahApps.Thickness.TabControl.WinUI.PageBorder.Right")]
        [Description("The line runs along whichever edge of the page the strip is on, so that the card of the tab showing always sits on it.")]
        public void TheLineGoesAlongTheEdgeTheStripIsOn(Dock placement, string expected)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            tabs.SetCurrentValue(TabControl.TabStripPlacementProperty, placement);

            this.Show(tabs);

            Assert.That(tabs.BorderThickness, Is.EqualTo((Thickness)Application.Current.FindResource(expected)));
        }

        [Test]
        [Description("An underline placement of its own moves that line off the edge the strip is on, which is the one thing about the underline the card does read.")]
        public void AnUnderlinePlacementMovesTheLine()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            TabControlHelper.SetUnderlinePlacement(tabs, Dock.Bottom);

            this.Show(tabs);

            Assert.That(tabs.BorderThickness, Is.EqualTo((Thickness)Application.Current.FindResource("MahApps.Thickness.TabControl.WinUI.PageBorder.Bottom")));
        }

        [TestCase(Dock.Top, "MahApps.Thickness.TabItem.WinUI.SelectedOverlap.Top")]
        [TestCase(Dock.Bottom, "MahApps.Thickness.TabItem.WinUI.SelectedOverlap.Bottom")]
        [TestCase(Dock.Left, "MahApps.Thickness.TabItem.WinUI.SelectedOverlap.Left")]
        [TestCase(Dock.Right, "MahApps.Thickness.TabItem.WinUI.SelectedOverlap.Right")]
        [Description("The card of the tab showing reaches a unit over the line of the page, so that the line stops where the card begins and the two read as one surface.")]
        public void TheCardOfTheTabShowingReachesOverTheLine(Dock placement, string expected)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            tabs.SetCurrentValue(TabControl.TabStripPlacementProperty, placement);

            this.Show(tabs);

            var showing = ((TabItem)tabs.Items[0]!).FindChild<Border>("TabContainer");

            Assert.That(showing, Is.Not.Null);
            Assert.That(showing!.Margin, Is.EqualTo((Thickness)Application.Current.FindResource(expected)));
        }

        [Test]
        [Description("A tab of the WinUI set is at least as wide as the TabView makes one, so a row of short headers reads as a row of tabs rather than as a line of words.")]
        public void ATabOfTheCardIsNoNarrowerThanTheTabViewMakesIt()
        {
            var tabs = this.Show(ARowOfTabs("MahApps.Styles.TabControl.WinUI"));

            var wanted = (double)Application.Current.FindResource("MahApps.Size.TabItem.WinUI.MinWidth");

            Assert.That(((TabItem)tabs.Items[0]!).ActualWidth, Is.GreaterThanOrEqualTo(wanted));
        }

        [Test]
        [Description("The card shares the room out between its tabs, so every one of them is the same width and the row fills the strip.")]
        public void TheCardSharesTheRoomOutBetweenItsTabs()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            tabs.Items.Add(new TabItem { Header = "A header that is a good deal longer", Content = "42" });

            this.Show(tabs);

            var widths = tabs.Items.OfType<TabItem>().Select(item => item.ActualWidth).ToArray();

            Assert.Multiple(() =>
                {
                    Assert.That(TabControlHelper.GetTabWidthMode(tabs), Is.EqualTo(TabWidthMode.Equal));
                    Assert.That(widths, Is.All.EqualTo(widths[0]).Within(0.5d), "the tabs do not share the room out evenly");
                    Assert.That(widths.Sum(), Is.EqualTo(tabs.Width).Within(1d), "the row does not fill the strip");
                });
        }

        [Test]
        [Description("What one tab gets stays between the floor and the ceiling the TabView holds it to, so two tabs in a wide control do not become two halves of it.")]
        public void TheSharedWidthStaysBetweenTheFloorAndTheCeiling()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            tabs.Width = 1200d;

            this.Show(tabs);

            var ceiling = (double)Application.Current.FindResource("MahApps.Size.TabItem.WinUI.MaxWidth");

            Assert.That(((TabItem)tabs.Items[0]!).ActualWidth, Is.EqualTo(ceiling).Within(0.5d));
        }

        [Test]
        [Description("Nobody else is asked to share anything out: a tab control the library has always drawn is still as wide as what is written on its tabs.")]
        public void TheLookThatHasAlwaysBeenHereStillFollowsItsContent()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl");

            this.Show(tabs);

            var widths = tabs.Items.OfType<TabItem>().Select(item => item.ActualWidth).ToArray();

            Assert.Multiple(() =>
                {
                    Assert.That(TabControlHelper.GetTabWidthMode(tabs), Is.EqualTo(TabWidthMode.SizeToContent));
                    Assert.That(widths[0], Is.Not.EqualTo(widths[1]).Within(0.5d), "two headers of different length came out the same width");
                    Assert.That(widths.Sum(), Is.LessThan(tabs.Width), "the row filled the strip, which only the shared out one does");
                });
        }

        [TestCase(Dock.Left)]
        [TestCase(Dock.Right)]
        [Description("A strip down either side has no room to share out, so the panel leaves that layout to the one it stands on.")]
        public void AStripDownTheSideIsLeftAlone(Dock placement)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI");
            tabs.SetCurrentValue(TabControl.TabStripPlacementProperty, placement);

            this.Show(tabs);

            var panel = tabs.FindChild<TabPanelEx>("HeaderPanel");
            var heights = tabs.Items.OfType<TabItem>().Select(item => item.ActualHeight).ToArray();

            Assert.That(panel, Is.Not.Null, "the strip is laid out by another panel than the one this is about");
            Assert.That(heights.Sum(), Is.LessThan(tabs.Height), "the tabs were stretched down the side");
        }

        [TestCase(UnderlinedType.None, false, false)]
        [TestCase(UnderlinedType.SelectedTabItem, false, true)]
        [TestCase(UnderlinedType.TabItems, true, true)]
        [TestCase(UnderlinedType.TabPanel, true, true)]
        [Description("The SelectorBar is the style that reads the kind of underline: none leaves the bar off, the selected item is its own behaviour, and the other two put one under every header.")]
        public void TheSelectorBarReadsTheKindOfUnderline(UnderlinedType kind, bool underTheOthers, bool underTheOneShowing)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI.SelectorBar");
            TabControlHelper.SetUnderlined(tabs, kind);

            this.Show(tabs);

            var showing = ((TabItem)tabs.Items[0]!).FindChild<Border>("Pill");
            var other = ((TabItem)tabs.Items[1]!).FindChild<Border>("Pill");

            Assert.Multiple(() =>
                {
                    Assert.That(showing, Is.Not.Null);
                    Assert.That(other, Is.Not.Null);
                    Assert.That(IsOn(showing), Is.EqualTo(underTheOneShowing), "the bar under the tab showing");
                    Assert.That(IsOn(other), Is.EqualTo(underTheOthers), "the bar under the tabs that are not");
                });

            static bool IsOn(Border? pill)
            {
                return pill is not null && pill.Visibility == Visibility.Visible && pill.Opacity > 0d;
            }
        }

        [Test]
        [Description("The bar under the tab showing is the accent, and the one under the others is the quieter brush, so the two never read alike.")]
        public void TheSelectorBarKeepsTheAccentForTheTabShowing()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI.SelectorBar");
            TabControlHelper.SetUnderlined(tabs, UnderlinedType.TabItems);

            this.Show(tabs);

            Assert.Multiple(() =>
                {
                    Assert.That(FillOf((TabItem)tabs.Items[0]!, "Pill"), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.TabItem.WinUI.SelectorBar.PillFill")));
                    Assert.That(FillOf((TabItem)tabs.Items[1]!, "Pill"), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.TabItem.WinUI.SelectorBar.PillFillPointerOver")));
                });
        }

        [TestCase("MahApps.Styles.TabControl.WinUI")]
        [TestCase("MahApps.Styles.TabControl.WinUI.SelectorBar")]
        [Description("The ground of a header is transparent rather than unpainted, so a click anywhere in it picks that tab instead of only one on the letters.")]
        public void TheWholeHeaderAnswersThePointer(string styleKey)
        {
            var tabs = this.Show(ARowOfTabs(styleKey));
            var item = (TabItem)tabs.Items[1]!;

            var justInside = item.TranslatePoint(new Point(2d, 2d), tabs);
            var hit = tabs.InputHitTest(justInside) as DependencyObject;

            Assert.That(hit, Is.Not.Null, "nothing at all under that point");
            Assert.That(SitsIn(hit!, item), Is.True, "the point is in the header but belongs to no part of that tab");

            static bool SitsIn(DependencyObject hit, DependencyObject item)
            {
                for (DependencyObject? node = hit; node is not null; node = VisualTreeHelper.GetParent(node))
                {
                    if (ReferenceEquals(node, item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [TestCase(null, 2, 1)]
        [TestCase(Dock.Top, 0, 1)]
        [TestCase(Dock.Bottom, 2, 1)]
        [TestCase(Dock.Left, 1, 0)]
        [TestCase(Dock.Right, 1, 2)]
        [Description("The bar goes to whichever side was asked for, and with nothing asked for it takes the one under the header, which is where a strip along the top wants it.")]
        public void TheSelectorBarPutsItsBarWhereTheUnderlineWasPlaced(Dock? placement, int row, int column)
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI.SelectorBar");
            TabControlHelper.SetUnderlinePlacement(tabs, placement);

            this.Show(tabs);

            var pill = ((TabItem)tabs.Items[0]!).FindChild<Border>("Pill");

            Assert.That(pill, Is.Not.Null);
            Assert.Multiple(() =>
                {
                    Assert.That(Grid.GetRow(pill!), Is.EqualTo(row));
                    Assert.That(Grid.GetColumn(pill!), Is.EqualTo(column));

                    // a bar down either side lies the other way round
                    var lyingFlat = placement is null or Dock.Top or Dock.Bottom;
                    Assert.That(pill!.Width > pill.Height, Is.EqualTo(lyingFlat));
                });
        }

        [Test]
        [Description("The margin of the underline moves the bar, the same property that moves the line of the shared tab.")]
        public void TheSelectorBarReadsTheMarginOfTheUnderline()
        {
            var tabs = ARowOfTabs("MahApps.Styles.TabControl.WinUI.SelectorBar");
            TabControlHelper.SetUnderlineMargin(tabs, new Thickness(0d, 0d, 0d, 6d));

            this.Show(tabs);

            Assert.That(((TabItem)tabs.Items[0]!).FindChild<Border>("Pill")?.Margin, Is.EqualTo(new Thickness(0d, 0d, 0d, 6d)));
        }

        [TestCase("MahApps.Styles.TabControl", Visibility.Hidden)]
        [TestCase("MahApps.Styles.TabControl.WinUI", Visibility.Visible)]
        [TestCase("MahApps.Styles.TabControl.WinUI.SelectorBar", Visibility.Visible)]
        [Description("TabControlHelper is where a plain tab says it can be closed, and every set reads it. The tab showing always has its button; on the others the WinUI sets show it as well, the way the TabView does, while the look this library has always drawn waits for the pointer.")]
        public void APlainTabCanBeClosedInEverySet(string styleKey, Visibility onTheOthers)
        {
            var tabs = ARowOfTabs(styleKey);
            TabControlHelper.SetCloseButtonEnabled(tabs, true);

            this.Show(tabs);

            var showing = (TabItem)tabs.Items[0]!;
            var other = (TabItem)tabs.Items[1]!;

            Assert.Multiple(() =>
                {
                    Assert.That(showing.FindChild<Button>("PART_CloseButton")?.Visibility, Is.EqualTo(Visibility.Visible), "the tab showing has no button");
                    Assert.That(other.FindChild<Button>("PART_CloseButton")?.Visibility, Is.EqualTo(onTheOthers), "the button of the other tab is not where this set puts it");
                });
        }

        [TestCase("MahApps.Styles.TabControl")]
        [TestCase("MahApps.Styles.TabControl.WinUI")]
        [TestCase("MahApps.Styles.TabControl.WinUI.SelectorBar")]
        [Description("The answer is inherited, so one on the tab control reaches every tab under it rather than only the one it was written on.")]
        public void OneAnswerOnTheControlReachesEveryTab(string styleKey)
        {
            var tabs = ARowOfTabs(styleKey);

            this.Show(tabs);

            Assert.That(tabs.Items.OfType<TabItem>().Select(item => item.FindChild<Button>("PART_CloseButton")?.Visibility),
                        Is.All.EqualTo(Visibility.Collapsed),
                        "a tab had a button before anybody asked for one");

            TabControlHelper.SetCloseButtonEnabled(tabs, true);
            ClipAssert.Pump();

            Assert.That(tabs.Items.OfType<TabItem>().Select(item => item.FindChild<Button>("PART_CloseButton")?.Visibility),
                        Is.All.Not.EqualTo(Visibility.Collapsed),
                        "a tab was left without a button");
        }

        [TestCase("MahApps.Styles.MetroTabControl.WinUI")]
        [TestCase("MahApps.Styles.MetroTabControl.WinUI.SelectorBar")]
        [Description("A tab that can be closed says so on itself rather than through TabControlHelper, so it hands its answer over, and the button of the WinUI tab is drawn from that.")]
        public void ATabThatCanBeClosedKeepsItsButtonInTheWinUISets(string styleKey)
        {
            Assert.That(this.window, Is.Not.Null);

            var tabs = new MetroTabControl
                       {
                           Width = 400,
                           Height = 160,
                           Style = (Style)Application.Current.FindResource(styleKey)
                       };

            var closeable = new MetroTabItem { Header = "notes", Content = "42", CloseButtonEnabled = true };
            tabs.Items.Add(closeable);
            tabs.Items.Add(new MetroTabItem { Header = "more notes", Content = "42" });

            this.window!.Content = tabs;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(TabControlHelper.GetCloseButtonEnabled(closeable), Is.True, "the tab hands its answer to the attached property");
                    Assert.That(closeable.FindChild<Button>("PART_CloseButton")?.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }
    }
}
