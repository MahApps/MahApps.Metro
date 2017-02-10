using System.Security;
using System.Windows.Automation.Peers;
using JetBrains.Annotations;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Controls.AutomationPeers
{
    public class BaseMetroDialogAutomationPeer : FrameworkElementAutomationPeer
    {
        [NotNull]
        private readonly BaseMetroDialog owner;

        public BaseMetroDialogAutomationPeer([NotNull] BaseMetroDialog owner)
            : base(owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets the name of the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>A string that contains the word "Window".</returns>
        protected override string GetClassNameCore()
        {
            return "Window";
        }

        /// <summary>
        /// Gets the control type for the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Window" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        /// <summary>
        /// Gets the title of the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.
        /// </summary>
        /// <returns>A string that contains the <see cref="BaseMetroDialog.Title" /> of the <see cref="BaseMetroDialog" />, or <see cref="F:System.String.Empty" /> if the title is null.</returns>
        [SecurityCritical]
        [SecuritySafeCritical]
        protected override string GetNameCore()
        {
            return this.owner.Title ?? string.Empty;
        }
    }
}
