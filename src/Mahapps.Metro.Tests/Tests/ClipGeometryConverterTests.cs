// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Converters;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The geometry this converter builds is used as a <see cref="UIElement.Clip"/>, which lives in the
    /// coordinates of the element it sits on. So it has to describe the area inside the border, not the
    /// border itself.
    /// </summary>
    [TestFixture]
    public class ClipGeometryConverterTests
    {
        private static Geometry Convert(double width, double height, CornerRadius cornerRadius, Thickness borderThickness, Thickness? padding = null)
        {
            object[] values = padding is null
                ? new object[] { width, height, cornerRadius, borderThickness }
                : new object[] { width, height, cornerRadius, borderThickness, padding.Value };

            var result = ClipGeometryConverter.Instance.Convert(values, typeof(Geometry), null!, CultureInfo.InvariantCulture);

            Assert.That(result, Is.InstanceOf<Geometry>(), "the converter should hand back a geometry");

            return (Geometry)result;
        }

        [Test]
        public void TheGeometryShouldCoverTheGivenSize()
        {
            var geometry = Convert(200, 45, new CornerRadius(22), new Thickness(2));

            Assert.That(geometry.Bounds.Width, Is.EqualTo(200).Within(0.001), "anything wider would leave the element unclipped on the right");
            Assert.That(geometry.Bounds.Height, Is.EqualTo(45).Within(0.001), "anything taller would leave it unclipped at the bottom");
            Assert.That(geometry.Bounds.X, Is.EqualTo(0).Within(0.001));
            Assert.That(geometry.Bounds.Y, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void EveryCornerShouldBeRounded()
        {
            var geometry = Convert(200, 45, new CornerRadius(22), new Thickness(2));

            Assert.That(geometry.FillContains(new Point(100, 22)), Is.True, "the middle belongs to the clip");

            Assert.That(geometry.FillContains(new Point(1, 1)), Is.False, "top left should be cut");
            Assert.That(geometry.FillContains(new Point(199, 1)), Is.False, "top right should be cut");
            Assert.That(geometry.FillContains(new Point(199, 44)), Is.False, "bottom right should be cut");
            Assert.That(geometry.FillContains(new Point(1, 44)), Is.False, "bottom left should be cut");
        }

        [Test]
        public void TheRadiusShouldLoseHalfTheBorderThickness()
        {
            // A corner radius of 22 with a border of 8 leaves 18 on the inside, which is how the WPF
            // Border draws its own inner edge. Taking the whole thickness would leave 14 and cut a
            // visible gap between the border and the content, and the two points below tell those apart.
            var geometry = Convert(200, 45, new CornerRadius(22), new Thickness(8));

            Assert.That(geometry.FillContains(new Point(5.5, 5.5)), Is.True, "the inner radius should be 18");
            Assert.That(geometry.FillContains(new Point(4.5, 4.5)), Is.False, "and not any smaller than that");
        }

        [Test]
        public void PaddingShouldCountWholeTowardsTheInside()
        {
            // Half of the border of 8 and the whole padding of 4 take 8 off the radius of 22, so the
            // corner is a radius of 14 here.
            var geometry = Convert(200, 45, new CornerRadius(22), new Thickness(8), new Thickness(4));

            Assert.That(geometry.FillContains(new Point(4.7, 4.7)), Is.True, "the padding pushes the content further in");
            Assert.That(geometry.FillContains(new Point(3.5, 3.5)), Is.False);
        }

        [Test]
        public void OneThickSideShouldNotReshapeTheOppositeCorner()
        {
            // Only the right side is thick, so the top left corner keeps its radius of 10 and stays a
            // circle. Reading the wrong side would flatten it into an ellipse, and the point below is
            // inside the circle and outside that ellipse.
            var geometry = Convert(200, 45, new CornerRadius(10), new Thickness(0, 0, 10, 0));

            Assert.That(geometry.FillContains(new Point(2, 9)), Is.True, "the top left corner should not care how thick the right side is");
        }

        [Test]
        public void CornersAskingForMoreThanThereIsShouldShareTheSide()
        {
            // Two radii of 30 on a side of 40 do not fit, so they end up with 20 each instead of
            // overlapping into a shape that folds back on itself.
            var geometry = Convert(40, 40, new CornerRadius(30), new Thickness(0));

            Assert.That(geometry.Bounds.Width, Is.EqualTo(40).Within(0.001));
            Assert.That(geometry.Bounds.Height, Is.EqualTo(40).Within(0.001));
            Assert.That(geometry.FillContains(new Point(20, 20)), Is.True, "the middle should survive");
            Assert.That(geometry.FillContains(new Point(1, 1)), Is.False, "the corners should still be cut");
            Assert.That(geometry.FillContains(new Point(39, 39)), Is.False);
        }

        [Test]
        public void ACornerRadiusOfZeroShouldGiveTheWholeRectangle()
        {
            var geometry = Convert(200, 45, new CornerRadius(0), new Thickness(2));

            Assert.That(geometry.FillContains(new Point(0.5, 0.5)), Is.True, "square corners belong to the clip");
            Assert.That(geometry.FillContains(new Point(199.5, 44.5)), Is.True);
            Assert.That(geometry.Bounds.Width, Is.EqualTo(200).Within(0.001));
            Assert.That(geometry.Bounds.Height, Is.EqualTo(45).Within(0.001));
        }
    }
}
