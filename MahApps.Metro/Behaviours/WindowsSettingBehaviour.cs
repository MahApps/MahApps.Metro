using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<Window>
    {
        protected override void OnAttached()
        {
            WindowSettings.SetSave(AssociatedObject, true);
        }
    }
}