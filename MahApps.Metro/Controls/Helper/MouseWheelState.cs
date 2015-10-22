namespace MahApps.Metro.Controls
{
    public enum MouseWheelState
    {
        /// <summary>
        /// Do not change the value of the slider if the user rotates the mouse wheel.
        /// </summary>
        None,
        /// <summary>
        /// Change the value of the slider only if the control is focused.
        /// </summary>
        ControlFocused,
        /// <summary>
        /// Changes the value of the slider if the mouse pointer is over this element.
        /// </summary>
        MouseHover
    }
}