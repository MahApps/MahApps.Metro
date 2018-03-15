namespace MahApps.Metro.Controls
{
    using System.Windows;

    /// <summary>
    ///     Provides calculated values that can be referenced as TemplatedParent sources when defining templates for a
    ///     <see cref="SplitView" />.
    ///     Not intended for general use.
    /// </summary>
    public sealed class SplitViewTemplateSettings : DependencyObject
    {
        internal static readonly DependencyProperty CompactPaneGridLengthProperty =
            DependencyProperty.Register("CompactPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), new PropertyMetadata(null));

        /// <summary>
        ///     Gets the <see cref="SplitView.CompactPaneLength" /> value as a GridLength.
        /// </summary>
        public GridLength CompactPaneGridLength
        {
            get { return (GridLength)this.GetValue(CompactPaneGridLengthProperty); }
            private set { this.SetValue(CompactPaneGridLengthProperty, value); }
        }

        internal static readonly DependencyProperty NegativeOpenPaneLengthProperty =
            DependencyProperty.Register("NegativeOpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the negative of the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double NegativeOpenPaneLength
        {
            get { return (double)this.GetValue(NegativeOpenPaneLengthProperty); }
            private set { this.SetValue(NegativeOpenPaneLengthProperty, value); }
        }

        internal static readonly DependencyProperty NegativeOpenPaneLengthMinusCompactLengthProperty =
            DependencyProperty.Register("NegativeOpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the negative of the value calculated by subtracting the <see cref="SplitView.CompactPaneLength" /> value from
        ///     the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double NegativeOpenPaneLengthMinusCompactLength
        {
            get { return (double)this.GetValue(NegativeOpenPaneLengthMinusCompactLengthProperty); }
            set { this.SetValue(NegativeOpenPaneLengthMinusCompactLengthProperty, value); }
        }

        internal static readonly DependencyProperty OpenPaneGridLengthProperty =
            DependencyProperty.Register("OpenPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), new PropertyMetadata(null));

        /// <summary>
        ///     Gets the <see cref="SplitView.OpenPaneLength" /> value as a GridLength.
        /// </summary>
        public GridLength OpenPaneGridLength
        {
            get { return (GridLength)this.GetValue(OpenPaneGridLengthProperty); }
            private set { this.SetValue(OpenPaneGridLengthProperty, value); }
        }

        internal static readonly DependencyProperty OpenPaneLengthProperty =
            DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), new PropertyMetadata(0d));

        /// <summary>
        ///     Gets the <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double OpenPaneLength
        {
            get { return (double)this.GetValue(OpenPaneLengthProperty); }
            private set { this.SetValue(OpenPaneLengthProperty, value); }
        }

        internal static readonly DependencyProperty OpenPaneLengthMinusCompactLengthProperty =
            DependencyProperty.Register("OpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), new PropertyMetadata(0d));

        /// <summary>
        ///     Gets a value calculated by subtracting the <see cref="SplitView.CompactPaneLength" /> value from the
        ///     <see cref="SplitView.OpenPaneLength" /> value.
        /// </summary>
        public double OpenPaneLengthMinusCompactLength
        {
            get { return (double)this.GetValue(OpenPaneLengthMinusCompactLengthProperty); }
            private set { this.SetValue(OpenPaneLengthMinusCompactLengthProperty, value); }
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