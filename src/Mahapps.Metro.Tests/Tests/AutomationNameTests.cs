// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A screen reader reads out the name of a button, and the name of a button comes from its content.
    /// Where that content is the path of a glyph or a letter out of a symbol font, that is what gets
    /// read out, so every button drawn from a glyph has to be given a name of its own.
    /// </summary>
    [TestFixture]
    public class AutomationNameTests
    {
        /// <summary>Path data, as a glyph button carries it: an M and a number, or a run of coordinates.</summary>
        private static readonly Regex LooksLikeAGlyph = new(@"^[Mm][ \d]|^F\d |,\d+\.?\d*[ L]", RegexOptions.Compiled);

        private AutomationNameWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<AutomationNameWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("Nothing in a window reads out as the path of the picture on it.")]
        public void NoButtonIsNamedAfterItsGlyph()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.TheFlyout.SetCurrentValue(Flyout.IsOpenProperty, true);
            ClipAssert.Pump();

            var caught = new List<string>();

            foreach (var button in ButtonsOf(this.window))
            {
                var name = UIElementAutomationPeer.CreatePeerForElement(button)?.GetName() ?? string.Empty;

                if (LooksLikeAGlyph.IsMatch(name))
                {
                    caught.Add($"{button.GetType().Name} \"{button.Name}\" reads out as {name.Substring(0, System.Math.Min(40, name.Length))}…");
                }
            }

            Assert.That(caught, Is.Empty);
        }

        [Test]
        [Description("The button that empties a box says what it does, rather than reading out the letter its cross is drawn from.")]
        public void TheClearButtonSaysWhatItDoes()
        {
            Assert.That(NameOf(this.window!.TheTextBox, "PART_ClearText"), Is.EqualTo("Clear"));
        }

        [Test]
        [Description("The button that opens a picker says what it opens, in the words WPF uses for its own.")]
        public void TheButtonOfAPickerSaysWhatItOpens()
        {
            Assert.That(NameOf(this.window!.TheDateTimePicker, "PART_Button"), Is.EqualTo("Show Calendar"), "the picker that carries a calendar");
            Assert.That(NameOf(this.window.TheDatePicker, "PART_Button"), Is.EqualTo("Show Calendar"), "and the plain date picker, in the words WPF uses for its own");
            Assert.That(NameOf(this.window.TheTimePicker, "PART_Button"), Is.EqualTo("Show Clock"), "a time picker has no calendar to show");
        }

        [Test]
        [Description("The arrows of a scroll bar say where they go.")]
        public void TheArrowsOfAScrollBarSayWhereTheyGo()
        {
            var scroller = this.window!.TheScrollViewer;

            Assert.That(NameOf(scroller, "HorizontalSmallDecrease"), Is.EqualTo("Scroll left"));
            Assert.That(NameOf(scroller, "HorizontalSmallIncrease"), Is.EqualTo("Scroll right"));
            Assert.That(NameOf(scroller, "VerticalSmallDecrease"), Is.EqualTo("Scroll up"));
            Assert.That(NameOf(scroller, "VerticalSmallIncrease"), Is.EqualTo("Scroll down"));
        }

        [Test]
        [Description("And the one that shuts a flyout says so.")]
        public void TheButtonThatShutsAFlyoutSaysSo()
        {
            this.window!.TheFlyout.SetCurrentValue(Flyout.IsOpenProperty, true);
            ClipAssert.Pump();

            Assert.That(NameOf(this.window.TheFlyout, "PART_BackButton"), Is.EqualTo("Back"));
        }

        private static string? NameOf(FrameworkElement root, string part)
        {
            var element = root.FindChild<ButtonBase>(part);
            Assert.That(element, Is.Not.Null, $"the template should carry {part}");

            return UIElementAutomationPeer.CreatePeerForElement(element!)?.GetName();
        }

        private static IEnumerable<ButtonBase> ButtonsOf(DependencyObject root)
        {
            if (root is ButtonBase button)
            {
                yield return button;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                foreach (var found in ButtonsOf(VisualTreeHelper.GetChild(root, i)))
                {
                    yield return found;
                }
            }
        }
    }
}
