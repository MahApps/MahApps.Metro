// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The tool tip of the two Windows sets, read off the ToolTip style of the UWP generic.xaml for
    /// the Windows 10 look and off ToolTip_themeresources.xaml of microsoft-ui-xaml for the WinUI one.
    /// Both write at 12, the ToolTipContentThemeFontSize, which is the Caption step of the ramp, and
    /// both wrap their text at 320.
    /// </summary>
    [TestFixture]
    public class ToolTipSetStyleTests : WindowTestFixture<TestWindow>
    {
        // the run of digits is longer than a line, so the text fills the tip to the width it wraps at
        private const string LongText = "42 is the answer, and the question takes a good deal longer to ask than the answer takes to give: 424242424242424242424242424242424242424242424242424242424242424242424242424242";

        [TestCase("MahApps.Styles.ToolTip.Win10", 8, 5, 8, 7, 0, "Segoe UI")]
        [TestCase("MahApps.Styles.ToolTip.WinUI", 9, 6, 9, 8, 4, "Segoe UI Variable Small, Segoe UI")]
        [Description("Padding, width, corners and font are the ones Microsoft gives its tool tip.")]
        public void TheToolTipIsTheOneOfMicrosoft(string key, double left, double top, double right, double bottom, double corner, string family)
        {
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key) };

            Assert.Multiple(() =>
                {
                    Assert.That(tip.Padding, Is.EqualTo(new Thickness(left, top, right, bottom)), "the padding");
                    Assert.That(tip.BorderThickness, Is.EqualTo(new Thickness(1)), "the frame");
                    Assert.That(ControlsHelper.GetCornerRadius(tip), Is.EqualTo(new CornerRadius(corner)), "the corners");
                    Assert.That(tip.FontSize, Is.EqualTo(12), "the size");
                    Assert.That(tip.FontFamily.Source, Is.EqualTo(family), "the family");
                });
        }

        [TestCase("MahApps.Styles.ToolTip.Win10", "MahApps.Colors.SystemChromeMediumLow", "MahApps.Brushes.SystemControlTransientBorder")]
        [TestCase("MahApps.Styles.ToolTip.WinUI", "MahApps.Colors.WinUI.AcrylicInAppFillDefault", "MahApps.Brushes.WinUI.SurfaceStrokeFlyout")]
        [Description("The Windows 10 tool tip stands on the chrome of its flyouts, the WinUI one on the colour its acrylic falls back to.")]
        public void TheToolTipWearsTheColoursOfItsLook(string key, string backgroundKey, string borderKey)
        {
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key) };

            Assert.Multiple(() =>
                {
                    Assert.That(((SolidColorBrush)tip.Background).Color, Is.EqualTo((Color)Application.Current.FindResource(backgroundKey)), "the fill");
                    Assert.That(tip.BorderBrush, Is.SameAs(Application.Current.FindResource(borderKey)), "the frame");
                });
        }

        [TestCase("Light.Blue", "#FFF9F9F9")]
        [TestCase("Dark.Blue", "#FF2C2C2C")]
        [Description("WPF has no acrylic, so the WinUI tool tip takes the FallbackColor of AcrylicInAppFillColorDefault, the colour Windows shows itself where acrylic is off.")]
        public void TheAcrylicFallsBackToTheColourOfMicrosoft(string theme, string expected)
        {
            var dictionary = new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{theme}.xaml", UriKind.Absolute) };

            Assert.That(dictionary["MahApps.Colors.WinUI.AcrylicInAppFillDefault"], Is.EqualTo((Color)ColorConverter.ConvertFromString(expected)));
        }

        [TestCase("MahApps.Styles.ToolTip.Win10", false)]
        [TestCase("MahApps.Styles.ToolTip.WinUI", true)]
        [TestCase("MahApps.Styles.ToolTip.WinUI", false)]
        [Description("A long text breaks into lines rather than running off, as it does in the tool tip of Windows, and the box is 320 wide whether a shadow takes room around it or not.")]
        public void ALongTextWraps(string key, bool shadow)
        {
            if (shadow)
            {
                Assume.That(SystemParameters.DropShadow, Is.True, "this desktop draws no shadows");
            }

            var target = new Button { Content = "42" };
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key), Content = LongText, PlacementTarget = target, HasDropShadow = shadow };
            target.ToolTip = tip;
            this.window!.Content = target;
            this.window.UpdateLayout();

            tip.IsOpen = true;
            try
            {
                this.window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
                tip.UpdateLayout();

                var text = Descendants<TextBlock>(tip).Single(t => t.Text == LongText);
                var box = tip.FindChild<Border>("Root")!;

                Assert.Multiple(() =>
                    {
                        Assert.That(text.TextWrapping, Is.EqualTo(TextWrapping.Wrap), "the text wraps");
                        // what can be seen of the tip, without the room its shadow takes
                        Assert.That(box.ActualWidth, Is.EqualTo(320).Within(8), "at the width of Microsoft, give or take the digit that no longer fits");
                    });
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        [Test]
        [Description("The WinUI tool tip casts the shadow of a WinUI flyout, the Windows 10 one casts none, as its UWP template has none.")]
        public void OnlyTheWinUIToolTipCastsAShadow()
        {
            Assume.That(SystemParameters.DropShadow, Is.True, "this desktop draws no shadows");

            var win10 = this.Open("MahApps.Styles.ToolTip.Win10");
            var winUI = this.Open("MahApps.Styles.ToolTip.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(win10, Is.Null, "Windows 10");
                    Assert.That(winUI, Is.InstanceOf<DropShadowEffect>(), "WinUI");
                    Assert.That(((DropShadowEffect)winUI!).Direction, Is.EqualTo(270), "falling straight down like the one of a flyout");
                });
        }

        [TestCase("MahApps.Styles.ToolTip.Win10")]
        [TestCase("MahApps.Styles.ToolTip.WinUI")]
        [Description("Windows shows its tool tip at once, so neither look fades it in the way the Metro one does.")]
        public void TheToolTipShowsAtOnce(string key)
        {
            var target = new Button { Content = "42" };
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key), Content = "42", PlacementTarget = target };
            target.ToolTip = tip;
            this.window!.Content = target;
            this.window.UpdateLayout();

            tip.IsOpen = true;
            try
            {
                tip.UpdateLayout();
                var box = tip.FindChild<Border>("Root")!;

                Assert.Multiple(() =>
                    {
                        Assert.That(box.Opacity, Is.EqualTo(1), "seen from the moment it opens");
                        Assert.That(VisualStateManager.GetVisualStateGroups(box), Is.Empty, "with no states to fade through");
                    });
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        [TestCase("MahApps.Styles.ToolTip.Win10")]
        [TestCase("MahApps.Styles.ToolTip.WinUI")]
        [Description("What is in the tip is clipped to the corners of its border, like everything else that is rounded in the library.")]
        public void TheToolTipClipsItsContentToItsBorder(string key)
        {
            var target = new Button { Content = "42" };
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key), Content = "42", PlacementTarget = target, Width = 120, Height = 60 };
            ControlsHelper.SetCornerRadius(tip, new CornerRadius(12));
            target.ToolTip = tip;
            this.window!.Content = target;
            this.window.UpdateLayout();

            tip.IsOpen = true;
            try
            {
                ClipAssert.Pump();
                Assume.That(tip.IsOpen, Is.True, "the tool tip did not stay open");

                var grid = ClipAssert.ContentGrid(tip);
                ClipAssert.CoversElement(grid, key);
                ClipAssert.CutsEveryCorner(grid, key);
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        [TestCase("MahApps.Styles.ToolTip")]
        [TestCase("MahApps.Styles.ToolTip.Win10")]
        [TestCase("MahApps.Styles.ToolTip.WinUI")]
        [Description("Scrolling takes what the tip explains away from under the pointer, and Windows closes the tip then rather than leaving it where it was, so all three looks do.")]
        public void TheToolTipClosesWhenWhatItExplainsIsScrolled(string key)
        {
            var (tip, scroller, _) = this.OpenInScroller(key);
            try
            {
                scroller.ScrollToVerticalOffset(50);
                this.window!.UpdateLayout();
                ClipAssert.Pump();

                Assert.That(tip.IsOpen, Is.False);
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        [Test]
        [Description("Scrolling something the control does not sit in leaves the tip alone.")]
        public void ScrollingSomethingElseLeavesTheToolTipOpen()
        {
            var (tip, _, other) = this.OpenInScroller("MahApps.Styles.ToolTip.WinUI");
            try
            {
                other.ScrollToVerticalOffset(50);
                this.window!.UpdateLayout();
                ClipAssert.Pump();

                Assert.That(tip.IsOpen, Is.True);
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        private (ToolTip Tip, ScrollViewer Scroller, ScrollViewer Other) OpenInScroller(string key)
        {
            var target = new Button { Content = "42" };
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key), Content = "42", PlacementTarget = target };
            target.ToolTip = tip;
            var scroller = new ScrollViewer { Height = 100, Content = new StackPanel { Height = 400, Children = { target } } };
            var other = new ScrollViewer { Height = 100, Content = new Border { Height = 400 } };
            this.window!.Content = new StackPanel { Children = { scroller, other } };
            this.window.UpdateLayout();

            tip.IsOpen = true;
            ClipAssert.Pump();
            Assume.That(tip.IsOpen, Is.True, "the tool tip did not stay open");

            return (tip, scroller, other);
        }

        private Effect? Open(string key)
        {
            var target = new Button { Content = "42" };
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key), Content = "42", PlacementTarget = target };
            target.ToolTip = tip;
            this.window!.Content = target;
            this.window.UpdateLayout();

            tip.IsOpen = true;
            try
            {
                this.window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
                tip.UpdateLayout();
                return tip.FindChild<Border>("Root")?.Effect;
            }
            finally
            {
                tip.IsOpen = false;
            }
        }

        private static IEnumerable<T> Descendants<T>(DependencyObject root)
            where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var descendant in Descendants<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
