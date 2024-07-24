using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class NumericUpdDownAutomationPeer : FrameworkElementAutomationPeer
    {
         public NumericUpdDownAutomationPeer([NotNull] NumericUpDown owner)
            : base(owner)
        {
        }
        protected override string GetClassNameCore()
        {
            return "NumericUpDown";
        }
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

    }

}
