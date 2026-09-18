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
                                                                                   keywords: "search suggest complete",
                                                                                   control: "Controls/AutoSuggestBox/AutoSuggestBox.cs"),
                                                                   new GalleryPage("Badged",
                                                                                   "Controls",
                                                                                   typeof(BadgedPage),
                                                                                   PackIconMaterialKind.Numeric1CircleOutline,
                                                                                   keywords: "badge count notification",
                                                                                   control: "Controls/Badged.cs"),
                                                                   new GalleryPage("ColorPicker",
                                                                                   "Controls",
                                                                                   typeof(ColorPickerPage),
                                                                                   PackIconMaterialKind.Palette,
                                                                                   keywords: "colour canvas palette",
                                                                                   control: "Controls/ColorPicker/ColorPicker.cs"),
                                                                   new GalleryPage("DateTimePicker",
                                                                                   "Controls",
                                                                                   typeof(DateTimePickerPage),
                                                                                   PackIconMaterialKind.CalendarClock,
                                                                                   keywords: "date time clock calendar",
                                                                                   control: "Controls/TimePicker/DateTimePicker.cs"),
                                                                   new GalleryPage("DropDownButton",
                                                                                   "Controls",
                                                                                   typeof(DropDownButtonPage),
                                                                                   PackIconMaterialKind.MenuDown,
                                                                                   keywords: "menu split button",
                                                                                   control: "Controls/DropDownButton.cs"),
                                                                   new GalleryPage("FlipView",
                                                                                   "Controls",
                                                                                   typeof(FlipViewPage),
                                                                                   PackIconMaterialKind.ViewCarouselOutline,
                                                                                   keywords: "carousel banner pages",
                                                                                   control: "Controls/FlipView.cs"),
                                                                   new GalleryPage("Flyout",
                                                                                   "Controls",
                                                                                   typeof(FlyoutPage),
                                                                                   PackIconMaterialKind.PageLayoutSidebarRight,
                                                                                   // the page over there is called flyouts
                                                                                   "controls/flyouts",
                                                                                   "panel slide settings",
                                                                                   "Controls/Flyout.cs"),
                                                                   new GalleryPage("FontIcon",
                                                                                   "Controls",
                                                                                   typeof(FontIconPage),
                                                                                   PackIconMaterialKind.FormatFont,
                                                                                   keywords: "glyph icon font segoe",
                                                                                   control: "Controls/Icons/FontIcon.cs"),
                                                                   new GalleryPage("HamburgerMenu",
                                                                                   "Controls",
                                                                                   typeof(HamburgerMenuPage),
                                                                                   PackIconMaterialKind.Menu,
                                                                                   keywords: "navigation pane drawer",
                                                                                   control: "Controls/HamburgerMenu/HamburgerMenu.cs"),
                                                                   new GalleryPage("NumericUpDown",
                                                                                   "Controls",
                                                                                   typeof(NumericUpDownPage),
                                                                                   PackIconMaterialKind.Counter,
                                                                                   keywords: "number spinner value",
                                                                                   control: "Controls/NumericUpDown.cs"),
                                                                   new GalleryPage("Button",
                                                                                   "Styles",
                                                                                   typeof(ButtonPage),
                                                                                   PackIconMaterialKind.GestureTapButton,
                                                                                   // the page over there is called buttons, not button
                                                                                   "styles/buttons",
                                                                                   "square circle flat accent",
                                                                                   // a style rather than a control, so the styles are where to look
                                                                                   "Styles/Controls.Buttons.xaml")
                                                               };
    }
}
