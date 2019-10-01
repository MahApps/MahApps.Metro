namespace MahApps.Metro.Controls
{
    public interface IHamburgerMenuItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether this item is enabled in the user interface (UI).
        /// </summary>
        /// <returns>
        /// true if the item is enabled; otherwise, false. The default value is true.
        /// </returns>
        bool IsEnabled { get; set; }
    }
}