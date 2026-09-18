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
                                                                   new GalleryPage("AutoSuggestBox",
                                                                                   "Controls",
                                                                                   typeof(AutoSuggestBoxPage),
                                                                                   PackIconMaterialKind.TextSearch,
                                                                                   keywords: "search suggest complete"),
                                                                   new GalleryPage("Badged",
                                                                                   "Controls",
                                                                                   typeof(BadgedPage),
                                                                                   PackIconMaterialKind.Numeric1CircleOutline,
                                                                                   keywords: "badge count notification"),
                                                                   new GalleryPage("ColorPicker",
                                                                                   "Controls",
                                                                                   typeof(ColorPickerPage),
                                                                                   PackIconMaterialKind.Palette,
                                                                                   keywords: "colour canvas palette"),
                                                                   new GalleryPage("DateTimePicker",
                                                                                   "Controls",
                                                                                   typeof(DateTimePickerPage),
                                                                                   PackIconMaterialKind.CalendarClock,
                                                                                   keywords: "date time clock calendar"),
                                                                   new GalleryPage("DropDownButton",
                                                                                   "Controls",
                                                                                   typeof(DropDownButtonPage),
                                                                                   PackIconMaterialKind.MenuDown,
                                                                                   keywords: "menu split button"),
                                                                   new GalleryPage("NumericUpDown",
                                                                                   "Controls",
                                                                                   typeof(NumericUpDownPage),
                                                                                   PackIconMaterialKind.Counter,
                                                                                   keywords: "number spinner value"),
                                                                   new GalleryPage("Button",
                                                                                   "Styles",
                                                                                   typeof(ButtonPage),
                                                                                   PackIconMaterialKind.GestureTapButton,
                                                                                   // the page over there is called buttons, not button
                                                                                   "styles/buttons",
                                                                                   "square circle flat accent")
                                                               };
    }
}
