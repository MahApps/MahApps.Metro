// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TreeViewPage.xaml
    /// </summary>
    public partial class TreeViewPage : UserControl
    {
        public TreeViewPage()
        {
            this.InitializeComponent();

            // more nodes than anybody writes down, which is what the virtualised style is for
            this.Big.ItemsSource = Enumerable
                                   .Range(1, 400)
                                   .Select(folder => new KeyValuePair<string, IEnumerable<string>>(
                                               $"Folder {folder}",
                                               Enumerable.Range(1, 42).Select(file => $"File {file}").ToList()))
                                   .ToList();

            this.TreeExample.Watch(this.Tree,
                                   Control.BorderThicknessProperty,
                                   Control.BorderBrushProperty,
                                   IsEnabledProperty);
            this.TreeExample.Watch("Attached", this.Tree, ControlsHelper.CornerRadiusProperty);
            this.TreeExample.Watch("The Invoices node", this.Invoices, TreeViewItem.IsSelectedProperty);
            this.TreeExample.Watch("Layout", this.Tree, WidthProperty, HeightProperty);

            this.ColoursExample.Watch("The open node",
                                      this.Coloured,
                                      TreeViewItem.IsSelectedProperty,
                                      ItemHelper.SelectedBackgroundBrushProperty,
                                      ItemHelper.SelectedForegroundBrushProperty,
                                      ItemHelper.ActiveSelectionBackgroundBrushProperty,
                                      ItemHelper.ActiveSelectionForegroundBrushProperty,
                                      ItemHelper.HoverBackgroundBrushProperty,
                                      ItemHelper.HoverForegroundBrushProperty);

            this.ExpanderExample.Watch(this.Branch, TreeViewItem.IsExpandedProperty);

            this.BigExample.Watch("Layout", this.Big, WidthProperty, HeightProperty);
        }
    }
}
