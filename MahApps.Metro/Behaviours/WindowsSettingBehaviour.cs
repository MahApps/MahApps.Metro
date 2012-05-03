using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        protected override void OnAttached()
        {
            WindowSettings.SetSave(AssociatedObject, AssociatedObject.SaveWindowPosition);
        }
    }
}