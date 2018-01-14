using System.Windows.Automation.Peers;

namespace MahApps.Metro.Controls.Dialogs
{
    public class MetroDialogAutomationPeer : FrameworkElementAutomationPeer
    {
        public MetroDialogAutomationPeer(BaseMetroDialog owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetNameCore()
        {
            string ownerTitle = ((BaseMetroDialog)this.Owner).Title;
            string nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(ownerTitle))
            {
                nameCore = $"{nameCore} {ownerTitle}";
            }

            return nameCore;
        }
    }
}