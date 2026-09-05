// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class ProgressBarTests
    {
        private ProgressBarWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<ProgressBarWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private static Color ColorOf(Brush? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "the test sets a solid colour, so this should be one");

            return ((SolidColorBrush)brush!).Color;
        }

        [Test]
        public void ForegroundShouldPaintTheIndicator()
        {
            Assert.That(this.window, Is.Not.Null);

            var progressBar = this.window.TheColouredProgressBar;
            progressBar.UpdateLayout();

            var indicator = progressBar.FindChild<Border>("PART_Indicator");
            Assert.That(indicator, Is.Not.Null, "the template should carry the indicator");
            Assert.That(ColorOf(indicator!.Background), Is.EqualTo(Colors.Red), "the indicator should be painted with Foreground");
        }

        [Test]
        public void ForegroundShouldPaintTheIndeterminateFill()
        {
            Assert.That(this.window, Is.Not.Null);

            var progressBar = this.window.TheIndeterminateProgressBar;
            progressBar.UpdateLayout();

            var fill = progressBar.FindChild<Rectangle>("IndeterminateSolidFill");
            Assert.That(fill, Is.Not.Null, "the template should carry the indeterminate fill");
            Assert.That(ColorOf(fill!.Fill), Is.EqualTo(Colors.Blue), "the stripes should run over Foreground as well");
        }

        [Test]
        public void ProgressBarWithoutAForegroundShouldKeepTheProgressBrush()
        {
            Assert.That(this.window, Is.Not.Null);

            var progressBar = this.window.TheProgressBar;
            progressBar.UpdateLayout();

            var progressBrush = progressBar.TryFindResource("MahApps.Brushes.Progress") as Brush;
            Assert.That(progressBrush, Is.Not.Null, "the theme should carry the progress brush");

            var indicator = progressBar.FindChild<Border>("PART_Indicator");
            Assert.That(indicator, Is.Not.Null);
            Assert.That(indicator!.Background, Is.SameAs(progressBrush), "leaving Foreground alone should keep the look it always had");

            var fill = this.window.TheDefaultIndeterminateProgressBar.FindChild<Rectangle>("IndeterminateSolidFill");
            Assert.That(fill, Is.Not.Null);
            Assert.That(fill!.Fill, Is.SameAs(progressBrush), "the same goes for the stripes");
        }
    }
}
