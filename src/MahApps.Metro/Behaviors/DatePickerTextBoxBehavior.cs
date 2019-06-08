﻿using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{

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