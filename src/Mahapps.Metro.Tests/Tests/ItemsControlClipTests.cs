// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The items of a list box, a list view and a tree view round their corners and clip what they show
    /// to them. A clip lives in the coordinates of the element carrying it, so the geometry has to match
    /// that element rather than the border around it.
    /// </summary>
    [TestFixture]
    public class ItemsControlClipTests
    {
        private TestWindow? window;
        private ResourceDictionary? listBoxDictionary;
        private ResourceDictionary? listViewDictionary;
        private ResourceDictionary? treeViewDictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.listBoxDictionary = Load("Controls.ListBox.xaml");
            this.listViewDictionary = Load("Controls.ListView.xaml");
            this.treeViewDictionary = Load("Controls.TreeView.xaml");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private static ResourceDictionary Load(string fileName)
        {
            return new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/{fileName}", UriKind.Absolute) };
        }

        private T Show<T>(T element)
            where T : FrameworkElement
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = element;
            this.window.UpdateLayout();
            element.UpdateLayout();
            ClipAssert.Pump();

            return element;
        }

        private T ShowItem<T>(T item, ResourceDictionary dictionary, string styleKey)
            where T : Control
        {
            item.Width = 200;
            item.Height = 32;
            item.SetValue(FrameworkElement.StyleProperty, dictionary[styleKey]);
            ControlsHelper.SetCornerRadius(item, new CornerRadius(12));

            return this.Show(item);
        }

        /// <summary>
        /// The element the template of the list puts its clip on. It is reached through the visual tree
        /// rather than by name, because every item in the list carries a grid of that name as well.
        /// </summary>
        private static FrameworkElement RootContent(Control list)
        {
            var root = VisualTreeHelper.GetChild(list, 0) as FrameworkElement;
            Assert.That(root, Is.InstanceOf<Border>(), "the template of the list should start with a border");

            var content = VisualTreeHelper.GetChild(root!, 0) as FrameworkElement;
            Assert.That(content, Is.InstanceOf<Grid>(), "the border should carry the grid that clips what the list shows");

            return content!;
        }

        /// <summary>
        /// The list itself rounds its frame, and the items paint an opaque background right up against
        /// it, so without a clip the first and the last item bite the corners out of the arc.
        /// </summary>
        private T ShowList<T>(T list, ResourceDictionary dictionary, string styleKey)
            where T : ItemsControl
        {
            list.Width = 190;
            list.Height = 120;
            list.BorderThickness = new Thickness(1);
            list.SetValue(FrameworkElement.StyleProperty, dictionary[styleKey]);
            ControlsHelper.SetCornerRadius(list, new CornerRadius(8));

            return this.Show(list);
        }

        [Test]
        public void AListBoxShouldClipItsItemsToItsBorder()
        {
            var listBox = new ListBox();
            listBox.Items.Add(new ListBoxItem { Content = "Ada Lovelace" });
            listBox.Items.Add(new ListBoxItem { Content = "Grace Hopper" });
            this.ShowList(listBox, this.listBoxDictionary!, "MahApps.Styles.ListBox");

            var content = RootContent(listBox);

            ClipAssert.CoversElement(content, "list box");
            ClipAssert.CutsEveryCorner(content, "list box");
        }

        [Test]
        public void AListViewShouldClipItsItemsToItsBorder()
        {
            var listView = new ListView();
            listView.Items.Add(new ListViewItem { Content = "Ada Lovelace" });
            listView.Items.Add(new ListViewItem { Content = "Grace Hopper" });
            this.ShowList(listView, this.listViewDictionary!, "MahApps.Styles.ListView");

            var content = RootContent(listView);

            ClipAssert.CoversElement(content, "list view");
            ClipAssert.CutsEveryCorner(content, "list view");
        }

        [Test]
        public void ATreeViewShouldClipItsItemsToItsBorder()
        {
            var treeView = new TreeView();
            treeView.Items.Add(new TreeViewItem { Header = "Ada Lovelace" });
            treeView.Items.Add(new TreeViewItem { Header = "Grace Hopper" });
            this.ShowList(treeView, this.treeViewDictionary!, "MahApps.Styles.TreeView");

            var content = RootContent(treeView);

            ClipAssert.CoversElement(content, "tree view");
            ClipAssert.CutsEveryCorner(content, "tree view");
        }

        [Test]
        public void AListBoxItemShouldClipItsContentToItsBorder()
        {
            var item = this.ShowItem(new ListBoxItem { Content = "Beam me up..." }, this.listBoxDictionary!, "MahApps.Styles.ListBoxItem");

            var grid = ClipAssert.ContentGrid(item);

            ClipAssert.CoversElement(grid, "list box item");
            ClipAssert.CutsEveryCorner(grid, "list box item");
        }

        [Test]
        public void AListBoxItemWithoutACornerRadiusShouldKeepTheWholeRectangle()
        {
            var item = this.ShowItem(new ListBoxItem { Content = "Beam me up..." }, this.listBoxDictionary!, "MahApps.Styles.ListBoxItem");

            ControlsHelper.SetCornerRadius(item, new CornerRadius(0));
            item.UpdateLayout();

            ClipAssert.KeepsEveryCorner(ClipAssert.ContentGrid(item), "list box item");
        }

        [Test]
        public void AListViewItemShouldClipItsContentToItsBorder()
        {
            var item = this.ShowItem(new ListViewItem { Content = "Beam me up..." }, this.listViewDictionary!, "MahApps.Styles.ListViewItem");

            var grid = ClipAssert.ContentGrid(item);

            ClipAssert.CoversElement(grid, "list view item");
            ClipAssert.CutsEveryCorner(grid, "list view item");
        }

        [Test]
        public void ANonSelectableListViewItemShouldClipItsContentToItsBorder()
        {
            var item = this.ShowItem(new ListViewItem { Content = "Beam me up..." }, this.listViewDictionary!, "MahApps.Styles.ListViewItem.NonSelectable");

            var grid = ClipAssert.ContentGrid(item);

            ClipAssert.CoversElement(grid, "list view item");
            ClipAssert.CutsEveryCorner(grid, "list view item");
        }

        [Test]
        public void ATreeViewItemShouldClipItsContentToItsBorder()
        {
            var item = this.ShowItem(new TreeViewItem { Header = "Beam me up..." }, this.treeViewDictionary!, "MahApps.Styles.TreeViewItem");

            var grid = ClipAssert.ContentGrid(item);

            ClipAssert.CoversElement(grid, "tree view item");
            ClipAssert.CutsEveryCorner(grid, "tree view item");
        }

        [Test]
        public void ANestedTreeViewItemShouldClipAtTheEdgeOfItsBorder()
        {
            var child = new TreeViewItem { Header = "Child" };
            var parent = new TreeViewItem { Header = "Parent", IsExpanded = true };
            parent.Items.Add(child);

            var tree = new TreeView { Width = 240, Height = 160 };
            tree.SetValue(FrameworkElement.StyleProperty, this.treeViewDictionary!["MahApps.Styles.TreeView"]);
            tree.ItemContainerStyle = (Style)this.treeViewDictionary["MahApps.Styles.TreeViewItem"];
            tree.Items.Add(parent);

            this.Show(tree);

            ControlsHelper.SetCornerRadius(parent, new CornerRadius(12));
            ControlsHelper.SetCornerRadius(child, new CornerRadius(12));
            tree.UpdateLayout();
            ClipAssert.Pump();

            var border = child.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the template should carry the border");

            var grid = ClipAssert.ContentGrid(child);

            // A child sits deeper in the tree, and the header inside it is indented by that depth. The
            // clip belongs on the whole inside of the border: rounding the indented header would cut a
            // corner into the middle of the item, where the expander sits.
            Assert.That(grid.Margin.Left, Is.EqualTo(0), "the clip should sit on the grid that fills the border, not on the indented one");
            // A border rounds its own thickness to whole device pixels, so the inside is that much off.
            var inside = border!.ActualWidth - border.BorderThickness.Left - border.BorderThickness.Right - border.Padding.Left - border.Padding.Right;
            Assert.That(grid.ActualWidth, Is.EqualTo(inside).Within(1), "and that grid covers the whole inside of the border");

            ClipAssert.CoversElement(grid, "nested tree view item");
            ClipAssert.CutsEveryCorner(grid, "nested tree view item");
        }
    }
}
