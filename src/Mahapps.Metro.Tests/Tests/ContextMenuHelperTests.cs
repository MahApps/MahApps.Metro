// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The menu on the right button of a text box is ours, and an item on it that is named after its
    /// command carries a word WPF reads once per process and then keeps. An application that sets a
    /// different <see cref="CultureInfo.CurrentUICulture"/> while it runs was therefore left with the
    /// language it started in, which is what GH-4155 reports.
    /// </summary>
    [TestFixture]
    public class ContextMenuHelperTests
    {
        private static readonly object[] TheEditingItems =
            {
                new object[] { ApplicationCommands.Cut, "TextBox_ContextMenu_Cut" },
                new object[] { ApplicationCommands.Copy, "TextBox_ContextMenu_Copy" },
                new object[] { ApplicationCommands.Paste, "TextBox_ContextMenu_Paste" }
            };

        private TestWindow? window;
        private CultureInfo? cultureBefore;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void KeepTheCulture()
        {
            this.cultureBefore = Thread.CurrentThread.CurrentUICulture;
        }

        [TearDown]
        public void PutTheCultureBack()
        {
            if (this.cultureBefore is not null)
            {
                Thread.CurrentThread.CurrentUICulture = this.cultureBefore;
            }

            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCaseSource(nameof(TheEditingItems))]
        public void TheWordOnAnItemShouldBeTheOneWpfWrites(ICommand command, string key)
        {
            var menu = this.ShowTextBoxMenu();
            var item = ItemFor(menu, command);

            Assert.That(item.Header, Is.EqualTo(SystemText(key)), "the item should say what the menu WPF builds for a text box says");
            Assert.That(item.Header, Is.Not.EqualTo(((RoutedUICommand)command).Text), "and not what the command is called, which is the text that never changes again");
        }

        [TestCaseSource(nameof(TheEditingItems))]
        public void TheWordShouldBeAskedForAgainOnEveryOpening(ICommand command, string key)
        {
            var menu = this.ShowTextBoxMenu();
            var item = ItemFor(menu, command);

            item.Header = "42";
            Open(menu);

            Assert.That(item.Header, Is.EqualTo(SystemText(key)), "an opening should write the word again, which is what carries a culture that changed since");
        }

        [TestCaseSource(nameof(TheEditingItems))]
        public void TheWordShouldFollowTheUiCulture(ICommand command, string key)
        {
            var menu = this.ShowTextBoxMenu();
            var item = ItemFor(menu, command);

            foreach (var name in new[] { "de-DE", "zh-Hans", "en-US" })
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);
                Open(menu);

                Assert.That(item.Header, Is.EqualTo(SystemText(key)), $"the item should say what WPF says in {name}");
            }
        }

        private ContextMenu ShowTextBoxMenu()
        {
            Assert.That(this.window, Is.Not.Null);

            var textBox = new TextBox { Width = 200, Text = "42" };

            this.window!.Content = textBox;
            this.window.UpdateLayout();
            textBox.UpdateLayout();

            Assert.That(textBox.ContextMenu, Is.Not.Null, "the TextBox style should put the MahApps menu on the box");

            return textBox.ContextMenu!;
        }

        private static MenuItem ItemFor(ContextMenu menu, ICommand command)
        {
            var item = menu.Items.OfType<MenuItem>().FirstOrDefault(candidate => ReferenceEquals(candidate.Command, command));

            Assert.That(item, Is.Not.Null, $"the menu should carry an item for {command}");

            return item!;
        }

        private static void Open(ContextMenu menu)
        {
            menu.RaiseEvent(new RoutedEventArgs(ContextMenu.OpenedEvent, menu));
        }

        /// <summary>
        /// The word WPF itself would write, read the same way the helper reads it but through a
        /// resource manager of this fixture, so the test asks the framework rather than the code
        /// under it.
        /// </summary>
        private static string? SystemText(string key)
        {
            foreach (var name in new[] { "FxResources.PresentationFramework.SR", "ExceptionStringTable" })
            {
                try
                {
                    var text = new ResourceManager(name, typeof(ContextMenu).Assembly).GetString(key);
                    if (text is not null)
                    {
                        return text;
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // the other name is the one this runtime uses
                }
            }

            Assert.Fail("WPF should carry the words of its own text box menu");

            return null;
        }
    }
}
