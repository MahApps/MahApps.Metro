// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A split button is the button of its set cut in two, so each of the three sets hands it the
    /// outfit of its own button and the chevron its own drop-downs carry. GH-3328 asked for the
    /// Windows looks; this is the split button in both of them.
    /// </summary>
    [TestFixture]
    public class SplitButtonSetStyleTests
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

        [TestCase("MahApps.Styles.SplitButton.Win10", "MahApps.Styles.Button.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI", "MahApps.Styles.Button.WinUI")]
        [Description("The split button wears the chrome of the button of its own set, so the two stand next to each other in a row the way Windows draws them.")]
        public void TheSplitButtonWearsTheChromeOfItsButton(string key, string buttonKey)
        {
            var splitButton = this.Show(key);
            var reference = new Button { Style = (Style)Application.Current.FindResource(buttonKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(splitButton.Background, Is.SameAs(reference.Background), "the fill");
                    Assert.That(splitButton.BorderBrush, Is.SameAs(reference.BorderBrush), "the frame");
                    Assert.That(splitButton.BorderThickness, Is.EqualTo(reference.BorderThickness), "how thick that frame is");
                    Assert.That(splitButton.Foreground, Is.SameAs(reference.Foreground), "the word on it");
                    Assert.That(splitButton.Padding, Is.EqualTo(reference.Padding), "the air around that word");
                    Assert.That(ControlsHelper.GetBottomBorderBrush(splitButton), Is.SameAs(ControlsHelper.GetBottomBorderBrush(reference)), "the edge along the bottom, if that set draws one");
                    Assert.That(ControlsHelper.GetCornerRadius(splitButton), Is.EqualTo(ControlsHelper.GetCornerRadius(reference)), "and the corners it is rounded by");
                });
        }

        [TestCase("MahApps.Styles.SplitButton.Win10", "MahApps.Styles.Button.Split.Win10", "MahApps.Styles.Button.Split.Arrow.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI", "MahApps.Styles.Button.Split.WinUI", "MahApps.Styles.Button.Split.Arrow.WinUI")]
        [Description("Both halves are the same button, and the one with the arrow stands on it and adds the line where the two meet.")]
        public void TheTwoHalvesAreTheSameButton(string key, string halfKey, string arrowKey)
        {
            var splitButton = this.Show(key);
            var half = (Style)Application.Current.FindResource(halfKey);
            var arrow = (Style)Application.Current.FindResource(arrowKey);

            Assert.Multiple(() =>
                {
                    Assert.That(splitButton.ButtonStyle, Is.SameAs(half), "the half with the word in it");
                    Assert.That(splitButton.ButtonArrowStyle, Is.SameAs(arrow), "and the one with the arrow");
                    Assert.That(arrow.BasedOn, Is.SameAs(half), "which is the same half underneath");
                });
        }

        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("A Windows split button says where the arrow half begins with a line. That line is the near edge of the half itself, so it stays where it is while the half fills under the pointer, and it is drawn in a colour of its own rather than in the frame, which a Windows 10 button shows only under the pointer.")]
        public void TheWindowsSetsDrawALineBetweenTheHalves(string key)
        {
            var splitButton = this.Show(key);

            var expander = splitButton.FindChild<Button>("PART_Expander");

            Assert.That(expander, Is.Not.Null, "the template should carry a half for the arrow");
            Assert.That(expander!.BorderThickness, Is.EqualTo(new Thickness(1, 0, 0, 0)), "the line stands along the edge where the two halves meet");
            Assert.That(expander.BorderBrush, Is.Not.Null, "and it is drawn in a colour of its own");
            // the frame of a Windows 10 button is see through until the pointer is over it, and this line is not
            Assert.That(((SolidColorBrush)expander.BorderBrush).Color.A, Is.GreaterThan(0), "in a colour that can be seen without the pointer");
        }

        [Test]
        [Description("Where the two halves stand one above the other, that line stands along the top of the lower one.")]
        public void TheLineFollowsTheWayTheHalvesStand()
        {
            var splitButton = this.Show("MahApps.Styles.SplitButton.WinUI");
            splitButton.Orientation = Orientation.Vertical;
            this.Settle();

            var expander = splitButton.FindChild<Button>("PART_Expander");

            Assert.That(expander, Is.Not.Null);
            Assert.That(expander!.BorderThickness, Is.EqualTo(new Thickness(0, 1, 0, 0)));
        }

        [Test]
        [Description("The Metro look has no such line, so nothing about it changes.")]
        public void TheMetroSplitButtonHasNoLineBetweenTheHalves()
        {
            var splitButton = this.Show("MahApps.Styles.SplitButton");

            var expander = splitButton.FindChild<Button>("PART_Expander");

            Assert.That(expander, Is.Not.Null);
            Assert.That(expander!.BorderThickness, Is.EqualTo(new Thickness(0)), "the Metro look draws no line between the two halves");
        }

        [TestCase("MahApps.Styles.SplitButton")]
        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("The halves fill the whole of the button and the frame lies over them, the way the button of the Windows looks is built. A frame holding them instead would keep a strip of its own thickness free all the way round, which is a gap wherever that frame is see through.")]
        public void TheHalvesFillTheWholeOfIt(string key)
        {
            var splitButton = this.Show(key);

            var container = splitButton.FindChild<Grid>("PART_Container");

            Assert.That(container, Is.Not.Null);
            Assert.That(container!.ActualWidth, Is.EqualTo(splitButton.ActualWidth), "the halves go the whole width");
            Assert.That(container.ActualHeight, Is.EqualTo(splitButton.ActualHeight), "and the whole height");
        }

        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("A button keeps the focus once it has been clicked, and the pointer is still over it while it does. The trigger for the pointer therefore has to stand after the ones for the focus, or the frame would answer the pointer until the first click and never again.")]
        public void ThePointerStillAnswersAfterAClick(string key)
        {
            var triggers = ((Style)Application.Current.FindResource(key)).Triggers.OfType<Trigger>().ToList();

            var pointer = triggers.FindLastIndex(trigger => trigger.Property == UIElement.IsMouseOverProperty);
            var focus = triggers.FindLastIndex(trigger => trigger.Property == UIElement.IsKeyboardFocusWithinProperty || trigger.Property == UIElement.IsFocusedProperty);

            Assert.That(pointer, Is.GreaterThan(focus));
        }

        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("The line stands where the two halves meet, and the arrow in the middle of the half behind it.")]
        public void TheArrowStandsInTheMiddleOfItsHalf(string key)
        {
            var splitButton = this.Show(key);

            var expander = splitButton.FindChild<Button>("PART_Expander");
            var chevron = splitButton.FindChild<FontIcon>(null);
            Assert.That(expander, Is.Not.Null);
            Assert.That(chevron, Is.Not.Null);

            var left = chevron!.TranslatePoint(new Point(0, 0), expander!).X;
            var right = expander!.ActualWidth - (left + chevron.ActualWidth);

            // a whole device pixel of slack either way, because the layout rounds to those and one of
            // them is a fraction of a unit on a screen that is not at 100 per cent
            Assert.Multiple(() =>
                {
                    Assert.That(expander.ActualWidth, Is.EqualTo(30).Within(1), "the half the arrow stands in is as wide as the chevron cell of this set's combo box");
                    Assert.That(left, Is.EqualTo(right).Within(1), "and the arrow stands in the middle of it");
                });
        }

        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("The arrow is the chevron the drop-downs of that set carry rather than the one the Metro look draws.")]
        public void TheArrowIsTheChevronOfItsSet(string key)
        {
            var splitButton = this.Show(key);

            var chevron = splitButton.FindChild<FontIcon>(null);

            Assert.That(chevron, Is.Not.Null, "the arrow half should carry the chevron of its set");
            Assert.That(chevron!.Glyph, Is.EqualTo("\uE70D"), "and that chevron is the one a combo box hangs its list behind");
        }

        [Test]
        [Description("The Metro look keeps the arrow it has always drawn.")]
        public void TheMetroSplitButtonKeepsItsArrow()
        {
            var splitButton = this.Show("MahApps.Styles.SplitButton");

            Assert.Multiple(() =>
                {
                    Assert.That(splitButton.FindChild<PathIcon>(null), Is.Not.Null, "the Metro arrow is a path");
                    Assert.That(splitButton.FindChild<FontIcon>(null), Is.Null, "and not a glyph out of a font");
                });
        }

        [TestCase("MahApps.Styles.SplitButton", "MahApps.Styles.ComboBoxItem")]
        [TestCase("MahApps.Styles.SplitButton.Win10", "MahApps.Styles.ComboBoxItem.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI", "MahApps.Styles.ComboBoxItem.WinUI")]
        [Description("The rows of the list are the rows of a combo box of the same set, so a list filled from an ItemsSource looks like one rather than like whatever the application happens to have for a ComboBoxItem.")]
        public void TheRowsOfTheListBelongToTheSet(string key, string rowKey)
        {
            var splitButton = this.Show(key);

            Assert.That(splitButton.ItemContainerStyle, Is.SameAs(Application.Current.FindResource(rowKey)));
        }

        [TestCase("MahApps.Styles.SplitButton.Win10", "MahApps.Styles.ComboBox.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI", "MahApps.Styles.ComboBox.WinUI")]
        [Description("And the list itself is the list of that set's combo box, down to the fill behind it, the frame around it and the corners it is rounded by.")]
        public void TheListIsTheListOfItsComboBox(string key, string comboBoxKey)
        {
            var splitButton = this.Show(key);
            var reference = new ComboBox { Style = (Style)Application.Current.FindResource(comboBoxKey) };

            Assert.Multiple(() =>
                {
                    Assert.That(ComboBoxHelper.GetDropDownBackground(splitButton), Is.SameAs(ComboBoxHelper.GetDropDownBackground(reference)), "the fill behind the rows");
                    Assert.That(ComboBoxHelper.GetDropDownBorderBrush(splitButton), Is.SameAs(ComboBoxHelper.GetDropDownBorderBrush(reference)), "the frame around them");
                    Assert.That(ComboBoxHelper.GetDropDownCornerRadius(splitButton), Is.EqualTo(ComboBoxHelper.GetDropDownCornerRadius(reference)), "and the corners that frame is rounded by");
                });
        }

        [Test]
        [Description("What a Windows 10 button says about the pointer it says with its frame, because the fill it carries under one is the fill it carries at rest. The split button answers with the same line, or it would look dead under the pointer next to a button that does not.")]
        public void TheWindows10SplitButtonAnswersThePointerTheWayItsButtonDoes()
        {
            var splitButton = (Style)Application.Current.FindResource("MahApps.Styles.SplitButton.Win10");
            var button = (Style)Application.Current.FindResource("MahApps.Styles.Button.Win10");

            Assert.That(PointerOver(splitButton, Control.BorderBrushProperty), Is.EqualTo(PointerOver(button, Control.BorderBrushProperty)));
        }

        [TestCase("MahApps.Styles.SplitButton.Win10")]
        [TestCase("MahApps.Styles.SplitButton.WinUI")]
        [Description("A Windows button that is switched off says so with its colours, the way the rest of that set does, rather than with the veil the Metro one draws over itself.")]
        public void AWindowsSplitButtonThatIsOffDrawsNoVeil(string key)
        {
            var splitButton = this.Show(key);
            splitButton.IsEnabled = false;
            this.Settle();

            Assert.That(splitButton.Opacity, Is.EqualTo(1));
        }

        [Test]
        [Description("The Metro look keeps its veil, so nothing about it changes.")]
        public void TheMetroSplitButtonKeepsItsVeil()
        {
            var splitButton = this.Show("MahApps.Styles.SplitButton");
            splitButton.IsEnabled = false;
            this.Settle();

            Assert.That(splitButton.Opacity, Is.EqualTo(0.55));
        }

        private static object? PointerOver(Style style, DependencyProperty property)
        {
            var value = style.Triggers
                             .OfType<Trigger>()
                             .Where(trigger => trigger.Property == UIElement.IsMouseOverProperty && Equals(trigger.Value, true))
                             .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                             .Where(setter => setter.Property == property)
                             .Select(setter => setter.Value)
                             .LastOrDefault();

            if (value is null)
            {
                return style.BasedOn is null ? null : PointerOver(style.BasedOn, property);
            }

            // what a setter holds is the request for a resource rather than the brush behind it
            return (value as DynamicResourceExtension)?.ResourceKey ?? value;
        }

        private SplitButton Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var splitButton = new SplitButton
                              {
                                  Style = (Style)Application.Current.FindResource(key),
                                  SelectedIndex = 0,
                                  Width = 200,
                                  HorizontalAlignment = HorizontalAlignment.Left,
                                  VerticalAlignment = VerticalAlignment.Top
                              };

            splitButton.ItemsSource = new[] { "Warp 1", "Warp 6", "Warp 9.975" };

            this.window!.Content = splitButton;
            this.Settle();

            return splitButton;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
