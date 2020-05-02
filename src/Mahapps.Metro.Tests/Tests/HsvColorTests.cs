using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Xunit;

namespace MahApps.Metro.Tests
{
  
    public class HsvColorTests
    {
        [Fact]
        public void TestHsvFromColor()
        {
            Parallel.For(0, byte.MaxValue + 1, (r) =>
            {
                Color color;
                for (byte g = 0; g < byte.MaxValue; g++)
                {
                    for (byte b = 0; b < byte.MaxValue; b++)
                    {
                        color = Color.FromRgb((byte)r, g, b);
                        Assert.Equal(color, new HSVColor(color).ToColor());
                    }
                }
            });
        }

        [Fact]
        public void TestColorPerComponent()
        {
            Color color;

            // Gray
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                color = Color.FromArgb(255, i, i, i);
                Assert.Equal(color, new HSVColor(color).ToColor());
            }

            // A
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                color = Color.FromArgb(i, 255, 255, 255);
                Assert.Equal(color, new HSVColor(color).ToColor());
            }

            // R
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                color = Color.FromArgb(255, i, 255, 255);
                Assert.Equal(color, new HSVColor(color).ToColor());
            }

            // G
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                color = Color.FromArgb(255, 255, i, 255);
                Assert.Equal(color, new HSVColor(color).ToColor());
            }

            // B
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                color = Color.FromArgb(255, 255, 255, i);
                Assert.Equal(color, new HSVColor(color).ToColor());
            }
        }

        [Fact]
        public void TestHsvFromColor_BuildInColors()
        {
            foreach (var color in typeof(Colors).GetProperties().Where(x => x.PropertyType == typeof(Color)).Select(x => (Color)x.GetValue(null)))
            {
                Assert.Equal(color, new HSVColor(color).ToColor());
            }
        }


        [Fact]
        public void TestHsvFromInput()
        {
            // Transparent
            Assert.Equal(Colors.Transparent, new HSVColor(0, 0, 0, 1).ToColor());

            // Black
            Assert.Equal(Colors.Black, new HSVColor(0, 0, 0).ToColor());

            // White
            Assert.Equal(Colors.White, new HSVColor(0, 0, 1).ToColor());

            // Gray
            Assert.Equal(Colors.Gray, new HSVColor(0, 0, 0.502).ToColor());

            // Red
            Assert.Equal(Colors.Red, new HSVColor(0, 1, 1).ToColor());

            // Yellow
            Assert.Equal(Colors.Yellow, new HSVColor(60, 1, 1).ToColor());

            // Lime (Green)
            Assert.Equal(Colors.Lime, new HSVColor(120, 1, 1).ToColor());

            // Aqua
            Assert.Equal(Colors.Aqua, new HSVColor(180, 1, 1).ToColor());

            // Blue
            Assert.Equal(Colors.Blue, new HSVColor(240, 1, 1).ToColor());

            // Magenta
            Assert.Equal(Colors.Magenta, new HSVColor(300, 1, 1).ToColor());
        }
    }
}
