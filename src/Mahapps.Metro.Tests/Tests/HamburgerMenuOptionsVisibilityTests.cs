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
    /// GH-4572: the options of a hamburger menu can be hidden. The property to do it with outlived
    /// the binding that read it, so for a long while it could be set and did nothing.
    /// </summary>
    [TestFixture]
    public class HamburgerMenuOptionsVisibilityTests
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

        private HamburgerMenu ShowMenu(Visibility optionsVisibility)
        {
            var menu = new HamburgerMenu
                       {
                           Width = 400,
                           Height = 300,
                           IsPaneOpen = true,
                           ItemsSource = new HamburgerMenuItemCollection { new HamburgerMenuGlyphItem { Label = "Home" } },
                           OptionsItemsSource = new HamburgerMenuItemCollection { new HamburgerMenuGlyphItem { Label = "Settings" } },
                           OptionsVisibility = optionsVisibility
                       };

            this.window.Content = menu;
            this.window.UpdateLayout();
            menu.UpdateLayout();
            ClipAssert.Pump();

            return menu;
        }

        private static ListBox OptionsOf(HamburgerMenu menu)
        {
            var options = menu.FindChild<ListBox>("OptionsListView");
            Assert.That(options, Is.Not.Null, "the template should carry the options list");

            return options!;
        }

        [Test]
        [Description("Told to go away, the options do.")]
        public void CollapsedOptionsAreNotShown()
        {
            var options = OptionsOf(this.ShowMenu(Visibility.Collapsed));

            Assert.That(options.Visibility, Is.EqualTo(Visibility.Collapsed));
            Assert.That(options.ActualHeight, Is.Zero, "and they take no room either");
        }

        [Test]
        [Description("Told to stay out of sight but keep their place, they do that instead.")]
        public void HiddenOptionsKeepTheirRoom()
        {
            var options = OptionsOf(this.ShowMenu(Visibility.Hidden));

            Assert.That(options.Visibility, Is.EqualTo(Visibility.Hidden));
            Assert.That(options.ActualHeight, Is.GreaterThan(0), "hidden is not the same as gone");
        }

        [Test]
        [Description("Told nothing, the options are there, which is what every menu from before does.")]
        public void TheOptionsAreThereByThemselves()
        {
            var options = OptionsOf(this.ShowMenu(Visibility.Visible));

            Assert.That(options.Visibility, Is.EqualTo(Visibility.Visible));
            Assert.That(options.ActualHeight, Is.GreaterThan(0));
        }
    }
}
