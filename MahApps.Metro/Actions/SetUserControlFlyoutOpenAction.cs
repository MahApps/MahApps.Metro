using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class SetUserControlFlyoutOpenAction : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(SetUserControlFlyoutOpenAction), new PropertyMetadata(default(bool)));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            ((UserControlFlyout) TargetObject).IsOpen = Value;
        }
    }
}
