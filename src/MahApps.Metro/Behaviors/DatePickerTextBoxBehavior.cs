// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.ValueBoxes;
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
            this.AssociatedObject.TemplatedParent?.SetCurrentValue(TextBoxHelper.HasTextProperty, BooleanBoxes.Box(this.AssociatedObject.Text.Length > 0));
        }
    }
}