// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The two Windows lists. Both put the accent behind the row that is picked, Windows 10 turned
    /// right down and WinUI at full strength, and neither draws the bar along the left edge that the
    /// WinUI list view has. GH-3328 asked for the Windows looks.
    /// </summary>
    [TestFixture]
    public class ListBoxSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Win10 = "MahApps.Styles.ListBox.Win10";
        private const string WinUI = "MahApps.Styles.ListBox.WinUI";
        private const string RowWin10 = "MahApps.Styles.ListBoxItem.Win10";
        private const string RowWinUI = "MahApps.Styles.ListBoxItem.WinUI";

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
        [Description("Neither Windows set gives the row that is picked a colour of its own to write in, unlike the Metro one, which turns the text over to the colour that reads on the accent. What is behind the row carries the state on its own.")]
        public void TheRowThatIsPickedKeepsTheColourOfItsText(string key)
        {
            var row = Row(this.Show(key), 1);

            Assert.Multiple(() =>
                {
                    Assert.That(row.IsSelected, Is.True);
                    Assert.That(ItemHelper.GetSelectedForegroundBrush(row), Is.SameAs(row.Foreground), "the row that is picked");
                    Assert.That(ItemHelper.GetHoverForegroundBrush(row), Is.SameAs(row.Foreground), "the row under the pointer");
                });
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The base style says nothing about the two pressed states, which takes the fill off the row under the button rather than putting one there. Both Windows sets name a fill for them, the way UWP and WinUI both do.")]
        public void ARowUnderTheButtonIsFilledRatherThanCleared(string key)
        {
            var row = Row(this.Show(key), 0);

            Assert.Multiple(() =>
                {
                    Assert.That(ItemHelper.GetMouseLeftButtonPressedBackgroundBrush(row), Is.Not.Null, "the left button");
                    Assert.That(ItemHelper.GetMouseRightButtonPressedBackgroundBrush(row), Is.Not.Null, "the right button");
                });
        }

        [Test]
        [Description("The WinUI list box has no bar along the left edge, which is a thing of that set's list view. The row that is picked says so with the accent behind the whole of it.")]
        public void TheWinUIRowSaysItWithWhatIsBehindItInstead()
        {
            var row = Row(this.Show(WinUI), 1);

            Assert.Multiple(() =>
                {
                    Assert.That(row.FindChild<Rectangle>("Pill"), Is.Null);
                    Assert.That(ItemHelper.GetSelectedBackgroundBrush(row), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.ListBox.WinUI.ItemBackgroundSelected")));
                });
        }

        [Test]
        [Description("The Windows 10 row has no bar of its own. It says which one is picked with the accent turned right down behind the whole row, the way UWP does.")]
        public void TheWindows10RowSaysItWithWhatIsBehindItInstead()
        {
            var row = Row(this.Show(Win10), 1);

            Assert.Multiple(() =>
                {
                    Assert.That(row.FindChild<Rectangle>("Pill"), Is.Null);
                    Assert.That(ItemHelper.GetSelectedBackgroundBrush(row), Is.SameAs(Application.Current.FindResource("MahApps.Brushes.ListBox.Win10.ItemBackgroundSelected")));
                });
        }

        private static ListBoxItem Row(ListBox list, int index)
        {
            return (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(index);
        }

        private ListBox Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var list = new ListBox
                       {
                           Style = (Style)Application.Current.FindResource(key),
                           Width = 220,
                           HorizontalAlignment = HorizontalAlignment.Left,
                           VerticalAlignment = VerticalAlignment.Top
                       };

            foreach (var name in new[] { "Ada Lovelace", "Grace Hopper", "Alan Turing" })
            {
                list.Items.Add(new ListBoxItem { Content = name });
            }

            list.SelectedIndex = 1;

            this.window!.Content = list;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return list;
        }
    }
}
