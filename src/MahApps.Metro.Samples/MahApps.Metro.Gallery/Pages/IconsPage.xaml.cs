// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// The glyphs of the Segoe icon fonts, a tab per font, laid out like the Iconography page of
    /// the WinUI 3 Gallery: a search, the glyphs, and the snippets for the one picked.
    /// </summary>
    public partial class IconsPage : UserControl
    {
        public IconsPage()
        {
            this.InitializeComponent();

            this.Tabs.ItemsSource = IconFont.Available((FontFamily)this.FindResource("MahApps.Fonts.Family.SymbolTheme"));
            this.Tabs.SelectedIndex = 0;
        }
    }
}
