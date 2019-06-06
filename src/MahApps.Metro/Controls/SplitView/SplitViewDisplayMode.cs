namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Constants that specify how the pane is shown in a <see cref="SplitView" />.
    /// </summary>
    public enum SplitViewDisplayMode
    {
        /// <summary>
        ///     The pane covers the content when it's open and does not take up space in the control layout.
        /// </summary>
        Overlay = 0,

        /// <summary>
        ///     The pane is shown side-by-side with the content and takes up space in the control layout.
        /// </summary>
        Inline = 1,

        /// <summary>
        ///     The amount of the pane defined by the <see cref="SplitView.CompactPaneLength" /> property is shown side-by-side
        ///     with the content and takes up space in the control layout.
        ///     The remaining part of the pane covers the content when it's open and does not take up space in the control layout.
        /// </summary>
        CompactOverlay = 2,

        /// <summary>
        ///     The amount of the pane defined by the <see cref="SplitView.CompactPaneLength" /> property is shown side-by-side
        ///     with the content and takes up space in the control layout.
        ///     The remaining part of the pane pushes the content to the side when it's open and takes up space in the control
        ///     layout.
        /// </summary>
        CompactInline = 3
    }
}