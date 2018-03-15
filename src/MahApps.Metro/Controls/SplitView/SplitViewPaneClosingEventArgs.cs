namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Provides event data for the <see cref="SplitView.PaneClosing" /> event.
    /// </summary>
    public sealed class SplitViewPaneClosingEventArgs
#if NET4
        : System.EventArgs
#endif
    {
        /// <summary>
        ///     Gets or sets a value that indicates whether the pane closing action should be canceled.
        /// </summary>
        /// <returns>true to cancel the pane closing action; otherwise, false.</returns>
        public bool Cancel { get; set; }
    }
}