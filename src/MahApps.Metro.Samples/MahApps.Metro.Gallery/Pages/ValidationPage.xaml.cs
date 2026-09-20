// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ValidationPage.xaml
    /// </summary>
    public partial class ValidationPage : UserControl
    {
        public ValidationPage()
        {
            this.InitializeComponent();

            // the samples are bindings, and a binding needs something to be bound to
            this.DataContext = new ValidationSample();

            this.ErrorExample.Watch(this.Age, IsEnabledProperty);
            this.ErrorExample.Watch("Attached", this.Age, ControlsHelper.CornerRadiusProperty);
            this.ErrorExample.Watch("Layout", this.Age, WidthProperty);

            this.ShowExample.Watch("Attached",
                                   this.Shown,
                                   ValidationHelper.ShowValidationErrorOnKeyboardFocusProperty,
                                   ValidationHelper.ShowValidationErrorOnMouseOverProperty,
                                   ValidationHelper.AlwaysShowValidationErrorProperty,
                                   ValidationHelper.CloseOnMouseLeftButtonDownProperty);

            this.ManyExample.Watch("Attached",
                                   this.Secret,
                                   ValidationHelper.AlwaysShowValidationErrorProperty);

            this.ElsewhereExample.Watch("The lower slider", this.Marked, IsEnabledProperty);
        }
    }
}
