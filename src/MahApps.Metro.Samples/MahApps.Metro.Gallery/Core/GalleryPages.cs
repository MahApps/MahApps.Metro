// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahApps.Metro.Gallery.Pages;
using MahApps.Metro.IconPacks;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// Every page there is, in the order the navigation shows them. Add a page here and it turns up
    /// in the navigation and in the search, both of which read nothing else.
    /// </summary>
    public static class GalleryPages
    {
        public static IReadOnlyList<GalleryPage> All { get; } = new[]
                                                               {
                                                                   new GalleryPage("NumericUpDown",
                                                                                   "Controls",
                                                                                   typeof(NumericUpDownPage),
                                                                                   PackIconMaterialKind.Counter,
                                                                                   "controls/numericupdown",
                                                                                   "number spinner value"),
                                                                   new GalleryPage("Badged",
                                                                                   "Controls",
                                                                                   typeof(BadgedPage),
                                                                                   PackIconMaterialKind.Numeric1CircleOutline,
                                                                                   "controls/badged",
                                                                                   "badge count notification"),
                                                                   new GalleryPage("Button",
                                                                                   "Styles",
                                                                                   typeof(ButtonPage),
                                                                                   PackIconMaterialKind.GestureTapButton,
                                                                                   "styles/buttons",
                                                                                   "square circle flat accent")
                                                               };
    }
}
