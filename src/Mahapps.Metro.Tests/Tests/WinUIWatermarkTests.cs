// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Every control of the WinUI set that can be typed into, or that stands empty until something
    /// is picked, writes its watermark the way the WinUI text box does: in the placeholder colours of
    /// that set and at full strength, a step quieter once the caret is in it. What these look at is
    /// the watermark that is on the screen, wherever the control draws it, its own or the one of the
    /// text box inside it.
    /// </summary>
    [TestFixture]
    public class WinUIWatermarkTests : WindowTestFixture<TestWindow>
    {
        private const string Watermark = "Where to";

        private static IEnumerable<TestCaseData> Controls()
        {
            yield return new TestCaseData("TextBox", false).SetArgDisplayNames("TextBox");
            yield return new TestCaseData("PasswordBox", false).SetArgDisplayNames("PasswordBox");
            yield return new TestCaseData("RichTextBox", false).SetArgDisplayNames("RichTextBox");
            yield return new TestCaseData("NumericUpDown", false).SetArgDisplayNames("NumericUpDown");
            yield return new TestCaseData("HotKeyBox", false).SetArgDisplayNames("HotKeyBox");
            yield return new TestCaseData("ComboBox", false).SetArgDisplayNames("ComboBox");
            yield return new TestCaseData("ComboBox", true).SetArgDisplayNames("ComboBox, editable");
            yield return new TestCaseData("AutoSuggestBox", true).SetArgDisplayNames("AutoSuggestBox");
            yield return new TestCaseData("MultiSelectionComboBox", false).SetArgDisplayNames("MultiSelectionComboBox");
            yield return new TestCaseData("MultiSelectionComboBox", true).SetArgDisplayNames("MultiSelectionComboBox, editable");
            yield return new TestCaseData("ColorPicker", false).SetArgDisplayNames("ColorPicker");
            yield return new TestCaseData("DatePicker", false).SetArgDisplayNames("DatePicker");
            yield return new TestCaseData("DateTimePicker", false).SetArgDisplayNames("DateTimePicker");
            yield return new TestCaseData("TimePicker", false).SetArgDisplayNames("TimePicker");
        }

        [TestCaseSource(nameof(Controls))]
        [Description("Untouched, the watermark is in the placeholder colour of the set and nothing is taken away from it on top.")]
        public void AnIdleControlWritesItsWatermarkInThePlaceholderColour(string control, bool editable)
        {
            var message = this.WatermarkOf(this.Show(control, editable));

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(message.Foreground), Is.EqualTo(Brush("MahApps.Brushes.TextControl.WinUI.PlaceholderForeground")));
                    Assert.That(message.Opacity, Is.EqualTo(1).Within(0.001), "at full strength");
                });
        }

        [TestCaseSource(nameof(Controls))]
        [Description("With the caret in it the watermark takes the quieter colour, and that colour is all that changes: it is not faded on top, the way the Metro look fades it.")]
        public void WithTheCaretTheWatermarkTakesTheQuieterColour(string control, bool editable)
        {
            var host = this.Show(control, editable);
            var message = this.WatermarkOf(host);

            var owner = (UIElement)message.TemplatedParent;
            owner.Focus();
            Keyboard.Focus(owner);
            ClipAssert.Pump();

            // a build agent hands the keyboard to one window at a time
            Assume.That(host.IsKeyboardFocusWithin, Is.True);

            // long enough for any fade to have run
            ClipAssert.Pump(400);

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(message.Foreground), Is.EqualTo(Brush("MahApps.Brushes.TextControl.WinUI.PlaceholderForegroundFocused")));
                    Assert.That(message.Opacity, Is.EqualTo(1).Within(0.001), "and not faded");
                });
        }

        [TestCaseSource(nameof(Controls))]
        [Description("A control that is switched off writes its watermark in the colour the set switches it off with.")]
        public void ASwitchedOffControlDimsItsWatermark(string control, bool editable)
        {
            var host = this.Show(control, editable);
            host.IsEnabled = false;
            ClipAssert.Pump();

            Assert.That(ColourOf(this.WatermarkOf(host).Foreground), Is.EqualTo(Brush("MahApps.Brushes.TextControl.WinUI.PlaceholderForegroundDisabled")));
        }

        [TestCase(false)]
        [TestCase(true)]
        [Description("The Metro combo box keeps its watermark: the foreground with some of it taken away, and fading further with the caret in it.")]
        public void TheMetroWatermarkStaysAsItWas(bool editable)
        {
            var box = new ComboBox { Style = (Style)Application.Current.FindResource("MahApps.Styles.ComboBox"), IsEditable = editable, Width = 280 };
            TextBoxHelper.SetWatermark(box, Watermark);
            box.Items.Add("Beam me up...");
            this.window!.Content = box;
            ClipAssert.Pump();

            var message = this.WatermarkOf(box);

            Assert.Multiple(() =>
                {
                    Assert.That(ColourOf(message.Foreground), Is.EqualTo(ColourOf(box.Foreground)), "in the foreground of the box");
                    Assert.That(message.Opacity, Is.EqualTo(0.6).Within(0.001), "with some of it taken away");
                    Assert.That(TextBoxHelper.GetFadeWatermarkOnFocus(box), Is.True, "and fading with the caret");
                });
        }

        private Control Show(string control, bool editable)
        {
            Assert.That(this.window, Is.Not.Null);

            Control host = control switch
                           {
                               "TextBox" => new TextBox(),
                               "PasswordBox" => new PasswordBox(),
                               "RichTextBox" => new RichTextBox(),
                               "NumericUpDown" => new NumericUpDown { Value = null },
                               "HotKeyBox" => new HotKeyBox(),
                               "ComboBox" => new ComboBox { IsEditable = editable, Items = { "Beam me up..." } },
                               "AutoSuggestBox" => new AutoSuggestBox(),
                               "MultiSelectionComboBox" => new MultiSelectionComboBox { IsEditable = editable, Items = { "Beam me up..." } },
                               "ColorPicker" => new ColorPicker { SelectedColor = null },
                               "DatePicker" => new DatePicker(),
                               "DateTimePicker" => new DateTimePicker(),
                               "TimePicker" => new TimePicker(),
                               _ => throw new System.ArgumentOutOfRangeException(nameof(control))
                           };

            host.Style = (Style)Application.Current.FindResource($"MahApps.Styles.{control}.WinUI");
            host.Width = 280;
            host.HorizontalAlignment = HorizontalAlignment.Left;
            host.VerticalAlignment = VerticalAlignment.Top;
            TextBoxHelper.SetWatermark(host, Watermark);

            this.window!.Content = host;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return host;
        }

        private TextBlock WatermarkOf(Control host)
        {
            var shown = Descendants<TextBlock>(host).Where(t => t.Text == Watermark && t.IsVisible).ToList();

            Assert.That(shown, Has.Count.EqualTo(1), "the control should show its watermark once");
            return shown[0];
        }

        private static Color Brush(string key)
        {
            return ColourOf(Application.Current.FindResource(key));
        }

        private static Color ColourOf(object? brush)
        {
            Assert.That(brush, Is.InstanceOf<SolidColorBrush>());
            return ((SolidColorBrush)brush!).Color;
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
