// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The hamburger menu of the WinUI set, read off the NavigationView. What is held here is what
    /// that look comes down to: a row is a tile inset in the pane with a bar of the accent beside
    /// the one showing, the tile keeps to the strip while the pane is closed, and the content is a
    /// card rounded where it meets the pane.
    /// </summary>
    [TestFixture]
    public class HamburgerMenuWinUIStyleTests : WindowTestFixture<TestWindow>
    {
        // layout rounding puts an edge on the nearest device pixel, which moves it by up to one at a scale above 100 percent
        private const double Rounding = 1d;

        private HamburgerMenu Show(SplitViewDisplayMode mode = SplitViewDisplayMode.CompactInline, bool open = true, SplitViewPanePlacement placement = SplitViewPanePlacement.Left)
        {
            Assert.That(this.window, Is.Not.Null);

            var menu = new HamburgerMenu
                       {
                           Width = 480,
                           Height = 320,
                           DisplayMode = mode,
                           IsPaneOpen = open,
                           PanePlacement = placement,
                           Style = (Style)Application.Current.FindResource("MahApps.Styles.HamburgerMenu.WinUI"),
                           ItemsSource = new HamburgerMenuItemCollection
                                         {
                                             new HamburgerMenuHeaderItem { Label = "Mail" },
                                             new HamburgerMenuGlyphItem { Label = "Inbox" },
                                             new HamburgerMenuGlyphItem { Label = "Sent" }
                                         },
                           SelectedIndex = 1
                       };

            this.window!.Content = menu;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return menu;
        }

        private static ListBoxItem Row(HamburgerMenu menu, int index)
        {
            var list = menu.FindChild<HamburgerMenuListBox>("ButtonsListView");
            Assert.That(list, Is.Not.Null, "the template should carry the list of items");

            var row = list!.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
            Assert.That(row, Is.Not.Null, $"there should be a row at {index}");

            return row!;
        }

        [Test]
        [Description("The bar of the accent stands beside the row showing and beside no other, and it is part of the look rather than an option.")]
        public void TheRowShowingCarriesTheBarOfTheAccent()
        {
            var menu = this.Show();

            var showing = Row(menu, 1).FindChild<Rectangle>("SelectionIndicator");
            var other = Row(menu, 2).FindChild<Rectangle>("SelectionIndicator");

            Assert.Multiple(() =>
                {
                    Assert.That(menu.ShowSelectionIndicator, Is.True);
                    Assert.That(showing, Is.Not.Null);
                    Assert.That(showing!.Opacity, Is.EqualTo(1d));
                    Assert.That(showing.ActualWidth, Is.EqualTo(3d).Within(Rounding));
                    Assert.That(showing.ActualHeight, Is.EqualTo(16d).Within(Rounding));
                    Assert.That(other!.Opacity, Is.EqualTo(0d));
                });
        }

        [Test]
        [Description("A row is a tile four units in from either side of the pane and two from the rows above and below, forty units tall all told, which is the row of the NavigationView.")]
        public void ARowIsATileInsetInThePane()
        {
            var menu = this.Show();
            var row = Row(menu, 1);
            var tile = row.FindChild<Border>("Border");

            Assert.That(tile, Is.Not.Null);

            var origin = tile!.TranslatePoint(new Point(0, 0), row);

            Assert.Multiple(() =>
                {
                    Assert.That(row.ActualHeight, Is.EqualTo(40d).Within(Rounding));
                    Assert.That(origin.X, Is.EqualTo(4d).Within(Rounding));
                    Assert.That(origin.Y, Is.EqualTo(2d).Within(Rounding));
                    Assert.That(tile.ActualWidth, Is.EqualTo(row.ActualWidth - 8).Within(Rounding));
                    Assert.That(tile.CornerRadius, Is.EqualTo(new CornerRadius(4)));
                });
        }

        [TestCase(SplitViewPanePlacement.Left, HorizontalAlignment.Left)]
        [TestCase(SplitViewPanePlacement.Right, HorizontalAlignment.Right)]
        [Description("A closed pane is cut down to the compact strip, so the tile keeps to that strip, on the side of the pane the strip shows, rather than running on past the edge.")]
        public void AClosedPaneKeepsTheTileToTheStrip(SplitViewPanePlacement placement, HorizontalAlignment side)
        {
            var menu = this.Show(open: false, placement: placement);
            var host = Row(menu, 1).FindChild<Grid>("TileHost");

            Assert.Multiple(() =>
                {
                    Assert.That(host, Is.Not.Null);
                    Assert.That(host!.ActualWidth, Is.EqualTo(menu.CompactPaneLength).Within(Rounding));
                    Assert.That(host.HorizontalAlignment, Is.EqualTo(side));
                });
        }

        [Test]
        [Description("A header is forty units tall while the pane is open and takes no room at all while it is closed.")]
        public void AHeaderIsGoneWhileThePaneIsClosed()
        {
            var menu = this.Show();
            var header = Row(menu, 0);

            Assert.That(header.ActualHeight, Is.EqualTo(40d).Within(Rounding));

            menu.IsPaneOpen = false;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(header.ActualHeight, Is.EqualTo(0d));
        }

        [TestCase(SplitViewDisplayMode.CompactInline, true, SplitViewPanePlacement.Left, "8 0 0 0", "1 1 0 0")]
        [TestCase(SplitViewDisplayMode.CompactOverlay, false, SplitViewPanePlacement.Left, "8 0 0 0", "1 1 0 0")]
        [TestCase(SplitViewDisplayMode.CompactInline, true, SplitViewPanePlacement.Right, "0 8 0 0", "0 1 1 0")]
        [TestCase(SplitViewDisplayMode.Overlay, false, SplitViewPanePlacement.Left, "0", "0 1 0 0")]
        [TestCase(SplitViewDisplayMode.Inline, false, SplitViewPanePlacement.Left, "0", "0 1 0 0")]
        [TestCase(SplitViewDisplayMode.Inline, true, SplitViewPanePlacement.Left, "8 0 0 0", "1 1 0 0")]
        [Description("The content is a card rounded where it meets the pane, and only edged along the top where no pane stands beside it.")]
        public void TheContentIsACardBesideThePane(SplitViewDisplayMode mode, bool open, SplitViewPanePlacement placement, string corner, string edge)
        {
            var menu = this.Show(mode, open, placement);
            var card = menu.FindChild<Border>("ContentBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(card, Is.Not.Null);
                    Assert.That(card!.CornerRadius, Is.EqualTo(new CornerRadiusConverter().ConvertFromInvariantString(corner)));
                    Assert.That(card.BorderThickness, Is.EqualTo(new ThicknessConverter().ConvertFromInvariantString(edge)));
                    Assert.That(card.Background, Is.SameAs(Application.Current.FindResource("MahApps.Brushes.HamburgerMenu.WinUI.ContentBackground")));
                });
        }

        [Test]
        [Description("The menu finds the ring for a row by key, and the WinUI style hands over its own under that key: a rounded ring around the tile rather than a square one around the row.")]
        public void TheRingOfARowIsTheOneOfTheSet()
        {
            var menu = this.Show();

            var template = menu.ItemFocusVisualStyle?.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == Control.TemplateProperty)?.Value as ControlTemplate;
            var ring = template?.LoadContent() as Border;

            Assert.Multiple(() =>
                {
                    Assert.That(ring, Is.Not.Null, "the ring should be the rounded one of the set");
                    Assert.That(ring!.Margin, Is.EqualTo(new Thickness(4, 2, 4, 2)));
                    Assert.That(ring.CornerRadius, Is.EqualTo(new CornerRadius(4)));
                });
        }

        [Test]
        [Description("The button that opens the pane is the size of the one the NavigationView draws and sits where that one sits.")]
        public void TheButtonIsTheOneOfTheNavigationView()
        {
            var menu = this.Show();
            var button = menu.FindChild<Button>("HamburgerButton");

            Assert.That(button, Is.Not.Null);

            var origin = button!.TranslatePoint(new Point(0, 0), menu);

            Assert.Multiple(() =>
                {
                    Assert.That(button.ActualWidth, Is.EqualTo(40d).Within(Rounding));
                    Assert.That(button.ActualHeight, Is.EqualTo(36d).Within(Rounding));
                    Assert.That(origin.X, Is.EqualTo(4d).Within(Rounding));
                    Assert.That(origin.Y, Is.EqualTo(6d).Within(Rounding));
                    Assert.That(ControlsHelper.GetCornerRadius(button), Is.EqualTo(new CornerRadius(4)));
                });
        }
    }
}
