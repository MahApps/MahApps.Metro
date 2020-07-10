// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TabControlExamples.xaml
    /// </summary>
    public partial class TabControlExamples : UserControl
    {
        public TabControlExamples()
        {
            this.InitializeComponent();
        }

        private void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            if (e.ClosingTabItem.Header.ToString().StartsWith("sizes"))
            {
                e.Cancel = true;
            }
        }

        private void TextBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;

            textBlock.SetCurrentValue(TextBlock.TextProperty, (int.Parse(textBlock.Text) + 1).ToString());
        }
    }

    public static class DockHelper
    {
        public static IEnumerable GetDockValues()
        {
            yield return null;
            foreach (var dockValue in Enum.GetValues(typeof(Dock)))
            {
                yield return dockValue;
            }
        }
    }
}