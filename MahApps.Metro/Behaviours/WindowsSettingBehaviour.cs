using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null && AssociatedObject.WindowsSettings != null)
            {
                AssociatedObject.WindowsSettings.SetSave(AssociatedObject, AssociatedObject.SaveWindowPosition);
            }
        }
    }
}