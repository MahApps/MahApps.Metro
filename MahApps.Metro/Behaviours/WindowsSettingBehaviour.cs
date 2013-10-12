using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                IMetroWindow window = AssociatedObject as IMetroWindow;
                if (window.SaveWindowPosition)
                {
                    // save with custom settings class or use the default way
                    var windowPlacementSettings = window.WindowPlacementSettings ?? new WindowApplicationSettings(window);
                    WindowSettings.SetSave((DependencyObject)window, windowPlacementSettings);
                }
            }
        }
    }
}