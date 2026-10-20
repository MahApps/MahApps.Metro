// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for PipsPagerPage.xaml
    /// </summary>
    public partial class PipsPagerPage : UserControl
    {
        public PipsPagerPage()
        {
            this.InitializeComponent();

            this.BasicExample.Watch(this.BasicPager,
                                    PipsPager.NumberOfPagesProperty,
                                    PipsPager.SelectedPageIndexProperty,
                                    PipsPager.MaxVisiblePipsProperty,
                                    PipsPager.OrientationProperty,
                                    PipsPager.PreviousButtonVisibilityProperty,
                                    PipsPager.NextButtonVisibilityProperty,
                                    PipsPager.WrapModeProperty);
        }
    }
}
