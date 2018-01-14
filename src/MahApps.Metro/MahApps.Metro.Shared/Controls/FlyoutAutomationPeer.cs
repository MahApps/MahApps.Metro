using System.Windows.Automation.Peers;

namespace MahApps.Metro.Controls
{
    public class FlyoutAutomationPeer : FrameworkElementAutomationPeer
    {
        public FlyoutAutomationPeer(Flyout owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "Flyout";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((Flyout)this.Owner).Header as string;
            }

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((Flyout)this.Owner).Name;
            }

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = GetClassNameCore();
            }

            return nameCore;
        }
    }
}