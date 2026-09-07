// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Every button style that rounds its corners clips its content to them. A clip lives in the
    /// coordinates of the element carrying it, so the geometry has to match that element and not the
    /// border around it.
    /// </summary>
    [TestFixture]
    public class ButtonClipTests
    {
        private static readonly object[] ClippedStyles =
            {
                new object[] { "MahApps.Styles.Button", false },
                new object[] { "MahApps.Styles.Button.Flat", false },
                new object[] { "MahApps.Styles.Button.Win10", false },
                new object[] { "MahApps.Styles.Button.Square", false },
                new object[] { "MahApps.Styles.Button.Square.Accent", false },
                new object[] { "MahApps.Styles.Button.Square.Highlight", false },
                new object[] { "MahApps.Styles.Button.DropDown", false },
                new object[] { "MahApps.Styles.ToggleButton", true },
                new object[] { "MahApps.Styles.ToggleButton.Flat", true }
            };

        private TestWindow? window;
        private ResourceDictionary? dictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.Buttons.xaml", UriKind.Absolute) };
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        private ContentControl ShowButton(string styleKey, bool isToggleButton)
        {
            Assert.That(this.window, Is.Not.Null);

            ContentControl button = isToggleButton ? new ToggleButton() : new Button();
            button.Width = 200;
            button.Height = 48;
            button.Content = "Beam me up...";
            button.SetValue(FrameworkElement.StyleProperty, this.dictionary![styleKey]);
            ControlsHelper.SetCornerRadius(button, new CornerRadius(12));

            this.window!.Content = button;
            this.window.UpdateLayout();
            button.UpdateLayout();

            return button;
        }

        [TestCaseSource(nameof(ClippedStyles))]
        public void TheClipShouldCoverTheGridItSitsOn(string styleKey, bool isToggleButton)
        {
            var button = this.ShowButton(styleKey, isToggleButton);

            var grid = button.FindChild<Grid>("ContentGrid");
            Assert.That(grid, Is.Not.Null, "the template should carry the grid the clip sits on");
            Assert.That(grid!.ActualWidth, Is.GreaterThan(0), "the button should be laid out, otherwise this test proves nothing");
            Assert.That(grid.Clip, Is.Not.Null, "the content should be clipped");

            Assert.That(grid.Clip!.Bounds.Width, Is.EqualTo(grid.ActualWidth).Within(0.001), "a wider clip leaves the content unclipped on the right");
            Assert.That(grid.Clip.Bounds.Height, Is.EqualTo(grid.ActualHeight).Within(0.001), "a taller clip leaves it unclipped at the bottom");
            Assert.That(grid.Clip.Bounds.X, Is.EqualTo(0).Within(0.001));
            Assert.That(grid.Clip.Bounds.Y, Is.EqualTo(0).Within(0.001));
        }

        [TestCaseSource(nameof(ClippedStyles))]
        public void TheClipShouldCutEveryCorner(string styleKey, bool isToggleButton)
        {
            var button = this.ShowButton(styleKey, isToggleButton);

            var grid = button.FindChild<Grid>("ContentGrid");
            Assert.That(grid, Is.Not.Null);

            var clip = grid!.Clip;
            Assert.That(clip, Is.Not.Null);

            var width = grid.ActualWidth;
            var height = grid.ActualHeight;

            Assert.That(clip!.FillContains(new Point(width / 2, height / 2)), Is.True, "the middle belongs to the clip");
            Assert.That(clip.FillContains(new Point(1, 1)), Is.False, "top left should be cut");
            Assert.That(clip.FillContains(new Point(width - 1, 1)), Is.False, "top right should be cut");
            Assert.That(clip.FillContains(new Point(width - 1, height - 1)), Is.False, "bottom right should be cut");
            Assert.That(clip.FillContains(new Point(1, height - 1)), Is.False, "bottom left should be cut");
        }

        [TestCaseSource(nameof(ClippedStyles))]
        public void WithoutACornerRadiusTheClipShouldKeepTheWholeRectangle(string styleKey, bool isToggleButton)
        {
            var button = this.ShowButton(styleKey, isToggleButton);

            // Some of these styles carry a corner radius of their own, so clearing the local value is
            // not the same as asking for square corners.
            ControlsHelper.SetCornerRadius(button, new CornerRadius(0));
            button.UpdateLayout();

            var grid = button.FindChild<Grid>("ContentGrid");
            Assert.That(grid, Is.Not.Null);
            Assert.That(grid!.Clip, Is.Not.Null);

            Assert.That(grid.Clip!.FillContains(new Point(0.5, 0.5)), Is.True, "square corners belong to the clip");
            Assert.That(grid.Clip.FillContains(new Point(grid.ActualWidth - 0.5, grid.ActualHeight - 0.5)), Is.True);
        }
    }
}
