namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An implementation of BaseMetroDialog allowing arbitrary content.
    /// </summary>
    public class CustomDialog : BaseMetroDialog
    {
        public CustomDialog()
            : this(null, null)
        {
        }

        public CustomDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        public CustomDialog(MetroDialogSettings settings)
            : this(null, settings)
        {
        }

        public CustomDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
        }
    }
}