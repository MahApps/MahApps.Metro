// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// A menu rounds four things: the menu bar, a context menu and the two submenu popups a menu item
    /// can open. A clip lives in the coordinates of the element carrying it, so each geometry has to
    /// match that element rather than the border around it.
    /// </summary>
    [TestFixture]
    public class MenuClipTests
    {
        private TestWindow? window;
        private ResourceDictionary? menuDictionary;
        private ResourceDictionary? contextMenuDictionary;
        private ResourceDictionary? menuItemDictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.menuDictionary = Load("Controls.Menu.xaml");
            this.contextMenuDictionary = Load("Controls.ContextMenu.xaml");
            this.menuItemDictionary = Load("Controls.MenuItem.xaml");
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

        /// <summary>
        /// Takes the content of a popup out of it and lays it out in the window instead. A menu only
        /// opens a submenu while it holds the mouse capture, and a popup closes again when its window
        /// loses activation, so opening one for real is nothing a test run with several hosts on one
        /// desktop can rely on. The geometry inside is the same either way, and the element names the
        /// clip binds to travel with it.
        /// </summary>
        private Border ShowPopupContent(FrameworkElement templatedControl)
        {
            var popup = templatedControl.FindChild<Popup>("PART_Popup");
            Assert.That(popup, Is.Not.Null, "the template should carry the popup");

            var content = popup!.Child as FrameworkElement;
            Assert.That(content, Is.Not.Null, "the popup should carry its content");

            popup.Child = null;

            var host = new Grid { Width = 220, Height = 120 };
            host.Children.Add(content);
            this.Show(host);

            var border = (content as Border) ?? content!.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the submenu should sit in a border");

            return border!;
        }

        private MenuItem ShowMenuItemWithTemplate(string templateResourceId)
        {
            var template = (ControlTemplate)this.menuItemDictionary![new ComponentResourceKey(typeof(MenuItem), templateResourceId)];

            var menuItem = new MenuItem { Header = "File", Width = 200, Height = 24 };
            menuItem.Items.Add(new MenuItem { Header = "Open" });
            menuItem.Items.Add(new MenuItem { Header = "Close" });
            menuItem.SetValue(Control.TemplateProperty, template);
            ControlsHelper.SetCornerRadius(menuItem, new CornerRadius(12));

            this.Show(menuItem);
            menuItem.ApplyTemplate();

            return menuItem;
        }

        [Test]
        public void TheMenuBarShouldClipItsContentToItsBorder()
        {
            var menu = new Menu { Width = 200, Height = 32 };
            menu.Items.Add(new MenuItem { Header = "File" });
            menu.SetValue(FrameworkElement.StyleProperty, this.menuDictionary!["MahApps.Styles.Menu"]);
            ControlsHelper.SetCornerRadius(menu, new CornerRadius(12));

            this.Show(menu);

            var grid = ClipAssert.ContentGrid(menu);

            ClipAssert.CoversElement(grid, "menu bar");
            ClipAssert.CutsEveryCorner(grid, "menu bar");
        }

        [Test]
        public void TheContextMenuShouldClipItsContentToItsBorder()
        {
            var owner = new Border { Width = 200, Height = 80 };
            var contextMenu = new ContextMenu { Width = 200, Height = 80 };
            contextMenu.Items.Add(new MenuItem { Header = "Cut" });
            contextMenu.Items.Add(new MenuItem { Header = "Copy" });
            contextMenu.SetValue(FrameworkElement.StyleProperty, this.contextMenuDictionary!["MahApps.Styles.ContextMenu"]);
            ControlsHelper.SetCornerRadius(contextMenu, new CornerRadius(12));

            // A context menu refuses to have a parent of its own, so it has to be opened to get a size.
            owner.ContextMenu = contextMenu;
            this.Show(owner);

            contextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, true);
            ClipAssert.Pump();
            Assert.That(contextMenu.IsOpen, Is.True, "the context menu should be open, otherwise there is nothing to measure");

            var grid = ClipAssert.ContentGrid(contextMenu);

            try
            {
                ClipAssert.CoversElement(grid, "context menu");
                ClipAssert.CutsEveryCorner(grid, "context menu");
            }
            finally
            {
                contextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, false);
                ClipAssert.Pump();
            }
        }

        [TestCase("TopLevelHeaderTemplateKey")]
        [TestCase("SubmenuHeaderTemplateKey")]
        public void ASubmenuShouldClipItsContentToItsBorder(string templateResourceId)
        {
            var menuItem = this.ShowMenuItemWithTemplate(templateResourceId);
            var border = this.ShowPopupContent(menuItem);

            var grid = ClipAssert.ContentGrid(border);

            ClipAssert.CoversElement(grid, "submenu");
        }
    }
}
