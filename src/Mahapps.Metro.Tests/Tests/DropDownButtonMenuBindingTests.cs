// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4494: what the menu items of a drop down button can be bound to, and what still holds
    /// once the items have been built a second time, which is what a navigation framework handing
    /// the view a new view model comes down to.
    /// </summary>
    [TestFixture]
    public class DropDownButtonMenuBindingTests
    {
        private DropDownButtonMenuWindow window = null!;
        private DropDownButtonView view = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<DropDownButtonMenuWindow>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        [SetUp]
        public void SetUp()
        {
            this.view = new DropDownButtonView();

            this.window.Host.SetCurrentValue(ContentControl.ContentProperty, this.view);
            ClipAssert.Pump();
        }

        private static object?[] CommandsOf(DropDownButton button)
        {
            var menu = button.FindChild<Button>("PART_Button")?.ContextMenu;
            Assert.That(menu, Is.Not.Null, "the template should carry the menu");

            // a menu that does not stay open shuts itself as soon as its window is not the one in
            // front, and takes the items it built with it
            menu!.SetCurrentValue(ContextMenu.StaysOpenProperty, true);
            button.SetCurrentValue(DropDownButton.IsExpandedProperty, true);
            ClipAssert.Pump();

            var commands = menu.Items
                               .Cast<object>()
                               .Select((_, index) => (menu.ItemContainerGenerator.ContainerFromIndex(index) as MenuItem)?.Command)
                               .Cast<object?>()
                               .ToArray();

            button.SetCurrentValue(DropDownButton.IsExpandedProperty, false);
            ClipAssert.Pump();

            Assert.That(commands, Has.Length.EqualTo(2), "the menu should have built an item per entry");

            return commands;
        }

        private void HandTheViewANewViewModel()
        {
            this.view.SetValue(FrameworkElement.DataContextProperty, new DropDownButtonViewModel());
            ClipAssert.Pump();
        }

        [Test]
        [Description("Reaching the view model through the menu that holds the item works.")]
        public void TheItemsReachTheViewModelThroughTheirMenu()
        {
            Assert.That(CommandsOf(this.view.ByTheMenu), Has.No.Null);

            this.HandTheViewANewViewModel();

            Assert.That(CommandsOf(this.view.ByTheMenu), Has.No.Null, "and it still does with a new view model");
        }

        [Test]
        [Description("So does reaching it through what the menu is placed on.")]
        public void TheItemsReachTheViewModelThroughThePlacementTarget()
        {
            Assert.That(CommandsOf(this.view.ByThePlacementTarget), Has.No.Null);

            this.HandTheViewANewViewModel();

            Assert.That(CommandsOf(this.view.ByThePlacementTarget), Has.No.Null, "and it still does with a new view model");
        }

        [Test]
        [Description("Reaching past the menu holds only until the items are built again, because a menu is a popup WPF allows no parent to walk up to.")]
        public void TheItemsCannotReachPastTheirMenuTwice()
        {
            Assert.That(CommandsOf(this.view.ByAnAncestorAboveTheMenu), Has.No.Null, "the first build finds the view");

            this.HandTheViewANewViewModel();

            Assert.That(CommandsOf(this.view.ByAnAncestorAboveTheMenu), Has.All.Null, "the second one does not");
        }
    }
}
