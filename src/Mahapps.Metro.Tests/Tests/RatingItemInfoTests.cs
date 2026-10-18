// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class RatingItemInfoTests
    {
        [Test]
        public void AFrozenFontInfoKeepsItsGlyphs()
        {
            var info = new RatingItemFontInfo { Glyph = "\uE735", UnsetGlyph = "\uE734" };
            info.Freeze();
            var clone = (RatingItemFontInfo)info.Clone();

            Assert.Multiple(() =>
            {
                Assert.That(info.IsFrozen, Is.True);
                Assert.That(clone.Glyph, Is.EqualTo("\uE735"));
                Assert.That(clone.UnsetGlyph, Is.EqualTo("\uE734"));
                Assert.That(clone.IsFrozen, Is.False);
            });
        }

        [Test]
        public void ChangingAGlyphRaisesChanged()
        {
            var info = new RatingItemFontInfo();
            var changed = 0;
            info.Changed += (_, _) => changed++;

            info.PointerOverGlyph = "\uE735";

            Assert.That(changed, Is.EqualTo(1));
        }

        [Test]
        public void ChangingAnImageRaisesChanged()
        {
            var info = new RatingItemImageInfo();
            var changed = 0;
            info.Changed += (_, _) => changed++;

            info.PlaceholderImage = new DrawingImage();

            Assert.That(changed, Is.EqualTo(1));
        }

        [Test]
        public void TheDefaultsAreNull()
        {
            var font = new RatingItemFontInfo();
            var image = new RatingItemImageInfo();

            Assert.Multiple(() =>
            {
                Assert.That(font.Glyph, Is.Null);
                Assert.That(font.UnsetGlyph, Is.Null);
                Assert.That(font.PlaceholderGlyph, Is.Null);
                Assert.That(font.PointerOverGlyph, Is.Null);
                Assert.That(font.PointerOverPlaceholderGlyph, Is.Null);
                Assert.That(font.DisabledGlyph, Is.Null);
                Assert.That(image.Image, Is.Null);
                Assert.That(image.UnsetImage, Is.Null);
                Assert.That(image.PlaceholderImage, Is.Null);
                Assert.That(image.PointerOverImage, Is.Null);
                Assert.That(image.PointerOverPlaceholderImage, Is.Null);
                Assert.That(image.DisabledImage, Is.Null);
            });
        }
    }
}
