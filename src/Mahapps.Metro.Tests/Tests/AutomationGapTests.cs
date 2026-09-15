// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4454: what was left over after the window, the tile, the badge and the range slider. Each of
    /// these holds something a client cannot get at: a key combination, a caption, a pressable button
    /// behind a list, the pages of a gallery, and seven sliders nobody named.
    /// </summary>
    [TestFixture]
    public class AutomationGapTests
    {
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
        [Description("The caption of a reveal image reaches the screen at all, which is where a reader picks it up.")]
        public void TheCaptionOfARevealImageReachesTheScreen()
        {
            var text = this.window!.TheRevealImage.FindChild<TextBlock>("PART_Text");

            Assert.That(text, Is.Not.Null);
            Assert.That(text!.Text, Is.EqualTo("a caption"));
        }

        [Test]
        [Description("A hot key box hands over the combination it holds, and says what kind of thing it is.")]
        public void AHotKeyBoxHandsOverTheCombination()
        {
            var box = this.window!.TheHotKeyBox;
            box.SetCurrentValue(HotKeyBox.HotKeyProperty, new HotKey(Key.F5, ModifierKeys.Control));
            ClipAssert.Pump();

            var peer = UIElementAutomationPeer.CreatePeerForElement(box);
            Assert.That(peer, Is.Not.Null, "the box should make a peer of its own");
            Assert.That(peer!.GetClassName(), Is.EqualTo("HotKeyBox"));

            var value = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            Assert.That(value, Is.Not.Null, "and hand over what it holds");
            Assert.That(value!.Value, Is.EqualTo(box.HotKey!.ToString()));
        }

        [Test]
        [Description("A drop down button is a button with a menu behind it, not a list.")]
        public void ADropDownButtonIsAButtonThatOpens()
        {
            var peer = UIElementAutomationPeer.CreatePeerForElement(this.window!.TheDropDownButton);

            Assert.That(peer, Is.Not.Null);
            Assert.That(peer!.GetAutomationControlType(), Is.EqualTo(AutomationControlType.Button));
            Assert.That(peer.GetClassName(), Is.EqualTo("DropDownButton"));
            Assert.That(peer.GetName(), Is.EqualTo("Actions"), "named after what it says on it");

            var expand = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
            Assert.That(expand, Is.Not.Null, "and it opens and shuts");
            Assert.That(expand!.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));
        }

        [Test]
        [Description("Opening it through a client opens it for real.")]
        public void ADropDownButtonOpensWhenAClientAsks()
        {
            var button = this.window!.TheDropDownButton;
            var expand = (IExpandCollapseProvider)UIElementAutomationPeer.CreatePeerForElement(button)!.GetPattern(PatternInterface.ExpandCollapse)!;

            expand.Expand();
            ClipAssert.Pump();

            Assert.That(button.IsExpanded, Is.True);

            expand.Collapse();
            ClipAssert.Pump();

            Assert.That(button.IsExpanded, Is.False);
        }

        [Test]
        [Description("A flip view says which of its pages is showing, since it builds nothing for the others.")]
        public void AFlipViewSaysWhichPageIsShowing()
        {
            var flip = this.window!.TheFlipView;

            flip.Items.Clear();
            flip.Items.Add(new FlipViewItem { Content = "the first page" });
            flip.Items.Add(new FlipViewItem { Content = "the second page" });
            flip.SetCurrentValue(FlipView.SelectedIndexProperty, 0);
            ClipAssert.Pump();

            var peer = UIElementAutomationPeer.CreatePeerForElement(flip);
            Assert.That(peer, Is.Not.Null);
            Assert.That(peer!.GetClassName(), Is.EqualTo("FlipView"));
            Assert.That(peer.GetItemStatus(), Is.EqualTo("1 / 2"));

            flip.SetCurrentValue(FlipView.SelectedIndexProperty, 1);
            ClipAssert.Pump();

            Assert.That(peer.GetItemStatus(), Is.EqualTo("2 / 2"), "and it keeps up as the pages turn");
        }

        [Test]
        [Description("Every channel of the colour canvas is named after the letter beside it.")]
        public void EveryChannelOfTheColourCanvasIsNamed()
        {
            var canvas = this.window!.TheColorCanvas;
            ClipAssert.Pump();

            var sliders = Sliders(canvas).ToList();
            Assert.That(sliders, Is.Not.Empty, "the canvas should carry a slider per channel");

            var nameless = sliders.Where(slider => string.IsNullOrEmpty(UIElementAutomationPeer.CreatePeerForElement(slider)?.GetName())).ToList();

            Assert.That(nameless, Is.Empty, $"{nameless.Count} of {sliders.Count} sliders have no name");
        }

        private static System.Collections.Generic.IEnumerable<Slider> Sliders(DependencyObject root)
        {
            if (root is Slider slider)
            {
                yield return slider;
            }

            for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
            {
                foreach (var found in Sliders(System.Windows.Media.VisualTreeHelper.GetChild(root, i)))
                {
                    yield return found;
                }
            }
        }
    }
}
