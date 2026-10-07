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
    /// The colours of the two content dialogs that follow Microsoft, taken from ContentDialog_themeresources.xaml of
    /// microsoft-ui-xaml and from the generic.xaml of the Windows 10 SDK.
    /// </summary>
    [TestFixture]
    public class ContentDialogBrushTests : WindowTestFixture<TestWindow>
    {
        [TestCase("Light", "MahApps.Brushes.ContentDialog.Win10.Background", "#FFFFFFFF")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.Win10.Background", "#FF000000")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.Win10.Foreground", "#FF000000")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.Win10.Foreground", "#FFFFFFFF")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.Win10.Border", "#33000000")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.Win10.Border", "#33FFFFFF")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.Win10.Overlay", "#99FFFFFF")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.Win10.Overlay", "#99000000")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.Background", "#FFF3F3F3")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.Background", "#FF202020")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.TopOverlay", "#FFFFFFFF")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.TopOverlay", "#0DFFFFFF")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.Foreground", "#E4000000")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.Foreground", "#FFFFFFFF")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.Border", "#66757575")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.Border", "#66757575")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.Separator", "#0F000000")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.Separator", "#19000000")]
        [TestCase("Light", "MahApps.Brushes.ContentDialog.WinUI.Overlay", "#4D000000")]
        [TestCase("Dark", "MahApps.Brushes.ContentDialog.WinUI.Overlay", "#4D000000")]
        public void TheColoursAreMicrosofts(string baseColor, string key, string expected)
        {
            ThemeManager.Current.ChangeTheme(this.window!, $"{baseColor}.Blue");

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
