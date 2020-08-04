// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Provides calculated values that can be referenced as TemplatedParent sources when defining templates for a
    ///     <see cref="SplitView" />.
    ///     Not intended for general use.
    /// </summary>
    public sealed class SplitViewTemplateSettings : DependencyObject
    {
        /// <summary>Identifies the <see cref="CompactPaneGridLength"/> dependency property.</summary>
        internal static readonly DependencyProperty CompactPaneGridLengthProperty
            = DependencyProperty.Register(nameof(CompactPaneGridLength),
                                          typeof(GridLength),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets the <see cref="SplitView.CompactPaneLength" /> value as a GridLength.
        /// </summary>
        public GridLength CompactPaneGridLength
        {
            get => (GridLength)this.GetValue(CompactPaneGridLengthProperty);
            private set => this.SetValue(CompactPaneGridLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeOpenPaneLength"/> dependency property.</summary>
        internal static readonly DependencyProperty NegativeOpenPaneLengthProperty
            = DependencyProperty.Register(nameof(NegativeOpenPaneLength),
                                          typeof(double),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the negative of the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double NegativeOpenPaneLength
        {
            get => (double)this.GetValue(NegativeOpenPaneLengthProperty);
            private set => this.SetValue(NegativeOpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeOpenPaneLengthMinusCompactLength"/> dependency property.</summary>
        internal static readonly DependencyProperty NegativeOpenPaneLengthMinusCompactLengthProperty
            = DependencyProperty.Register(nameof(NegativeOpenPaneLengthMinusCompactLength),
                                          typeof(double),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the negative of the value calculated by subtracting the <see cref="SplitView.CompactPaneLength" /> value from
        ///     the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double NegativeOpenPaneLengthMinusCompactLength
        {
            get => (double)this.GetValue(NegativeOpenPaneLengthMinusCompactLengthProperty);
            set => this.SetValue(NegativeOpenPaneLengthMinusCompactLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="OpenPaneGridLength"/> dependency property.</summary>
        internal static readonly DependencyProperty OpenPaneGridLengthProperty
            = DependencyProperty.Register(nameof(OpenPaneGridLength),
                                          typeof(GridLength),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets the <see cref="SplitView.OpenPaneLength" /> value as a GridLength.
        /// </summary>
        public GridLength OpenPaneGridLength
        {
            get => (GridLength)this.GetValue(OpenPaneGridLengthProperty);
            private set => this.SetValue(OpenPaneGridLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="OpenPaneLength"/> dependency property.</summary>
        internal static readonly DependencyProperty OpenPaneLengthProperty
            = DependencyProperty.Register(nameof(OpenPaneLength),
                                          typeof(double),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double OpenPaneLength
        {
            get => (double)this.GetValue(OpenPaneLengthProperty);
            private set => this.SetValue(OpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="OpenPaneLengthMinusCompactLength"/> dependency property.</summary>
        internal static readonly DependencyProperty OpenPaneLengthMinusCompactLengthProperty
            = DependencyProperty.Register(nameof(OpenPaneLengthMinusCompactLength),
                                          typeof(double),
                                          typeof(SplitViewTemplateSettings),
                                          new PropertyMetadata(0d));

        /// <summary>
        ///     Gets a value calculated by subtracting the <see cref="SplitView.CompactPaneLength" /> value from the
        ///     <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double OpenPaneLengthMinusCompactLength
        {
            get => (double)this.GetValue(OpenPaneLengthMinusCompactLengthProperty);
            private set => this.SetValue(OpenPaneLengthMinusCompactLengthProperty, value);
        }

        internal SplitViewTemplateSettings(SplitView owner)
        {
            this.Owner = owner;
            this.Update();
        }

        internal SplitView Owner { get; }

        internal void Update()
        {
            this.CompactPaneGridLength = new GridLength(this.Owner.CompactPaneLength, GridUnitType.Pixel);
            this.OpenPaneGridLength = new GridLength(this.Owner.OpenPaneLength, GridUnitType.Pixel);

            this.OpenPaneLength = this.Owner.OpenPaneLength;
            this.OpenPaneLengthMinusCompactLength = this.Owner.OpenPaneLength - this.Owner.CompactPaneLength;

            this.NegativeOpenPaneLength = -this.OpenPaneLength;
            this.NegativeOpenPaneLengthMinusCompactLength = -this.OpenPaneLengthMinusCompactLength;
        }
    }
}