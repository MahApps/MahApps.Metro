// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for OtherExamples.xaml
    /// </summary>
    public partial class OtherExamples : UserControl
    {
        public OtherExamples()
        {
            InitializeComponent();

            var t = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, Tick, this.Dispatcher);
        }

        void Tick(object sender, EventArgs e)
        {
            if (this.IsVisible == false)
            {
                return;
            }

            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock { Text = "Transitioning Content! " + dateTime, SnapsToDevicePixels = true };
            customTransitioning.Content = new TextBlock { Text = "Custom transistion! " + dateTime, SnapsToDevicePixels = true };
            SecondcustomTransitioning.Content = new TextBlock { Text = "Second custom transistion! " + dateTime, SnapsToDevicePixels = true };
        }
    }
}