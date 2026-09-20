// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using ControlzEx;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for BadgedPage.xaml
    /// </summary>
    public partial class BadgedPage : UserControl
    {
        public BadgedPage()
        {
            this.InitializeComponent();

            this.BadgeExample.Watch(this.Badge,
                                    BadgedEx.BadgeProperty,
                                    BadgedEx.BadgePlacementModeProperty,
                                    BadgedEx.BadgeFontSizeProperty,
                                    BadgedEx.BadgeMarginProperty);
        }
    }
}
