// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for ButtonsExample.xaml
    /// </summary>
    public partial class ButtonsExample : UserControl
    {
        public ButtonsExample()
        {
            this.InitializeComponent();
        }

        private void CountingButton_OnClick(object sender, RoutedEventArgs e)
        {
            var badge = (this.CountingBadge.Badge as int?).GetValueOrDefault(0);
            var next = badge + 1;
            this.CountingBadge.SetCurrentValue(Badged.BadgeProperty, next < 43 ? next : null);
        }

        private void SplitButton_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ((Selector)sender).SelectedIndex;
            var item = ((Selector)sender).SelectedItem;
            var value = ((Selector)sender).SelectedValue;
            Debug.WriteLine($">> SplitButton SelectionChanged: index={index}, item={item}, value={value}");
        }
    }
}