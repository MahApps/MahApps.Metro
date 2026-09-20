// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TilePage.xaml
    /// </summary>
    public partial class TilePage : UserControl
    {
        public TilePage()
        {
            this.InitializeComponent();

            this.TileExample.Watch(this.Simple,
                                   Tile.TitleProperty,
                                   Tile.TitleFontSizeProperty,
                                   Tile.HorizontalTitleAlignmentProperty,
                                   Tile.VerticalTitleAlignmentProperty);
            this.TileExample.Watch("Layout", this.Simple, WidthProperty, HeightProperty);

            this.CountExample.Watch(this.Counted,
                                    Tile.TitleProperty,
                                    Tile.CountProperty,
                                    Tile.CountFontSizeProperty,
                                    HorizontalContentAlignmentProperty,
                                    VerticalContentAlignmentProperty);
        }
    }
}
