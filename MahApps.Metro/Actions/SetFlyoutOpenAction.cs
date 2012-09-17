using System.Windows;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class SetFlyoutOpenAction : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(SetFlyoutOpenAction), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty InvertTargetProperty = DependencyProperty.Register("InvertTarget", typeof(bool), typeof(SetFlyoutOpenAction), new PropertyMetadata(default(bool)));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool InvertTarget
        {
            get { return (bool)GetValue(InvertTargetProperty); }
            set { SetValue(InvertTargetProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            Flyout targetFlyout = TargetObject as Flyout;
            if (InvertTarget) 
                targetFlyout.IsOpen = !targetFlyout.IsOpen;
            else 
                targetFlyout.IsOpen = Value;
        }
    }
}
