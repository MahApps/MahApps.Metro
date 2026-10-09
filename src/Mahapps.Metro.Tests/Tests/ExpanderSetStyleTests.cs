// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ExpanderSetStyleTests : WindowTestFixture<TestWindow>
    {
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

        [TestCase(ExpandDirection.Down)]
        [TestCase(ExpandDirection.Up)]
        [TestCase(ExpandDirection.Right)]
        [TestCase(ExpandDirection.Left)]
        [Description("Every direction plays the same two storyboards, since which way the content travels comes from the direction the expander opens rather than from the storyboard.")]
        public void EveryDirectionPlaysTheSameStoryboards(ExpandDirection direction)
        {
            var expander = this.Show("MahApps.Styles.Expander.Win10");
            expander.SetCurrentValue(Expander.ExpandDirectionProperty, direction);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(ExpanderHelper.GetExpandStoryboard(expander), Is.SameAs(Application.Current.FindResource("MahApps.Storyboard.Expander.Win10.Expand")));
                    Assert.That(ExpanderHelper.GetCollapseStoryboard(expander), Is.SameAs(Application.Current.FindResource("MahApps.Storyboard.Expander.Win10.Collapse")));
                });
        }

        [TestCase("MahApps.Styles.Expander.Win10", ExpandDirection.Down, 0, -1)]
        [TestCase("MahApps.Styles.Expander.Win10", ExpandDirection.Up, 0, 1)]
        [TestCase("MahApps.Styles.Expander.Win10", ExpandDirection.Right, -1, 0)]
        [TestCase("MahApps.Styles.Expander.Win10", ExpandDirection.Left, 1, 0)]
        [TestCase("MahApps.Styles.Expander.WinUI", ExpandDirection.Down, 0, -1)]
        [TestCase("MahApps.Styles.Expander.WinUI", ExpandDirection.Up, 0, 1)]
        [Description("WinUI slides the content out from behind the header by the whole of its height, from ContentHeight or NegativeContentHeight of its TemplateSettings to 0. Here the storyboard says how much of the content is still behind the header, and the content moves by that much of its own size, towards the header.")]
        public void TheContentStartsWhollyBehindTheHeader(string key, ExpandDirection direction, int x, int y)
        {
            var expander = this.Show(key);
            expander.SetCurrentValue(Expander.ExpandDirectionProperty, direction);
            this.Settle();

            // what the opening left running is set aside, so that the value alone says where the content stands
            var site = expander.FindChild<Border>("ExpandSite")!;
            site.BeginAnimation(ExpanderHelper.ContentSlideProperty, null);
            ExpanderHelper.SetContentSlide(site, 1);
            this.Settle();

            var transform = (TranslateTransform)site.RenderTransform;

            Assert.Multiple(() =>
                {
                    Assert.That(transform.X, Is.EqualTo(x * site.ActualWidth).Within(0.01), "across");
                    Assert.That(transform.Y, Is.EqualTo(y * site.ActualHeight).Within(0.01), "down");
                });

            ExpanderHelper.SetContentSlide(site, 0);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(transform.X, Is.Zero, "and in its place once it has arrived");
                    Assert.That(transform.Y, Is.Zero);
                });
        }

        [Test]
        [Description("Out over a third of a second and back over a sixth, on the splines of the ExpandStates of WinUI: all deceleration on the way out, all acceleration on the way back. The content is gone at 0.2, as in CollapseDown.")]
        public void TheContentKeepsTheTimesOfWinUI()
        {
            var expand = Slide("MahApps.Storyboard.Expander.Win10.Expand");
            var collapse = Slide("MahApps.Storyboard.Expander.Win10.Collapse");
            var gone = Animations<ObjectAnimationUsingKeyFrames>("MahApps.Storyboard.Expander.Win10.Collapse").Single().KeyFrames.Cast<ObjectKeyFrame>().Last();

            Assert.Multiple(() =>
                {
                    Assert.That(expand.KeyFrames[0].Value, Is.EqualTo(1), "out from wholly behind the header");
                    Assert.That(expand.KeyFrames[1].Value, Is.Zero);
                    Assert.That(expand.KeyFrames[1].KeyTime.TimeSpan, Is.EqualTo(TimeSpan.FromSeconds(0.333)));
                    Assert.That(((SplineDoubleKeyFrame)expand.KeyFrames[1]).KeySpline.ToString(), Is.EqualTo(new KeySpline(0, 0, 0, 1).ToString()));

                    Assert.That(collapse.KeyFrames[0].Value, Is.Zero, "back from where it stands");
                    Assert.That(collapse.KeyFrames[1].Value, Is.EqualTo(1));
                    Assert.That(collapse.KeyFrames[1].KeyTime.TimeSpan, Is.EqualTo(TimeSpan.FromSeconds(0.167)));
                    Assert.That(((SplineDoubleKeyFrame)collapse.KeyFrames[1]).KeySpline.ToString(), Is.EqualTo(new KeySpline(1, 1, 0, 1).ToString()));

                    Assert.That(gone.Value, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(gone.KeyTime.TimeSpan, Is.EqualTo(TimeSpan.FromSeconds(0.2)), "gone at the time CollapseDown takes it away");
                });
        }

        [TestCase("MahApps.Storyboard.Expander.Win10.Expand")]
        [TestCase("MahApps.Storyboard.Expander.Win10.Collapse")]
        [Description("WinUI does not fade the content, it only moves it, so neither storyboard takes its opacity anywhere but to 1.")]
        public void TheContentDoesNotFade(string key)
        {
            var opacity = Animations<DoubleAnimationUsingKeyFrames>(key).Where(a => Storyboard.GetTargetProperty(a).Path == "(UIElement.Opacity)");

            Assert.Multiple(() =>
                {
                    Assert.That(Animations<DoubleAnimation>(key), Is.Empty, "no fade");
                    Assert.That(opacity.SelectMany(a => a.KeyFrames.Cast<DoubleKeyFrame>()).Select(f => f.Value), Is.All.EqualTo(1));
                });
        }

        [TestCase("MahApps.Styles.ToggleButton.ExpanderHeader.Win10.Down")]
        [TestCase("MahApps.Styles.ToggleButton.ExpanderHeader.WinUI.Down")]
        [Description("The chevron turns in a tenth of a second at an even pace, the way the Checked state of the WinUI 2 header turns ExpandCollapseChevronRotateTransform. WinUI 3 plays a Lottie animation there, which WPF has nothing for.")]
        public void TheChevronTurnsInATenthOfASecond(string key)
        {
            var style = (Style)Application.Current.FindResource(key);
            var template = (ControlTemplate)Setters(style).First(s => s.Property == Control.TemplateProperty).Value;
            var trigger = template.Triggers.OfType<Trigger>().Single(t => t.Property == ToggleButton.IsCheckedProperty);
            var turns = trigger.EnterActions.Concat(trigger.ExitActions).OfType<BeginStoryboard>().SelectMany(b => b.Storyboard.Children).OfType<DoubleAnimation>().ToList();

            Assert.That(turns, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
                {
                    Assert.That(turns.Select(a => a.Duration.TimeSpan), Is.All.EqualTo(TimeSpan.FromSeconds(0.1)));
                    Assert.That(turns.Select(a => a.EasingFunction), Is.All.Null, "at an even pace");
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
        [Description("And it really travels: opening the expander animates how much of the content still stands behind the header.")]
        public void OpeningTheExpanderSetsTheContentTravelling(string key)
        {
            var expander = this.Show(key);
            expander.SetCurrentValue(Expander.IsExpandedProperty, false);
            this.Settle();

            expander.SetCurrentValue(Expander.IsExpandedProperty, true);
            this.Settle();

            var site = expander.FindChild<Border>("ExpandSite");

            Assert.That(site, Is.Not.Null);
            Assert.That(DependencyPropertyHelper.GetValueSource(site!, ExpanderHelper.ContentSlideProperty).IsAnimated, Is.True);
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

        private static DoubleAnimationUsingKeyFrames Slide(string key)
        {
            return Animations<DoubleAnimationUsingKeyFrames>(key).Single(a => Storyboard.GetTargetProperty(a).PathParameters.Contains(ExpanderHelper.ContentSlideProperty));
        }

        private static IEnumerable<T> Animations<T>(string key)
            where T : Timeline
        {
            return ((Storyboard)Application.Current.FindResource(key)).Children.OfType<T>();
        }

        private static IEnumerable<Setter> Setters(Style style)
        {
            for (var s = style; s is not null; s = s.BasedOn)
            {
                foreach (var setter in s.Setters.OfType<Setter>())
                {
                    yield return setter;
                }
            }
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
