// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class InfoBarPanelTests
    {
        private static (InfoBarPanel Panel, Border First, Border Second) TwoBoxes()
        {
            var first = new Border { Width = 50, Height = 20 };
            var second = new Border { Width = 60, Height = 20, HorizontalAlignment = HorizontalAlignment.Left };
            InfoBarPanel.SetHorizontalOrientationMargin(second, new Thickness(12, 0, 0, 0));
            InfoBarPanel.SetVerticalOrientationMargin(second, new Thickness(0, 4, 0, 0));

            var panel = new InfoBarPanel();
            panel.Children.Add(first);
            panel.Children.Add(second);

            return (panel, first, second);
        }

        private static void Layout(UIElement panel, double width)
        {
            panel.Measure(new Size(width, double.PositiveInfinity));
            panel.Arrange(new Rect(0, 0, width, panel.DesiredSize.Height));
        }

        [Test]
        public void TwoItemsThatFitStandSideBySide()
        {
            var (panel, _, second) = TwoBoxes();

            Layout(panel, 300);

            Assert.That(VisualTreeHelper.GetOffset(second), Is.EqualTo(new Vector(62, 0)));
        }

        [Test]
        public void TheLastItemTakesTheRestOfTheLine()
        {
            var (panel, _, second) = TwoBoxes();
            second.Width = double.NaN;
            second.MinWidth = 60;
            second.HorizontalAlignment = HorizontalAlignment.Right;

            Layout(panel, 300);

            Assert.That(second.ActualWidth + VisualTreeHelper.GetOffset(second).X, Is.EqualTo(300).Within(0.5));
        }

        [Test]
        public void TwoItemsThatDoNotFitStandOneUnderTheOther()
        {
            var (panel, _, second) = TwoBoxes();

            Layout(panel, 100);

            Assert.That(VisualTreeHelper.GetOffset(second), Is.EqualTo(new Vector(0, 24)));
        }

        [Test]
        public void OneItemAloneStandsInTheVerticalLayout()
        {
            var lone = new Border { Width = 50, Height = 20 };
            var panel = new InfoBarPanel { VerticalOrientationPadding = new Thickness(0, 14, 0, 18) };
            panel.Children.Add(lone);

            Layout(panel, 300);

            Assert.That(panel.DesiredSize.Height, Is.EqualTo(52));
        }

        [Test]
        public void AnItemOfNoSizeDoesNotCount()
        {
            var (panel, first, second) = TwoBoxes();
            first.Width = 0;

            Layout(panel, 300);

            Assert.That(VisualTreeHelper.GetOffset(second).X, Is.EqualTo(0), "the first item with a size gets no leading margin");
        }
    }
}
