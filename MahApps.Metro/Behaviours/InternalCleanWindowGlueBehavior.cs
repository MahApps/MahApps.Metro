using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    //in order to get around some short comings in XAML, I needed a code behind class that I could manipulate the window from.
    internal class InternalCleanWindowGlueBehavior : Behavior<Window>
    {
        public MetroWindow AssociatedMetroWindow { get { return this.AssociatedObject as MetroWindow; } }

        protected override void OnAttached()
        {
            if (AssociatedMetroWindow.IsLoaded)
            {
                SetWindowCommandButtonsToBlackBrush();
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = new RoutedEventHandler((sender, e) =>
                    {
                        SetWindowCommandButtonsToBlackBrush();

                        AssociatedMetroWindow.Loaded -= handler;
                    });

                AssociatedMetroWindow.Loaded += handler;
            }

            AssociatedMetroWindow.FlyoutsStatusChanged += AssociatedMetroWindow_FlyoutsStatusChanged;

            base.OnAttached();
        }

        private void SetWindowCommandButtonsToBlackBrush()
        {
            foreach (Button b in (AssociatedMetroWindow.WindowCommandsPresenter.Content as WindowCommands).FindChildren<Button>())
            {
                b.SetResourceReference(Button.ForegroundProperty, "BlackColorBrush");
            }
        }

        protected override void OnDetaching()
        {
            AssociatedMetroWindow.FlyoutsStatusChanged -= AssociatedMetroWindow_FlyoutsStatusChanged;

            base.OnDetaching();
        }

        void AssociatedMetroWindow_FlyoutsStatusChanged(object sender, RoutedEventArgs e)
        {
            MetroWindow.FlyoutStatusChangedRoutedEventArgs args = e as MetroWindow.FlyoutStatusChangedRoutedEventArgs;
            var flyout = args.ChangedFlyout;

            if (flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                if (flyout.IsOpen)
                {
                    foreach (Button b in (AssociatedMetroWindow.WindowCommandsPresenter.Content as WindowCommands).FindChildren<Button>())
                    {
                        b.SetValue(Button.ForegroundProperty, Brushes.White);
                    }
                    AssociatedMetroWindow.WindowButtonCommands.SetValue(System.Windows.Controls.Control.ForegroundProperty, Brushes.White);
                }
                else
                {
                    //var resource = AssociatedMetroWindow.TitleForeground;

                    SetWindowCommandButtonsToBlackBrush();
                    AssociatedMetroWindow.WindowButtonCommands.SetResourceReference(System.Windows.Controls.Control.ForegroundProperty, "BlackColorBrush");
                }
            }
        }
    }
}
