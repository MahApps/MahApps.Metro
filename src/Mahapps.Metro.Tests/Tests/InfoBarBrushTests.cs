// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The colours of the info bar, taken from InfoBar_themeresources.xaml of microsoft-ui-xaml. The informational
    /// icon is the one exception: it is the accent of the theme with the foreground worked out for that accent.
    /// </summary>
    [TestFixture]
    public class InfoBarBrushTests : WindowTestFixture<TestWindow>
    {
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Error.Background", "#FFFDE7E9")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Error.Background", "#FF442726")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Warning.Background", "#FFFFF4CE")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Warning.Background", "#FF433519")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Success.Background", "#FFDFF6DD")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Success.Background", "#FF393D1B")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Informational.Background", "#80F6F6F6")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Informational.Background", "#08FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Error.IconBackground", "#FFC42B1C")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Error.IconBackground", "#FFFF99A4")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Warning.IconBackground", "#FF9D5D00")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Warning.IconBackground", "#FFFCE100")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Success.IconBackground", "#FF0F7B0F")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Success.IconBackground", "#FF6CCB5F")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Error.IconForeground", "#FFFFFFFF")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Error.IconForeground", "#E4000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Warning.IconForeground", "#FFFFFFFF")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Warning.IconForeground", "#E4000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Success.IconForeground", "#FFFFFFFF")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Success.IconForeground", "#E4000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Informational.IconBackground", "#FF0078D7")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Informational.IconBackground", "#FF0078D7")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Informational.IconForeground", "#FFFFFFFF")]
        [TestCase("Light.Yellow", "MahApps.Brushes.InfoBar.Informational.IconForeground", "#FF000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Border", "#0F000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Border", "#19000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.InfoBar.Foreground", "#E4000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.InfoBar.Foreground", "#FFFFFFFF")]
        public void TheColoursAreMicrosofts(string theme, string key, string expected)
        {
            ThemeManager.Current.ChangeTheme(this.window!, theme);

            try
            {
                var brush = (SolidColorBrush)this.window!.FindResource(key);

                Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString(expected)));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }
    }
}
