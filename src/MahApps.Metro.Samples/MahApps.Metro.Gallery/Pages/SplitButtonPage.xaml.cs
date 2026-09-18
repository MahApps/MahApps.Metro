// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for SplitButtonPage.xaml
    /// </summary>
    public partial class SplitButtonPage : UserControl
    {
        public SplitButtonPage()
        {
            this.InitializeComponent();

            this.SplitExample.Watch(this.Split, Selector.SelectedIndexProperty, IsEnabledProperty);
            this.SplitExample.Watch("Layout", this.Split, WidthProperty);

            this.IconExample.Watch(this.WithIcon, Selector.SelectedIndexProperty, SplitButton.OrientationProperty);
        }
    }
}
