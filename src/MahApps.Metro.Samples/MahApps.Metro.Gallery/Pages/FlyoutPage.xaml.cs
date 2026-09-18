// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for FlyoutPage.xaml
    /// </summary>
    public partial class FlyoutPage : UserControl
    {
        private Flyout? side;
        private Flyout? top;

        public FlyoutPage()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.side is not null)
            {
                return;
            }

            // a flyout only works inside a window that has somewhere to put it, and the gallery
            // keeps a FlyoutsControl for exactly this
            if (Window.GetWindow(this) is not MetroWindow window || window.Flyouts is null)
            {
                return;
            }

            this.side = this.TakeFlyout("SideFlyout");
            this.top = this.TakeFlyout("TopFlyout");

            window.Flyouts.Items.Add(this.side);
            window.Flyouts.Items.Add(this.top);

            this.SideExample.Watch(this.side,
                                   Flyout.IsOpenProperty,
                                   Flyout.PositionProperty,
                                   Flyout.HeaderProperty,
                                   Flyout.ThemeProperty,
                                   Flyout.IsPinnedProperty,
                                   Flyout.IsModalProperty,
                                   Flyout.CloseButtonVisibilityProperty,
                                   Flyout.TitleVisibilityProperty);
            this.SideExample.Watch("Layout", this.side, WidthProperty);

            this.TopExample.Watch(this.top,
                                  Flyout.IsOpenProperty,
                                  Flyout.PositionProperty,
                                  Flyout.HeaderProperty,
                                  Flyout.ThemeProperty);
            this.TopExample.Watch("Layout", this.top, HeightProperty);
        }

        /// <summary>
        /// Takes the flyout out of the display that holds it, so that it can be a child of the
        /// window instead. The display has done its job by then: the build has written the markup
        /// down and the card looks it up by its key.
        /// </summary>
        private Flyout TakeFlyout(string key)
        {
            var display = (XamlDisplay)this.FindResource(key);
            var flyout = (Flyout)display.Content;

            display.Content = null;

            return flyout;
        }

        private void OnShowSide(object sender, RoutedEventArgs e)
        {
            if (this.side is not null)
            {
                this.side.SetCurrentValue(Flyout.IsOpenProperty, true);
            }
        }

        private void OnShowTop(object sender, RoutedEventArgs e)
        {
            if (this.top is not null)
            {
                this.top.SetCurrentValue(Flyout.IsOpenProperty, true);
            }
        }
    }
}
