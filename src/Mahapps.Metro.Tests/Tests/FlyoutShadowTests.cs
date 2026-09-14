// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-3306: a flyout casting a shadow onto what lies behind it. The effect goes on a pane of its
    /// own behind the content, because an effect over a control takes everything inside it through a
    /// bitmap on the way to the screen, and text drawn that way loses its sharpness.
    /// </summary>
    [TestFixture]
    public class FlyoutShadowTests
    {
        private MetroWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<MetroWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("A flyout nobody asked for a shadow from casts none, so what is there already stays as it was.")]
        public void AFlyoutCastsNoShadowUntilItIsAskedFor()
        {
            var flyout = this.Show();

            Assert.That(flyout.ShadowEffect, Is.Null, "no shadow to start with");
            Assert.That(Pane(flyout).Effect, Is.Null, "and nothing behind it carrying one");
        }

        [Test]
        [Description("The shadow goes on the pane, which is behind the content and draws nothing of its own.")]
        public void TheShadowIsCastByThePaneBehindTheContent()
        {
            var flyout = this.Show();
            var shadow = new DropShadowEffect { BlurRadius = 20, ShadowDepth = 6 };

            flyout.ShadowEffect = shadow;
            this.Settle();

            Assert.That(Pane(flyout).Effect, Is.SameAs(shadow), "the pane should carry what was handed over");
        }

        [Test]
        [Description("And nothing over the content carries it, or every letter in the flyout would go through the effect on its way to the screen.")]
        public void NothingOverTheContentCarriesTheShadow()
        {
            var flyout = this.Show();

            flyout.ShadowEffect = new DropShadowEffect { BlurRadius = 20, ShadowDepth = 6 };
            this.Settle();

            var content = flyout.FindChild<FrameworkElement>("PART_Content");
            Assert.That(content, Is.Not.Null, "the template should carry its content part");

            for (DependencyObject node = content!; node is not null; node = VisualTreeHelper.GetParent(node))
            {
                Assert.That((node as UIElement)?.Effect,
                            Is.Null,
                            $"{(node as FrameworkElement)?.Name ?? node.GetType().Name} over the content carries an effect, which takes the content through it as well");

                if (ReferenceEquals(node, flyout))
                {
                    break;
                }
            }
        }

        /// <summary>The pane the shadow is cast by.</summary>
        private static UIElement Pane(Flyout flyout)
        {
            var pane = flyout.FindChild<FrameworkElement>("PART_Shadow");

            Assert.That(pane, Is.Not.Null, "the template should carry a pane for the shadow");

            return pane!;
        }

        private Flyout Show()
        {
            Assert.That(this.window, Is.Not.Null);

            var flyout = new Flyout
                         {
                             Header = "Settings",
                             Position = Position.Left,
                             Width = 220,
                             IsOpen = true,
                             Content = new TextBlock { Text = "a line of text" }
                         };

            var flyouts = new FlyoutsControl();
            flyouts.Items.Add(flyout);

            this.window!.Flyouts = flyouts;
            this.Settle();

            Assert.That(flyout.IsLoaded, Is.True, "the flyout should be up before a test looks at it");

            return flyout;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
