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
            this.SplitExample.Watch("Bound", this.SplitBound, Selector.SelectedIndexProperty, IsEnabledProperty);
            this.SplitExample.Watch("Layout", this.Split, WidthProperty);

            this.IconExample.Watch(this.WithIcon, Selector.SelectedIndexProperty, SplitButton.OrientationProperty);

            this.Win10Example.Watch(this.Win10,
                                    Selector.SelectedIndexProperty,
                                    SplitButton.OrientationProperty,
                                    IsEnabledProperty);
            this.Win10Example.Watch("Attached", this.Win10, ControlsHelper.CornerRadiusProperty);
            this.Win10Example.Watch("Bound", this.Win10Bound, Selector.SelectedIndexProperty, IsEnabledProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    Selector.SelectedIndexProperty,
                                    SplitButton.OrientationProperty,
                                    IsEnabledProperty);
            this.WinUIExample.Watch("Attached", this.WinUI, ControlsHelper.CornerRadiusProperty);
            this.WinUIExample.Watch("Bound", this.WinUIBound, Selector.SelectedIndexProperty, IsEnabledProperty);
        }
    }
}
