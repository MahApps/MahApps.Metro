using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroHeaderControl : GroupBox
    {
        static MetroHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroHeaderControl), new FrameworkPropertyMetadata(typeof(MetroHeaderControl)));
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="M:System.Windows.UIElement.OnCreateAutomationPeer" />)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroHeaderControlAutomationPeer(this);
        }
    }

    public class MetroHeaderControlAutomationPeer : GroupBoxAutomationPeer
    {
        public MetroHeaderControlAutomationPeer(GroupBox owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "MetroHeaderControl";
        }
    }
}