// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for RatingControlPage.xaml
    /// </summary>
    public partial class RatingControlPage : UserControl
    {
        public RatingControlPage()
        {
            this.InitializeComponent();

            this.BasicExample.Watch(this.BasicRating,
                                    RatingControl.ValueProperty,
                                    RatingControl.MaxRatingProperty,
                                    RatingControl.PlaceholderValueProperty,
                                    RatingControl.InitialSetValueProperty,
                                    RatingControl.IsClearEnabledProperty,
                                    RatingControl.IsHalfStepEnabledProperty,
                                    RatingControl.IsReadOnlyProperty,
                                    UIElement.IsEnabledProperty,
                                    RatingControl.CaptionProperty);
        }
    }
}
