using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroHeader : GroupBox
    {
        static MetroHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroHeader), new FrameworkPropertyMetadata(typeof(MetroHeader)));
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="M:System.Windows.UIElement.OnCreateAutomationPeer" />)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroHeaderAutomationPeer(this);
        }
    }
}