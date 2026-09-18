// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for FontIconPage.xaml
    /// </summary>
    public partial class FontIconPage : UserControl
    {
        public FontIconPage()
        {
            this.InitializeComponent();

            this.GlyphExample.Watch(this.Glyph,
                                    FontIcon.GlyphProperty,
                                    Control.FontFamilyProperty,
                                    Control.FontSizeProperty);
        }
    }
}
