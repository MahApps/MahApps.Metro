using System.Windows.Automation;
using System.Windows.Automation.Peers;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>Initializes a new instance of the <see cref="T:MahApps.Metro.Controls.ToggleSwitchAutomationPeer" /> class.</summary>
        /// <param name="owner">The <see cref="T:MahApps.Metro.Controls.ToggleSwitch" /> associated with this <see cref="T:MahApps.Metro.Controls.ToggleSwitchAutomationPeer" />.</param>
        public ToggleSwitchAutomationPeer([NotNull] ToggleSwitch owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "ToggleSwitch";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        public ToggleState ToggleState => ((ToggleSwitch)this.Owner).IsOn ? ToggleState.On : ToggleState.Off;

        public void Toggle()
        {
            if (this.IsEnabled())
            {
                ((ToggleSwitch)this.Owner).AutomationPeerToggle();
            }
        }
    }
}