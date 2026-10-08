// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.IconPacks;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// The type ramps of the three looks, one row per step, laid out like the Typography page of the WinUI 3 Gallery.
    /// </summary>
    public partial class TypographyPage : UserControl
    {
        private static readonly Dictionary<string, string[]> Steps = new()
        {
            ["Metro"] = new[] { "Caption", "Body", "Subtitle", "Subheader", "Header" },
            ["Win10"] = new[] { "Caption", "Body", "Base", "Subtitle", "Title", "Subheader", "Header" },
            ["WinUI"] = new[] { "Caption", "Body", "BodyStrong", "BodyLarge", "BodyLargeStrong", "Subtitle", "Title", "TitleLarge", "Display" }
        };

        // the line heights Microsoft gives the WinUI ramp; WPF and WinUI both take them from the font
        private static readonly Dictionary<string, int> WinUILineHeights = new()
        {
            ["Caption"] = 16,
            ["Body"] = 20,
            ["BodyStrong"] = 20,
            ["BodyLarge"] = 24,
            ["BodyLargeStrong"] = 24,
            ["Subtitle"] = 28,
            ["Title"] = 36,
            ["TitleLarge"] = 52,
            ["Display"] = 92
        };

        private readonly TextBlock sizeHeader = Header("Size/Line height", 2);

        public TypographyPage()
        {
            this.InitializeComponent();

            AddColumns(this.Columns);
            this.Columns.Children.Add(Header("Example", 0));
            this.Columns.Children.Add(Header("Font", 1));
            this.Columns.Children.Add(this.sizeHeader);
            this.Columns.Children.Add(Header("Style", 3));

            this.WinUILook.IsChecked = true;
        }

        private void OnLookChecked(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: string look })
            {
                return;
            }

            // Microsoft gives line heights for the WinUI ramp only
            this.sizeHeader.Text = look == "WinUI" ? "Size/Line height" : "Size";

            this.Ramp.Items.Clear();
            var steps = Steps[look];
            for (var i = 0; i < steps.Length; i++)
            {
                var step = steps[i];
                var key = look == "Metro" ? $"MahApps.Styles.TextBlock.{step}" : $"MahApps.Styles.TextBlock.{step}.{look}";
                this.Ramp.Items.Add(Row(look, step, key, i % 2 == 0));
            }
        }

        private static void AddColumns(Grid grid)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 220 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, SharedSizeGroup = "Font" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, SharedSizeGroup = "Size" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, SharedSizeGroup = "Style" });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, SharedSizeGroup = "Copy" });
        }

        private static TextBlock Header(string text, int column)
        {
            var header = new TextBlock { Text = text, Margin = new Thickness(0, 0, 32, 0), VerticalAlignment = VerticalAlignment.Center };
            header.SetResourceReference(StyleProperty, "MahApps.Styles.TextBlock.Caption.WinUI");
            header.SetResourceReference(TextBlock.ForegroundProperty, "MahApps.Brushes.WinUI.TextSecondary");
            Grid.SetColumn(header, column);
            return header;
        }

        private static FrameworkElement Row(string look, string step, string key, bool tinted)
        {
            var sample = new TextBlock { Text = Spaced(step), TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 32, 0) };
            sample.SetResourceReference(StyleProperty, key);

            var font = Cell(1);
            var size = Cell(2);

            // what a row says about the step is read off the sample, so it shows what the style gives
            sample.Loaded += (_, _) =>
                {
                    font.Text = $"{FontName(look, sample.FontFamily)}, {WeightName(sample.FontWeight)}";
                    size.Text = look == "WinUI" && WinUILineHeights.TryGetValue(step, out var lineHeight)
                                    ? string.Format(CultureInfo.InvariantCulture, "{0:0.###}/{1} epx", sample.FontSize, lineHeight)
                                    : string.Format(CultureInfo.InvariantCulture, "{0:0.###} epx", sample.FontSize);
                };

            var name = new TextBlock { Text = key, FontFamily = new FontFamily("Consolas"), FontSize = 12, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) };
            Grid.SetColumn(name, 3);

            var copy = new Button
                       {
                           Command = GalleryCommands.CopyText,
                           CommandParameter = key,
                           ToolTip = "Copy the style key",
                           VerticalAlignment = VerticalAlignment.Center,
                           Content = new PackIconMaterial { Kind = PackIconMaterialKind.ContentCopy, Width = 14, Height = 14 }
                       };
            copy.SetResourceReference(StyleProperty, "MahApps.Gallery.Styles.Button.CardLink");
            Grid.SetColumn(copy, 4);

            var grid = new Grid();
            AddColumns(grid);
            grid.Children.Add(sample);
            grid.Children.Add(font);
            grid.Children.Add(size);
            grid.Children.Add(name);
            grid.Children.Add(copy);

            var row = new Border { Child = grid, Padding = new Thickness(20, 16, 20, 16), CornerRadius = new CornerRadius(4), MinHeight = 64 };
            if (tinted)
            {
                row.SetResourceReference(Border.BackgroundProperty, "MahApps.Brushes.WinUI.CardBackgroundFillDefault");
            }

            return row;
        }

        private static TextBlock Cell(int column)
        {
            var cell = new TextBlock { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 32, 0) };
            Grid.SetColumn(cell, column);
            return cell;
        }

        // BodyLargeStrong reads as Body Large Strong
        private static string Spaced(string step)
        {
            return string.Concat(step.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        }

        // the first family of the list; for Segoe UI Variable the cut is what tells the steps apart
        private static string FontName(string look, FontFamily family)
        {
            var first = family.Source.Split(',')[0].Trim();
            return look == "WinUI" ? first.Replace("Segoe UI Variable ", string.Empty) : first;
        }

        // the names Microsoft's type ramps use; WPF has no name for SemiLight
        private static string WeightName(FontWeight weight)
        {
            if (weight == FontWeights.Normal)
            {
                return "Regular";
            }

            return weight == FontWeight.FromOpenTypeWeight(350) ? "SemiLight" : weight.ToString();
        }
    }
}
