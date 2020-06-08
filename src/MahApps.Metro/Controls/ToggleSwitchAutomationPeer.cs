using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider
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

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.Toggle ? this : base.GetPattern(patternInterface);
        }

        // BUG 1555137: Never inline, as we don't want to unnecessarily link the automation DLL
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal virtual void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(oldValue), ConvertToToggleState(newValue));
            }
        }

        private static ToggleState ConvertToToggleState(bool value)
        {
            return value ? ToggleState.On : ToggleState.Off;
        }

        public ToggleState ToggleState => ConvertToToggleState(((ToggleSwitch)this.Owner).IsOn);

        public void Toggle()
        {
            if (this.IsEnabled())
            {
                ((ToggleSwitch)this.Owner).AutomationPeerToggle();
            }
        }
    }
}