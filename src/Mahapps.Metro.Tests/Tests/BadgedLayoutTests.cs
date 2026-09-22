// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A badge over a certain font size jumped between two places and the app stopped
    /// answering. Two things fed each other. The badge hangs over its corner by half of itself, and
    /// that half came out of the container's DesiredSize, which has the margin already taken off
    /// it, so each pass answered with half of what the last one had left over and the two answers
    /// kept swapping places. And the badge was round because its MinWidth was bound to its own
    /// ActualHeight, a layout property fed by a layout result, which needs a second pass to come
    /// out right and moves the box while it does.
    /// </summary>
    [TestFixture]
    public class BadgedLayoutTests
    {
        private static FrameworkElement MeasureOnce(object badge, double fontSize)
        {
            var badged = new Badged
                         {
                             Badge = badge,
                             BadgeFontSize = fontSize,
                             Content = new TextBlock { Text = "Mail" }
                         };

            badged.Measure(new Size(400, 200));

            var container = badged.FindChild<FrameworkElement>("PART_BadgeContainer");
            Assert.That(container, Is.Not.Null, "the template should carry the badge container");
            return container!;
        }

        [TestCase("4")]
        [TestCase("42")]
        [Description("A short badge is a circle, and it is one after the first measure rather than after a second pass that reads the height back.")]
        public void AShortBadgeIsRoundAfterOneMeasure(string badge)
        {
            var container = MeasureOnce(badge, 32);

            Assert.That(container.DesiredSize.Width,
                        Is.EqualTo(container.DesiredSize.Height).Within(0.01),
                        "the badge should be as wide as it is high straight away");
        }

        [Test]
        [Description("A badge with more in it than fits into a circle keeps its own width, so it becomes a pill rather than growing into a square.")]
        public void ALongBadgeKeepsItsWidth()
        {
            var container = MeasureOnce("1234567", 32);

            Assert.That(container.DesiredSize.Width,
                        Is.GreaterThan(container.DesiredSize.Height),
                        "seven digits are wider than they are high");
        }

        [Test]
        [Description("The badge hangs over its corner by half of itself, and it lands on the same place every pass. An overhang worked out from what is left of the badge after the last one swaps between two values and never comes to rest.")]
        public void TheOverhangIsHalfTheBadgeAndStaysThereWhenTheBadgeGrows()
        {
            var badged = new Badged
                         {
                             Badge = "42",
                             BadgeFontSize = 11,
                             Content = new TextBlock { Text = "Mail" }
                         };

            var room = new Size(400, 200);
            var slot = new Rect(0, 0, 400, 200);

            badged.Measure(room);
            badged.Arrange(slot);

            var container = badged.FindChild<FrameworkElement>("PART_BadgeContainer");
            Assert.That(container, Is.Not.Null);

            // the reader turns the font up, so the badge grows while it already carries an overhang
            badged.BadgeFontSize = 32;

            var first = Overhang();
            var second = Overhang();

            Assert.That(second, Is.EqualTo(first), "the overhang swaps between two values instead of coming to rest");

            Assert.That(first.Left,
                        Is.EqualTo(0 - (container!.ActualWidth / 2)).Within(0.01),
                        "the badge should hang over by half of itself");
            Assert.That(first.Top,
                        Is.EqualTo(0 - (container.ActualHeight / 2)).Within(0.01),
                        "the badge should hang over by half of itself");

            Thickness Overhang()
            {
                badged.InvalidateMeasure();
                badged.Measure(room);
                badged.Arrange(slot);
                return container!.Margin;
            }
        }
    }
}
