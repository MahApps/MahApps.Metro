// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Focus colours the border a control already has. It used to make it thicker as well, through
    /// ControlsHelper.FocusBorderThickness, which moved the content of the control every time it was
    /// focused.
    /// </summary>
    [TestFixture]
    public class FocusBorderTests
    {
        private TestWindow? window;
        private ResourceDictionary? buttonDictionary;
        private ResourceDictionary? eyeDropperDictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.buttonDictionary = Load("Styles/Controls.Buttons.xaml");
            this.eyeDropperDictionary = Load("Themes/ColorPicker/ColorEyeDropper.xaml");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private static ResourceDictionary Load(string path)
        {
            return new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/{path}", UriKind.Absolute) };
        }

        private Control Show(Control control)
        {
            Assert.That(this.window, Is.Not.Null);

            control.Width = 200;
            control.Height = 48;
            control.BorderThickness = new Thickness(3);

            this.window!.Content = control;
            this.window.UpdateLayout();
            control.UpdateLayout();
            ClipAssert.Pump();

            return control;
        }

        private static Border GetBorder(Control control)
        {
            var border = control.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the template should carry the border");

            return border!;
        }

        [Test]
        public void AFocusedButtonShouldKeepTheThicknessOfItsBorder()
        {
            var button = new Button { Content = "Beam me up..." };
            button.SetValue(FrameworkElement.StyleProperty, this.buttonDictionary!["MahApps.Styles.Button"]);

            this.Show(button);

            var border = GetBorder(button);
            var thickness = border.BorderThickness;
            Assert.That(thickness, Is.EqualTo(new Thickness(3)), "the border should start out as thick as the button says");

            button.Focus();
            button.UpdateLayout();
            ClipAssert.Pump();

            // A build agent does not always hand out the keyboard focus, and without it there is
            // nothing to look at here.
            Assume.That(button.IsKeyboardFocusWithin, Is.True);
            Assert.That(border.BorderThickness, Is.EqualTo(thickness), "focus should not change how thick the border is, only what colour it has");
        }

        [Test]
        public void AFocusedButtonShouldTakeTheFocusBrushForItsBorder()
        {
            var button = new Button { Content = "Beam me up..." };
            button.SetValue(FrameworkElement.StyleProperty, this.buttonDictionary!["MahApps.Styles.Button"]);
            ControlsHelper.SetFocusBorderBrush(button, Brushes.Red);

            this.Show(button);

            var border = GetBorder(button);

            button.Focus();
            button.UpdateLayout();
            ClipAssert.Pump();

            // A build agent does not always hand out the keyboard focus, and without it there is
            // nothing to look at here.
            Assume.That(button.IsKeyboardFocusWithin, Is.True);
            Assert.That(border.BorderBrush, Is.SameAs(Brushes.Red), "colouring the border is what marks the focus now");
        }

        [Test]
        public void AFocusedEyeDropperShouldKeepTheThicknessOfItsBorder()
        {
            var eyeDropper = new ColorEyeDropper();
            eyeDropper.SetValue(FrameworkElement.StyleProperty, this.eyeDropperDictionary!["MahApps.Styles.ColorEyeDropper"]);

            this.Show(eyeDropper);

            var border = GetBorder(eyeDropper);
            var thickness = border.BorderThickness;
            Assert.That(thickness, Is.EqualTo(new Thickness(3)));

            eyeDropper.Focus();
            eyeDropper.UpdateLayout();
            ClipAssert.Pump();

            // A build agent does not always hand out the keyboard focus, and without it there is
            // nothing to look at here.
            Assume.That(eyeDropper.IsKeyboardFocusWithin, Is.True);
            Assert.That(border.BorderThickness, Is.EqualTo(thickness), "focus should not change how thick the border is, only what colour it has");
        }
    }
}
