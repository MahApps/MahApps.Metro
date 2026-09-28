// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// An app that keeps its words in a resource dictionary swaps that dictionary when the language
    /// changes, and every label written as a <c>DynamicResource</c> is meant to follow. A menu item
    /// only does that while it sits in the logical tree of its menu, which is what GH-4492 was about.
    /// </summary>
    [TestFixture]
    public class HamburgerMenuDynamicResourceTests : WindowTestFixture<TestWindow>
    {
        private const string Key = "MahApps.Tests.MenuLabel";

        private ResourceDictionary? spoken;

        [TearDown]
        public void DropTheDictionary()
        {
            if (this.spoken is not null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(this.spoken);
                this.spoken = null;
            }
        }

        /// <summary>
        /// Puts the given word in place of whatever the dictionary before it said, the way an app
        /// swaps one language for another.
        /// </summary>
        private void Say(string word)
        {
            var next = new ResourceDictionary { [Key] = word };

            if (this.spoken is not null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(this.spoken);
            }

            Application.Current.Resources.MergedDictionaries.Add(next);
            this.spoken = next;
        }

        private static string Markup(string itemsProperty, string itemType)
        {
            return @"
<mah:HamburgerMenu xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                   xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                   xmlns:mah='http://metro.mahapps.com/winfx/xaml/controls'
                   Width='300' Height='400' DisplayMode='Inline' IsPaneOpen='True'>
    <mah:HamburgerMenu.ItemTemplate>
        <DataTemplate>
            <TextBlock x:Name='PART_Label' Text='{Binding Label}' />
        </DataTemplate>
    </mah:HamburgerMenu.ItemTemplate>
    <mah:HamburgerMenu.OptionsItemTemplate>
        <DataTemplate>
            <TextBlock x:Name='PART_Label' Text='{Binding Label}' />
        </DataTemplate>
    </mah:HamburgerMenu.OptionsItemTemplate>
    <mah:HamburgerMenu." + itemsProperty + @">
        <mah:HamburgerMenuItemCollection>
            <mah:" + itemType + @" Label='{DynamicResource " + Key + @"}' />
        </mah:HamburgerMenuItemCollection>
    </mah:HamburgerMenu." + itemsProperty + @">
</mah:HamburgerMenu>";
        }

        private HamburgerMenu Show(string itemsProperty = "ItemsSource", string itemType = "HamburgerMenuItem")
        {
            var menu = (HamburgerMenu)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(Markup(itemsProperty, itemType))));

            this.window!.Content = menu;
            this.window.UpdateLayout();
            this.window.Dispatcher.Invoke(() => { }, DispatcherPriority.Loaded);
            this.window.UpdateLayout();

            return menu;
        }

        private static HamburgerMenuItemBase Item(HamburgerMenu menu, string itemsProperty)
        {
            var source = itemsProperty == "ItemsSource" ? menu.ItemsSource : menu.OptionsItemsSource;
            return ((HamburgerMenuItemCollection)source!)[0];
        }

        [TestCase("ItemsSource")]
        [TestCase("OptionsItemsSource")]
        [Description("A label written as a DynamicResource reads the word that is in the dictionary now, not the one that was there when the menu was built.")]
        public void ALabelFollowsTheDictionaryThatIsSwappedUnderIt(string itemsProperty)
        {
            this.Say("Ada Lovelace");

            var menu = this.Show(itemsProperty);
            var item = (HamburgerMenuItem)Item(menu, itemsProperty);

            Assert.That(item.Label, Is.EqualTo("Ada Lovelace"), "the word it was built with");

            this.Say("Grace Hopper");
            this.window!.UpdateLayout();

            Assert.That(item.Label, Is.EqualTo("Grace Hopper"), "and the one that took its place");
        }

        [Test]
        [Description("A header carries a label of its own and follows the same way.")]
        public void AHeaderFollowsTheDictionaryAsWell()
        {
            this.Say("Ada Lovelace");

            var menu = this.Show(itemType: "HamburgerMenuHeaderItem");
            var header = (HamburgerMenuHeaderItem)Item(menu, "ItemsSource");

            this.Say("Grace Hopper");
            this.window!.UpdateLayout();

            Assert.That(header.Label, Is.EqualTo("Grace Hopper"));
        }

        [Test]
        [Description("What the row shows follows too, since it reads the label of the item behind it.")]
        public void TheRowOnScreenFollowsTheDictionary()
        {
            this.Say("Ada Lovelace");

            var menu = this.Show();
            var text = menu.FindChildren<TextBlock>(true).FirstOrDefault(t => t.Name == "PART_Label");

            Assert.That(text, Is.Not.Null, "the row should be there to read");
            Assert.That(text!.Text, Is.EqualTo("Ada Lovelace"));

            this.Say("Grace Hopper");
            this.window!.UpdateLayout();

            Assert.That(text.Text, Is.EqualTo("Grace Hopper"));
        }

        [Test]
        [Description("The menu takes its items into its logical tree, which is the only place a resource reference can look up from.")]
        public void TheMenuTakesItsItemsIntoItsLogicalTree()
        {
            this.Say("Ada Lovelace");

            var menu = this.Show();
            var item = Item(menu, "ItemsSource");

            Assert.Multiple(() =>
                {
                    Assert.That(item.Parent, Is.SameAs(menu), "the item should belong to the menu");
                    Assert.That(LogicalTreeHelper.GetChildren(menu).OfType<HamburgerMenuItemBase>(), Does.Contain(item), "and be named among its children, or the walk that invalidates the tree never reaches it");
                });
        }

        [Test]
        [Description("An item taken out of the source is let go of again, so nothing keeps it hanging on the menu.")]
        public void AnItemThatGoesIsLetGoOf()
        {
            this.Say("Ada Lovelace");

            var menu = this.Show();
            var items = (HamburgerMenuItemCollection)menu.ItemsSource!;
            var item = items[0];

            items.RemoveAt(0);
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(item.Parent, Is.Null, "the menu should have let go");
                    Assert.That(LogicalTreeHelper.GetChildren(menu).OfType<HamburgerMenuItemBase>(), Does.Not.Contain(item));
                });
        }
    }
}
