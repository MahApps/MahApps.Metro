// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A closed box of the two Windows sets that cannot be typed into opens its list with Enter and
    /// with the space bar, the way the ComboBox of UWP and WinUI does in its MainKeyDown. WPF opens it
    /// with F4 and Alt with an arrow only, which the Metro box keeps.
    /// </summary>
    [TestFixture]
    public class ComboBoxOpenWithEnterTests : WindowTestFixture<TestWindow>
    {
        [TestCase("MahApps.Styles.ComboBox.Win10", Key.Enter)]
        [TestCase("MahApps.Styles.ComboBox.Win10", Key.Space)]
        [TestCase("MahApps.Styles.ComboBox.WinUI", Key.Enter)]
        [TestCase("MahApps.Styles.ComboBox.WinUI", Key.Space)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", Key.Enter)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.Win10", Key.Space)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", Key.Enter)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", Key.Space)]
        [Description("Enter and the space bar open the list of a closed box of the Windows sets.")]
        public void TheKeyOpensTheList(string key, Key pressed)
        {
            var box = this.Show(key, false);

            var opened = Press(box, pressed);

            Assert.That(opened, Is.True);
        }

        [TestCase("MahApps.Styles.ComboBox", Key.Enter)]
        [TestCase("MahApps.Styles.ComboBox", Key.Space)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox", Key.Enter)]
        [TestCase("MahApps.Styles.MultiSelectionComboBox", Key.Space)]
        [Description("The Metro box keeps to what WPF does, where neither key opens the list.")]
        public void TheMetroBoxStaysShut(string key, Key pressed)
        {
            var box = this.Show(key, false);

            var opened = Press(box, pressed);

            Assert.That(opened, Is.False);
        }

        [TestCase("MahApps.Styles.ComboBox.Win10", Key.Enter)]
        [TestCase("MahApps.Styles.ComboBox.Win10", Key.Space)]
        [TestCase("MahApps.Styles.ComboBox.WinUI", Key.Enter)]
        [TestCase("MahApps.Styles.ComboBox.WinUI", Key.Space)]
        [Description("A box that can be typed into takes the space as a character and Enter as the end of what was typed, the way WinUI leaves it, so neither opens the list there.")]
        public void AnEditableBoxStaysShut(string key, Key pressed)
        {
            var box = this.Show(key, true);

            var opened = Press(box, pressed);

            Assert.That(opened, Is.False);
        }

        private ComboBox Show(string key, bool editable)
        {
            Assert.That(this.window, Is.Not.Null);

            ComboBox box = key.Contains("MultiSelection")
                ? new MultiSelectionComboBox { ItemsSource = new[] { "Beam me up...", "Warp nine" } }
                : new ComboBox { ItemsSource = new[] { "Beam me up...", "Warp nine" } };
            box.Style = (Style)Application.Current.FindResource(key);
            box.IsEditable = editable;
            box.Width = 280;
            box.VerticalAlignment = VerticalAlignment.Top;

            this.window!.Content = box;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return box;
        }

        /// <summary>
        /// Presses the key on the box and says whether that opened the list. The list is closed again
        /// before anything else runs: a popup holds itself open with the mouse capture and closes when
        /// its window loses activation, which a test cannot rely on either way.
        /// </summary>
        private static bool Press(ComboBox box, Key key)
        {
            var source = PresentationSource.FromVisual(box);
            Assert.That(source, Is.Not.Null, "the box should be on screen");

            var down = new KeyEventArgs(Keyboard.PrimaryDevice, source!, 0, key) { RoutedEvent = Keyboard.PreviewKeyDownEvent };
            box.RaiseEvent(down);
            if (!down.Handled)
            {
                box.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source!, 0, key) { RoutedEvent = Keyboard.KeyDownEvent });
            }

            var opened = box.IsDropDownOpen;
            box.SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);

            return opened;
        }
    }
}
