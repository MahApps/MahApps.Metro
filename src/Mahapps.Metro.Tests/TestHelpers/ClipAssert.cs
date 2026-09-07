// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// What every template that rounds its corners has to hold up: a clip lives in the coordinates of
    /// the element carrying it, so the geometry has to match that element and not the border around it.
    /// </summary>
    public static class ClipAssert
    {
        /// <summary>
        /// Reaches the grid a template puts its clip on.
        /// </summary>
        public static Grid ContentGrid(FrameworkElement root, string name = "ContentGrid")
        {
            var grid = root.FindChild<Grid>(name);
            Assert.That(grid, Is.Not.Null, $"the template should carry the grid named {name} that the clip sits on");

            return grid!;
        }

        /// <summary>
        /// The clip covers the element exactly. Anything wider or taller leaves it unclipped along that
        /// edge, which is what a geometry built from the size of the surrounding border does.
        /// </summary>
        public static void CoversElement(FrameworkElement element, string what)
        {
            Assert.That(element.ActualWidth, Is.GreaterThan(0), $"the {what} should be laid out, otherwise this test proves nothing");
            Assert.That(element.Clip, Is.Not.Null, $"the {what} should be clipped");

            Assert.That(element.Clip!.Bounds.Width, Is.EqualTo(element.ActualWidth).Within(0.001), $"a wider clip leaves the {what} unclipped on the right");
            Assert.That(element.Clip.Bounds.Height, Is.EqualTo(element.ActualHeight).Within(0.001), $"a taller clip leaves the {what} unclipped at the bottom");
            Assert.That(element.Clip.Bounds.X, Is.EqualTo(0).Within(0.001), $"the clip of the {what} should start at its left edge");
            Assert.That(element.Clip.Bounds.Y, Is.EqualTo(0).Within(0.001), $"the clip of the {what} should start at its top edge");
        }

        /// <summary>
        /// All four corners are cut and the middle survives.
        /// </summary>
        public static void CutsEveryCorner(FrameworkElement element, string what)
        {
            Assert.That(element.Clip, Is.Not.Null, $"the {what} should be clipped");

            var clip = element.Clip!;
            var width = element.ActualWidth;
            var height = element.ActualHeight;

            Assert.That(clip.FillContains(new Point(width / 2, height / 2)), Is.True, $"the middle of the {what} belongs to the clip");
            Assert.That(clip.FillContains(new Point(1, 1)), Is.False, $"top left of the {what} should be cut");
            Assert.That(clip.FillContains(new Point(width - 1, 1)), Is.False, $"top right of the {what} should be cut");
            Assert.That(clip.FillContains(new Point(width - 1, height - 1)), Is.False, $"bottom right of the {what} should be cut");
            Assert.That(clip.FillContains(new Point(1, height - 1)), Is.False, $"bottom left of the {what} should be cut");
        }

        /// <summary>
        /// Nothing is cut, which is what a corner radius of zero has to give.
        /// </summary>
        public static void KeepsEveryCorner(FrameworkElement element, string what)
        {
            Assert.That(element.Clip, Is.Not.Null, $"the {what} should still be clipped, just not rounded");

            var clip = element.Clip!;

            Assert.That(clip.FillContains(new Point(0.5, 0.5)), Is.True, $"square corners of the {what} belong to the clip");
            Assert.That(clip.FillContains(new Point(element.ActualWidth - 0.5, element.ActualHeight - 0.5)), Is.True, $"and so does the far corner of the {what}");
        }

        /// <summary>
        /// Lets the dispatcher work, which the templates need before anything has a size.
        /// </summary>
        public static void Pump(int milliseconds = 300)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(milliseconds), DispatcherPriority.Background, (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);

            try
            {
                Dispatcher.PushFrame(frame);
            }
            finally
            {
                timer.Stop();
            }
        }
    }
}
