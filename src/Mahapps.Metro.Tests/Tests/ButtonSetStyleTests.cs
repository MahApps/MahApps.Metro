// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// The button of the WinUI set is the Windows 10 one in the colours Windows 11 gives it, plus an
    /// edge along its bottom drawn a shade stronger than the rest of the frame. That edge is what
    /// makes the button look as if it stood a little above the page, and it is a border of its own in
    /// the template, so a set that draws no such edge leaves the brush for it unset.
    /// </summary>
    [TestFixture]
    public class ButtonSetStyleTests
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

        [TestCase("MahApps.Styles.Button.WinUI")]
        [TestCase("MahApps.Styles.Button.Accent.WinUI")]
        [Description("A WinUI button says it stands above the page with a bottom edge in a brush of its own.")]
        public void TheWinUIButtonStandsAboveThePage(string key)
        {
            var button = this.Show(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(button).BorderBrush, Is.Not.Null, "the edge should be drawn");
                    Assert.That(Edge(button).BorderBrush, Is.Not.SameAs(button.BorderBrush), "in a brush of its own rather than the one the frame takes");
                    Assert.That(Frame(button).BorderBrush, Is.SameAs(button.BorderBrush), "and the frame should be left alone");
                });
        }

        [TestCase("MahApps.Styles.Button.Win10")]
        [TestCase("MahApps.Styles.Button.Accent.Win10")]
        [Description("The Windows 10 set draws no such edge, so the brush stays unset and nothing is drawn.")]
        public void TheWindows10ButtonDrawsNoEdge(string key)
        {
            var button = this.Show(key);

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetBottomBorderBrush(button), Is.Null, "no brush on the button");
                    Assert.That(Edge(button).BorderBrush, Is.Null, "and nothing along the bottom of it");
                });
        }

        [Test]
        [Description("The edge is the same one unit on a button of any height, because it is a border of its own rather than part of the frame.")]
        public void TheWinUIEdgeIsTheSameOneUnitEitherWay()
        {
            var small = this.Show("MahApps.Styles.Button.WinUI");
            var tallEdge = Edge(this.Show("MahApps.Styles.Button.WinUI", 96)).BorderThickness;

            Assert.Multiple(() =>
                {
                    Assert.That(Edge(small).BorderThickness, Is.EqualTo(new Thickness(0, 0, 0, 1)), "on a button of the usual height");
                    Assert.That(tallEdge, Is.EqualTo(new Thickness(0, 0, 0, 1)), "and on one three times as tall");
                });
        }

        [TestCase("MahApps.Styles.Button.WinUI")]
        [TestCase("MahApps.Styles.Button.Accent.WinUI")]
        [Description("The frame leaves its bottom line to the edge, so that the two do not lie on top of each other and add up to a line brighter than either.")]
        public void TheFrameLeavesItsBottomLineToTheEdge(string key)
        {
            var button = this.Show(key);

            Assert.Multiple(() =>
                {
                    Assert.That(Frame(button).BorderThickness.Bottom, Is.Zero, "the frame draws nothing along the bottom");
                    Assert.That(Edge(button).BorderThickness.Bottom, Is.EqualTo(1), "and the edge draws the one line there is");
                    Assert.That(Frame(button).BorderThickness.Top, Is.EqualTo(1), "while the rest of the frame is the same one unit");
                });
        }

        [Test]
        [Description("The frame of an accented button is a lit edge above and a shadow below rather than the grey of the quiet one.")]
        public void TheAccentedButtonIsFramedByALitEdgeAndAShadow()
        {
            var accented = this.Show("MahApps.Styles.Button.Accent.WinUI");
            var quiet = this.Show("MahApps.Styles.Button.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(accented.BorderBrush, Is.Not.SameAs(quiet.BorderBrush), "the frame is not the one the quiet button wears");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(accented), Is.Not.SameAs(ControlsHelper.GetBottomBorderBrush(quiet)), "and neither is the edge under it");
                });
        }

        [Test]
        [Description("A button and the box beside it are the same height, so a form putting one next to the other comes out level.")]
        public void TheWinUIButtonIsAsTallAsTheBoxBesideIt()
        {
            var button = new Button { Content = "Beam me up...", Style = (Style)Application.Current.FindResource("MahApps.Styles.Button.WinUI") };
            var box = new TextBox { Text = "42", Width = 120, Style = (Style)Application.Current.FindResource("MahApps.Styles.TextBox.WinUI") };

            var row = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Top };
            row.Children.Add(button);
            row.Children.Add(box);

            this.window!.Content = row;
            this.Settle();

            Assert.That(button.ActualHeight, Is.EqualTo(box.ActualHeight), $"the button is {button.ActualHeight} and the box {box.ActualHeight}");
        }

        [Test]
        [Description("A button with nothing but an icon in it is still as tall as the box beside it, because the set gives it the smallest height it gives everything else.")]
        public void AButtonHoldingAnIconIsStillAsTallAsTheBox()
        {
            var button = this.Show("MahApps.Styles.Button.WinUI");
            button.Content = new Border { Width = 16, Height = 16, Background = Brushes.SteelBlue };
            this.Settle();

            Assert.That(button.ActualHeight, Is.EqualTo(32), $"the button is {button.ActualHeight}");
        }

        private Button Show(string key, double height = double.NaN)
        {
            Assert.That(this.window, Is.Not.Null);

            var button = new Button
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             Content = "Beam me up...",
                             VerticalAlignment = VerticalAlignment.Top
                         };

            if (!double.IsNaN(height))
            {
                button.Height = height;
            }

            this.window!.Content = button;
            this.Settle();

            return button;
        }

        private static Border Frame(Button button)
        {
            return (Border)Part(button, "Border");
        }

        private static Border Edge(Button button)
        {
            return (Border)Part(button, "BottomEdge");
        }

        private static FrameworkElement Part(Button button, string name)
        {
            var part = button.Template?.FindName(name, button) as FrameworkElement;
            Assert.That(part, Is.Not.Null, $"the template should carry {name}");

            return part!;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
