using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace MetroDemo.ExampleWindows
{
    public partial class VSDemo
    {
        public VSDemo()
        {
            InitializeComponent();
        }

        public ICommand QuickLaunchBarFocusCommand => new ActionCommand(FocusQuickLaunchBar);

        private void FocusQuickLaunchBar()
        {
            QuickLaunchBar.Focus();
        }
    }
}