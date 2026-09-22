// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-3532: a window that sizes itself to its content and carries window commands came up with
    /// the pane of its hamburger menu wide open, although nobody had opened it, and it took two
    /// clicks on the hamburger button to put it right.
    ///
    /// The pane column is set by the storyboard of the closed state, and the length it is set to
    /// came out of a binding that had nothing to give at that moment. So the column ended up with
    /// the share of the room that <see cref="GridLengthAnimation"/> made up for it, and the pane
    /// kept that share for the rest of the window's life.
    /// </summary>
    [TestFixture]
    public class SplitViewPaneColumnTests
    {
        /// <summary>
        /// The window of the report: it sizes itself to its content, it has a command on the right,
        /// and a hamburger menu is all it shows.
        /// </summary>
        public class SizeToContentWindow : MetroWindow
        {
            public SizeToContentWindow()
            {
                this.SizeToContent = SizeToContent.Height;
                this.RightWindowCommands = new WindowCommands { Items = { new Button { Content = "42" } } };
                this.Content = new HamburgerMenu
                               {
                                   ItemsSource = new HamburgerMenuItemCollection { new HamburgerMenuGlyphItem { Label = "Home" } },
                                   OptionsItemsSource = new HamburgerMenuItemCollection { new HamburgerMenuGlyphItem { Label = "Settings" } }
                               };
            }

            public HamburgerMenu Menu => (HamburgerMenu)this.Content;
        }

        private SizeToContentWindow window = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<SizeToContentWindow>().ConfigureAwait(true);
            this.window.Settle();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        private ColumnDefinition PaneColumn()
        {
            var splitView = this.window.Menu.FindChild<SplitView>("MainSplitView");
            Assert.That(splitView, Is.Not.Null, "the menu should carry the split view");

            var root = splitView!.FindChild<Grid>("root");
            Assert.That(root, Is.Not.Null, "the split view should carry the grid its pane sits in");

            return root!.ColumnDefinitions[0];
        }

        [Test]
        [Description("A menu nobody has opened shows the compact strip, not half the window.")]
        public void TheClosedPaneIsAsWideAsTheCompactStrip()
        {
            Assert.That(this.window.Menu.IsPaneOpen, Is.False, "nobody opened it");

            Assert.That(PaneColumn().Width, Is.EqualTo(new GridLength(this.window.Menu.CompactPaneLength)));
        }

        [Test]
        [Description("And the content starts where the strip ends.")]
        public void TheContentStartsAfterTheCompactStrip()
        {
            Assert.That(PaneColumn().ActualWidth, Is.EqualTo(this.window.Menu.CompactPaneLength).Within(0.01));
        }
    }
}
