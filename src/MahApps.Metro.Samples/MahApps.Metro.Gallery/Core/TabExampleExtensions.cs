// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Controls;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// What every card about a row of tabs offers. TabControlHelper is where the library keeps the
    /// knobs for a tab strip, and a reader looking at one of those cards wants all of them rather
    /// than the handful a single card happens to be about, so the three pages hand the whole lot
    /// over in one line.
    /// </summary>
    public static class TabExampleExtensions
    {
        /// <summary>
        /// Puts every property TabControlHelper carries for a tab control into the options of a
        /// card, beside the other attached ones.
        /// </summary>
        /// <param name="example">The card the options belong to.</param>
        /// <param name="tabControl">The tab control of that card, the first of the three it shows.</param>
        /// <param name="theCloseButtonToo">
        /// Whether the switch for the close button belongs on the control of this card. That one is
        /// an inherited property, so an answer on the control reaches every tab under it, but only
        /// where the tabs have nothing of their own to say: a tab that carries the answer itself,
        /// which the one that can be closed does, and a tab whose style sets it, which the Visual
        /// Studio one does, both beat what came down from above. Those cards put the switch on the
        /// tab instead and leave this one out.
        /// </param>
        public static void WatchTheTabControlHelper(this ControlExample example, DependencyObject tabControl, bool theCloseButtonToo = true)
        {
            // the group is the one the rest of the gallery puts an attached property in, and the
            // rows already carry the name of the helper, so a group named after it would say the
            // same thing twice on every line
            example.Watch("Attached",
                          tabControl,
                          TabControlHelper.UnderlinedProperty,
                          TabControlHelper.UnderlinePlacementProperty,
                          TabControlHelper.UnderlineMarginProperty,
                          TabControlHelper.UnderlineBrushProperty,
                          TabControlHelper.UnderlineSelectedBrushProperty,
                          TabControlHelper.UnderlineMouseOverBrushProperty,
                          TabControlHelper.UnderlineMouseOverSelectedBrushProperty,
                          TabControlHelper.TransitionProperty,
                          TabControlHelper.TabWidthModeProperty);

            if (theCloseButtonToo)
            {
                example.Watch("Attached", tabControl, TabControlHelper.CloseButtonEnabledProperty);
            }
        }
    }
}
