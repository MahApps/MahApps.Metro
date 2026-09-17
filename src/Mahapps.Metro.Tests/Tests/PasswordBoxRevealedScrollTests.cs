// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The box that shows a revealed password keeps a caret while it is hidden, and a caret asks to be
    /// scrolled into view whenever what stands under it changes. A page carrying such a password box
    /// therefore jumped down to it while it was still coming up, which is nothing anybody asked for.
    /// </summary>
    [TestFixture]
    public class PasswordBoxRevealedScrollTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TestCase("MahApps.Styles.PasswordBox.Revealed")]
        [TestCase("MahApps.Styles.PasswordBox.Win8")]
        [Description("A page with a password box at the far end of it comes up at the top and stays there, however the password arrives.")]
        public void APasswordBoxDoesNotDragThePageDownToItself(string key)
        {
            var box = new PasswordBox
                      {
                          Style = (Style)Application.Current.FindResource(key),
                          Width = 200,
                          Password = "Beam me up..."
                      };

            // something tall, with the password box at the far end of it
            var page = new StackPanel();
            page.Children.Add(new Border { Height = 2000 });
            page.Children.Add(box);

            var scroller = new ScrollViewer { Content = page, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

            this.window!.Content = scroller;
            this.Settle();

            Assume.That(scroller.ScrollableHeight, Is.GreaterThan(0), "the page should be longer than the window, otherwise this test proves nothing");

            Assert.That(scroller.VerticalOffset, Is.EqualTo(0), $"the page should still be at the top once it is up, and it is at {scroller.VerticalOffset:0}");

            box.Password = "Warp nine";
            this.Settle();

            Assert.That(scroller.VerticalOffset, Is.EqualTo(0), $"and a password that arrives later changes nothing about that, yet it is at {scroller.VerticalOffset:0}");
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
            ClipAssert.Pump();
        }
    }
}
