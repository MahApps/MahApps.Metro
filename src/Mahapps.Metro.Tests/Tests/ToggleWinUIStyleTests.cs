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
    /// The WinUI check box and radio button are the Fluent 2 pair Windows 11 draws, and both of them
    /// are the template of the default style with a different set of brushes and sizes in front of
    /// it. These tests hold the wiring rather than the palette: what they compare against are the
    /// brush resources themselves, so a value may be corrected in the generator parameters without a
    /// test having to be edited.
    /// </summary>
    [TestFixture]
    public class ToggleWinUIStyleTests
    {
        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("The box is a rounded twenty with a hairline round it, which is what tells it from the square Windows 10 one.")]
        public void TheBoxIsRoundedAndCarriesAHairline()
        {
            var box = (CheckBox)this.Show(new CheckBox { Content = "Beam me up..." }, "MahApps.Styles.CheckBox.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(CheckBoxHelper.GetCheckSize(box), Is.EqualTo(20d));
                    Assert.That(CheckBoxHelper.GetCheckStrokeThickness(box), Is.EqualTo(1d), "a hairline, where the Windows 10 box has two units");
                    Assert.That(CheckBoxHelper.GetCheckCornerRadius(box), Is.EqualTo(new CornerRadius(4)));
                });
        }

        [Test]
        [Description("Ticked, the box is the accent with the ideal foreground of that accent in it, and the frame around the control stays out of the way.")]
        public void ATickedBoxIsTheAccent()
        {
            var box = (CheckBox)this.Show(new CheckBox { Content = "Beam me up...", IsChecked = true }, "MahApps.Styles.CheckBox.WinUI");

            var rectangle = box.FindChild<Rectangle>("NormalRectangle");
            Assert.That(rectangle, Is.Not.Null, "the template should carry the box");

            Assert.Multiple(() =>
                {
                    Assert.That(rectangle!.Fill, Is.SameAs(box.FindResource("MahApps.Brushes.WinUI.AccentFillDefault")), "the fill of a ticked box");
                    Assert.That(CheckBoxHelper.GetCheckGlyphForegroundChecked(box), Is.SameAs(box.FindResource("MahApps.Brushes.WinUI.TextOnAccentPrimary")), "and the tick in it");
                    Assert.That(ColourOf(box.FindResource("MahApps.Brushes.WinUI.TextOnAccentPrimary")), Is.EqualTo((Color)box.FindResource("MahApps.Colors.IdealForeground")), "which is the ideal foreground of the accent rather than a colour of its own");
                });
        }

        [Test]
        [Description("Untouched, the box is a shade off the page with the strong stroke round it.")]
        public void AnIdleBoxCarriesTheAltFillAndTheStrongStroke()
        {
            var box = (CheckBox)this.Show(new CheckBox { Content = "Beam me up..." }, "MahApps.Styles.CheckBox.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(CheckBoxHelper.GetCheckBackgroundFillUnchecked(box), Is.SameAs(box.FindResource("MahApps.Brushes.WinUI.ControlAltFillSecondary")));
                    Assert.That(CheckBoxHelper.GetCheckBackgroundStrokeUnchecked(box), Is.SameAs(box.FindResource("MahApps.Brushes.WinUI.ControlStrongStrokeDefault")));
                });
        }

        [Test]
        [Description("The ring of the radio button is the same twenty with the same hairline, and the dot in it is twelve.")]
        public void TheRingMatchesTheBox()
        {
            var radio = (RadioButton)this.Show(new RadioButton { Content = "Beam me up..." }, "MahApps.Styles.RadioButton.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(RadioButtonHelper.GetRadioSize(radio), Is.EqualTo(20d));
                    Assert.That(RadioButtonHelper.GetRadioStrokeThickness(radio), Is.EqualTo(1d));
                    Assert.That(RadioButtonHelper.GetRadioCheckSize(radio), Is.EqualTo(12d));
                    Assert.That(RadioButtonHelper.GetOuterEllipseCheckedFill(radio), Is.SameAs(radio.FindResource("MahApps.Brushes.WinUI.AccentFillDefault")), "picked, the ring is filled with the accent");
                    Assert.That(RadioButtonHelper.GetCheckGlyphFill(radio), Is.SameAs(radio.FindResource("MahApps.Brushes.WinUI.TextOnAccentPrimary")), "and the dot is the ideal foreground of it");
                });
        }

        [Test]
        [Description("Held down, the dot shrinks. Under the pointer it stays where it is: WinUI grows it to fourteen there, which leaves three units of ring, and without the animation WinUI plays over it the jump is worse than nothing.")]
        public void TheDotShrinksWhileTheButtonIsHeldDown()
        {
            var radio = (RadioButton)this.Show(new RadioButton { Content = "Beam me up...", IsChecked = true }, "MahApps.Styles.RadioButton.WinUI");

            // IsPressed and IsMouseOver are read-only, so the states are asked of the style rather
            // than of a pointer no test has
            var pressed = TriggerFor(radio.Style, ButtonBase.IsPressedProperty);

            Assert.That(pressed, Is.Not.Null, "the style should answer the button being held down");

            Assert.Multiple(() =>
                {
                    Assert.That(pressed!.Setters, Has.Exactly(1).Matches<SetterBase>(setter =>
                                                                                        setter is Setter { Property: var property, Value: var value }
                                                                                        && property == RadioButtonHelper.RadioCheckSizeProperty
                                                                                        && Equals(value, 10d)));
                    Assert.That(TriggerFor(radio.Style, UIElement.IsMouseOverProperty), Is.Null, "and the pointer alone should not change the size of anything");
                });
        }

        private static Trigger? TriggerFor(Style style, DependencyProperty property)
        {
            foreach (var trigger in style.Triggers)
            {
                if (trigger is Trigger one && one.Property == property)
                {
                    return one;
                }
            }

            return null;
        }

        private static Color ColourOf(object brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>(), "this one should be a brush of a single colour");

            return ((SolidColorBrush)brush).Color;
        }

        private Control Show(Control control, string key)
        {
            Assert.That(this.window, Is.Not.Null);

            control.Style = (Style)Application.Current.FindResource(key);

            this.window!.Content = control;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return control;
        }
    }
}
