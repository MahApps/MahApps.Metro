namespace MahApps.Metro.Behaviours
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;
    using System.Windows.Threading;
    using MahApps.Metro.Controls;

    public class DatePickerTextBoxBehavior : Behavior<DatePickerTextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += this.OnTextChanged;
            this.BeginInvoke(this.SetHasTextProperty, DispatcherPriority.Loaded);
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.TextChanged -= this.OnTextChanged;
            base.OnDetaching();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetHasTextProperty();
        }

        private void SetHasTextProperty()
        {
            this.AssociatedObject.TemplatedParent?.SetValue(TextBoxHelper.HasTextProperty, this.AssociatedObject.Text.Length > 0);
        }
    }
}