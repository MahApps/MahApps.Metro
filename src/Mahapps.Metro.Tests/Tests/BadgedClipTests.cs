// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// GH-2994: the badge on a tab header came out cut in half, and which header it happened to
    /// looked like chance. A badge hangs over the edge of what it is put on by half its own size, so
    /// a layout clip anywhere above it takes the whole badge with it, and WPF hands one out as soon
    /// as an element is arranged a hair smaller than it asked for.
    /// </summary>
    [TestFixture]
    public class BadgedClipTests
    {
        private TestWindow? window;

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

        [Test]
        [Description("Arranged smaller than it asked for, anything else would be given a layout clip. A badged control must not take one, or the badge it is there to show is the first thing to go.")]
        public void ABadgedControlSqueezedIntoTooLittleRoomKeepsNoClip()
        {
            var badged = new Badged { Badge = "12", Content = new TextBlock { Text = "squeezed" } };

            // a canvas hands out exactly the room it is told to and nothing more
            var canvas = new Canvas { Width = 20, Height = 10 };
            canvas.Children.Add(badged);

            this.window!.Content = canvas;
            this.Settle();

            Assume.That(badged.DesiredSize.Width, Is.GreaterThan(20), "the control should want more room than it is about to get");

            badged.Arrange(new Rect(0, 0, 20, 10));
            this.Settle();

            Assert.That(VisualTreeHelper.GetClip(badged), Is.Null, "a clip here would cut the badge off");
        }

        [TestCase(41.3)]
        [TestCase(37.7)]
        [TestCase(50.5)]
        [Description("On a tab header the squeeze comes from rounding to whole device pixels, a tenth of a pixel at a time, which is why the report saw it on one caption and not the next. Nothing along the way from the tab down to the badge may hold a clip.")]
        public void NothingOverABadgeInATabHeaderHoldsAClip(double wide)
        {
            var badged = new Badged
                         {
                             Badge = "2",
                             // a width that does not land on a whole device pixel, which is what the
                             // rounding in the tab template trips over
                             Content = new TextBlock { Text = "Tab", Width = wide }
                         };

            var item = new TabItem { Header = badged, Content = new TextBlock { Text = "content" } };
            var tabs = new TabControl();
            tabs.Items.Add(item);

            this.window!.Content = tabs;
            this.Settle();

            var badge = item.FindChild<FrameworkElement>("PART_BadgeContainer");
            Assert.That(badge, Is.Not.Null, "the header should carry a badge");

            for (DependencyObject node = badge!; node is not null; node = VisualTreeHelper.GetParent(node))
            {
                Assert.That(VisualTreeHelper.GetClip((Visual)node),
                            Is.Null,
                            $"{(node as FrameworkElement)?.Name ?? node.GetType().Name} over the badge holds a clip, which cuts the badge off");

                if (ReferenceEquals(node, item))
                {
                    break;
                }
            }
        }

        [Test]
        [Description("The host that carries a tab header switched layout rounding off while the grid around it rounds, and a host that asks for a tenth of a pixel more than that grid hands out is given a clip for its trouble. Both round the same way now.")]
        public void TheHostOfATabHeaderRoundsTheWayTheGridAroundItDoes()
        {
            var item = new TabItem { Header = new Badged { Badge = "2", Content = new TextBlock { Text = "Tab" } }, Content = new TextBlock { Text = "content" } };
            var tabs = new TabControl();
            tabs.Items.Add(item);

            this.window!.Content = tabs;
            this.Settle();

            var host = item.FindChild<ContentControlEx>("ContentSite");
            Assert.That(host, Is.Not.Null, "the tab template should carry its header host");

            var around = VisualTreeHelper.GetParent(host!) as FrameworkElement;
            Assert.That(around, Is.Not.Null);

            Assert.That(host!.UseLayoutRounding,
                        Is.EqualTo(around!.UseLayoutRounding),
                        "the host rounds one way and what holds it rounds another, which is a clip waiting to happen");
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
