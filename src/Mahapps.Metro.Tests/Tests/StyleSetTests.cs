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
    /// The library has three sets an application can merge: the default one, the Windows 10 one and
    /// the WinUI one. Each of the latter two stands on the one before it, so merging a set means
    /// getting every style the library has, with that set's own in front. GH-3326 asked for the
    /// Windows 10 look as the default; it is a set you can pick instead.
    /// </summary>
    [TestFixture]
    public class StyleSetTests
    {
        private const string Default = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
        private const string Win10 = "pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml";
        private const string WinUI = "pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml";

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

        [TestCase(Win10, typeof(Button), "MahApps.Styles.Button.Win10")]
        [TestCase(Win10, typeof(RepeatButton), "MahApps.Styles.Button.Win10")]
        [TestCase(Win10, typeof(CheckBox), "MahApps.Styles.CheckBox.Win10")]
        [TestCase(Win10, typeof(RadioButton), "MahApps.Styles.RadioButton.Win10")]
        [TestCase(Win10, typeof(ComboBox), "MahApps.Styles.ComboBox.Win10")]
        [TestCase(Win10, typeof(ComboBoxItem), "MahApps.Styles.ComboBoxItem.Win10")]
        [TestCase(Win10, typeof(AutoSuggestBox), "MahApps.Styles.AutoSuggestBox.Win10")]
        [TestCase(Win10, typeof(HotKeyBox), "MahApps.Styles.HotKeyBox.Win10")]
        [TestCase(Win10, typeof(MultiSelectionComboBox), "MahApps.Styles.MultiSelectionComboBox.Win10")]
        [TestCase(Win10, typeof(SplitButton), "MahApps.Styles.SplitButton.Win10")]
        [TestCase(Win10, typeof(Menu), "MahApps.Styles.Menu.Win10")]
        [TestCase(Win10, typeof(MenuItem), "MahApps.Styles.MenuItem.Win10")]
        [TestCase(Win10, typeof(ContextMenu), "MahApps.Styles.ContextMenu.Win10")]
        [TestCase(Win10, typeof(ColorPicker), "MahApps.Styles.ColorPicker.Win10")]
        [TestCase(Win10, typeof(ColorCanvas), "MahApps.Styles.ColorCanvas.Win10")]
        [TestCase(Win10, typeof(ColorPalette), "MahApps.Styles.ColorPalette.Win10")]
        [TestCase(Win10, typeof(ColorEyeDropper), "MahApps.Styles.ColorEyeDropper.Win10")]
        [TestCase(Win10, typeof(TextBox), "MahApps.Styles.TextBox.Win10")]
        [TestCase(Win10, typeof(PasswordBox), "MahApps.Styles.PasswordBox.Win10")]
        [TestCase(Win10, typeof(RichTextBox), "MahApps.Styles.RichTextBox.Win10")]
        [TestCase(Win10, typeof(NumericUpDown), "MahApps.Styles.NumericUpDown.Win10")]
        [TestCase(Win10, typeof(IntegerUpDown), "MahApps.Styles.NumericUpDown.Win10")]
        [TestCase(Win10, typeof(Calendar), "MahApps.Styles.Calendar.Win10")]
        [TestCase(Win10, typeof(DatePicker), "MahApps.Styles.DatePicker.Win10")]
        [TestCase(Win10, typeof(TimePicker), "MahApps.Styles.TimePicker.Win10")]
        [TestCase(Win10, typeof(DateTimePicker), "MahApps.Styles.DateTimePicker.Win10")]
        [TestCase(Win10, typeof(AnalogClock), "MahApps.Styles.AnalogClock.Win10")]
        [TestCase(WinUI, typeof(Button), "MahApps.Styles.Button.WinUI")]
        [TestCase(WinUI, typeof(RepeatButton), "MahApps.Styles.Button.WinUI")]
        [TestCase(WinUI, typeof(CheckBox), "MahApps.Styles.CheckBox.WinUI")]
        [TestCase(WinUI, typeof(RadioButton), "MahApps.Styles.RadioButton.WinUI")]
        [TestCase(WinUI, typeof(ComboBox), "MahApps.Styles.ComboBox.WinUI")]
        [TestCase(WinUI, typeof(ComboBoxItem), "MahApps.Styles.ComboBoxItem.WinUI")]
        [TestCase(WinUI, typeof(AutoSuggestBox), "MahApps.Styles.AutoSuggestBox.WinUI")]
        [TestCase(WinUI, typeof(HotKeyBox), "MahApps.Styles.HotKeyBox.WinUI")]
        [TestCase(WinUI, typeof(MultiSelectionComboBox), "MahApps.Styles.MultiSelectionComboBox.WinUI")]
        [TestCase(WinUI, typeof(SplitButton), "MahApps.Styles.SplitButton.WinUI")]
        [TestCase(WinUI, typeof(Menu), "MahApps.Styles.Menu.WinUI")]
        [TestCase(WinUI, typeof(MenuItem), "MahApps.Styles.MenuItem.WinUI")]
        [TestCase(WinUI, typeof(ContextMenu), "MahApps.Styles.ContextMenu.WinUI")]
        [TestCase(WinUI, typeof(ColorPicker), "MahApps.Styles.ColorPicker.WinUI")]
        [TestCase(WinUI, typeof(ColorCanvas), "MahApps.Styles.ColorCanvas.WinUI")]
        [TestCase(WinUI, typeof(ColorPalette), "MahApps.Styles.ColorPalette.WinUI")]
        [TestCase(WinUI, typeof(ColorEyeDropper), "MahApps.Styles.ColorEyeDropper.WinUI")]
        [TestCase(WinUI, typeof(TextBox), "MahApps.Styles.TextBox.WinUI")]
        [TestCase(WinUI, typeof(PasswordBox), "MahApps.Styles.PasswordBox.WinUI")]
        [TestCase(WinUI, typeof(RichTextBox), "MahApps.Styles.RichTextBox.WinUI")]
        [TestCase(WinUI, typeof(NumericUpDown), "MahApps.Styles.NumericUpDown.WinUI")]
        [TestCase(WinUI, typeof(IntegerUpDown), "MahApps.Styles.NumericUpDown.WinUI")]
        [TestCase(WinUI, typeof(Calendar), "MahApps.Styles.Calendar.WinUI")]
        [TestCase(WinUI, typeof(DatePicker), "MahApps.Styles.DatePicker.WinUI")]
        [TestCase(WinUI, typeof(TimePicker), "MahApps.Styles.TimePicker.WinUI")]
        [TestCase(WinUI, typeof(DateTimePicker), "MahApps.Styles.DateTimePicker.WinUI")]
        [TestCase(WinUI, typeof(AnalogClock), "MahApps.Styles.AnalogClock.WinUI")]
        [Description("A set puts its own style in front of the one the default set declares for that type.")]
        public void ASetPutsItsOwnStyleInFront(string set, Type target, string expected)
        {
            var dictionary = Load(set);

            var implicitStyle = dictionary[target] as Style;

            Assert.That(implicitStyle, Is.Not.Null, $"the set declares no implicit style for {target.Name}");
            Assert.That(implicitStyle!.BasedOn, Is.SameAs(dictionary[expected]), $"the implicit style for {target.Name} should stand on {expected}");
        }

        [TestCase("MahApps.Styles.Button.WinUI", typeof(Button))]
        [TestCase("MahApps.Styles.Button.Accent.WinUI", typeof(Button))]
        [TestCase("MahApps.Styles.TextBox.WinUI", typeof(TextBox))]
        [TestCase("MahApps.Styles.PasswordBox.WinUI", typeof(PasswordBox))]
        [TestCase("MahApps.Styles.RichTextBox.WinUI", typeof(RichTextBox))]
        [TestCase("MahApps.Styles.ComboBox.WinUI", typeof(ComboBox))]
        [TestCase("MahApps.Styles.ComboBoxItem.WinUI", typeof(ComboBoxItem))]
        [TestCase("MahApps.Styles.AutoSuggestBox.WinUI", typeof(AutoSuggestBox))]
        [TestCase("MahApps.Styles.HotKeyBox.WinUI", typeof(HotKeyBox))]
        [TestCase("MahApps.Styles.MultiSelectionComboBox.WinUI", typeof(MultiSelectionComboBox))]
        [TestCase("MahApps.Styles.ColorPicker.WinUI", typeof(ColorPicker))]
        [TestCase("MahApps.Styles.ColorEyeDropper.WinUI", typeof(ColorEyeDropper))]
        [TestCase("MahApps.Styles.DatePicker.WinUI", typeof(DatePicker))]
        [TestCase("MahApps.Styles.TimePicker.WinUI", typeof(TimePicker))]
        [TestCase("MahApps.Styles.DateTimePicker.WinUI", typeof(DateTimePicker))]
        [TestCase("MahApps.Styles.NumericUpDown.WinUI", typeof(NumericUpDown))]
        [TestCase("MahApps.Styles.SplitButton.WinUI", typeof(SplitButton))]
        [Description("WinUI rounds a control by ControlCornerRadius and nothing else, so no style of that set carries a radius of its own and a box and the picker standing beside it are rounded alike.")]
        public void EveryWinUIControlIsRoundedByTheSameNumber(string key, Type type)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.SetValue(FrameworkElement.StyleProperty, Application.Current.FindResource(key));

            this.window!.Content = control;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(ControlsHelper.GetCornerRadius(control), Is.EqualTo(Application.Current.FindResource("MahApps.CornerRadius.WinUI.Control")));
        }

        [TestCase(typeof(Button), "MahApps.Styles.Button")]
        [TestCase(typeof(TextBox), "MahApps.Styles.TextBox")]
        [TestCase(typeof(ComboBox), "MahApps.Styles.ComboBox")]
        [TestCase(typeof(ComboBoxItem), "MahApps.Styles.ComboBoxItem")]
        [TestCase(typeof(PasswordBox), "MahApps.Styles.PasswordBox")]
        [TestCase(typeof(RichTextBox), "MahApps.Styles.RichTextBox")]
        [TestCase(typeof(CheckBox), "MahApps.Styles.CheckBox")]
        [TestCase(typeof(RadioButton), "MahApps.Styles.RadioButton")]
        [TestCase(typeof(Calendar), "MahApps.Styles.Calendar")]
        [TestCase(typeof(DatePicker), "MahApps.Styles.DatePicker")]
        [TestCase(typeof(TimePicker), "MahApps.Styles.TimePicker")]
        [TestCase(typeof(DateTimePicker), "MahApps.Styles.DateTimePicker")]
        [TestCase(typeof(AnalogClock), "MahApps.Styles.AnalogClock")]
        [TestCase(typeof(Menu), "MahApps.Styles.Menu")]
        [TestCase(typeof(MenuItem), "MahApps.Styles.MenuItem")]
        [TestCase(typeof(ContextMenu), "MahApps.Styles.ContextMenu")]
        [Description("Merging the default set changes nothing about what it has always drawn, whatever the other two do.")]
        public void TheDefaultSetIsStillTheDefaultSet(Type target, string expected)
        {
            var dictionary = Load(Default);

            var implicitStyle = dictionary[target] as Style;

            Assert.That(implicitStyle, Is.Not.Null);
            Assert.That(implicitStyle!.BasedOn, Is.SameAs(dictionary[expected]));
        }

        [TestCase(Default, "MahApps.Styles.TextBox")]
        [TestCase(Win10, "MahApps.Styles.TextBox.Win10")]
        [TestCase(WinUI, "MahApps.Styles.TextBox.WinUI")]
        [Description("A set merged into one corner of the tree reaches the controls in that corner and nothing else.")]
        public void ASetMergedIntoACornerReachesTheControlsThere(string set, string expected)
        {
            var dictionary = Load(set);

            var corner = new Border { Resources = dictionary };
            var box = new TextBox();
            corner.Child = box;

            var elsewhere = new TextBox();

            var page = new StackPanel();
            page.Children.Add(corner);
            page.Children.Add(elsewhere);

            this.window!.Content = page;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(box.Style?.BasedOn, Is.SameAs(dictionary[expected]), "the box in the corner");
                    Assert.That(elsewhere.Style?.BasedOn, Is.SameAs(Application.Current.FindResource("MahApps.Styles.TextBox")), "the box outside it keeps what the application gave it");
                });
        }

        private static ResourceDictionary Load(string set)
        {
            return new ResourceDictionary { Source = new Uri(set, UriKind.Absolute) };
        }
    }
}
