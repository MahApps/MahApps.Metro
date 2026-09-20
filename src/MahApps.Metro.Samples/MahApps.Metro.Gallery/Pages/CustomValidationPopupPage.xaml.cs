// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for CustomValidationPopupPage.xaml
    /// </summary>
    public partial class CustomValidationPopupPage : UserControl
    {
        public CustomValidationPopupPage()
        {
            this.InitializeComponent();

            // the popup arrives with a validation error, and an error needs something to be wrong
            this.DataContext = new ValidationSample();

            this.InPlaceExample.Watch("Attached",
                                      this.Age,
                                      ValidationHelper.AlwaysShowValidationErrorProperty,
                                      ValidationHelper.ShowValidationErrorOnMouseOverProperty,
                                      ValidationHelper.CloseOnMouseLeftButtonDownProperty);

            this.MovingExample.Watch("The content around the box",
                                     this.Moving,
                                     MetroContentControl.TransitionsEnabledProperty,
                                     MetroContentControl.ReverseTransitionProperty,
                                     MetroContentControl.IsTransitioningProperty);
            this.MovingExample.Watch("The box",
                                     this.Email,
                                     ValidationHelper.AlwaysShowValidationErrorProperty);
        }

        /// <summary>
        /// The transition runs on load and on becoming visible, so playing it in between takes an
        /// ask, and the popup has to get out of the way while it does.
        /// </summary>
        private void OnReload(object sender, RoutedEventArgs e)
        {
            this.Moving.Reload();
        }
    }
}
