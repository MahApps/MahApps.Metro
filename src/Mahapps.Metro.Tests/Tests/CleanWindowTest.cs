// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class CleanWindowTest
    {
        [Test]
        public async Task DefaultWindowCommandColorIsBlack()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<CleanWindow>();

            var theme = ThemeManager.Current.DetectTheme(window);
            Assert.That(theme, Is.Not.Null);

            var brush = theme.Resources["MahApps.Brushes.ThemeForeground"] as SolidColorBrush;
            Assert.That(brush, Is.Not.Null);

            var blackBrushColor = brush.Color;

            window.AssertWindowCommandsColor(blackBrushColor);

            window.Close();
        }
    }
}