// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security;
using System.Windows.Media;
using Microsoft.Win32;

namespace MahApps.Metro.Theming
{
    /// <summary>
    /// The accent with the three lighter and three darker shades Windows derives from it, the ones WinUI knows as
    /// SystemAccentColorLight1 to Light3 and Dark1 to Dark3.
    /// </summary>
    public readonly struct AccentPalette : IEquatable<AccentPalette>
    {
        // How far each shade goes from the accent towards its end. Fitted against the palette Windows 11 keeps for
        // its own blue, which every shade then meets within a ΔE of half a unit.
        private static readonly double[] LightSteps = { 0.155, 0.495, 0.815 };
        private static readonly double[] DarkSteps = { 0.15, 0.50, 0.815 };

        private const string AccentKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent";
        private const string AccentPaletteValue = "AccentPalette";

        /// <summary>Creates a palette from the accent and its six shades.</summary>
        public AccentPalette(Color accent, Color light1, Color light2, Color light3, Color dark1, Color dark2, Color dark3)
        {
            this.Accent = accent;
            this.Light1 = light1;
            this.Light2 = light2;
            this.Light3 = light3;
            this.Dark1 = dark1;
            this.Dark2 = dark2;
            this.Dark3 = dark3;
        }

        /// <summary>Gets the accent itself.</summary>
        public Color Accent { get; }

        /// <summary>Gets the lightest but two of the shades, SystemAccentColorLight1.</summary>
        public Color Light1 { get; }

        /// <summary>Gets the middle one of the lighter shades, SystemAccentColorLight2.</summary>
        public Color Light2 { get; }

        /// <summary>Gets the lightest shade, SystemAccentColorLight3.</summary>
        public Color Light3 { get; }

        /// <summary>Gets the darkest but two of the shades, SystemAccentColorDark1.</summary>
        public Color Dark1 { get; }

        /// <summary>Gets the middle one of the darker shades, SystemAccentColorDark2.</summary>
        public Color Dark2 { get; }

        /// <summary>Gets the darkest shade, SystemAccentColorDark3.</summary>
        public Color Dark3 { get; }

        /// <summary>
        /// Works the shades out the way Windows does in principle. The colourfulness of the accent is taken to full
        /// lightness and to none, the accent is mixed with those two ends, and no shade comes out greyer than the
        /// accent. The accent itself stays what it is, made opaque.
        /// </summary>
        public static AccentPalette Create(Color accent)
        {
            var opaque = Color.FromRgb(accent.R, accent.G, accent.B);
            var (_, a, b) = ToLab(opaque);
            var light = FromLab(100, a, b);
            var dark = FromLab(0, a, b);
            var saturation = ToHsl(opaque).S;

            Color Shade(Color end, double step) => NoGreyerThan(Mix(opaque, end, step), saturation);

            return new AccentPalette(opaque,
                                     Shade(light, LightSteps[0]),
                                     Shade(light, LightSteps[1]),
                                     Shade(light, LightSteps[2]),
                                     Shade(dark, DarkSteps[0]),
                                     Shade(dark, DarkSteps[1]),
                                     Shade(dark, DarkSteps[2]));
        }

        /// <summary>
        /// Gets the palette Windows keeps for the accent of the user, or <see langword="null"/> where there is none
        /// to read.
        /// </summary>
        public static AccentPalette? FromWindows()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(AccentKey))
                {
                    return FromRegistryValue(key?.GetValue(AccentPaletteValue) as byte[]);
                }
            }
            catch (Exception e) when (e is SecurityException or UnauthorizedAccessException or IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads the AccentPalette value Windows writes to the registry: eight colours of four bytes each, red, green,
        /// blue and one unused, in the order Light3, Light2, Light1, accent, Dark1, Dark2, Dark3 and one that is not
        /// part of the palette. Gives <see langword="null"/> for a value of another shape.
        /// </summary>
        public static AccentPalette? FromRegistryValue(byte[]? value)
        {
            if (value is null || value.Length < 28)
            {
                return null;
            }

            Color At(int index) => Color.FromRgb(value[index * 4], value[index * 4 + 1], value[index * 4 + 2]);

            return new AccentPalette(At(3), At(2), At(1), At(0), At(4), At(5), At(6));
        }

        /// <inheritdoc />
        public bool Equals(AccentPalette other)
        {
            return this.Accent == other.Accent
                   && this.Light1 == other.Light1
                   && this.Light2 == other.Light2
                   && this.Light3 == other.Light3
                   && this.Dark1 == other.Dark1
                   && this.Dark2 == other.Dark2
                   && this.Dark3 == other.Dark3;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is AccentPalette other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = this.Accent.GetHashCode();
                hash = (hash * 397) ^ this.Light1.GetHashCode();
                hash = (hash * 397) ^ this.Light2.GetHashCode();
                hash = (hash * 397) ^ this.Light3.GetHashCode();
                hash = (hash * 397) ^ this.Dark1.GetHashCode();
                hash = (hash * 397) ^ this.Dark2.GetHashCode();
                hash = (hash * 397) ^ this.Dark3.GetHashCode();
                return hash;
            }
        }

        /// <summary>Compares two palettes shade by shade.</summary>
        public static bool operator ==(AccentPalette left, AccentPalette right) => left.Equals(right);

        /// <summary>Compares two palettes shade by shade.</summary>
        public static bool operator !=(AccentPalette left, AccentPalette right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Light3} {this.Light2} {this.Light1} {this.Accent} {this.Dark1} {this.Dark2} {this.Dark3}";
        }

        private static Color Mix(Color from, Color to, double step)
        {
            byte Channel(byte a, byte b) => (byte)Math.Round(a + (b - a) * step, MidpointRounding.AwayFromZero);

            return Color.FromRgb(Channel(from.R, to.R), Channel(from.G, to.G), Channel(from.B, to.B));
        }

        private static Color NoGreyerThan(Color color, double saturation)
        {
            var (h, s, l) = ToHsl(color);
            return s < saturation ? FromHsl(h, saturation, l) : color;
        }

        // CIELAB with the D65 white, through linear sRGB and XYZ

        private const double WhiteX = 0.95047;
        private const double WhiteZ = 1.08883;
        private const double Epsilon = 216.0 / 24389.0;
        private const double Kappa = 24389.0 / 27.0;

        private static (double L, double A, double B) ToLab(Color color)
        {
            double r = ToLinear(color.R), g = ToLinear(color.G), b = ToLinear(color.B);

            var fx = LabF((0.4124564 * r + 0.3575761 * g + 0.1804375 * b) / WhiteX);
            var fy = LabF(0.2126729 * r + 0.7151522 * g + 0.0721750 * b);
            var fz = LabF((0.0193339 * r + 0.1191920 * g + 0.9503041 * b) / WhiteZ);

            return (116 * fy - 16, 500 * (fx - fy), 200 * (fy - fz));
        }

        // an end outside of sRGB is clamped channel by channel, which is what lets a light shade of blue lean towards cyan
        private static Color FromLab(double l, double a, double b)
        {
            var fy = (l + 16) / 116;
            var x = LabFInverse(fy + a / 500) * WhiteX;
            var y = LabFInverse(fy);
            var z = LabFInverse(fy - b / 200) * WhiteZ;

            return Color.FromRgb(FromLinear(3.2404542 * x - 1.5371385 * y - 0.4985314 * z),
                                 FromLinear(-0.9692660 * x + 1.8760108 * y + 0.0415560 * z),
                                 FromLinear(0.0556434 * x - 0.2040259 * y + 1.0572252 * z));
        }

        private static double LabF(double t) => t > Epsilon ? Math.Pow(t, 1.0 / 3.0) : (Kappa * t + 16) / 116;

        private static double LabFInverse(double t) => t * t * t > Epsilon ? t * t * t : (116 * t - 16) / Kappa;

        private static double ToLinear(byte channel)
        {
            var v = channel / 255.0;
            return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        private static byte FromLinear(double v)
        {
            var s = v <= 0.0031308 ? 12.92 * v : 1.055 * Math.Pow(v, 1 / 2.4) - 0.055;
            return ToByte(s);
        }

        private static byte ToByte(double unit) => (byte)Math.Max(0, Math.Min(255, Math.Round(unit * 255, MidpointRounding.AwayFromZero)));

        // HSL, hue as a fraction of the circle

        private static (double H, double S, double L) ToHsl(Color color)
        {
            double r = color.R / 255.0, g = color.G / 255.0, b = color.B / 255.0;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var l = (max + min) / 2;

            if (max == min)
            {
                return (0, 0, l);
            }

            var d = max - min;
            var s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
            double h;
            if (max == r)
            {
                h = (g - b) / d + (g < b ? 6 : 0);
            }
            else if (max == g)
            {
                h = (b - r) / d + 2;
            }
            else
            {
                h = (r - g) / d + 4;
            }

            return (h / 6, s, l);
        }

        private static Color FromHsl(double h, double s, double l)
        {
            var c = (1 - Math.Abs(2 * l - 1)) * s;
            var hp = h * 6;
            var x = c * (1 - Math.Abs(hp % 2 - 1));
            var m = l - c / 2;

            var (r, g, b) = ((int)Math.Floor(hp) % 6) switch
            {
                0 => (c, x, 0d),
                1 => (x, c, 0d),
                2 => (0d, c, x),
                3 => (0d, x, c),
                4 => (x, 0d, c),
                _ => (c, 0d, x),
            };

            return Color.FromRgb(ToByte(r + m), ToByte(g + m), ToByte(b + m));
        }
    }
}
