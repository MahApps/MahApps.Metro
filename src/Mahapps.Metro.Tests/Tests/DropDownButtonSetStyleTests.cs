// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A drop-down button is the button of its set with a chevron after the word and a menu under
    /// it, so each of the three sets hands it the outfit of its own button and the menu its own
    /// right button opens. GH-3328 asked for the Windows looks; this is the drop-down button in
    /// both of them.
    /// </summary>
    [TestFixture]
    public class DropDownButtonSetStyleTests
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

        [TestCase("MahApps.Styles.DropDownButton.Win10", "MahApps.Styles.Button.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI", "MahApps.Styles.Button.WinUI")]
        [Description("The drop-down button wears the chrome of the button of its own set, so the two stand next to each other in a row the way Windows draws them.")]
        public void TheDropDownButtonWearsTheChromeOfItsButton(string key, string buttonKey)
        {
            var dropDownButton = this.Show(key);
            var reference = new Button { Style = (Style)Application.Current.FindResource(buttonKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(dropDownButton.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(dropDownButton.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(dropDownButton.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(dropDownButton.Foreground, Is.SameAs(reference.Foreground), "and the colour of the word in it");
                    Assert.That(dropDownButton.Padding, Is.EqualTo(reference.Padding), "and the air it keeps around the word");
                });
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10", "MahApps.Styles.Button.DropDown.Win10", "MahApps.Styles.Button.Split.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI", "MahApps.Styles.Button.DropDown.WinUI", "MahApps.Styles.Button.Split.WinUI")]
        [Description("What lies under the frame is the borderless button of that set, the same one a half of a split button is.")]
        public void TheButtonUnderItIsTheOneOfItsSet(string key, string buttonKey, string halfKey)
        {
            var dropDownButton = this.Show(key);
            var button = (Style)Application.Current.FindResource(buttonKey);

            Assert.Multiple(() =>
                {
                    Assert.That(dropDownButton.ButtonStyle, Is.SameAs(button));
                    Assert.That(button.BasedOn, Is.SameAs(Application.Current.FindResource(halfKey)), "which is the half of a split button under another name");
                });
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10", "MahApps.Styles.ContextMenu.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI", "MahApps.Styles.ContextMenu.WinUI")]
        [Description("The list under the button is the menu that set puts on the right button, so it comes up in the same flyout with the same rows in it.")]
        public void TheListIsTheMenuOfItsSet(string key, string menuKey)
        {
            var dropDownButton = this.Show(key);

            Assert.That(dropDownButton.MenuStyle, Is.SameAs(Application.Current.FindResource(menuKey)));
        }

        [TestCase("MahApps.Styles.DropDownButton")]
        [TestCase("MahApps.Styles.DropDownButton.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI")]
        [Description("The button fills the whole of it and the frame lies over it, the way the button of the Windows looks is built. A frame holding it instead would keep a strip of its own thickness free all the way round, which is a gap wherever that frame is see through.")]
        public void TheButtonFillsTheWholeOfIt(string key)
        {
            var dropDownButton = this.Show(key);

            var container = dropDownButton.FindChild<Grid>("PART_Container");

            Assert.That(container, Is.Not.Null);
            Assert.That(container!.ActualWidth, Is.EqualTo(dropDownButton.ActualWidth), "the button goes the whole width");
            Assert.That(container.ActualHeight, Is.EqualTo(dropDownButton.ActualHeight), "and the whole height");
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI")]
        [Description("The padding of the control lies around the whole of what the button carries rather than around the word alone, so the arrow ends the padding away from the frame instead of standing against it.")]
        public void TheArrowEndsWhereThePaddingSaysItEnds(string key)
        {
            var dropDownButton = this.Show(key);

            var arrow = dropDownButton.FindChild<ContentControl>("PART_Arrow");
            Assert.That(arrow, Is.Not.Null);

            var right = dropDownButton.ActualWidth - arrow!.TranslatePoint(new Point(arrow.ActualWidth, 0), dropDownButton).X;

            // a unit, or what the layout rounding of the screen it is drawn on makes of it
            Assert.That(right, Is.EqualTo(dropDownButton.Padding.Right).Within(1));
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI")]
        [Description("Each set hands in the chevron its own controls point down with, which is the one a combo box hangs its list behind.")]
        public void TheArrowIsTheChevronOfItsSet(string key)
        {
            var dropDownButton = this.Show(key);

            var chevron = dropDownButton.FindChild<FontIcon>(null);

            Assert.That(chevron, Is.Not.Null, "the button should carry the chevron of its set");
            Assert.That(chevron!.Glyph, Is.EqualTo("\uE70D"));
        }

        [Test]
        [Description("The Metro look draws its own arrow as a path, so nothing about it changes.")]
        public void TheMetroDropDownButtonKeepsItsArrow()
        {
            var dropDownButton = this.Show("MahApps.Styles.DropDownButton");

            Assert.Multiple(() =>
                {
                    Assert.That(dropDownButton.FindChild<PathIcon>(null), Is.Not.Null, "the Metro arrow is a path");
                    Assert.That(dropDownButton.FindChild<FontIcon>(null), Is.Null, "and not a glyph out of a font");
                });
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI")]
        [Description("A Windows button that is switched off says so with its colours, so no veil is drawn over it.")]
        public void AWindowsDropDownButtonThatIsOffDrawsNoVeil(string key)
        {
            var dropDownButton = this.Show(key);
            dropDownButton.IsEnabled = false;
            this.Settle();

            Assert.That(dropDownButton.Opacity, Is.EqualTo(1));
        }

        [Test]
        [Description("The Metro look says it with the veil it has always drawn.")]
        public void TheMetroDropDownButtonKeepsItsVeil()
        {
            var dropDownButton = this.Show("MahApps.Styles.DropDownButton");
            dropDownButton.IsEnabled = false;
            this.Settle();

            Assert.That(dropDownButton.Opacity, Is.LessThan(1));
        }

        [TestCase("MahApps.Styles.DropDownButton.Win10")]
        [TestCase("MahApps.Styles.DropDownButton.WinUI")]
        [Description("A button keeps the focus once it has been clicked, and the pointer is still over it while it does. The trigger for the pointer therefore has to stand after the ones for the focus, or the frame would answer the pointer until the first click and never again.")]
        public void ThePointerStillAnswersAfterAClick(string key)
        {
            var triggers = ((Style)Application.Current.FindResource(key)).Triggers.OfType<Trigger>().ToList();

            var pointer = triggers.FindLastIndex(trigger => trigger.Property == UIElement.IsMouseOverProperty);
            var focus = triggers.FindLastIndex(trigger => trigger.Property == UIElement.IsKeyboardFocusWithinProperty || trigger.Property == UIElement.IsFocusedProperty);

            Assert.That(pointer, Is.GreaterThan(focus));
        }

        private DropDownButton Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var dropDownButton = new DropDownButton
                                 {
                                     Style = (Style)Application.Current.FindResource(key),
                                     Content = "Export",
                                     Width = 200,
                                     HorizontalAlignment = HorizontalAlignment.Left,
                                     VerticalAlignment = VerticalAlignment.Top
                                 };

            dropDownButton.ItemsSource = new[] { "As a picture", "As a table", "As it is" };

            this.window!.Content = dropDownButton;
            this.Settle();

            return dropDownButton;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
