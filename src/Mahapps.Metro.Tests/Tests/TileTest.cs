// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class TileTest
    {
        [Test]
        public async Task TemplateBindingShouldGetTheFontSize()
        {
            var testTile = new Tile();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>(w =>
                {
                    var grid = new Grid();
                    grid.Children.Add(testTile);
                    w.Content = grid;
                });

            // default values

            var accessText = testTile.FindChild<AccessText>(string.Empty);
            var findChild = testTile.FindChild<TextBlock>(string.Empty);

            Assert.That(accessText, Is.Not.Null);
            Assert.That(findChild, Is.Not.Null);

            Assert.That(accessText.FontSize, Is.EqualTo(16d));
            Assert.That(findChild.FontSize, Is.EqualTo(28d));

            // now change it

            var fontSize = 42d;

            testTile.TitleFontSize = fontSize;
            Assert.That(accessText.FontSize, Is.EqualTo(fontSize));

            testTile.CountFontSize = fontSize;
            Assert.That(findChild.FontSize, Is.EqualTo(fontSize));

            window.Close();
        }
    }
}