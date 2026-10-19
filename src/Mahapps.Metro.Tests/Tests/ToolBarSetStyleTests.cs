// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The ToolBar in the Windows 10 and the WinUI look, read off the CommandBar of UWP and of
    /// microsoft-ui-xaml (WinUI 3). GH-2045 asked for the command strip; WPF has the ToolBar for it,
    /// so these are styles for that one rather than a control of their own.
    /// </summary>
    [TestFixture]
    public class ToolBarSetStyleTests : WindowTestFixture<TestWindow>
    {
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(Button), "MahApps.Styles.Button.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(ToggleButton), "MahApps.Styles.ToggleButton.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(CheckBox), "MahApps.Styles.ToggleButton.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(RadioButton), "MahApps.Styles.ToggleButton.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(Separator), "MahApps.Styles.Separator.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(TextBox), "MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.ToolBar.Win10", typeof(ComboBox), "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(Button), "MahApps.Styles.Button.ToolBar.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(ToggleButton), "MahApps.Styles.ToggleButton.ToolBar.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(CheckBox), "MahApps.Styles.ToggleButton.ToolBar.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(RadioButton), "MahApps.Styles.ToggleButton.ToolBar.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(Separator), "MahApps.Styles.Separator.ToolBar.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(TextBox), "MahApps.Styles.TextBox.WinUI")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", typeof(ComboBox), "MahApps.Styles.ComboBox.WinUI")]
        [Description("The bar's own style hands its items the look of its set, so a Windows 10 bar in a window of the default set still has Windows 10 buttons.")]
        public void TheBarHandsItsItemsTheLookOfItsSet(string barKey, Type itemType, string expectedKey)
        {
            var item = (FrameworkElement)Activator.CreateInstance(itemType)!;
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource(barKey) };
            toolBar.Items.Add(item);
            this.window.Content = toolBar;
            this.window.UpdateLayout();

            Assert.That(StandsOn(item.Style, (Style)this.window.FindResource(expectedKey)), Is.True, $"the {itemType.Name} in the bar should stand on {expectedKey}");
        }

        [TestCase("MahApps.Styles.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.WinUI")]
        [Description("The three dots are only there while something has gone into the drop-down, as with the CommandBar.")]
        public void TheDotsShowOnlyWhenSomethingOverflows(string barKey)
        {
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource(barKey), HorizontalAlignment = HorizontalAlignment.Left };
            toolBar.Items.Add(new Button { Content = "Save" });
            this.window.Content = toolBar;
            this.window.UpdateLayout();

            var dots = toolBar.FindChild<ToggleButton>("OverflowButton")!;
            var withoutOverflow = dots.Visibility;

            toolBar.Width = 100;
            toolBar.Items.Add(new Button { Content = "Open the folder" });
            toolBar.Items.Add(new Button { Content = "Print the page" });
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(withoutOverflow, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(toolBar.HasOverflowItems, Is.True);
                    Assert.That(dots.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [TestCase("MahApps.Styles.ToolBar.Win10")]
        [TestCase("MahApps.Styles.ToolBar.WinUI")]
        [Description("In the drop-down the commands stand one under the other, each a line as wide as the widest, so the highlight under the pointer is the same width on every line. The panel WPF has there would put short ones side by side.")]
        public void TheDropDownIsAListOfEqualLines(string barKey)
        {
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource(barKey), HorizontalAlignment = HorizontalAlignment.Left };
            var buttons = new[] { "Cut", "Copy", "Paste with the formatting of the target" }
                          .Select(text => new Button { Content = text })
                          .ToList();
            foreach (var button in buttons)
            {
                ToolBar.SetOverflowMode(button, OverflowMode.Always);
                toolBar.Items.Add(button);
            }

            this.window.Content = toolBar;
            this.window.UpdateLayout();

            toolBar.IsOverflowOpen = true;
            try
            {
                ClipAssert.Pump();

                // the drop-down is a popup of its own, so the panel is found from an item rather than from the bar
                var panel = (Panel)VisualTreeHelper.GetParent(buttons[0]);
                var tops = buttons.Select(b => b.TranslatePoint(default, panel).Y).ToList();

                Assert.Multiple(() =>
                    {
                        Assert.That(panel.ActualWidth, Is.GreaterThanOrEqualTo(150));
                        Assert.That(tops, Is.Ordered.Ascending.And.Unique, "every command on a line of its own");
                        foreach (var button in buttons)
                        {
                            Assert.That(button.ActualWidth, Is.EqualTo(panel.ActualWidth).Within(0.5), $"the line of {button.Content}");
                            Assert.That(button.HorizontalContentAlignment, Is.EqualTo(HorizontalAlignment.Left), $"the line of {button.Content}");
                        }
                    });
            }
            finally
            {
                toolBar.IsOverflowOpen = false;
            }
        }

        [TestCase("MahApps.Styles.ToolBar.Win10", "MahApps.Brushes.ToolBar.Win10.ToggleButtonBackgroundChecked", "MahApps.Brushes.ToolBar.Win10.ToggleButtonForegroundChecked")]
        [TestCase("MahApps.Styles.ToolBar.WinUI", "MahApps.Brushes.ToolBar.WinUI.ToggleButtonBackgroundChecked", "MahApps.Brushes.ToolBar.WinUI.ToggleButtonForegroundChecked")]
        [Description("A checked button of the bar takes the accent, as the AppBarToggleButton does.")]
        public void ACheckedButtonTakesTheAccent(string barKey, string backgroundKey, string foregroundKey)
        {
            var toggle = new ToggleButton { Content = "Bold", IsChecked = true };
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource(barKey) };
            toolBar.Items.Add(toggle);
            this.window.Content = toolBar;
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(toggle.Background, Is.SameAs(this.window.FindResource(backgroundKey)));
                    Assert.That(toggle.Foreground, Is.SameAs(this.window.FindResource(foregroundKey)));
                });
        }

        [TestCase("MahApps.Styles.ToolBar.Win10", 40d)]
        [TestCase("MahApps.Styles.ToolBar.WinUI", 48d)]
        [Description("A bar of buttons is as high as the compact CommandBar of its set: 40 in UWP, 48 in WinUI, where the button stands six units in from either edge.")]
        public void TheBarIsAsHighAsTheCompactCommandBar(string barKey, double height)
        {
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource(barKey), VerticalAlignment = VerticalAlignment.Top };
            ToolBarTray.SetIsLocked(toolBar, true);
            toolBar.Items.Add(new Button { Content = "Save" });
            this.window.Content = toolBar;
            this.window.UpdateLayout();

            Assert.That(toolBar.ActualHeight, Is.EqualTo(height));
        }

        [Test]
        [Description("While the drop-down is open the WinUI bar takes its colour and a frame, as the CommandBar does, so the two read as one surface; closed, it is transparent again.")]
        public void TheOpenWinUIBarTakesTheColourOfItsDropDown()
        {
            var toolBar = new ToolBar { Style = (Style)this.window!.FindResource("MahApps.Styles.ToolBar.WinUI"), HorizontalAlignment = HorizontalAlignment.Left };
            var share = new Button { Content = "Share" };
            ToolBar.SetOverflowMode(share, OverflowMode.Always);
            toolBar.Items.Add(new Button { Content = "Add" });
            toolBar.Items.Add(share);
            this.window.Content = toolBar;
            this.window.UpdateLayout();

            var border = toolBar.FindChild<Border>("Border")!;
            var frame = toolBar.FindChild<Border>("OpenBorder")!;
            var closedBackground = border.Background;
            var closedFrame = frame.Visibility;
            var sizeClosed = toolBar.FindChild<ToolBarPanel>("PART_ToolBarPanel")!.TranslatePoint(default, toolBar);

            toolBar.IsOverflowOpen = true;
            try
            {
                ClipAssert.Pump();

                Assert.Multiple(() =>
                    {
                        Assert.That(closedBackground, Is.SameAs(this.window.FindResource("MahApps.Brushes.ToolBar.WinUI.Background")));
                        Assert.That(closedFrame, Is.EqualTo(Visibility.Collapsed));
                        Assert.That(border.Background, Is.SameAs(this.window.FindResource("MahApps.Brushes.ToolBar.WinUI.BackgroundOpen")));
                        Assert.That(frame.Visibility, Is.EqualTo(Visibility.Visible));
                        Assert.That(toolBar.FindChild<ToolBarPanel>("PART_ToolBarPanel")!.TranslatePoint(default, toolBar), Is.EqualTo(sizeClosed), "the frame lies over the bar and moves nothing");
                    });
            }
            finally
            {
                toolBar.IsOverflowOpen = false;
            }
        }

        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.Background", "#FFE6E6E6")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.Win10.Background", "#FF1F1F1F")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonBackgroundPointerOver", "#19000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonBackgroundPointerOver", "#19FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonBackgroundPressed", "#33000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonBackgroundPressed", "#33FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonForeground", "#FF000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonForeground", "#FFFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.ButtonForegroundDisabled", "#66000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.ToggleButtonBackgroundChecked", "#FF0078D7")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.Win10.ToggleButtonForegroundChecked", "#FFFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.Win10.Separator", "#66000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonBackgroundPointerOver", "#09000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonBackgroundPointerOver", "#0FFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonBackgroundPressed", "#06000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonBackgroundPressed", "#0AFFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonForeground", "#E4000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonForegroundPressed", "#C5FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.ButtonForegroundDisabled", "#5C000000")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.ToggleButtonBackgroundChecked", "#FF0067C3")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.ToggleButtonBackgroundChecked", "#FF4DC1FF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.Separator", "#0F000000")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.Separator", "#15FFFFFF")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.BackgroundOpen", "#FFF9F9F9")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.BackgroundOpen", "#FF2C2C2C")]
        [TestCase("Light.Blue", "MahApps.Brushes.ToolBar.WinUI.BorderBrushOpen", "#FFEBEBEB")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.BorderBrushOpen", "#FF1C1C1C")]
        [TestCase("Dark.Blue", "MahApps.Brushes.ToolBar.WinUI.OverflowBackground", "#FF2C2C2C")]
        public void TheColoursAreMicrosofts(string theme, string key, string expected)
        {
            ThemeManager.Current.ChangeTheme(this.window!, theme);

            try
            {
                var brush = (SolidColorBrush)this.window!.FindResource(key);

                Assert.That(brush.Color, Is.EqualTo((Color)ColorConverter.ConvertFromString(expected)));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }

        private static bool StandsOn(Style? style, Style expected)
        {
            for (var current = style; current is not null; current = current.BasedOn)
            {
                if (ReferenceEquals(current, expected))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
