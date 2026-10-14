// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.Converters;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.IconPacks;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// The brushes of the themes, a tile each, grouped the way the Color page of the WinUI 3 Gallery
    /// groups the brushes of WinUI, and the classic keys of MahApps.Metro on a tab of their own.
    /// Only keys the themes really have are listed; what WinUI has beyond them is left out.
    /// </summary>
    public partial class ColorPage : UserControl
    {
        private static readonly ReadableForegroundConverter Readable = new();

        /// <summary>
        /// Every tab of the page with its groups, which is also the list of keys the page shows.
        /// </summary>
        internal static readonly Tab[] All =
        {
            new("Text",
                new Group("Text", "For labels and static text", "MahApps.Brushes.WinUI.TextPrimary",
                          new Tile("Text / Primary", "Rest or hover", "MahApps.Brushes.WinUI.TextPrimary"),
                          new Tile("Text / Secondary", "Rest or hover", "MahApps.Brushes.WinUI.TextSecondary"),
                          new Tile("Text / Tertiary", "Pressed only (not accessible)", "MahApps.Brushes.WinUI.TextTertiary"),
                          new Tile("Text / Disabled", "Disabled only (not accessible)", "MahApps.Brushes.WinUI.TextDisabled"),
                          new Tile("Text / Inverse", "On a surface of the other theme", "MahApps.Brushes.WinUI.TextInverse")),
                new Group("Accent Text", "For links", "MahApps.Brushes.WinUI.AccentTextFillPrimary",
                          new Tile("Accent Text / Primary", "Rest or hover", "MahApps.Brushes.WinUI.AccentTextFillPrimary"),
                          new Tile("Accent Text / Secondary", "Rest or hover", "MahApps.Brushes.WinUI.AccentTextFillSecondary"),
                          new Tile("Accent Text / Tertiary", "Pressed only (not accessible)", "MahApps.Brushes.WinUI.AccentTextFillTertiary")),
                new Group("Text On Accent", "For text on accent coloured controls and fills", "MahApps.Brushes.WinUI.TextOnAccentPrimary",
                          new Tile("Text On Accent / Primary", "Rest or hover", "MahApps.Brushes.WinUI.TextOnAccentPrimary"),
                          new Tile("Text On Accent / Secondary", "Pressed only (not accessible)", "MahApps.Brushes.WinUI.TextOnAccentSecondary"),
                          new Tile("Text On Accent / Disabled", "Disabled only (not accessible)", "MahApps.Brushes.WinUI.TextOnAccentDisabled"))
                    { Card = "MahApps.Brushes.WinUI.AccentFillDefault" }),
            new("Fill",
                new Group("Control Fill", "For the surface of standard controls",
                          new Tile("Control Fill / Default", "Rest", "MahApps.Brushes.WinUI.ControlFillDefault"),
                          new Tile("Control Fill / Secondary", "Hover", "MahApps.Brushes.WinUI.ControlFillSecondary"),
                          new Tile("Control Fill / Tertiary", "Pressed", "MahApps.Brushes.WinUI.ControlFillTertiary"),
                          new Tile("Control Fill / Disabled", "Disabled", "MahApps.Brushes.WinUI.ControlFillDisabled"),
                          new Tile("Control Fill / Input Active", "A text box with the focus", "MahApps.Colors.WinUI.ControlFillInputActive")),
                new Group("Control Alt Fill", "For toggle controls while they are off",
                          new Tile("Control Alt Fill / Secondary", "Rest", "MahApps.Brushes.WinUI.ControlAltFillSecondary"),
                          new Tile("Control Alt Fill / Tertiary", "Hover", "MahApps.Brushes.WinUI.ControlAltFillTertiary"),
                          new Tile("Control Alt Fill / Quarternary", "Pressed", "MahApps.Brushes.WinUI.ControlAltFillQuarternary"),
                          new Tile("Control Alt Fill / Disabled", "Disabled", "MahApps.Brushes.WinUI.ControlAltFillDisabled")),
                new Group("Control Solid Fill", "For the thumb of a slider, which hides the track under it",
                          new Tile("Control Solid Fill / Default", "Rest", "MahApps.Brushes.WinUI.ControlSolidFillDefault")),
                new Group("Control Strong Fill", "For controls that need a contrast of 3:1",
                          new Tile("Control Strong Fill / Default", "Rest or hover", "MahApps.Brushes.WinUI.ControlStrongFillDefault"),
                          new Tile("Control Strong Fill / Disabled", "Disabled only (not accessible)", "MahApps.Brushes.WinUI.ControlStrongFillDisabled")),
                new Group("Subtle Fill", "For list items and buttons that show no surface until the pointer comes",
                          new Tile("Subtle Fill / Secondary", "Hover", "MahApps.Brushes.WinUI.SubtleFillSecondary"),
                          new Tile("Subtle Fill / Tertiary", "Pressed", "MahApps.Brushes.WinUI.SubtleFillTertiary")),
                new Group("Accent Fill", "For controls filled with the accent",
                          new Tile("Accent Fill / Default", "Rest", "MahApps.Brushes.WinUI.AccentFillDefault"),
                          new Tile("Accent Fill / Secondary", "Hover", "MahApps.Brushes.WinUI.AccentFillSecondary"),
                          new Tile("Accent Fill / Tertiary", "Pressed", "MahApps.Brushes.WinUI.AccentFillTertiary"),
                          new Tile("Accent Fill / Disabled", "Disabled", "MahApps.Brushes.WinUI.AccentFillDisabled"))),
            new("Stroke",
                new Group("Control Stroke", "For the edges of controls",
                          new Tile("Control Stroke / Default", "The edge of a control", "MahApps.Brushes.WinUI.ControlStrokeDefault"),
                          new Tile("Control Stroke / Secondary", "The lower edge of a control", "MahApps.Brushes.WinUI.ControlStrokeSecondary"),
                          new Tile("Control Stroke / On Accent Default", "The edge of an accent control", "MahApps.Brushes.WinUI.ControlStrokeOnAccentDefault"),
                          new Tile("Control Stroke / On Accent Secondary", "The lower edge of an accent control", "MahApps.Brushes.WinUI.ControlStrokeOnAccentSecondary")),
                new Group("Control Strong Stroke", "For edges that need a contrast of 3:1",
                          new Tile("Control Strong Stroke / Default", "Rest", "MahApps.Brushes.WinUI.ControlStrongStrokeDefault"),
                          new Tile("Control Strong Stroke / Disabled", "Disabled", "MahApps.Brushes.WinUI.ControlStrongStrokeDisabled")),
                new Group("Card Stroke", "For the edges of cards and layers",
                          new Tile("Card Stroke / Default", "Cards and layers", "MahApps.Brushes.WinUI.CardStrokeDefault")),
                new Group("Surface Stroke", "For the edges of surfaces such as windows, dialogs and flyouts",
                          new Tile("Surface Stroke / Default", "Windows and dialogs", "MahApps.Brushes.WinUI.SurfaceStrokeDefault"),
                          new Tile("Surface Stroke / Flyout", "Flyouts of controls", "MahApps.Brushes.WinUI.SurfaceStrokeFlyout")),
                new Group("Divider Stroke", "For dividers and lines in graphics",
                          new Tile("Divider Stroke / Default", "Between parts of the content", "MahApps.Brushes.WinUI.DividerStrokeDefault")),
                new Group("Focus Stroke", "For the focus rectangle, dark in the light theme and light in the dark one",
                          new Tile("Focus Stroke / Outer", "The outer line", "MahApps.Colors.WinUI.FocusStrokeOuter"),
                          new Tile("Focus Stroke / Inner", "The inner line", "MahApps.Colors.WinUI.FocusStrokeInner"))),
            new("Background",
                new Group("Card Background", "For cards, blocks of content on a page or a layer",
                          new Tile("Card Background / Default", "A card", "MahApps.Brushes.WinUI.CardBackgroundFillDefault"),
                          new Tile("Card Background / Secondary", "A card a little darker", "MahApps.Brushes.WinUI.CardBackgroundFillSecondary")),
                new Group("Smoke", "For dimming what lies behind a dialog",
                          new Tile("Smoke / Default", "Behind a dialog", "MahApps.Brushes.WinUI.SmokeFillDefault")),
                new Group("Layer", "For layers on a background",
                          new Tile("Layer / Alt", "Another layer of content", "MahApps.Brushes.WinUI.LayerFillAlt")),
                new Group("Layer On Mica Base Alt", "For the tabs of a tab view",
                          new Tile("Layer On Mica Base Alt / Default", "The selected tab", "MahApps.Brushes.WinUI.LayerOnMicaBaseAltFillDefault"),
                          new Tile("Layer On Mica Base Alt / Secondary", "A tab under the pointer", "MahApps.Brushes.WinUI.LayerOnMicaBaseAltFillSecondary")),
                new Group("Solid Background", "For the ground that layers, cards and controls sit on",
                          new Tile("Solid Background / Base", "The lowest layer", "MahApps.Brushes.WinUI.SolidBackgroundFillBase"),
                          new Tile("Solid Background / Tertiary", "A layer of content", "MahApps.Brushes.WinUI.SolidBackgroundFillTertiary")),
                new Group("Acrylic", "For flyouts and surfaces inside the application",
                          new Tile("Acrylic In App / Default", "Flyouts", "MahApps.Brushes.WinUI.AcrylicInAppFillDefault"))),
            new("Signal",
                new Group("System Fill", "For badges and the icons of states",
                          new Tile("Success", "Badge", "MahApps.Brushes.WinUI.SystemFillSuccess"),
                          new Tile("Caution", "Badge", "MahApps.Brushes.WinUI.SystemFillCaution"),
                          new Tile("Critical", "Badge", "MahApps.Brushes.WinUI.SystemFillCritical"),
                          new Tile("Attention", "Badge", "MahApps.Brushes.WinUI.SystemFillAttention")),
                new Group("System Fill Background", "For the background of an info bar",
                          new Tile("Success Background", "Info bar", "MahApps.Brushes.WinUI.SystemFillSuccessBackground"),
                          new Tile("Caution Background", "Info bar", "MahApps.Brushes.WinUI.SystemFillCautionBackground"),
                          new Tile("Critical Background", "Info bar", "MahApps.Brushes.WinUI.SystemFillCriticalBackground"),
                          new Tile("Attention Background", "Info bar", "MahApps.Brushes.WinUI.SystemFillAttentionBackground"))),
            new("Metro",
                new Group("Accent", "The accent of the theme and the lighter steps the Metro styles paint with",
                          new Tile("Highlight", "Selected and highlighted parts", "MahApps.Brushes.Highlight"),
                          new Tile("Accent Base", "The accent itself", "MahApps.Brushes.AccentBase"),
                          new Tile("Accent", "80 %", "MahApps.Brushes.Accent"),
                          new Tile("Accent 2", "60 %", "MahApps.Brushes.Accent2"),
                          new Tile("Accent 3", "40 %", "MahApps.Brushes.Accent3"),
                          new Tile("Accent 4", "20 %", "MahApps.Brushes.Accent4"),
                          new Tile("Ideal Foreground", "Text on the accent", "MahApps.Brushes.IdealForeground"))
                    { Columns = 4 },
                new Group("Shades Of The Accent", "Three lighter and three darker shades, taken from Windows when the accent is the one of Windows",
                          new Tile("Light 3", "The lightest", "MahApps.Brushes.SystemAccentLight3"),
                          new Tile("Light 2", "Fills in the dark theme", "MahApps.Brushes.SystemAccentLight2"),
                          new Tile("Light 1", string.Empty, "MahApps.Brushes.SystemAccentLight1"),
                          new Tile("System Accent", "The accent itself", "MahApps.Colors.SystemAccent"),
                          new Tile("Dark 1", "Fills in the light theme", "MahApps.Brushes.SystemAccentDark1"),
                          new Tile("Dark 2", string.Empty, "MahApps.Brushes.SystemAccentDark2"),
                          new Tile("Dark 3", "The darkest", "MahApps.Brushes.SystemAccentDark3"))
                    { Columns = 4 },
                new Group("Theme", "The foreground and background of the light or dark theme",
                          new Tile("Theme Foreground", "Text", "MahApps.Brushes.ThemeForeground"),
                          new Tile("Theme Background", "Windows and pages", "MahApps.Brushes.ThemeBackground"),
                          new Tile("Ideal Foreground Disabled", "Disabled text on the accent", "MahApps.Brushes.IdealForegroundDisabled")),
                new Group("Gray", "The grays of the theme, from the strongest to the faintest",
                          Enumerable.Range(1, 10).Select(i => new Tile($"Gray {i}", string.Empty, $"MahApps.Brushes.Gray{i}")).ToArray()))
        };

        public ColorPage()
        {
            this.InitializeComponent();

            foreach (var tab in All)
            {
                var groups = new StackPanel { Margin = new Thickness(0, 24, 0, 0) };
                foreach (var group in tab.Groups)
                {
                    groups.Children.Add(Card(group));
                    groups.Children.Add(Tiles(group));
                }

                this.Tabs.Items.Add(new TabItem { Header = tab.Header, Content = groups });
            }

            this.Tabs.SelectedIndex = 0;
        }

        private static FrameworkElement Card(Group group)
        {
            var title = Text(group.Title, "MahApps.Styles.TextBlock.Subtitle.WinUI");
            var description = Text(group.Description, "MahApps.Styles.TextBlock.Caption.WinUI");
            description.Margin = new Thickness(0, 4, 0, 0);

            var content = new StackPanel();
            content.Children.Add(title);
            content.Children.Add(description);

            if (group.Sample is not null)
            {
                var sample = new TextBlock { Text = "Aa", FontSize = 40, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 8, 0, 8) };
                sample.SetResourceReference(TextBlock.ForegroundProperty, group.Sample);
                content.Children.Add(sample);
            }

            var card = new Border { Child = content, Padding = new Thickness(16), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Margin = new Thickness(0, 0, 0, 8) };
            card.SetResourceReference(Border.BackgroundProperty, group.Card ?? "MahApps.Brushes.WinUI.CardBackgroundFillDefault");
            card.SetResourceReference(Border.BorderBrushProperty, "MahApps.Brushes.WinUI.CardStrokeDefault");

            // on a card in the accent the words take the colour that reads on it
            if (group.Card is not null)
            {
                title.SetResourceReference(TextBlock.ForegroundProperty, "MahApps.Brushes.WinUI.TextOnAccentPrimary");
                description.SetResourceReference(TextBlock.ForegroundProperty, "MahApps.Brushes.WinUI.TextOnAccentPrimary");
            }

            return card;
        }

        private static FrameworkElement Tiles(Group group)
        {
            var grid = new UniformGrid { Columns = group.Columns ?? Math.Min(group.Tiles.Length, 5) };

            // the ground under the tiles, so that a brush with some transparency shows what it does
            var backdrop = new Border { Child = grid, Padding = new Thickness(4, 4, 0, 0), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Margin = new Thickness(0, 0, 0, 32) };
            backdrop.SetResourceReference(Border.BackgroundProperty, "MahApps.Brushes.WinUI.SolidBackgroundFillBase");
            backdrop.SetResourceReference(Border.BorderBrushProperty, "MahApps.Brushes.WinUI.CardStrokeDefault");

            foreach (var tile in group.Tiles)
            {
                grid.Children.Add(TileFor(tile, backdrop));
            }

            return backdrop;
        }

        private static FrameworkElement TileFor(Tile tile, Border backdrop)
        {
            var name = Text(tile.Name, "MahApps.Styles.TextBlock.BodyStrong.WinUI");
            var use = Text(tile.Use, "MahApps.Styles.TextBlock.Caption.WinUI");
            Grid.SetRow(use, 1);
            // a key breaks at its dots rather than inside a word
            var key = Text(tile.Key.Replace(".", ".​"), "MahApps.Styles.TextBlock.Caption.WinUI");
            key.VerticalAlignment = VerticalAlignment.Bottom;
            Grid.SetRow(key, 2);
            Grid.SetColumnSpan(key, 2);

            var copy = new Button
                       {
                           Command = GalleryCommands.CopyText,
                           CommandParameter = tile.Key,
                           ToolTip = "Copy the resource key",
                           VerticalAlignment = VerticalAlignment.Top,
                           Content = new PackIconMaterial { Kind = PackIconMaterialKind.ContentCopy, Width = 14, Height = 14 }
                       };
            copy.SetResourceReference(StyleProperty, "MahApps.Gallery.Styles.Button.CardLink");
            Grid.SetColumn(copy, 1);
            Grid.SetRowSpan(copy, 2);

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Children.Add(name);
            grid.Children.Add(use);
            grid.Children.Add(copy);
            grid.Children.Add(key);

            var swatch = new Border { Child = grid, Height = 128, Padding = new Thickness(12), CornerRadius = new CornerRadius(4), Margin = new Thickness(0, 0, 4, 4) };

            // a key of a colour rather than a brush is shown through a brush of that colour
            if (tile.Key.StartsWith("MahApps.Colors.", StringComparison.Ordinal))
            {
                swatch.SetResourceReference(TagProperty, tile.Key);
                swatch.SetBinding(Border.BackgroundProperty, new Binding(nameof(Tag)) { RelativeSource = RelativeSource.Self, Converter = ColorToSolidColorBrushConverter.DefaultInstance });
            }
            else
            {
                swatch.SetResourceReference(Border.BackgroundProperty, tile.Key);
            }

            var foreground = new MultiBinding { Converter = Readable };
            foreground.Bindings.Add(new Binding(nameof(Border.Background)) { Source = swatch });
            foreground.Bindings.Add(new Binding(nameof(Border.Background)) { Source = backdrop });
            swatch.SetBinding(TextElement.ForegroundProperty, foreground);
            copy.SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath(TextElement.ForegroundProperty), Source = swatch });

            return swatch;
        }

        private static TextBlock Text(string text, string style)
        {
            var block = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap };
            block.SetResourceReference(StyleProperty, style);
            return block;
        }

        internal sealed class Tab
        {
            public Tab(string header, params Group[] groups)
            {
                this.Header = header;
                this.Groups = groups;
            }

            public string Header { get; }

            public IReadOnlyList<Group> Groups { get; }
        }

        internal sealed class Group
        {
            public Group(string title, string description, params Tile[] tiles)
                : this(title, description, null, tiles)
            {
            }

            public Group(string title, string description, string? sample, params Tile[] tiles)
            {
                this.Title = title;
                this.Description = description;
                this.Sample = sample;
                this.Tiles = tiles;
            }

            public string Title { get; }

            public string Description { get; }

            /// <summary>
            /// The key of the brush a large Aa on the card is written in, for the groups of text.
            /// </summary>
            public string? Sample { get; }

            /// <summary>
            /// The key of the brush the card is filled with, where the default card is the wrong ground.
            /// </summary>
            public string? Card { get; set; }

            /// <summary>
            /// How many tiles go in a row, where seven read better as four and three than as five and two.
            /// </summary>
            public int? Columns { get; set; }

            public Tile[] Tiles { get; }
        }

        internal sealed class Tile
        {
            public Tile(string name, string use, string key)
            {
                this.Name = name;
                this.Use = use;
                this.Key = key;
            }

            public string Name { get; }

            public string Use { get; }

            public string Key { get; }
        }
    }
}
