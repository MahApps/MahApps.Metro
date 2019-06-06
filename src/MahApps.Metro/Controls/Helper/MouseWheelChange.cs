namespace MahApps.Metro.Controls
{
    using System.Windows.Controls.Primitives;

    public enum MouseWheelChange
    {
        /// <summary>
        /// Change the value of the slider if the user rotates the mouse wheel by the value defined for <see cref="RangeBase.SmallChange"/>
        /// </summary>
        SmallChange,
        /// <summary>
        /// Change the value of the slider if the user rotates the mouse wheel by the value defined for <see cref="RangeBase.LargeChange"/>
        /// </summary>
        LargeChange
    }
}