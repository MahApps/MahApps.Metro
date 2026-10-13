// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ControlzEx.Theming;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The on state of MahApps.Styles.ToggleSwitch.Win10, after generic.xaml of the Windows SDK: the accent at rest,
    /// the accent a little see-through under the pointer, and the medium base colour while the switch is held.
    /// </summary>
    [TestFixture]
    public class ToggleSwitchWin10BrushTests : WindowTestFixture<TestWindow>
    {
        [TestCase("Light.Blue", "FillOn", "SystemControlHighlightAccent")]
        [TestCase("Dark.Blue", "FillOn", "SystemControlHighlightAccent")]
        [TestCase("Light.Blue", "FillOnPointerOver", "SystemControlHighlightAltListAccentHigh")]
        [TestCase("Dark.Blue", "FillOnPointerOver", "SystemControlHighlightAltListAccentHigh")]
        [TestCase("Light.Blue", "FillOnPressed", "SystemControlHighlightBaseMedium")]
        [TestCase("Dark.Blue", "FillOnPressed", "SystemControlHighlightBaseMedium")]
        [TestCase("Light.Blue", "StrokeOnPointerOver", "SystemControlHighlightListAccentHigh")]
        [TestCase("Dark.Blue", "StrokeOnPointerOver", "SystemControlHighlightListAccentHigh")]
        [TestCase("Light.Blue", "StrokeOnPressed", "SystemControlHighlightBaseMedium")]
        [TestCase("Dark.Blue", "StrokeOnPressed", "SystemControlHighlightBaseMedium")]
        public void TheOnStateIsTheOneOfTheWindowsSdk(string theme, string key, string expected)
        {
            ThemeManager.Current.ChangeTheme(this.window!, theme);

            try
            {
                Assert.That(this.window!.FindResource($"MahApps.Brushes.ToggleSwitch.Win10.{key}"), Is.SameAs(this.window.FindResource($"MahApps.Brushes.{expected}")));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }
    }
}
