// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The expander of the two Windows sets: a header with a chevron at its far end, a content area
    /// under it and, in the WinUI look, two cards with rounded corners. GH-3328 asked for the
    /// Windows looks; this is the expander in both of them.
    /// </summary>
    [TestFixture]
    public class ExpanderSetStyleTests
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

        [TestCase("MahApps.Styles.Expander.Win10", "\uE70D")]
        [TestCase("MahApps.Styles.Expander.WinUI", "\uE70D")]
        [Description("The header of a Windows set points with the chevron that set's own controls point down with, out of the symbol font rather than drawn as a path.")]
        public void TheHeaderPointsWithTheChevronOfItsSet(string key, string glyph)
        {
            var expander = this.Show(key);

            var chevron = expander.FindChild<FontIcon>("Chevron");

            Assert.That(chevron, Is.Not.Null, "the header should carry the chevron of its set");
            Assert.That(chevron!.Glyph, Is.EqualTo(glyph));
        }

        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("That chevron turns rather than being swapped for another glyph, which is what WinUI animates as well, so it hangs on a rotation of its own.")]
        public void TheChevronTurnsRatherThanBeingSwapped(string key)
        {
            var expander = this.Show(key);

            var chevron = expander.FindChild<FontIcon>("Chevron");

            Assert.That(chevron, Is.Not.Null);
            Assert.That(chevron!.RenderTransform, Is.TypeOf<RotateTransform>());
            Assert.That(chevron.RenderTransformOrigin, Is.EqualTo(new Point(0.5, 0.5)), "and it turns around its middle");
        }

        [Test]
        [Description("The Metro look draws its own arrow inside a ring, so nothing about it changes.")]
        public void TheMetroHeaderKeepsItsRingedArrow()
        {
            var expander = this.Show("MahApps.Styles.Expander");

            Assert.Multiple(() =>
                {
                    Assert.That(expander.FindChild<Ellipse>("Circle"), Is.Not.Null, "the Metro glyph is a ring");
                    Assert.That(expander.FindChild<FontIcon>("Chevron"), Is.Null, "and not a glyph out of a font");
                });
        }

        [TestCase(ExpandDirection.Down, "MahApps.Storyboard.Expander.Win10.Expand.Down", "MahApps.Storyboard.Expander.Win10.Collapse.Down")]
        [TestCase(ExpandDirection.Up, "MahApps.Storyboard.Expander.Win10.Expand.Up", "MahApps.Storyboard.Expander.Win10.Collapse.Up")]
        [TestCase(ExpandDirection.Right, "MahApps.Storyboard.Expander.Win10.Expand.Right", "MahApps.Storyboard.Expander.Win10.Collapse.Right")]
        [TestCase(ExpandDirection.Left, "MahApps.Storyboard.Expander.Win10.Expand.Left", "MahApps.Storyboard.Expander.Win10.Collapse.Left")]
        [Description("The content leaves by the side it came out of, so which way it travels follows the direction the expander opens.")]
        public void TheContentTravelsTheWayTheExpanderOpens(ExpandDirection direction, string expandKey, string collapseKey)
        {
            var expander = this.Show("MahApps.Styles.Expander.Win10");
            expander.SetCurrentValue(Expander.ExpandDirectionProperty, direction);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ExpanderHelper.GetExpandStoryboard(expander), Is.SameAs(Application.Current.FindResource(expandKey)));
                    Assert.That(ExpanderHelper.GetCollapseStoryboard(expander), Is.SameAs(Application.Current.FindResource(collapseKey)));
                });
        }

        [TestCase("MahApps.Styles.Expander")]
        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("What a set slides in slides inside a frame that clips, so that the part still on its way stays behind the header rather than over it.")]
        public void TheContentSlidesInsideAFrameThatClips(string key)
        {
            var expander = this.Show(key);

            var clip = expander.FindChild<Border>("ExpandSiteClip");
            var site = expander.FindChild<Border>("ExpandSite");

            Assert.That(clip, Is.Not.Null);
            Assert.That(clip!.ClipToBounds, Is.True, "and that frame cuts what stands outside it");
            Assert.That(site, Is.Not.Null);
            Assert.That(site!.RenderTransform, Is.TypeOf<TranslateTransform>(), "what travels is the content, on a transform of its own");
        }

        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("And it really travels: opening the expander puts the animation on that transform rather than leaving it where it was.")]
        public void OpeningTheExpanderSetsTheContentTravelling(string key)
        {
            var expander = this.Show(key);
            expander.SetCurrentValue(Expander.IsExpandedProperty, false);
            this.Settle();

            expander.SetCurrentValue(Expander.IsExpandedProperty, true);
            this.Settle();

            var site = expander.FindChild<Border>("ExpandSite");

            Assert.That(site, Is.Not.Null);
            Assert.That(site!.RenderTransform.HasAnimatedProperties, Is.True);
        }

        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("Where the content stands inside the frame is what HorizontalContentAlignment says, so the frame itself keeps the width of the expander however it is aligned.")]
        public void TheFrameKeepsItsWidthWhateverTheContentDoes(string key)
        {
            var expander = this.Show(key);
            expander.SetCurrentValue(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
            this.Settle();

            var site = expander.FindChild<Border>("ExpandSite");

            Assert.That(site, Is.Not.Null);
            Assert.That(site!.ActualWidth, Is.EqualTo(expander.ActualWidth).Within(0.5));
        }

        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("A Windows expander that is switched off says so with its colours, so no veil is drawn over it.")]
        public void AWindowsExpanderThatIsOffDrawsNoVeil(string key)
        {
            var expander = this.Show(key);

            Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(expander), Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        [Description("The Metro look says it with the veil it has always drawn.")]
        public void TheMetroExpanderKeepsItsVeil()
        {
            var expander = this.Show("MahApps.Styles.Expander");

            Assert.That(ControlsHelper.GetDisabledVisualElementVisibility(expander), Is.EqualTo(Visibility.Visible));
        }

        [Test]
        [Description("The Windows 10 header is a list row: the page underneath until the pointer arrives, and then the fill a row of that set takes.")]
        public void TheWindows10HeaderAnswersThePointerWithItsRow()
        {
            var expander = this.Show("MahApps.Styles.Expander.Win10");

            Assert.Multiple(() =>
                {
                    Assert.That(HeaderedControlHelper.GetHeaderBackgroundMouseOver(expander), Is.Not.SameAs(HeaderedControlHelper.GetHeaderBackground(expander)), "the row is what answers");
                    Assert.That(ExpanderHelper.GetToggleButtonBackgroundMouseOver(expander), Is.SameAs(ExpanderHelper.GetToggleButtonBackground(expander)), "and the square behind the chevron stays as it is");
                });
        }

        [Test]
        [Description("In the WinUI look it is the other way round: the card keeps its fill and the square behind the chevron is what lights up, which is how WinUI has it.")]
        public void TheWinUIHeaderAnswersThePointerWithItsChevron()
        {
            var expander = this.Show("MahApps.Styles.Expander.WinUI");

            Assert.Multiple(() =>
                {
                    Assert.That(HeaderedControlHelper.GetHeaderBackgroundMouseOver(expander), Is.SameAs(HeaderedControlHelper.GetHeaderBackground(expander)), "the card stays as it is");
                    Assert.That(ExpanderHelper.GetToggleButtonBackgroundMouseOver(expander), Is.Not.SameAs(ExpanderHelper.GetToggleButtonBackground(expander)), "and the square behind the chevron is what answers");
                });
        }

        [Test]
        [Description("The WinUI expander is rounded the way that set rounds a control, and the Windows 10 one is not rounded at all.")]
        public void TheWinUIExpanderIsRoundedAndTheWindows10OneIsNot()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetCornerRadius(this.Show("MahApps.Styles.Expander.Win10")), Is.EqualTo(new CornerRadius(0)));
                    Assert.That(ControlsHelper.GetCornerRadius(this.Show("MahApps.Styles.Expander.WinUI")), Is.EqualTo((CornerRadius)Application.Current.FindResource("MahApps.CornerRadius.WinUI.Control")));
                });
        }

        [TestCase("MahApps.Styles.Expander.Win10")]
        [TestCase("MahApps.Styles.Expander.WinUI")]
        [Description("Nothing in the Windows sets is set in capitals, so the header is written the way it was given.")]
        public void TheHeaderIsWrittenAsItWasGiven(string key)
        {
            var expander = this.Show(key);

            Assert.That(ControlsHelper.GetContentCharacterCasing(expander), Is.EqualTo(CharacterCasing.Normal));
        }

        private Expander Show(string key)
        {
            Assert.That(this.window, Is.Not.Null);

            var expander = new Expander
                           {
                               Style = (Style)Application.Current.FindResource(key),
                               Header = "This text is in the header",
                               Content = new TextBlock { Text = "And this text is in the content area." },
                               IsExpanded = true,
                               Width = 320,
                               HorizontalAlignment = HorizontalAlignment.Left,
                               VerticalAlignment = VerticalAlignment.Top
                           };

            this.window!.Content = expander;
            this.Settle();

            return expander;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
