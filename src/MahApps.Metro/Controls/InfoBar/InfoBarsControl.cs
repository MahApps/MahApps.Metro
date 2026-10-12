// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A host for <see cref="InfoBar"/>s that lie over other content, the way <see cref="MetroWindow.InfoBars"/> lays them over the
    /// top of a window. Open bars stand one under the other, closed ones take no room.
    /// </summary>
    public class InfoBarsControl : ItemsControl
    {
        private static readonly DependencyPropertyKey IsHostedPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("IsHosted",
                                                          typeof(bool),
                                                          typeof(InfoBarsControl),
                                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the IsHosted attached property.</summary>
        public static readonly DependencyProperty IsHostedProperty = IsHostedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether a bar stands in an <see cref="InfoBarsControl"/>, and so over other content.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(InfoBar))]
        public static bool GetIsHosted(DependencyObject element)
        {
            return (bool)element.GetValue(IsHostedProperty);
        }

        static InfoBarsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoBarsControl), new FrameworkPropertyMetadata(typeof(InfoBarsControl)));
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new InfoBar();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is InfoBar;
        }

        /// <inheritdoc />
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            // set here rather than by a style, so that an item container style of the user's own keeps it
            element.SetValue(IsHostedPropertyKey, BooleanBoxes.TrueBox);
        }

        /// <inheritdoc />
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            element.ClearValue(IsHostedPropertyKey);

            base.ClearContainerForItemOverride(element, item);
        }
    }
}
