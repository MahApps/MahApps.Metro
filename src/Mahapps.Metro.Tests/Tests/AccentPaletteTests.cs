// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows.Media;
using MahApps.Metro.Theming;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The three lighter and three darker shades of the accent, the ones WinUI knows as SystemAccentColorLight1 to
    /// Dark3. Windows keeps them for the accent of the user; for any other accent they are worked out.
    /// </summary>
    [TestFixture]
    public class AccentPaletteTests
    {
        // the palette Windows 11 wrote for its own blue, read from the registry of a real machine
        private static readonly byte[] WindowsBlue =
            {
                0x99, 0xEB, 0xFF, 0x00, 0x4C, 0xC2, 0xFF, 0x00, 0x00, 0x91, 0xF8, 0x00, 0x00, 0x78, 0xD4, 0x00,
                0x00, 0x67, 0xC0, 0x00, 0x00, 0x3E, 0x92, 0x00, 0x00, 0x1A, 0x68, 0x00, 0xF7, 0x63, 0x0C, 0x00,
            };

        private static Color C(string hex) => (Color)ColorConverter.ConvertFromString(hex);

        [Test]
        public void TheRegistryValueIsReadInTheOrderWindowsWritesIt()
        {
            var palette = AccentPalette.FromRegistryValue(WindowsBlue);

            Assert.That(palette, Is.EqualTo(new AccentPalette(C("#0078D4"), C("#0091F8"), C("#4CC2FF"), C("#99EBFF"), C("#0067C0"), C("#003E92"), C("#001A68"))));
        }

        [TestCase(null)]
        [TestCase(new byte[0])]
        [TestCase(new byte[] { 1, 2, 3 })]
        public void ARegistryValueOfAnotherShapeIsNoPalette(byte[]? value)
        {
            Assert.That(AccentPalette.FromRegistryValue(value), Is.Null);
        }

        [Test]
        [Description("The calculation is calibrated against what Windows makes of its own blue.")]
        public void TheBlueOfWindowsComesOutAsWindowsHasIt()
        {
            var windows = AccentPalette.FromRegistryValue(WindowsBlue)!.Value;
            var mine = AccentPalette.Create(C("#0078D4"));

            Assert.Multiple(() =>
                {
                    Assert.That(DeltaE(mine.Light1, windows.Light1), Is.LessThan(1), "Light1");
                    Assert.That(DeltaE(mine.Light2, windows.Light2), Is.LessThan(1), "Light2");
                    Assert.That(DeltaE(mine.Light3, windows.Light3), Is.LessThan(1), "Light3");
                    Assert.That(DeltaE(mine.Dark1, windows.Dark1), Is.LessThan(1), "Dark1");
                    Assert.That(DeltaE(mine.Dark2, windows.Dark2), Is.LessThan(1), "Dark2");
                    Assert.That(DeltaE(mine.Dark3, windows.Dark3), Is.LessThan(1), "Dark3");
                    Assert.That(mine.Accent, Is.EqualTo(C("#0078D4")), "the accent stays what it is");
                });
        }

        [TestCase("#FF0078D7")]
        [TestCase("#FFF0A30A")]
        [TestCase("#FFE51400")]
        [TestCase("#FF60A917")]
        [TestCase("#FFFFFF00")]
        [TestCase("#FF0000FF")]
        [TestCase("#FF000000")]
        [TestCase("#FFFFFFFF")]
        [TestCase("#FF808080")]
        [TestCase("#FF6A00FF")]
        public void TheShadesRunFromLightToDark(string accent)
        {
            var palette = AccentPalette.Create(C(accent));
            var steps = new[] { palette.Light3, palette.Light2, palette.Light1, palette.Accent, palette.Dark1, palette.Dark2, palette.Dark3 };
            var lightness = steps.Select(c => Lab(c).L).ToArray();

            Assert.Multiple(() =>
                {
                    for (var i = 1; i < lightness.Length; i++)
                    {
                        Assert.That(lightness[i], Is.LessThanOrEqualTo(lightness[i - 1] + 0.5), $"step {i}");
                    }

                    Assert.That(steps, Has.All.Matches<Color>(c => c.A == 255), "every shade is opaque");
                });
        }

        [Test]
        public void TheAccentKeepsNoAlphaOfItsOwn()
        {
            Assert.That(AccentPalette.Create(C("#CC0078D7")).Accent, Is.EqualTo(C("#FF0078D7")));
        }

        // CIE76 on CIELAB with the D65 white, the scale the calibration was done on
        private static double DeltaE(Color a, Color b)
        {
            var x = Lab(a);
            var y = Lab(b);
            return Math.Sqrt(Square(x.L - y.L) + Square(x.A - y.A) + Square(x.B - y.B));
        }

        private static double Square(double v) => v * v;

        private static (double L, double A, double B) Lab(Color c)
        {
            double Linear(byte channel)
            {
                var v = channel / 255.0;
                return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            }

            double F(double t) => t > 216.0 / 24389.0 ? Math.Pow(t, 1.0 / 3.0) : (24389.0 / 27.0 * t + 16.0) / 116.0;

            double r = Linear(c.R), g = Linear(c.G), b = Linear(c.B);
            var fx = F((0.4124564 * r + 0.3575761 * g + 0.1804375 * b) / 0.95047);
            var fy = F(0.2126729 * r + 0.7151522 * g + 0.0721750 * b);
            var fz = F((0.0193339 * r + 0.1191920 * g + 0.9503041 * b) / 1.08883);

            return (116 * fy - 16, 500 * (fx - fy), 200 * (fy - fz));
        }
    }
}
