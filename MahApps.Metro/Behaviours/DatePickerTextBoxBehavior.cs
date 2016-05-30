namespace MahApps.Metro.Behaviours
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;
    using MahApps.Metro.Controls;

    public class DatePickerTextBoxBehavior : Behavior<DatePickerTextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AssociatedObject.TemplatedParent.SetValue(TextBoxHelper.HasTextProperty, AssociatedObject.Text.Length > 0);
        }
    }
}
