// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The brushes the DataGrid of the Windows 10 and the WinUI set paints with. Each has to resolve in a light and in a
    /// dark theme, and the few that are worked out from the accent or read off TableView have to carry the value it gives.
    /// </summary>
    [TestFixture]
    public class DataGridSetThemeTests : WindowTestFixture<TestWindow>
    {
        private static readonly string[] WinUIBrushes =
            new[]
            {
                "RowBackground", "RowBackgroundPointerOver", "RowBackgroundPressed", "RowBackgroundSelected", "RowBackgroundSelectedPointerOver",
                "RowForeground", "RowForegroundDisabled", "SelectionIndicator", "SelectionIndicatorDisabled",
                "HeaderBackground", "HeaderBackgroundPointerOver", "HeaderBackgroundPressed", "HeaderForeground", "HeaderSeparator",
                "ResizeGripperSeparator", "SortIndicator", "GridLine", "OuterBorder", "GroupHeaderBackground", "GroupHeaderForeground"
            }.Select(name => "MahApps.Brushes.DataGrid.WinUI." + name).ToArray();

        private static readonly string[] Win10Brushes =
            new[]
            {
                "HeaderBackground", "HeaderBackgroundPointerOver", "HeaderBackgroundPressed", "HeaderForeground", "GridLine",
                "RowBackgroundPointerOver", "RowBackgroundSelected", "RowBackgroundSelectedPointerOver", "RowForeground", "RowForegroundDisabled",
                "RowHeaderBackground", "RowHeaderForeground", "GroupHeaderBackground", "GroupHeaderBackgroundPointerOver", "GroupHeaderBackgroundPressed",
                "GroupHeaderForeground", "DetailsBackground", "FocusVisualPrimary", "FocusVisualSecondary", "Invalid"
            }.Select(name => "MahApps.Brushes.DataGrid.Win10." + name).ToArray();

        private static readonly string[] AllBrushes = WinUIBrushes.Concat(Win10Brushes).ToArray();

        [TearDown]
        public void TearDown()
        {
            ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
        }

        [TestCase("Light.Blue")]
        [TestCase("Dark.Blue")]
        [Description("Every brush the two DataGrid looks name is there in a light and in a dark theme.")]
        public void EveryDataGridBrushOfTheSetsResolves(string theme)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            Assert.Multiple(() =>
                {
                    foreach (var key in AllBrushes)
                    {
                        Assert.That(Application.Current.TryFindResource(key), Is.InstanceOf<Brush>(), $"{key} in {theme}");
                    }
                });
        }

        [TestCase("Light.Blue", "MahApps.Colors.SystemAccentDark1")]
        [TestCase("Dark.Blue", "MahApps.Colors.SystemAccentLight2")]
        [Description("The pill of a selected row takes the shade of the accent Windows paints it with, Dark1 in the light theme and Light2 in the dark one.")]
        public void ThePillTakesTheShadeOfTheAccentWindowsUses(string theme, string colourKey)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            var pill = (SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.DataGrid.WinUI.SelectionIndicator");

            Assert.That(pill.Color, Is.EqualTo((Color)Application.Current.FindResource(colourKey)));
        }

        [TestCase("Light.Blue", "#09000000", "#26000000")]
        [TestCase("Dark.Blue", "#0FFFFFFF", "#26FFFFFF")]
        [Description("The tint of a hovered row and the one of the header row are the ones TableView gives them.")]
        public void TheWinUIRowTintsAreTheOnesTableViewUses(string theme, string pointerOver, string header)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, theme);

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf("MahApps.Brushes.DataGrid.WinUI.RowBackgroundPointerOver"), Is.EqualTo(ColorConverter.ConvertFromString(pointerOver)));
                    Assert.That(ColourOf("MahApps.Brushes.DataGrid.WinUI.HeaderBackground"), Is.EqualTo(ColorConverter.ConvertFromString(header)));
                });
        }

        private static object ColourOf(string key)
        {
            return ((SolidColorBrush)Application.Current.FindResource(key)).Color;
        }
    }
}
