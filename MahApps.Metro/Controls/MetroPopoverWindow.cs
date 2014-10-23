using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public class MetroPopoverWindow : ContentControl
    {
        readonly MetroPopover _popover;

        static MetroPopoverWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroPopoverWindow), new FrameworkPropertyMetadata(typeof(MetroPopoverWindow)));
        }

        public MetroPopoverWindow(MetroPopover popover)
        {
            _popover = popover;
        }


        /// <summary>
        /// Waits for the popover to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public void Show()
        {
            Dispatcher.VerifyAccess();


            this.Opacity = 1.0; 
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            Dispatcher.VerifyAccess();

            this.Opacity = 0.0;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

    }

}
