// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The button that stays pressed, in the two Windows sets: the button of that set while it is up
    /// and the accent while it is down. GH-3328 asked for the Windows looks; this is the toggle
    /// button in both of them.
    /// </summary>
    [TestFixture]
    public class ToggleButtonSetStyleTests
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

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCase("MahApps.Styles.ToggleButton.Win10", "MahApps.Brushes.ToggleButton.Win10.BackgroundChecked", "MahApps.Brushes.ToggleButton.Win10.ForegroundChecked")]
        [TestCase("MahApps.Styles.ToggleButton.WinUI", "MahApps.Brushes.ToggleButton.WinUI.BackgroundChecked", "MahApps.Brushes.ToggleButton.WinUI.ForegroundChecked")]
        [Description("A toggle that is down fills with the accent of its set and writes over it in the foreground that set picks for the accent.")]
        public void AToggleThatIsDownFillsWithTheAccent(string key, string fill, string foreground)
        {
            var toggle = this.Show(key, true);

            Assert.Multiple(() =>
                {
                    Assert.That(toggle.Background, Is.SameAs(Application.Current.FindResource(fill)));
                    Assert.That(TextElement.GetForeground(toggle), Is.SameAs(Application.Current.FindResource(foreground)));
                });
        }

        [TestCase("MahApps.Styles.ToggleButton.Win10", "MahApps.Brushes.ToggleButton.Win10.Background")]
        [TestCase("MahApps.Styles.ToggleButton.WinUI", "MahApps.Brushes.ToggleButton.WinUI.Background")]
        [Description("A toggle that is up is the ordinary button of its set.")]
        public void AToggleThatIsUpIsTheButtonOfItsSet(string key, string fill)
        {
            var toggle = this.Show(key, false);

            Assert.That(toggle.Background, Is.SameAs(Application.Current.FindResource(fill)));
        }

        [TestCase("MahApps.Styles.ToggleButton.Win10", "MahApps.Brushes.ToggleButton.Win10.Background")]
        [TestCase("MahApps.Styles.ToggleButton.WinUI", "MahApps.Brushes.ToggleButton.WinUI.Background")]
        [Description("Windows draws the third state exactly like the unchecked one, so a three state toggle in the middle looks as if it were up.")]
        public void TheThirdStateIsDrawnLikeTheUncheckedOne(string key, string fill)
        {
            var toggle = this.Show(key, null);
            toggle.SetCurrentValue(ToggleButton.IsThreeStateProperty, true);
            this.Settle();

            Assert.That(toggle.Background, Is.SameAs(Application.Current.FindResource(fill)));
        }

        [TestCase("MahApps.Styles.ToggleButton.Win10", "MahApps.Styles.Button.Win10")]
        [TestCase("MahApps.Styles.ToggleButton.WinUI", "MahApps.Styles.Button.WinUI")]
        [Description("A toggle and a button of the same set standing beside each other are the same control until one of them latches, so they are the same size as well.")]
        public void AToggleIsTheSizeOfTheButtonBesideIt(string toggleKey, string buttonKey)
        {
            var toggle = this.Show(toggleKey, false);
            var toggleHeight = toggle.ActualHeight;

            var button = new Button
                         {
                             Style = (Style)Application.Current.FindResource(buttonKey),
                             Content = "Bold",
                             MinWidth = 72,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };
            this.window!.Content = button;
            this.Settle();

            Assert.That(toggleHeight, Is.EqualTo(button.ActualHeight).Within(0.5));
        }

        [TestCase("Win10")]
        [TestCase("WinUI")]
        [Description("Which means the pointer too: what a toggle of a set does when the pointer arrives is what that set's button does.")]
        public void AToggleAnswersThePointerTheWayItsButtonDoes(string set)
        {
            Assert.Multiple(() =>
                {
                    Assert.That(Application.Current.FindResource($"MahApps.Brushes.ToggleButton.{set}.BackgroundPointerOver"),
                                Is.SameAs(Application.Current.FindResource($"MahApps.Brushes.Button.{(set == "Win10" ? string.Empty : set + ".")}BackgroundPointerOver")));
                    Assert.That(Application.Current.FindResource($"MahApps.Brushes.ToggleButton.{set}.BackgroundCheckedPointerOver"),
                                Is.SameAs(Application.Current.FindResource($"MahApps.Brushes.Button.{(set == "Win10" ? string.Empty : set + ".")}AccentBackgroundPointerOver")));
                });
        }

        [Test]
        [Description("The WinUI toggle carries the edge along the bottom its button carries, and that edge goes over to the accent once the toggle is down.")]
        public void TheWinUIToggleCarriesTheEdgeAlongItsBottom()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetBottomBorderBrush(this.Show("MahApps.Styles.ToggleButton.WinUI", false)),
                                Is.SameAs(Application.Current.FindResource("MahApps.Brushes.Button.WinUI.BottomBorderBrush")));
                    Assert.That(ControlsHelper.GetBottomBorderBrush(this.Show("MahApps.Styles.ToggleButton.WinUI", true)),
                                Is.SameAs(Application.Current.FindResource("MahApps.Brushes.Button.WinUI.AccentBottomBorderBrush")));
                });
        }

        [TestCase("MahApps.Styles.ToggleButton.Win10")]
        [TestCase("MahApps.Styles.ToggleButton.WinUI")]
        [Description("Nothing in the Windows sets is set in capitals, so the content is written the way it was given.")]
        public void TheContentIsWrittenAsItWasGiven(string key)
        {
            var toggle = this.Show(key, false);

            Assert.That(ControlsHelper.GetContentCharacterCasing(toggle), Is.EqualTo(CharacterCasing.Normal));
        }

        private ToggleButton Show(string key, bool? isChecked)
        {
            Assert.That(this.window, Is.Not.Null);

            var toggle = new ToggleButton
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             Content = "Bold",
                             IsChecked = isChecked,
                             MinWidth = 72,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };

            this.window!.Content = toggle;
            this.Settle();

            return toggle;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
