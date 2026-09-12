// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A header can be painted differently while the mouse is over it and while it is held down. Neither
    /// of those is a state a test can put a control into, so what is measured here is the brush at rest
    /// and that nothing is taken away from a header nobody handed one of them to.
    /// </summary>
    [TestFixture]
    public class HeaderStateColoursTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("A brush of its own for the glyph wins over the foreground the header hands it.")]
        public void TheGlyphTakesTheColourItWasGiven()
        {
            var expander = this.Show(new Expander { Header = "a header" });
            var arrow = expander.FindChild<Path>("Arrow");
            var circle = expander.FindChild<Ellipse>("Circle");

            Assert.That(arrow, Is.Not.Null);
            Assert.That(circle, Is.Not.Null);
            Assert.That(arrow!.Stroke, Is.SameAs(HeaderOf(expander).Foreground), "at rest the glyph follows the header");

            ExpanderHelper.SetToggleButtonForeground(HeaderOf(expander), Brushes.HotPink);
            this.Settle();

            Assert.That(arrow.Stroke, Is.SameAs(Brushes.HotPink));
            Assert.That(circle!.Stroke, Is.SameAs(Brushes.HotPink));
        }

        [Test]
        [Description("The brush is given to the expander, which is where somebody sets it, and has to reach the toggle button inside.")]
        public void TheGlyphTakesTheColourTheExpanderWasGiven()
        {
            var expander = this.Show(new Expander { Header = "a header" });
            var arrow = expander.FindChild<Path>("Arrow");

            ExpanderHelper.SetToggleButtonForeground(expander, Brushes.HotPink);
            this.Settle();

            Assert.That(arrow!.Stroke, Is.SameAs(Brushes.HotPink), "the expander should hand it to the toggle button the way it hands over ShowToggleButton");
        }

        [Test]
        [Description("Opening an expander is not one of the two states, so the glyph stays as it was.")]
        public void TheGlyphKeepsItsColourWhenTheExpanderIsOpened()
        {
            var expander = this.Show(new Expander { Header = "a header" });
            var arrow = expander.FindChild<Path>("Arrow");
            var toggle = HeaderOf(expander);

            ExpanderHelper.SetToggleButtonForeground(toggle, Brushes.HotPink);
            ExpanderHelper.SetToggleButtonForegroundMouseOver(toggle, Brushes.Lime);
            ExpanderHelper.SetToggleButtonForegroundPressed(toggle, Brushes.Orange);
            this.Settle();

            expander.IsExpanded = true;
            this.Settle();

            Assert.That(arrow!.Stroke, Is.SameAs(Brushes.HotPink), "neither the mouse nor a press is what opened it");
        }

        [Test]
        [Description("Left unset, none of them may take anything away from the header as it was.")]
        public void AHeaderNobodyGaveThoseBrushesStaysAsItWas()
        {
            var expander = this.Show(new Expander { Header = "a header" });
            var headerSite = expander.FindChild<Border>("HeaderSite");
            var arrow = expander.FindChild<Path>("Arrow");

            HeaderedControlHelper.SetHeaderBackground(expander, Brushes.SlateGray);
            this.Settle();

            var foreground = HeaderOf(expander).Foreground;

            expander.IsExpanded = true;
            this.Settle();

            Assert.That(headerSite!.Background, Is.SameAs(Brushes.SlateGray), "the header should keep its background");
            Assert.That(HeaderOf(expander).Foreground, Is.SameAs(foreground), "and its foreground");
            Assert.That(arrow!.Stroke, Is.SameAs(foreground), "and the glyph should still follow the header");
        }

        /// <summary>The toggle button an expander puts its header in.</summary>
        private static ToggleButton HeaderOf(Expander expander)
        {
            var toggle = expander.FindChild<ToggleButton>("ToggleSite");
            Assert.That(toggle, Is.Not.Null, "the expander should put its header in a toggle button");

            return toggle!;
        }

        private Expander Show(Expander expander)
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.Content = expander;
            this.Settle();

            Assert.That(expander.IsLoaded, Is.True, "the expander should be up before a test looks at it");

            return expander;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
