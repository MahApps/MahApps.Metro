// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MetroWindowInfoBarsTests : WindowTestFixture<TestWindow>
    {
        [TearDown]
        public void Reset()
        {
            this.window!.ClearValue(MetroWindow.InfoBarsProperty);
            this.window.ClearValue(MetroWindow.TitleAlignmentProperty);
            this.window.UpdateLayout();
        }

        [Test]
        public void TheBarsBelongToTheWindowAndShareItsDataContext()
        {
            this.window!.DataContext = 42;
            var host = new InfoBarsControl();

            this.window.InfoBars = host;

            Assert.Multiple(() =>
                {
                    Assert.That(LogicalTreeHelper.GetParent(host), Is.SameAs(this.window));
                    Assert.That(host.DataContext, Is.EqualTo(42));
                });

            this.window.ClearValue(FrameworkElement.DataContextProperty);
        }

        [Test]
        public void TheBarsFollowADataContextSetAfterThem()
        {
            var host = new InfoBarsControl();
            this.window!.InfoBars = host;

            this.window.DataContext = 42;

            try
            {
                Assert.That(host.DataContext, Is.EqualTo(42), "the usual order is InitializeComponent first and the view model after it");
            }
            finally
            {
                this.window.ClearValue(FrameworkElement.DataContextProperty);
            }
        }

        [TestCase(HorizontalAlignment.Left)]
        [TestCase(HorizontalAlignment.Center)]
        public void WithoutATitleBarTheBarsStayClearOfTheWindowButtons(HorizontalAlignment titleAlignment)
        {
            this.window!.TitleAlignment = titleAlignment;
            this.window.ShowTitleBar = false;
            this.window.InfoBars = new InfoBarsControl();
            this.window.UpdateLayout();

            try
            {
                var infoBars = this.window.FindChild<ContentControl>("PART_InfoBars")!;
                var buttons = this.window.FindChild<FrameworkElement>("PART_WindowButtonCommands")!;

                Assert.That(buttons.ActualHeight, Is.GreaterThan(0), "the window buttons should be there");
                Assert.That(infoBars.TranslatePoint(new Point(), buttons).Y, Is.GreaterThanOrEqualTo(buttons.ActualHeight), "a click on the close button of a bar must not close the window");
            }
            finally
            {
                this.window.ClearValue(MetroWindow.ShowTitleBarProperty);
            }
        }

        [Test]
        public void TheBarsShowInTheirPlace()
        {
            var host = new InfoBarsControl();
            this.window!.InfoBars = host;
            this.window.UpdateLayout();

            Assert.That(this.window.FindChild<ContentControl>("PART_InfoBars")!.Content, Is.SameAs(host));
        }

        [TestCase(HorizontalAlignment.Left)]
        [TestCase(HorizontalAlignment.Center)]
        public void TheBarsLieOverTheContentAndUnderEverythingElse(HorizontalAlignment titleAlignment)
        {
            this.window!.TitleAlignment = HorizontalAlignment.Left;
            this.window.UpdateLayout();
            var leftTemplate = this.window.Template;

            this.window.TitleAlignment = titleAlignment;
            this.window.UpdateLayout();

            var content = this.window.FindChild<FrameworkElement>("PART_Content")!;
            var infoBars = this.window.FindChild<FrameworkElement>("PART_InfoBarsArea")!;
            var modal = this.window.FindChild<Rectangle>("PART_FlyoutModal")!;
            var flyouts = this.window.FindChild<ContentControl>("PART_Flyouts")!;
            var grid = (Panel)VisualTreeHelper.GetParent(infoBars);

            Assert.Multiple(() =>
                {
                    Assert.That(this.window.Template, titleAlignment == HorizontalAlignment.Center ? Is.Not.SameAs(leftTemplate) : Is.SameAs(leftTemplate), "each case should look at its own template");
                    Assert.That(VisualTreeHelper.GetParent(content), Is.SameAs(grid));
                    Assert.That(VisualTreeHelper.GetParent(modal), Is.SameAs(grid));
                    Assert.That(grid.Children.IndexOf(infoBars), Is.EqualTo(grid.Children.IndexOf(content) + 1), "right after the content");
                    Assert.That(grid.Children.IndexOf(infoBars), Is.LessThan(grid.Children.IndexOf(modal)), "under the cover of a modal flyout");
                    Assert.That(grid.Children.IndexOf(infoBars), Is.LessThan(grid.Children.IndexOf(flyouts)));
                    Assert.That(infoBars.ReadLocalValue(Panel.ZIndexProperty), Is.EqualTo(DependencyProperty.UnsetValue), "a ZIndex would lift it over the cover");
                    Assert.That(Grid.GetRow(infoBars), Is.EqualTo(1), "under the title bar");
                    Assert.That(infoBars.VerticalAlignment, Is.EqualTo(VerticalAlignment.Top));
                });
        }
    }
}
