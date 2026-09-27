// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The five dots Windows 10 sends along a bar that cannot say how far it has got.
    /// <para/>
    /// How far they travel depends on how wide the bar is, and UWP reads those numbers off
    /// TemplateSettings. WPF has nothing of the sort and a Storyboard carries no lookups, so the
    /// numbers have to be written into a copy of it whenever the bar changes size. This is the
    /// arithmetic behind that, kept in one place because both <see cref="MetroProgressBar"/> and
    /// <see cref="ProgressBarHelper"/> do the writing.
    /// </summary>
    internal static class ProgressBarEllipseAnimation
    {
        /// <summary>The part the dots live in, and the one the Storyboard is played on.</summary>
        internal const string ContainingGridName = "ContainingGrid";

        /// <summary>The key a template hands its unadjusted Storyboard over under.</summary>
        internal const string StoryboardKey = "IndeterminateStoryboard";

        /// <summary>The name of the state the dots belong to.</summary>
        internal const string IndeterminateStateName = "Indeterminate";

        /// <summary>The animation that carries the whole row of dots across the bar.</summary>
        private const string ContainerAnimationName = "MainDoubleAnim";

        private static readonly string[] EllipseNames = { "E1", "E2", "E3", "E4", "E5" };

        /// <summary>A wider bar gets bigger dots, in the three steps Windows 10 picks between.</summary>
        internal static double DiameterFor(double size)
        {
            return size <= 180 ? 4d : (size <= 280 ? 5d : 6d);
        }

        /// <summary>And more room between them.</summary>
        internal static double OffsetFor(double size)
        {
            return size <= 180 ? 4d : (size <= 280 ? 7d : 9d);
        }

        /// <summary>Where the row of dots waits before it sets off, which is off the left edge.</summary>
        internal static double ContainerStartFor(double size)
        {
            return size <= 180 ? -34d : (size <= 280 ? -50.5d : -63d);
        }

        /// <summary>And where it has got to when the turn is over.</summary>
        internal static double ContainerEndFor(double size)
        {
            var firstPart = 0.4352 * size;
            return size <= 180 ? firstPart - 25.731 : (size <= 280 ? firstPart + 27.84 : firstPart + 58.862);
        }

        /// <summary>The point a third of the way along where the dots gather before they run on.</summary>
        internal static double WellFor(double size)
        {
            return size * 1.0 / 3.0;
        }

        /// <summary>And the one two thirds along they leave for.</summary>
        internal static double EndFor(double size)
        {
            return size * 2.0 / 3.0;
        }

        /// <summary>
        /// Takes a copy of the template's Storyboard and writes the numbers for a bar of this size
        /// into it.
        /// </summary>
        internal static Storyboard Rebuild(Storyboard template, double size)
        {
            var storyboard = template.Clone();

            var containerAnimation = storyboard.Children.First(child => child.Name == ContainerAnimationName);
            containerAnimation.SetValue(DoubleAnimation.FromProperty, ContainerStartFor(size));
            containerAnimation.SetValue(DoubleAnimation.ToProperty, ContainerEndFor(size));

            var well = WellFor(size);
            var end = EndFor(size);

            foreach (var name in EllipseNames)
            {
                var animation = (DoubleAnimationUsingKeyFrames)storyboard.Children.First(child => child.Name == name + "Anim");

                // the first dot sets off straight away and so has one key frame less than the four behind it
                var offset = name == EllipseNames[0] ? 1 : 2;

                var first = animation.KeyFrames[offset];
                var second = animation.KeyFrames[offset + 1];
                var third = animation.KeyFrames[offset + 2];

                first.Value = well;
                second.Value = well;
                third.Value = end;

                first.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                second.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                third.InvalidateProperty(DoubleKeyFrame.ValueProperty);

                animation.InvalidateProperty(Storyboard.TargetPropertyProperty);
                animation.InvalidateProperty(Storyboard.TargetNameProperty);
            }

            return storyboard;
        }

        /// <summary>
        /// Finds the state the dots belong to, which is the one the adjusted Storyboard is hung on.
        /// </summary>
        internal static VisualState? FindIndeterminateState(FrameworkElement containingGrid)
        {
            var groups = VisualStateManager.GetVisualStateGroups(containingGrid);
            return groups?.OfType<VisualStateGroup>()
                         .SelectMany(group => group.States.OfType<VisualState>())
                         .FirstOrDefault(state => state.Name == IndeterminateStateName);
        }
    }
}
