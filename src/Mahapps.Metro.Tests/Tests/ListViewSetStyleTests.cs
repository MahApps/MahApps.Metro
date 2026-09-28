// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two Windows list views. A list view is a list box with columns bolted on, and both
    /// Windows sets draw its rows the way they draw a list box: Windows 10 with the accent turned
    /// right down behind the row that is picked, WinUI with a rounded tile and the accent kept for
    /// a short bar along the left edge. GH-3328 asked for the Windows looks.
    /// </summary>
    [TestFixture]
    public class ListViewSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Win10 = "MahApps.Styles.ListView.Win10";
        private const string WinUI = "MahApps.Styles.ListView.WinUI";
        private const string Metro = "MahApps.Styles.ListView";
        private const string RowWin10 = "MahApps.Styles.ListViewItem.Win10";
        private const string RowWinUI = "MahApps.Styles.ListViewItem.WinUI";

        [TestCase(Win10, 32d)]
        [TestCase(WinUI, 40d)]
        [Description("A Windows 10 row is thirty two high, a WinUI one forty, which is what a list view draws and a good deal more room than the older set gives a line.")]
        public void EachSetDrawsItsRowItsOwnHeight(string key, double height)
        {
            var row = Row(this.Show(key), 0);

            Assert.That(row.ActualHeight, Is.EqualTo(height).Within(1d));
        }

        [TestCase(Win10, RowWin10)]
        [TestCase(WinUI, RowWinUI)]
        [Description("A list hands its rows the style of its own set, so that a list wearing one of these on its own looks right without the whole set merged behind it.")]
        public void AListHandsItsRowsTheStyleOfItsOwnSet(string key, string expected)
        {
            var row = Row(this.Show(key), 0);

            Assert.That(row.Style, Is.SameAs(Application.Current.FindResource(expected)));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Neither Windows set gives the row that is picked a colour of its own to write in, unlike the Metro one, and neither draws a line around it either. What is behind the row carries the state on its own.")]
        public void TheRowThatIsPickedKeepsTheColourOfItsTextAndHasNoLineAroundIt(string key)
        {
            var row = Row(this.Show(key), 1);

            Assert.Multiple(() =>
                {
                    Assert.That(row.IsSelected, Is.True);
                    Assert.That(ItemHelper.GetSelectedForegroundBrush(row), Is.SameAs(row.Foreground), "the row that is picked");
                    Assert.That(ItemHelper.GetHoverForegroundBrush(row), Is.SameAs(row.Foreground), "the row under the pointer");
                    Assert.That(ItemHelper.GetSelectedBorderBrush(row), Is.Null, "the line around the row that is picked");
                    Assert.That(ItemHelper.GetHoverBorderBrush(row), Is.Null, "the line around the row under the pointer");
                });
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Windows scrolls a list by the pixel rather than by the row, which is also what a group header needs to be held at the top of the list.")]
        public void TheWindowsListsScrollByThePixel(string key)
        {
            var list = this.Show(key);

            Assert.That(VirtualizingPanel.GetScrollUnit(list), Is.EqualTo(ScrollUnit.Pixel));
        }

        [Test]
        [Description("The bar along the left edge is the whole of what says which WinUI row is picked, so it shows on that row and on no other.")]
        public void TheBarOnTheLeftSaysWhichWinUIRowIsPicked()
        {
            var list = this.Show(WinUI);

            var picked = Row(list, 1).FindChild<Rectangle>("Pill");
            var rest = Row(list, 0).FindChild<Rectangle>("Pill");

            Assert.Multiple(() =>
                {
                    Assert.That(picked, Is.Not.Null);
                    Assert.That(picked!.Visibility, Is.EqualTo(Visibility.Visible), "the row that is picked");
                    Assert.That(picked.ActualWidth, Is.EqualTo(3d).Within(1d));
                    Assert.That(picked.ActualHeight, Is.EqualTo(16d).Within(1d));
                    Assert.That(rest, Is.Not.Null);
                    Assert.That(rest!.Visibility, Is.EqualTo(Visibility.Collapsed), "every other row");
                });
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The padding of a row sits on its content rather than on the frame around it, so that a row carrying a GridView keeps its cells under the headers over them rather than standing off to the right of them.")]
        public void ThePaddingOfARowLeavesTheColumnsWhereTheirHeadersAre(string key)
        {
            var list = this.Show(key, columns: true);
            var row = Row(list, 0);

            var presenter = row.FindChild<GridViewRowPresenter>("PART_RowPresenter");

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter!.TranslatePoint(new Point(0d, 0d), row).X, Is.EqualTo(0d).Within(0.5d));
        }

        [TestCase(Win10, 32d)]
        [TestCase(WinUI, 40d)]
        [Description("A column header stands as tall as a row of the same set, so the head of a list keeps the rhythm of what is under it.")]
        public void EachSetDrawsItsColumnHeaderAsTallAsARow(string key, double height)
        {
            var header = Headers(this.Show(key, columns: true)).First(one => one.Role == GridViewColumnHeaderRole.Normal);

            Assert.That(header.ActualHeight, Is.EqualTo(height).Within(1d));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The row of column headers ends in one more header, the one filling whatever width the columns leave over. It stands for no column, so the handle for pulling a column wider has nothing to pull there and would only show as a line of its own at the right edge.")]
        public void TheHeaderThatFillsTheRestCarriesNoHandle(string key)
        {
            var headers = Headers(this.Show(key, columns: true)).ToList();

            var filler = headers.SingleOrDefault(one => one.Role == GridViewColumnHeaderRole.Padding);
            var column = headers.First(one => one.Role == GridViewColumnHeaderRole.Normal);

            Assert.Multiple(() =>
                {
                    Assert.That(column.FindChild<Thumb>("PART_HeaderGripper"), Is.Not.Null, "a header standing for a column has one");
                    Assert.That(filler, Is.Not.Null);
                    Assert.That(filler!.FindChild<Thumb>("PART_HeaderGripper"), Is.Null, "the one filling the rest has none");
                });
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A box beside every row moves the cells of that row along, so the row of headers over them moves by the same amount. What the two stand apart by is what it was without the box, which is the little the cells of a row are inset by on their own.")]
        public void TheColumnsStayUnderTheirHeadersOnceTheBoxIsThere(string key)
        {
            var plain = Apart(this.Show(key, columns: true));
            var withBoxes = Apart(this.Show(key, columns: true, boxes: true));

            Assert.That(withBoxes, Is.EqualTo(plain).Within(1d));
        }

        /// <summary>How far the row of column headers stands from the cells under it.</summary>
        private static double Apart(ListView list)
        {
            var headers = list.FindChild<GridViewHeaderRowPresenter>();
            var cells = Row(list, 0).FindChild<GridViewRowPresenter>("PART_RowPresenter");

            Assert.That(headers, Is.Not.Null);
            Assert.That(cells, Is.Not.Null);

            return headers!.TranslatePoint(new Point(0d, 0d), list).X - cells!.TranslatePoint(new Point(0d, 0d), list).X;
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Windows writes the name of a column the way it was given. The Metro header shouts it in capitals, which is where this one parts company with it.")]
        public void TheWindowsColumnHeadersAreWrittenTheWayTheyWereGiven(string key)
        {
            var header = Headers(this.Show(key, columns: true)).First(one => one.Role == GridViewColumnHeaderRole.Normal);

            Assert.That(ControlsHelper.GetContentCharacterCasing(header), Is.EqualTo(CharacterCasing.Normal));
        }

        [Test]
        [Description("A group header is held at the top of the list while the rows of its own group pass under it, and it goes no further down than the bottom of that group, so the next group pushes it out of the way rather than being drawn over.")]
        public void TheHeaderOfAGroupIsHeldAtTheTopOfTheList()
        {
            var list = this.ShowGrouped(WinUI);

            var scrollViewer = list.FindChild<ScrollViewer>();
            Assert.That(scrollViewer, Is.Not.Null);

            var header = Header(list);
            Assert.That(header, Is.Not.Null);
            Assert.That(Offset(header!), Is.EqualTo(0d).Within(0.5d), "at rest the header stands where it was laid out");

            scrollViewer!.ScrollToVerticalOffset(40d);
            this.Settle();

            Assert.That(Offset(Header(list)!), Is.GreaterThan(0d), "once the group has gone past the top the header follows it down");
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Windows shows a box beside every row once the list lets more than one be picked. The box only says whether the row is picked, so it lets the pointer through to the row under it, and it says nothing at all while only one row can be picked.")]
        public void ARowShowsABoxOnceMoreThanOneCanBePicked(string key)
        {
            var quiet = Row(this.Show(key), 0).FindChild<CheckBox>("PART_CheckBox");

            Assert.That(quiet, Is.Not.Null);
            Assert.That(quiet!.Visibility, Is.EqualTo(Visibility.Collapsed), "a list of one pick at a time");

            var list = this.Show(key, boxes: true);
            var box = Row(list, 1).FindChild<CheckBox>("PART_CheckBox");

            Assert.Multiple(() =>
                {
                    Assert.That(box, Is.Not.Null);
                    Assert.That(box!.Visibility, Is.EqualTo(Visibility.Visible), "a list of several picks");
                    Assert.That(box.IsChecked, Is.True, "the row that is picked");
                    Assert.That(box.IsHitTestVisible, Is.False, "the box leaves the pointer to the row");
                    Assert.That(Row(list, 0).FindChild<CheckBox>("PART_CheckBox")!.IsChecked, Is.False, "a row that is not");
                });
        }

        [TestCase(Win10, SelectionMode.Single)]
        [TestCase(Win10, SelectionMode.Extended)]
        [TestCase(WinUI, SelectionMode.Single)]
        [TestCase(WinUI, SelectionMode.Extended)]
        [Description("The box goes again the moment the list stops letting more than one row be picked, even though the property asking for it is still on.")]
        public void TheBoxGoesAgainWhenOnlyOneRowCanBePicked(string key, SelectionMode mode)
        {
            var list = this.Show(key, boxes: true);
            var box = Row(list, 0).FindChild<CheckBox>("PART_CheckBox");

            Assert.That(box!.Visibility, Is.EqualTo(Visibility.Visible), "while several rows can be picked");

            list.SelectedItems.Clear();
            list.SelectionMode = mode;
            this.Settle();

            Assert.That(Row(list, 0).FindChild<CheckBox>("PART_CheckBox")!.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        [Description("The row of the default set shows the same box, so a list that never asked for a Windows look answers the property the same way and keeps its columns under their headers too.")]
        public void TheRowOfTheDefaultSetShowsTheBoxAsWell()
        {
            var box = Row(this.Show(Metro, boxes: true), 0).FindChild<CheckBox>("PART_CheckBox");

            Assert.That(box, Is.Not.Null);
            Assert.That(box!.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        [Description("The bar along the left edge and the box say the same thing, so the WinUI row drops the bar once the box is there.")]
        public void TheBarGivesWayToTheBox()
        {
            var row = Row(this.Show(WinUI, boxes: true), 1);

            Assert.Multiple(() =>
                {
                    Assert.That(row.IsSelected, Is.True);
                    Assert.That(row.FindChild<Rectangle>("Pill")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                });
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The box in a group header stands for every row of that group: ticking it picks them all and clearing it lets them all go.")]
        public void TheBoxInAGroupHeaderPicksTheWholeGroup(string key)
        {
            var list = this.ShowGrouped(key, boxes: true);
            var group = Group(list, 0);
            var items = ((CollectionViewGroup)group.Content).Items;

            GroupItemHelper.SetIsGroupSelected(group, true);

            Assert.That(items.All(item => list.SelectedItems.Contains(item)), Is.True, "every row of the group");

            GroupItemHelper.SetIsGroupSelected(group, false);

            Assert.That(items.Any(item => list.SelectedItems.Contains(item)), Is.False, "and none of them again");
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A group only some of whose rows are picked shows neither state, which is what the third state of a box is for.")]
        public void TheBoxInAGroupHeaderShowsNeitherStateWhileOnlySomeArePicked(string key)
        {
            var list = this.ShowGrouped(key, boxes: true);
            var group = Group(list, 0);

            this.PickTheRowsOfAGroupOneAtATime(list, group, () => GroupItemHelper.GetIsGroupSelected(group));
        }

        [TestCase(Win10, 32d)]
        [TestCase(WinUI, 40d)]
        [Description("A group opens on the rhythm its rows keep, so the header over it stands as tall as one row of the same set. It reads that height off the same resource the row does, which is what keeps the two from drifting apart.")]
        public void AGroupHeaderIsAsTallAsARow(string key, double height)
        {
            var header = Header(this.ShowGrouped(key));

            Assert.That(header, Is.Not.Null);
            Assert.That(header!.ActualHeight, Is.EqualTo(height).Within(1d));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Both group properties are asked for on the list and reach the groups by inheritance, which is what makes them one line to turn off.")]
        public void TheGroupPropertiesReachTheGroupsFromTheList(string key)
        {
            var held = this.ShowGrouped(key);

            Assert.Multiple(() =>
                {
                    Assert.That(GroupItemHelper.GetIsHeaderSticky(Group(held, 0)), Is.True, "the set holds its headers");
                    Assert.That(GroupItemHelper.GetCanSelectAllItems(Group(held, 0)), Is.True, "and lets a header pick its group");
                });

            var loose = this.ShowGrouped(key, sticky: false);
            var scrollViewer = loose.FindChild<ScrollViewer>();

            Assert.That(GroupItemHelper.GetIsHeaderSticky(Group(loose, 0)), Is.False);

            scrollViewer!.ScrollToVerticalOffset(40d);
            this.Settle();

            Assert.That(Offset(Header(loose)!), Is.EqualTo(0d).Within(0.5d), "a header let go of scrolls away with its group");
        }

        [TestCase(Win10, "MahApps.Styles.GroupItem.ListView.Win10")]
        [TestCase(WinUI, "MahApps.Styles.GroupItem.ListView.WinUI")]
        [Description("The set hands a grouped list the header that belongs to it, so that nothing has to be written for it in the application. It hands over a style rather than a whole GroupStyle, since a GroupStyle is no dependency object and would have to be built the moment anybody asked for it.")]
        public void TheSetHandsAGroupedListItsOwnHeader(string key, string expected)
        {
            var list = this.ShowGrouped(key);

            Assert.Multiple(() =>
                {
                    Assert.That(list.GroupStyle, Has.Count.EqualTo(1));
                    Assert.That(list.GroupStyle[0].ContainerStyle, Is.SameAs(Application.Current.FindResource(expected)));
                });
        }

        [TestCase(Win10, "MahApps.Styles.GroupItem.ListView.Win10", "MahApps.Styles.GroupItem.ListView.Win10.WithCheckBox")]
        [TestCase(WinUI, "MahApps.Styles.GroupItem.ListView.WinUI", "MahApps.Styles.GroupItem.ListView.WinUI.WithCheckBox")]
        [Description("The box in a group header is a second header rather than a trigger in the first, and the helper picks between the two. A trigger there would name an attached property by prefix, which costs a thrown exception every time WPF builds the style from inside a page.")]
        public void AGroupHeaderTakesTheBoxOnlyWhileTheWholeGroupCanBePicked(string key, string plain, string withBox)
        {
            var quiet = this.ShowGrouped(key);
            var quietGroup = Group(quiet, 0);

            Assert.Multiple(() =>
                {
                    Assert.That(quiet.GroupStyle[0].ContainerStyle, Is.SameAs(Application.Current.FindResource(plain)), "one row at a time");
                    Assert.That(quietGroup.Template.FindName("PART_CheckBox", quietGroup), Is.Null, "and no box over the group");
                });

            var picking = this.ShowGrouped(key, boxes: true);
            var pickingGroup = Group(picking, 0);

            Assert.Multiple(() =>
                {
                    Assert.That(picking.GroupStyle[0].ContainerStyle, Is.SameAs(Application.Current.FindResource(withBox)), "several rows at a time");
                    Assert.That(pickingGroup.Template.FindName("PART_CheckBox", pickingGroup), Is.Not.Null, "and a box over the group");
                });
        }

        [Test]
        [Description("The box over a group says what the group says. It is tied to that from the helper rather than in the template, for the same reason the two headers are two styles.")]
        public void TheBoxOverAGroupFollowsWhatIsPickedInIt()
        {
            var list = this.ShowGrouped(WinUI, boxes: true);
            var group = Group(list, 0);
            var box = (CheckBox)group.Template.FindName("PART_CheckBox", group);

            this.PickTheRowsOfAGroupOneAtATime(list, group, () => box.IsChecked);
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("An application that brings a header of its own keeps it. The set only hands one over where there is none.")]
        public void AListThatBringsItsOwnHeaderKeepsIt(string key)
        {
            var mine = new GroupStyle();
            var list = this.ShowGrouped(key, groupStyle: mine);

            Assert.Multiple(() =>
                {
                    Assert.That(list.GroupStyle, Has.Count.EqualTo(1));
                    Assert.That(list.GroupStyle[0], Is.SameAs(mine));
                });
        }

        private static double Offset(FrameworkElement header)
        {
            return header.RenderTransform is TranslateTransform transform ? transform.Y : 0d;
        }

        private static FrameworkElement? Header(ListView list)
        {
            var groupItem = Group(list, 0);

            return groupItem.Template?.FindName("PART_Header", groupItem) as FrameworkElement;
        }

        private static ListViewItem Row(ListView list, int index)
        {
            return (ListViewItem)list.ItemContainerGenerator.ContainerFromIndex(index);
        }

        /// <summary>
        /// Picks the rows of a group one at a time and reads the state after every step. The
        /// property on the group and the box in its header answer the same question, so both tests
        /// walk the same way and only differ in where they read the answer.
        /// </summary>
        private void PickTheRowsOfAGroupOneAtATime(ListView list, GroupItem group, Func<bool?> read)
        {
            var items = ((CollectionViewGroup)group.Content).Items;

            Assert.That(read(), Is.False, "nothing picked to begin with");

            list.SelectedItems.Add(items[0]);
            this.Settle();

            Assert.That(read(), Is.Null, "one of several picked");

            foreach (var item in items)
            {
                if (!list.SelectedItems.Contains(item))
                {
                    list.SelectedItems.Add(item);
                }
            }

            this.Settle();

            Assert.That(read(), Is.True, "all of them picked");
        }

        private ListView Show(string key, bool columns = false, bool boxes = false)
        {
            Assert.That(this.window, Is.Not.Null);

            var list = new ListView
                       {
                           Style = (Style)Application.Current.FindResource(key),
                           Width = 260,
                           Height = 160,
                           HorizontalAlignment = HorizontalAlignment.Left,
                           VerticalAlignment = VerticalAlignment.Top
                       };

            if (columns)
            {
                var header = key switch
                             {
                                 WinUI => "MahApps.Styles.GridViewColumnHeader.WinUI",
                                 Win10 => "MahApps.Styles.GridViewColumnHeader.Win10",
                                 _ => "MahApps.Styles.GridViewColumnHeader"
                             };
                var view = new GridView { ColumnHeaderContainerStyle = (Style)Application.Current.FindResource(header) };
                view.Columns.Add(new GridViewColumn { Header = "Name", Width = 140, DisplayMemberBinding = new Binding("Key") });
                view.Columns.Add(new GridViewColumn { Header = "Born", Width = 80, DisplayMemberBinding = new Binding("Value") });
                list.View = view;
                list.ItemsSource = Rows();
            }
            else
            {
                foreach (var name in new[] { "Ada Lovelace", "Grace Hopper", "Alan Turing" })
                {
                    list.Items.Add(new ListViewItem { Content = name });
                }
            }

            if (boxes)
            {
                list.SelectionMode = SelectionMode.Multiple;
                ItemHelper.SetIsMultiSelectCheckBoxEnabled(list, true);
            }

            list.SelectedIndex = 1;

            this.window!.Content = list;
            this.Settle();

            return list;
        }

        private ListView ShowGrouped(string key, bool boxes = false, bool sticky = true, GroupStyle? groupStyle = null)
        {
            Assert.That(this.window, Is.Not.Null);

            var view = new ListCollectionView(Rows());
            view.GroupDescriptions.Add(new PropertyGroupDescription("Key"));

            var list = new ListView
                       {
                           Style = (Style)Application.Current.FindResource(key),
                           DisplayMemberPath = "Value",
                           ItemsSource = view,
                           Width = 260,
                           Height = 140,
                           HorizontalAlignment = HorizontalAlignment.Left,
                           VerticalAlignment = VerticalAlignment.Top
                       };

            if (groupStyle is not null)
            {
                list.GroupStyle.Add(groupStyle);
            }

            if (boxes)
            {
                list.SelectionMode = SelectionMode.Multiple;
                ItemHelper.SetIsMultiSelectCheckBoxEnabled(list, true);
            }

            if (!sticky)
            {
                GroupItemHelper.SetIsHeaderSticky(list, false);
            }

            this.window!.Content = list;
            this.Settle();

            return list;
        }

        private static IEnumerable<GridViewColumnHeader> Headers(ListView list)
        {
            return list.FindChildren<GridViewColumnHeader>(true);
        }

        private static GroupItem Group(ListView list, int index)
        {
            // a group is in the visual tree only, so the logical walk of FindChildren goes straight past it
            return list.FindChildren<GroupItem>(true).ElementAt(index);
        }

        private static List<KeyValuePair<string, string>> Rows()
        {
            return new List<KeyValuePair<string, string>>
                   {
                       new("Pioneers", "Ada Lovelace"),
                       new("Pioneers", "Charles Babbage"),
                       new("Pioneers", "Konrad Zuse"),
                       new("Compilers", "Grace Hopper"),
                       new("Compilers", "John Backus"),
                       new("Theory", "Alan Turing"),
                       new("Theory", "Edsger Dijkstra")
                   };
        }

        private void Settle()
        {
            // twice: the first round puts the list in the tree, and only then do the bindings that
            // look up the tree for the list itself have anything to find.
            this.window!.UpdateLayout();
            ClipAssert.Pump();
            this.window.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
