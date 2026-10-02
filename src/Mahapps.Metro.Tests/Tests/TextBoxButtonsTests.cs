// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The buttons an application puts beside the one a text control brings itself, and where in
    /// the row each of them ends up.
    /// </summary>
    [TestFixture]
    public class TextBoxButtonsTests
    {
        private TextBoxButtonsWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TextBoxButtonsWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.window?.TestTextBox.ClearDependencyProperties([
                TextBoxHelper.ButtonsAlignmentProperty.Name,
                TextBoxHelper.ButtonsPlacementProperty.Name
            ]);
            this.window?.UpdateLayout();
        }

        private ItemsControl Strip => this.window!.TestTextBox.FindChild<ItemsControl>("PART_Buttons")!;

        private Button ClearButton => this.window!.TestTextBox.FindChild<Button>("PART_ClearText")!;

        private Grid Host => this.window!.TestTextBox.FindChild<Grid>("PART_ButtonsHost")!;

        [Test]
        public void TheButtonsOfTheBoxAreTheOnesTheRowShows()
        {
            Assert.That(this.window, Is.Not.Null);
            Assert.That(this.Strip.Items, Has.Count.EqualTo(2));

            // a button is a visual element, so it stands in the row as itself rather than in a wrapper
            Assert.That(this.Strip.ItemContainerGenerator.ContainerFromIndex(0), Is.SameAs(this.window.TestFolderButton));
            Assert.That(this.Strip.ItemContainerGenerator.ContainerFromIndex(1), Is.SameAs(this.window.TestSearchButton));
        }

        [Test]
        public void TheClearButtonIsTheOuterOneUntilItIsToldOtherwise()
        {
            Assert.That(Grid.GetColumn(this.Strip), Is.EqualTo(0));
            Assert.That(Grid.GetColumn(this.ClearButton), Is.EqualTo(1));
        }

        [Test]
        public void TheTwoChangePlacesWhenTheButtonsGoOutside()
        {
            TextBoxHelper.SetButtonsPlacement(this.window!.TestTextBox, ButtonsPlacement.Outside);
            this.window.UpdateLayout();

            Assert.That(Grid.GetColumn(this.Strip), Is.EqualTo(1));
            Assert.That(Grid.GetColumn(this.ClearButton), Is.EqualTo(0));
        }

        [Test]
        public void TheRowTurnsRoundWithTheButtonsOnTheLeft()
        {
            TextBoxHelper.SetButtonsAlignment(this.window!.TestTextBox, ButtonsAlignment.Left);
            this.window.UpdateLayout();

            // the row moves to the other side of the text, and inside it the clear button is still
            // the outer one, which over there is the left one
            Assert.That(Grid.GetColumn(this.Host), Is.EqualTo(0));
            Assert.That(Grid.GetColumn(this.Strip), Is.EqualTo(1));
            Assert.That(Grid.GetColumn(this.ClearButton), Is.EqualTo(0));
        }

        [Test]
        public void TheRowTurnsRoundOnTheLeftWithTheButtonsOutsideAsWell()
        {
            TextBoxHelper.SetButtonsAlignment(this.window!.TestTextBox, ButtonsAlignment.Left);
            TextBoxHelper.SetButtonsPlacement(this.window.TestTextBox, ButtonsPlacement.Outside);
            this.window.UpdateLayout();

            Assert.That(Grid.GetColumn(this.Strip), Is.EqualTo(0));
            Assert.That(Grid.GetColumn(this.ClearButton), Is.EqualTo(1));
        }

        [Test]
        public void APressLeavesTheCaretInTheBox()
        {
            // the Win10 and the WinUI box show their clear button only while the box has the keyboard,
            // so a button of the row that took it away would make that one disappear under the pointer
            Assert.That(this.window!.TestFolderButton.Focusable, Is.False);
            Assert.That(this.window.TestFolderButton.IsTabStop, Is.False);
        }

        [Test]
        public void TheClearButtonStaysWhileOneOfThemHasTheKeyboard()
        {
            Assert.That(this.window, Is.Not.Null);

            var clear = this.window.TestWin10Box.FindChild<Button>("PART_ClearText")!;

            Assert.That(this.window.TestWin10Box.Focus(), Is.True);
            this.window.UpdateLayout();
            Assert.That(clear.Visibility, Is.EqualTo(Visibility.Visible));

            Assert.That(this.window.TestTabStopButton.Focus(), Is.True);
            this.window.UpdateLayout();
            Assert.That(clear.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void TheRowWearsTheStyleOfTheSetTheBoxIsDrawnIn()
        {
            Assert.That(this.window, Is.Not.Null);

            // an icon written as an element takes its colour from the button rather than from the
            // chrome around it, so the button has to carry the colours of the set the box wears
            Assert.That(this.window.TestTabStopButton.Style,
                        Is.SameAs(this.window.TryFindResource("MahApps.Styles.TextBoxButton.Win10")));
            Assert.That(this.window.TestFolderButton.Style,
                        Is.SameAs(this.window.TryFindResource("MahApps.Styles.TextBoxButton")));
        }

        [Test]
        public void TheComboBoxHandsThemToTheHalfOfItThatDrawsThem()
        {
            // the row of a combo box is drawn inside the toggle button that opens the list
            var strip = this.window!.TestComboBox.FindChild<ItemsControl>("PART_Buttons");

            Assert.That(strip, Is.Not.Null);
            Assert.That(strip.Items, Has.Count.EqualTo(1));
            Assert.That(strip.ItemContainerGenerator.ContainerFromIndex(0), Is.SameAs(this.window.TestComboBoxButton));
        }
    }
}
