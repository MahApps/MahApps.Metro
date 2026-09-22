// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-3532: the animation used to answer with a star length whatever it was given, and it was
    /// given nothing whenever the binding behind its destination had not come through yet. A column
    /// of "*" where 48 pixels belonged took half the control and stayed there.
    /// </summary>
    [TestFixture]
    public class GridLengthAnimationTests
    {
        private static GridLength CurrentValue(GridLengthAnimation animation, GridLength origin, GridLength destination)
        {
            return (GridLength)animation.GetCurrentValue(origin, destination, animation.CreateClock());
        }

        [Test]
        [Description("A destination that is not there yet leaves the length the property already has.")]
        public void AnAnimationWithoutADestinationChangesNothing()
        {
            var animation = new GridLengthAnimation();

            var value = CurrentValue(animation, new GridLength(240), new GridLength(240));

            Assert.That(value, Is.EqualTo(new GridLength(240)));
        }

        [Test]
        [Description("With a destination and nothing to count from, the animation is there at once.")]
        public void AnAnimationWithoutAStartGoesStraightToItsDestination()
        {
            var animation = new GridLengthAnimation { To = new GridLength(48) };

            var value = CurrentValue(animation, new GridLength(240), new GridLength(240));

            Assert.That(value, Is.EqualTo(new GridLength(48)));
        }

        [Test]
        [Description("Pixels stay pixels on the way, rather than turning into a share of what is left.")]
        public void AnAnimationBetweenTwoLengthsKeepsTheirUnit()
        {
            var animation = new GridLengthAnimation { From = new GridLength(240), To = new GridLength(48) };

            var value = CurrentValue(animation, new GridLength(240), new GridLength(48));

            Assert.That(value.GridUnitType, Is.EqualTo(GridUnitType.Pixel));
            Assert.That(value.Value, Is.InRange(48, 240));
        }

        [Test]
        [Description("A star destination is still a star destination.")]
        public void AnAnimationToAShareOfWhatIsLeftSaysSo()
        {
            var animation = new GridLengthAnimation { To = new GridLength(1, GridUnitType.Star) };

            var value = CurrentValue(animation, new GridLength(48), new GridLength(48));

            Assert.That(value, Is.EqualTo(new GridLength(1, GridUnitType.Star)));
        }
    }
}
