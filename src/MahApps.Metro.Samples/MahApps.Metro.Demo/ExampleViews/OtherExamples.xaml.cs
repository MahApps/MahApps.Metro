﻿using System;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;

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

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    flipview.BannerText = "Cupcakes!";
                    break;
                case 1:
                    flipview.BannerText = "Xbox!";
                    break;
                case 2:
                    flipview.BannerText = "Chess!";
                    break;
            }
        }
    }
}
