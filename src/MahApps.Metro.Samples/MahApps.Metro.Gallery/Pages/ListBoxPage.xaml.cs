// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ListBoxPage.xaml
    /// </summary>
    public partial class ListBoxPage : UserControl
    {
        public ListBoxPage()
        {
            this.InitializeComponent();

            this.ListExample.Watch(this.List,
                                   ListBox.SelectionModeProperty,
                                   IsEnabledProperty);
            this.ListExample.Watch("Layout", this.List, WidthProperty, HeightProperty);

            this.BorderExample.Watch(this.Bordered,
                                     Control.BorderThicknessProperty,
                                     ListBox.SelectionModeProperty);
            this.BorderExample.Watch("Layout", this.Bordered, WidthProperty, HeightProperty);
        }
    }
}
