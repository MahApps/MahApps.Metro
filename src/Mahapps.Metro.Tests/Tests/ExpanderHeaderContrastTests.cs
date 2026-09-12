// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
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
    /// GH-4386: the arrow in an expander header used to turn grey while the mouse was over it, which on
    /// the accent coloured header it sits on left almost nothing to see. Touching a thing must not make it
    /// harder to make out than it was at rest.
    /// </summary>
    [TestFixture]
    public class ExpanderHeaderContrastTests
    {
        /// <summary>What the accessibility guidelines put under a shape somebody has to make out.</summary>
        private const double LeastContrast = 3.0;

        /// <summary>The two shapes the glyph is drawn from, with the keys each of them reads its stroke from.</summary>
        private static readonly (string Shape, string AtRest, string UnderTheMouse, string UnderAPress)[] TheGlyph =
        {
            ("Circle",
                "ExpanderToggleButtonEllipseThemeStrokeThickness",
                "ExpanderToggleButtonEllipseThemeStrokeThicknessMouseOver",
                "ExpanderToggleButtonEllipseThemeStrokeThicknessPressed"),
            ("Arrow",
                "ExpanderToggleButtonArrowThemeStrokeThickness",
                "ExpanderToggleButtonArrowThemeStrokeThicknessMouseOver",
                "ExpanderToggleButtonArrowThemeStrokeThicknessPressed")
        };

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

        /// <summary>
        /// A test that tries a value of its own leaves it behind in the window all the tests here share,
        /// so whatever was put there goes again, even when the test it belonged to did not get that far.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            foreach (var shape in TheGlyph)
            {
                foreach (var key in new[] { shape.AtRest, shape.UnderTheMouse, shape.UnderAPress })
                {
                    this.window?.Resources.Remove(key);
                }
            }
        }

        [TestCase(ExpandDirection.Down)]
        [TestCase(ExpandDirection.Up)]
        [TestCase(ExpandDirection.Left)]
        [TestCase(ExpandDirection.Right)]
        [Description("Neither the mouse over nor the pressed state may cost the glyph any of the contrast it has at rest.")]
        public void TheGlyphKeepsItsContrastWhileTheHeaderIsTouched(ExpandDirection direction)
        {
            var toggle = this.HeaderOf(direction);
            var header = this.HeaderGroundOf(toggle);
            var atRest = ColourOf(toggle.Foreground);

            Assert.That(atRest, Is.Not.Null, "the glyph should be drawn in a plain colour");

            var resting = Contrast(atRest!.Value, header);
            Assert.That(resting, Is.GreaterThanOrEqualTo(LeastContrast), "the glyph should be readable before anybody touches it");

            foreach (var state in new[] { UIElement.IsMouseOverProperty, ButtonBase.IsPressedProperty })
            {
                foreach (var shape in new[] { "Arrow", "Circle" })
                {
                    var touched = StrokeIn(toggle, state, shape) ?? atRest.Value;
                    var contrast = Contrast(touched, header);

                    Assert.That(contrast,
                                Is.GreaterThanOrEqualTo(resting - 0.01),
                                $"{shape} loses contrast under {state.Name}, {contrast:0.00} to 1 against {resting:0.00} to 1 at rest");
                    Assert.That(contrast, Is.GreaterThanOrEqualTo(LeastContrast), $"{shape} should stay readable under {state.Name}");
                }
            }
        }

        [TestCase(ExpandDirection.Down)]
        [TestCase(ExpandDirection.Up)]
        [TestCase(ExpandDirection.Left)]
        [TestCase(ExpandDirection.Right)]
        [Description("Keeping the colour is only half of it, both states still have to show that the header answers.")]
        public void TouchingTheHeaderStillChangesTheGlyph(ExpandDirection direction)
        {
            var toggle = this.HeaderOf(direction);

            foreach (var state in new[] { UIElement.IsMouseOverProperty, ButtonBase.IsPressedProperty })
            {
                Assert.That(SettersIn(toggle, state), Is.Not.Empty, $"{state.Name} should still be answered with something");
            }
        }

        [TestCase(ExpandDirection.Down)]
        [TestCase(ExpandDirection.Up)]
        [TestCase(ExpandDirection.Left)]
        [TestCase(ExpandDirection.Right)]
        [Description("How far the two strokes grow under the mouse and under a press is the theme to say, the way the size of the circle already is.")]
        public void TheGlyphTakesTheThicknessTheThemeAsksFor(ExpandDirection direction)
        {
            var toggle = this.HeaderOf(direction);

            foreach (var shape in TheGlyph)
            {
                foreach (var state in new[] { (Property: UIElement.IsMouseOverProperty, Key: shape.UnderTheMouse), (Property: ButtonBase.IsPressedProperty, Key: shape.UnderAPress) })
                {
                    var setter = SettersIn(toggle, state.Property)
                                 .LastOrDefault(candidate => candidate.TargetName == shape.Shape && candidate.Property == Shape.StrokeThicknessProperty);

                    Assert.That(setter, Is.Not.Null, $"{state.Property.Name} should say how thick {shape.Shape} is drawn");
                    Assert.That(setter!.Value, Is.InstanceOf<DynamicResourceExtension>(), $"{shape.Shape} under {state.Property.Name} should read that from the theme rather than carry a number");
                    Assert.That(((DynamicResourceExtension)setter.Value).ResourceKey, Is.EqualTo(state.Key));
                    Assert.That(toggle.TryFindResource(state.Key), Is.Not.Null, "and the theme should hold that key");

                    // what being settable comes down to: a value of somebody own is what the template then reads
                    this.window!.Resources[state.Key] = 5d;
                    Assert.That(Resolve(setter.Value, toggle), Is.EqualTo(5d), $"{state.Key} should be the way to change it");
                }
            }
        }

        [TestCase(ExpandDirection.Down)]
        [TestCase(ExpandDirection.Up)]
        [TestCase(ExpandDirection.Left)]
        [TestCase(ExpandDirection.Right)]
        [Description("What nobody is touching is drawn from the theme as well, so all of them can be kept in step.")]
        public void TheGlyphAtRestTakesTheThicknessTheThemeAsksFor(ExpandDirection direction)
        {
            var toggle = this.HeaderOf(direction);

            foreach (var shape in TheGlyph)
            {
                var drawn = toggle.FindChild<Shape>(shape.Shape);

                Assert.That(drawn, Is.Not.Null, $"the header should carry {shape.Shape}");
                Assert.That(toggle.TryFindResource(shape.AtRest), Is.Not.Null, "the theme should hold that key");
                Assert.That(drawn!.StrokeThickness, Is.EqualTo(toggle.TryFindResource(shape.AtRest)), $"{shape.Shape} should be drawn as thick as the theme says");

                // this one is read off the shape itself, so a value of somebody own has to arrive there
                this.window!.Resources[shape.AtRest] = 5d;
                this.window.UpdateLayout();

                Assert.That(drawn.StrokeThickness, Is.EqualTo(5d), $"{shape.AtRest} should be the way to change it");
            }
        }

        /// <summary>The toggle button an expander puts its header in, laid out and ready to be read.</summary>
        private ToggleButton HeaderOf(ExpandDirection direction)
        {
            Assert.That(this.window, Is.Not.Null);

            var expander = new Expander { Header = "a header", Content = new TextBlock { Text = "some content" }, ExpandDirection = direction };

            this.window!.Content = expander;
            this.window.UpdateLayout();
            expander.UpdateLayout();

            ClipAssert.Pump();

            var toggle = expander.FindChild<ToggleButton>("ToggleSite");
            Assert.That(toggle, Is.Not.Null, "the expander should put its header in a toggle button");

            var arrow = toggle!.FindChild<Path>("Arrow");
            Assert.That(arrow, Is.Not.Null, "the header should carry the arrow");
            Assert.That(arrow!.ActualWidth, Is.GreaterThan(0), "the arrow should be laid out, otherwise this test proves nothing");

            return toggle;
        }

        /// <summary>
        /// What the glyph is seen against. The header brush carries an alpha of its own, so what counts is
        /// the header over whatever the window puts behind it.
        /// </summary>
        private Color HeaderGroundOf(ToggleButton toggle)
        {
            var expander = toggle.TryFindParent<Expander>();
            Assert.That(expander, Is.Not.Null);

            var header = ColourOf(HeaderedControlHelper.GetHeaderBackground(expander!));
            Assert.That(header, Is.Not.Null, "the header should be painted in a plain colour");

            var behind = ColourOf(this.window!.Background) ?? Colors.White;

            return Over(header!.Value, behind);
        }

        /// <summary>What a template trigger for that state makes of the stroke of one of the two shapes.</summary>
        private static Color? StrokeIn(ToggleButton toggle, DependencyProperty state, string shape)
        {
            var setter = SettersIn(toggle, state)
                         .LastOrDefault(candidate => candidate.TargetName == shape && candidate.Property == Shape.StrokeProperty);

            return setter is null ? null : ColourOf(Resolve(setter.Value, toggle));
        }

        private static Setter[] SettersIn(ToggleButton toggle, DependencyProperty state)
        {
            Assert.That(toggle.Template, Is.Not.Null);

            return toggle.Template.Triggers
                         .OfType<Trigger>()
                         .Where(trigger => trigger.Property == state && Equals(trigger.Value, true))
                         .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                         .ToArray();
        }

        /// <summary>A setter that was never applied still holds the resource reference rather than the brush.</summary>
        private static object? Resolve(object? value, FrameworkElement scope)
        {
            return value is DynamicResourceExtension reference ? scope.TryFindResource(reference.ResourceKey) : value;
        }

        private static Color? ColourOf(object? brush)
        {
            return brush is SolidColorBrush solid ? solid.Color : null;
        }

        /// <summary>One colour painted over another, which is what an alpha in a brush comes down to.</summary>
        private static Color Over(Color colour, Color ground)
        {
            var alpha = colour.A / 255d;

            return Color.FromRgb((byte)Math.Round((colour.R * alpha) + (ground.R * (1 - alpha))),
                                 (byte)Math.Round((colour.G * alpha) + (ground.G * (1 - alpha))),
                                 (byte)Math.Round((colour.B * alpha) + (ground.B * (1 - alpha))));
        }

        /// <summary>The ratio the accessibility guidelines are written in, where 1 to 1 is invisible.</summary>
        private static double Contrast(Color one, Color other)
        {
            var first = Luminance(one);
            var second = Luminance(other);

            return (Math.Max(first, second) + 0.05) / (Math.Min(first, second) + 0.05);
        }

        private static double Luminance(Color colour)
        {
            return (0.2126 * Channel(colour.R)) + (0.7152 * Channel(colour.G)) + (0.0722 * Channel(colour.B));
        }

        private static double Channel(byte value)
        {
            var channel = value / 255d;

            return channel <= 0.03928 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);
        }
    }
}
