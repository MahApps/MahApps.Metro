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
        [TestCase(WinUI, typeof(CheckBox), "MahApps.Styles.CheckBox.WinUI")]
        [TestCase(WinUI, typeof(RadioButton), "MahApps.Styles.RadioButton.WinUI")]
        [TestCase(WinUI, typeof(TextBox), "MahApps.Styles.TextBox.WinUI")]
        [TestCase(WinUI, typeof(PasswordBox), "MahApps.Styles.PasswordBox.WinUI")]
        [TestCase(WinUI, typeof(RichTextBox), "MahApps.Styles.RichTextBox.WinUI")]
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

        [TestCase(typeof(Button), "MahApps.Styles.Button.Win10")]
        [TestCase(typeof(NumericUpDown), "MahApps.Styles.NumericUpDown.Win10")]
        [Description("The WinUI set has no style of its own for these yet and hands down the Windows 10 one rather than the default look.")]
        public void WhatTheWinUISetHasNoStyleForKeepsTheWindows10One(Type target, string expected)
        {
            var dictionary = Load(WinUI);

            var implicitStyle = dictionary[target] as Style;

            Assert.That(implicitStyle, Is.Not.Null, $"nothing reaches {target.Name} at all");
            Assert.That(implicitStyle!.BasedOn, Is.SameAs(dictionary[expected]));
        }

        [TestCase(typeof(Button), "MahApps.Styles.Button")]
        [TestCase(typeof(TextBox), "MahApps.Styles.TextBox")]
        [TestCase(typeof(PasswordBox), "MahApps.Styles.PasswordBox")]
        [TestCase(typeof(RichTextBox), "MahApps.Styles.RichTextBox")]
        [TestCase(typeof(CheckBox), "MahApps.Styles.CheckBox")]
        [TestCase(typeof(RadioButton), "MahApps.Styles.RadioButton")]
        [TestCase(typeof(Calendar), "MahApps.Styles.Calendar")]
        [TestCase(typeof(DatePicker), "MahApps.Styles.DatePicker")]
        [TestCase(typeof(TimePicker), "MahApps.Styles.TimePicker")]
        [TestCase(typeof(DateTimePicker), "MahApps.Styles.DateTimePicker")]
        [TestCase(typeof(AnalogClock), "MahApps.Styles.AnalogClock")]
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
