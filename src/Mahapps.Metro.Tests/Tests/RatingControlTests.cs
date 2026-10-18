// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class RatingControlTests : WindowTestFixture<TestWindow>
    {
        private RatingControl Show(RatingControl rating)
        {
            this.window!.Content = rating;
            this.window.UpdateLayout();
            return rating;
        }

        private static KeyEventArgs PressKey(RatingControl rating, Key key)
        {
            var args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(rating)!, 0, key)
                       {
                           RoutedEvent = Keyboard.KeyDownEvent
                       };
            rating.RaiseEvent(args);
            return args;
        }

        [TestCase(-5, -1)]
        [TestCase(0, 1)]
        [TestCase(0.4, 1)]
        [TestCase(3.5, 3.5)]
        [TestCase(9, 5)]
        public void ValueIsCoerced(double set, double expected)
            => Assert.That(new RatingControl { Value = set }.Value, Is.EqualTo(expected));

        [TestCase(-3, -1)]
        [TestCase(0, 0)]
        [TestCase(0.4, 0.4)]
        [TestCase(9, 5)]
        public void PlaceholderValueIsCoerced(double set, double expected)
            => Assert.That(new RatingControl { PlaceholderValue = set }.PlaceholderValue, Is.EqualTo(expected));

        [Test]
        public void MaxRatingIsAtLeastOne()
        {
            var rating = new RatingControl { MaxRating = 0 };
            Assert.That(rating.MaxRating, Is.EqualTo(1));

            rating.MaxRating = -4;
            Assert.That(rating.MaxRating, Is.EqualTo(1));
        }

        [Test]
        public void LoweringMaxRatingClampsBothValues()
        {
            var rating = new RatingControl { Value = 4, PlaceholderValue = 4.5, MaxRating = 3 };

            Assert.Multiple(() =>
                {
                    Assert.That(rating.Value, Is.EqualTo(3));
                    Assert.That(rating.PlaceholderValue, Is.EqualTo(3));
                });
        }

        [Test]
        public void RightFromUnsetTakesTheInitialSetValue()
        {
            var rating = this.Show(new RatingControl { InitialSetValue = 3, Value = -1 });
            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void LeftFromOneClearsTheRating()
        {
            var rating = this.Show(new RatingControl { Value = 1 });
            PressKey(rating, Key.Left);
            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void LeftFromOneStaysWhenClearIsDisabled()
        {
            var rating = this.Show(new RatingControl { IsClearEnabled = false, Value = 1 });
            PressKey(rating, Key.Left);
            Assert.That(rating.Value, Is.EqualTo(1));
        }

        [Test]
        public void RightAtTheMaximumStays()
        {
            var rating = this.Show(new RatingControl { Value = 5 });
            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(5));
        }

        [Test]
        public void HomeClearsAndEndMaxes()
        {
            var rating = this.Show(new RatingControl { Value = 3 });

            PressKey(rating, Key.Home);
            Assert.That(rating.Value, Is.EqualTo(-1));

            PressKey(rating, Key.End);
            Assert.That(rating.Value, Is.EqualTo(5));
        }

        [Test]
        public void LeftAndRightAreMirroredRightToLeft()
        {
            var rating = this.Show(new RatingControl { FlowDirection = FlowDirection.RightToLeft, Value = 2 });

            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(1));

            PressKey(rating, Key.Up);
            Assert.That(rating.Value, Is.EqualTo(2));
        }

        [Test]
        public void AFractionIsDroppedFirst()
        {
            var left = this.Show(new RatingControl { Value = 2.5 });
            PressKey(left, Key.Left);
            Assert.That(left.Value, Is.EqualTo(2));

            var right = this.Show(new RatingControl { Value = 2.5 });
            PressKey(right, Key.Right);
            Assert.That(right.Value, Is.EqualTo(3));
        }

        [Test]
        public void ReadOnlyIgnoresTheKeyboard()
        {
            var rating = this.Show(new RatingControl { IsReadOnly = true, Value = 2 });

            var args = PressKey(rating, Key.Right);

            Assert.Multiple(() =>
                {
                    Assert.That(rating.Value, Is.EqualTo(2));
                    Assert.That(args.Handled, Is.False);
                });
        }

        [Test]
        public void ValueChangedComesWithInputOnly()
        {
            var rating = this.Show(new RatingControl { Value = 2 });
            var changes = new List<(double Old, double New)>();
            rating.ValueChanged += (_, e) => changes.Add((e.OldValue, e.NewValue));

            rating.Value = 4;
            PressKey(rating, Key.Right);

            Assert.That(changes, Is.EqualTo(new[] { (4.0, 5.0) }));
        }

        private static StackPanel BackgroundItems(RatingControl rating)
            => rating.FindChild<StackPanel>("PART_RatingBackgroundStackPanel")!;

        private static StackPanel ForegroundItems(RatingControl rating)
            => rating.FindChild<StackPanel>("PART_RatingForegroundStackPanel")!;

        private static double[] ClipWidths(RatingControl rating)
            => ForegroundItems(rating).Children.Cast<UIElement>().Select(item => ((RectangleGeometry)item.Clip).Rect.Width).ToArray();

        private static string? CurrentState(RatingControl rating)
        {
            var root = (FrameworkElement)VisualTreeHelper.GetChild(rating, 0);
            return VisualStateManager.GetVisualStateGroups(root)
                                     .Cast<VisualStateGroup>()
                                     .Single(group => group.Name == "CommonStates")
                                     .CurrentState?.Name;
        }

        private static void RaiseMouse(RatingControl rating, double x, RoutedEvent routedEvent)
        {
            // the position of a raised event can't be set, so the control reads it through a field the test replaces
            typeof(RatingControl).GetField("pointerPosition", BindingFlags.Instance | BindingFlags.NonPublic)!
                                 .SetValue(rating, new Func<MouseEventArgs, IInputElement, Point>((_, _) => new Point(x, 8)));

            var args = routedEvent == UIElement.MouseLeftButtonDownEvent || routedEvent == UIElement.MouseLeftButtonUpEvent
                ? new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                : new MouseEventArgs(Mouse.PrimaryDevice, 0);
            args.RoutedEvent = routedEvent;
            BackgroundItems(rating).RaiseEvent(args);
        }

        private static void Move(RatingControl rating, double x)
        {
            RaiseMouse(rating, x, Mouse.MouseEnterEvent);
            RaiseMouse(rating, x, Mouse.MouseMoveEvent);
        }

        private static void Press(RatingControl rating, double x) => RaiseMouse(rating, x, UIElement.MouseLeftButtonDownEvent);

        private static void Release(RatingControl rating, double x) => RaiseMouse(rating, x, UIElement.MouseLeftButtonUpEvent);

        private static void Click(RatingControl rating, double x)
        {
            Press(rating, x);
            Release(rating, x);
        }

        // the stars are 16 wide and 8 apart, so the 3rd one spans 48 to 64
        private const double ThirdStar = 56;

        [Test]
        public void FiveStarsAreStamped()
        {
            var rating = this.Show(new RatingControl());

            Assert.Multiple(() =>
                {
                    Assert.That(BackgroundItems(rating).Children, Has.Count.EqualTo(5));
                    Assert.That(ForegroundItems(rating).Children, Has.Count.EqualTo(5));
                });
        }

        [Test]
        public void ChangingMaxRatingRestampsTheItems()
        {
            var rating = this.Show(new RatingControl());

            rating.MaxRating = 3;

            Assert.Multiple(() =>
                {
                    Assert.That(BackgroundItems(rating).Children, Has.Count.EqualTo(3));
                    Assert.That(ForegroundItems(rating).Children, Has.Count.EqualTo(3));
                });
        }

        [TestCase(2.5, new[] { 32.0, 32.0, 16.0, 0.0, 0.0 })]
        [TestCase(-1, new[] { 0.0, 0.0, 0.0, 0.0, 0.0 })]
        public void TheClipFollowsTheValue(double value, double[] widths)
        {
            var rating = this.Show(new RatingControl { Value = value });

            Assert.That(ClipWidths(rating), Is.EqualTo(widths));
        }

        [Test]
        public void ThePlaceholderShowsWhenUnset()
        {
            var rating = this.Show(new RatingControl { Value = -1, PlaceholderValue = 3.5 });

            Assert.Multiple(() =>
                {
                    Assert.That(ClipWidths(rating), Is.EqualTo(new[] { 32.0, 32.0, 32.0, 16.0, 0.0 }));
                    Assert.That(CurrentState(rating), Is.EqualTo("Placeholder"));
                });
        }

        [Test]
        public void TheGlyphsFallBack()
        {
            var rating = this.Show(new RatingControl { ItemInfo = new RatingItemFontInfo { Glyph = "A", PlaceholderGlyph = "P", PointerOverPlaceholderGlyph = string.Empty } });

            var unset = BackgroundItems(rating).Children.Cast<TextBlock>().Select(item => item.Text).ToList();
            Move(rating, ThirdStar);
            var pointerOverPlaceholder = ForegroundItems(rating).Children.Cast<TextBlock>().Select(item => item.Text).ToList();

            Assert.Multiple(() =>
                {
                    Assert.That(unset, Is.All.EqualTo("A"));
                    Assert.That(CurrentState(rating), Is.EqualTo("PointerOverPlaceholder"));
                    Assert.That(pointerOverPlaceholder, Is.All.EqualTo("P"));
                });
        }

        [Test]
        public void TheImagesFallBack()
        {
            var image = new DrawingImage();
            var rating = this.Show(new RatingControl { ItemInfo = new RatingItemImageInfo { Image = image } });

            Assert.That(BackgroundItems(rating).Children.Cast<Image>().Select(item => item.Source), Is.All.SameAs(image));
        }

        [Test]
        public void SwitchingToAnImageInfoStampsImages()
        {
            var rating = this.Show(new RatingControl());

            rating.ItemInfo = new RatingItemImageInfo { Image = new DrawingImage() };

            var items = BackgroundItems(rating).Children.Cast<UIElement>().Concat(ForegroundItems(rating).Children.Cast<UIElement>()).ToList();
            Assert.Multiple(() =>
                {
                    Assert.That(items, Has.Count.EqualTo(10));
                    Assert.That(items, Is.All.InstanceOf<Image>());
                    Assert.That(items.OfType<Image>().Select(item => (item.Width, item.Height)), Is.All.EqualTo((32.0, 32.0)));
                });
        }

        [Test]
        public void ChangingAGlyphOfAnUnfrozenInfoShowsAtOnce()
        {
            var info = new RatingItemFontInfo { Glyph = "A" };
            var rating = this.Show(new RatingControl { ItemInfo = info });

            info.Glyph = "B";

            Assert.Multiple(() =>
                {
                    Assert.That(BackgroundItems(rating).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("B"));
                    Assert.That(ForegroundItems(rating).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("B"));
                });
        }

        [Test]
        public void TwoControlsShareTheItemInfoOfTheStyle()
        {
            var first = new RatingControl();
            var second = new RatingControl();
            this.window!.Content = new StackPanel { Children = { first, second } };
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(first.ItemInfo, Is.SameAs(second.ItemInfo));
                    Assert.That(first.ItemInfo!.IsFrozen, Is.True);
                    Assert.That(ForegroundItems(first).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("\uE735"));
                    Assert.That(ForegroundItems(second).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("\uE735"));
                    Assert.That(BackgroundItems(second).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("\uE734"));
                });
        }

        [Test]
        public void AClickSetsTheStarUnderThePointer()
        {
            var rating = this.Show(new RatingControl());

            Click(rating, ThirdStar);

            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void AClickOnTheCurrentStarClears()
        {
            var rating = this.Show(new RatingControl { Value = 3 });

            Click(rating, ThirdStar);

            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void AClickOnTheCurrentStarStaysWithoutClear()
        {
            var rating = this.Show(new RatingControl { Value = 3, IsClearEnabled = false });

            Click(rating, ThirdStar);

            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void ReleasingLeftOfTheStarsClears()
        {
            var rating = this.Show(new RatingControl { Value = 3 });

            Press(rating, 32);
            Release(rating, -10);

            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void ReadOnlyIgnoresTheMouse()
        {
            var rating = this.Show(new RatingControl { Value = 2, IsReadOnly = true });

            Move(rating, ThirdStar);
            var widths = ClipWidths(rating);
            Click(rating, ThirdStar);

            Assert.Multiple(() =>
                {
                    Assert.That(rating.Value, Is.EqualTo(2));
                    Assert.That(widths, Is.EqualTo(new[] { 32.0, 32.0, 0.0, 0.0, 0.0 }));
                });
        }

        private sealed class RatingSource
        {
            public double Rating { get; set; } = -1;
        }

        [Test]
        public void AClickUpdatesABoundValue()
        {
            var source = new RatingSource();
            var rating = new RatingControl();
            rating.SetBinding(RatingControl.ValueProperty, new Binding(nameof(RatingSource.Rating)) { Source = source, Mode = BindingMode.TwoWay });
            this.Show(rating);

            Click(rating, ThirdStar);

            Assert.That(source.Rating, Is.EqualTo(3));
        }

        // a click given in window coordinates, which the stub maps onto whatever element the control asks for,
        // so measuring against the wrong element or mirroring the wrong way shows up as the wrong star
        private void ClickInWindow(RatingControl rating, double xInWindow)
        {
            var window = this.window!;
            var y = rating.TransformToAncestor(window).Transform(new Point(0, rating.ActualHeight / 2)).Y;
            var position = new Func<MouseEventArgs, IInputElement, Point>((_, relativeTo) => window.TranslatePoint(new Point(xInWindow, y), (UIElement)relativeTo));
            var field = typeof(RatingControl).GetField("pointerPosition", BindingFlags.Instance | BindingFlags.NonPublic)!;

            foreach (var routedEvent in new[] { UIElement.MouseLeftButtonDownEvent, UIElement.MouseLeftButtonUpEvent })
            {
                field.SetValue(rating, position);
                BackgroundItems(rating).RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = routedEvent });
            }
        }

        private Rect BoundsInWindow(FrameworkElement element)
            => element.TransformToAncestor(this.window!).TransformBounds(new Rect(element.RenderSize));

        [Test]
        public void AClickInAStretchedControlHitsTheStarUnderThePointer()
        {
            var width = this.window!.Width;
            try
            {
                this.window.Width = 600;
                var rating = this.Show(new RatingControl { HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(100, 0, 0, 0) });
                var bounds = this.BoundsInWindow(rating);

                this.ClickInWindow(rating, bounds.Left + ThirdStar);

                Assert.Multiple(() =>
                    {
                        Assert.That(bounds.Width, Is.GreaterThan(300));
                        Assert.That(rating.Value, Is.EqualTo(3));
                    });
            }
            finally
            {
                this.window.Width = width;
            }
        }

        [Test]
        public void AClickRightToLeftHitsTheStarUnderThePointer()
        {
            var rating = this.Show(new RatingControl { FlowDirection = FlowDirection.RightToLeft, Margin = new Thickness(100, 0, 0, 0) });

            // right to left the stars count from the right edge
            this.ClickInWindow(rating, this.BoundsInWindow(rating).Right - ThirdStar);

            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void HoverShowsThePreview()
        {
            var rating = this.Show(new RatingControl { Value = -1 });

            // the 4th star spans 72 to 88
            Move(rating, 80);

            Assert.Multiple(() =>
                {
                    Assert.That(ClipWidths(rating), Is.EqualTo(new[] { 32.0, 32.0, 32.0, 32.0, 0.0 }));
                    Assert.That(CurrentState(rating), Is.EqualTo("PointerOverPlaceholder"));
                });
        }

        // the brush properties are internal, the styles of the library are the only ones setting them
        private static DependencyProperty StateBrushProperty(string name)
            => (DependencyProperty)typeof(RatingControl).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

        [Test]
        public void TheBrushesComeFromTheStyleOrFallBackToForeground()
        {
            var rating = this.Show(new RatingControl { Value = 3 });

            // without a brush for the state the items keep the Foreground of the control
            rating.SetValue(StateBrushProperty("SelectedBrushProperty"), null);
            rating.SetValue(StateBrushProperty("UnselectedBrushProperty"), null);
            var fallback = (TextElement.GetForeground(ForegroundItems(rating)), TextElement.GetForeground(BackgroundItems(rating)));

            rating.ClearValue(StateBrushProperty("SelectedBrushProperty"));
            rating.ClearValue(StateBrushProperty("UnselectedBrushProperty"));
            rating.Resources["MahApps.Brushes.RatingControl.Selected"] = Brushes.Red;
            rating.Resources["MahApps.Brushes.RatingControl.Unselected"] = Brushes.Green;

            Assert.Multiple(() =>
                {
                    Assert.That(fallback, Is.EqualTo((rating.Foreground, rating.Foreground)));
                    Assert.That(TextElement.GetForeground(ForegroundItems(rating)), Is.SameAs(Brushes.Red));
                    Assert.That(TextElement.GetForeground(BackgroundItems(rating)), Is.SameAs(Brushes.Green));
                });
        }

        [Test]
        public void TheCaptionTakesTheTextFontNotTheSymbolFont()
        {
            var rating = this.Show(new RatingControl { Caption = "42 ratings" });

            var caption = rating.FindChild<TextBlock>("PART_Caption")!;

            Assert.That(caption.FontFamily, Is.Not.EqualTo(rating.FindResource("MahApps.Fonts.Family.SymbolTheme")));
        }

        private static double[] Scales(StackPanel panel)
            => panel.Children.Cast<UIElement>().Select(item => ((ScaleTransform)item.RenderTransform).ScaleX).ToArray();

        private RatingControl ShowWithStyle(string styleKey)
            => this.Show(new RatingControl { Style = (Style)this.window!.FindResource(styleKey) });

        [Test]
        public void TheWin10StarsGrowUnderThePointer()
        {
            var rating = this.ShowWithStyle("MahApps.Styles.RatingControl.Win10");

            Move(rating, ThirdStar);

            Assert.Multiple(() =>
                {
                    Assert.That(Scales(BackgroundItems(rating))[2], Is.EqualTo(0.8).Within(1e-9));
                    Assert.That(Scales(ForegroundItems(rating))[2], Is.EqualTo(0.8).Within(1e-9));
                    Assert.That(Scales(BackgroundItems(rating))[0], Is.EqualTo(0.5));
                    Assert.That(Scales(ForegroundItems(rating))[0], Is.EqualTo(0.5));
                    // 24 off the centre: -0.0005 * 0.8 * 24 * 24 + 0.8
                    Assert.That(Scales(BackgroundItems(rating))[3], Is.EqualTo(0.5696).Within(1e-9));
                });
        }

        [Test]
        public void TheWin10StarsShrinkBackOnLeave()
        {
            var rating = this.ShowWithStyle("MahApps.Styles.RatingControl.Win10");

            Move(rating, ThirdStar);
            RaiseMouse(rating, ThirdStar, Mouse.MouseLeaveEvent);

            Assert.Multiple(() =>
                {
                    Assert.That(Scales(BackgroundItems(rating)), Is.All.EqualTo(0.5));
                    Assert.That(Scales(ForegroundItems(rating)), Is.All.EqualTo(0.5));
                });
        }

        [Test]
        public void TheStarsShrinkBackOnLeaveAfterTheLookChanged()
        {
            var rating = this.ShowWithStyle("MahApps.Styles.RatingControl.Win10");

            Move(rating, ThirdStar);
            rating.Style = (Style)this.window!.FindResource("MahApps.Styles.RatingControl");
            RaiseMouse(rating, ThirdStar, Mouse.MouseLeaveEvent);

            Assert.That(Scales(BackgroundItems(rating)).Concat(Scales(ForegroundItems(rating))), Is.All.EqualTo(0.5));
        }

        [TestCase("MahApps.Styles.RatingControl")]
        [TestCase("MahApps.Styles.RatingControl.WinUI")]
        public void TheOtherLooksDoNotGrow(string styleKey)
        {
            var rating = this.ShowWithStyle(styleKey);

            Move(rating, ThirdStar);

            Assert.Multiple(() =>
                {
                    Assert.That(Scales(BackgroundItems(rating)), Is.All.EqualTo(0.5));
                    Assert.That(Scales(ForegroundItems(rating)), Is.All.EqualTo(0.5));
                });
        }

        [Test]
        public void TheWin10UnsetStarIsTheFilledGlyph()
        {
            var rating = this.ShowWithStyle("MahApps.Styles.RatingControl.Win10");

            Assert.That(BackgroundItems(rating).Children.Cast<TextBlock>().Select(item => item.Text), Is.All.EqualTo("\uE735"));
        }

        [TestCase("MahApps.Styles.RatingControl", "MahApps.Fonts.Family.Control", 14.0)]
        [TestCase("MahApps.Styles.RatingControl.Win10", "MahApps.Fonts.Family.Control.Win10", 12.0)]
        [TestCase("MahApps.Styles.RatingControl.WinUI", "MahApps.Fonts.Family.Caption.WinUI", 12.0)]
        public void TheCaptionTakesTheFontOfTheLook(string styleKey, string fontKey, double fontSize)
        {
            var rating = this.ShowWithStyle(styleKey);
            rating.Caption = "42 ratings";
            this.window!.UpdateLayout();

            var caption = rating.FindChild<TextBlock>("PART_Caption")!;

            Assert.Multiple(() =>
                {
                    Assert.That(caption.FontFamily, Is.SameAs(rating.FindResource(fontKey)));
                    Assert.That(caption.FontSize, Is.EqualTo(fontSize));
                });
        }

        [TestCase("MahApps.Styles.RatingControl.Win10", "MahApps.Styles.FocusVisualStyle.Win10")]
        [TestCase("MahApps.Styles.RatingControl.WinUI", "MahApps.Styles.FocusVisualStyle.WinUI")]
        public void TheFocusFollowsTheLook(string styleKey, string focusVisual)
        {
            var rating = this.ShowWithStyle(styleKey);

            Assert.Multiple(() =>
                {
                    Assert.That(rating.FocusVisualStyle, Is.Null);
                    Assert.That(FocusVisualHelper.GetFocusVisualStyle(rating), Is.SameAs(rating.FindResource(focusVisual)));
                });
        }

        [TestCase("MahApps.Sizes.RatingControl.FontSizeForRendering", 32.0)]
        [TestCase("MahApps.Sizes.RatingControl.ItemSpacing", 8.0)]
        public void TheSizesAreResources(string key, double expected)
        {
            var rating = this.Show(new RatingControl());

            Assert.That(rating.TryFindResource(key), Is.EqualTo(expected));
        }

        [Test]
        public void LosingTheCaptureToSomeoneElseEndsThePress()
        {
            var rating = this.Show(new RatingControl());

            Move(rating, ThirdStar);
            Press(rating, ThirdStar);

            // another element takes the capture while the button is already up, so no button up comes here
            RaiseMouse(rating, ThirdStar, Mouse.LostMouseCaptureEvent);
            RaiseMouse(rating, ThirdStar, Mouse.MouseLeaveEvent);

            Assert.That(ClipWidths(rating), Is.All.EqualTo(0.0));
        }

        // the 3rd star spans 48 to 64, so 52 is in its left half and 60 in its right one
        private const double ThirdStarLeftHalf = 52;
        private const double ThirdStarRightHalf = 60;

        [Test]
        public void AClickOnTheLeftHalfGivesAHalfStar()
        {
            var left = this.Show(new RatingControl { IsHalfStepEnabled = true });
            Click(left, ThirdStarLeftHalf);
            Assert.That(left.Value, Is.EqualTo(2.5));

            var right = this.Show(new RatingControl { IsHalfStepEnabled = true });
            Click(right, ThirdStarRightHalf);
            Assert.That(right.Value, Is.EqualTo(3));
        }

        [Test]
        public void AClickInTheGapGivesTheFullStarBefore()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true });

            // the gap between the 3rd and the 4th star spans 64 to 72
            Click(rating, 68);

            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void AClickWithoutHalfStepsStaysWhole()
        {
            var rating = this.Show(new RatingControl());

            Click(rating, ThirdStarLeftHalf);

            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [Test]
        public void HoverShowsTheHalfPreview()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = -1 });

            Move(rating, ThirdStarLeftHalf);

            Assert.That(ClipWidths(rating), Is.EqualTo(new[] { 32.0, 32.0, 16.0, 0.0, 0.0 }));
        }

        [Test]
        public void AClickOnTheCurrentHalfClears()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 2.5 });

            Click(rating, ThirdStarLeftHalf);

            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void ReleasingLeftOfTheStarsClearsWithHalfSteps()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 3 });

            Press(rating, 32);
            Release(rating, -10);

            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void TheKeysCountInHalves()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 2 });

            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(2.5));

            PressKey(rating, Key.Left);
            Assert.That(rating.Value, Is.EqualTo(2));

            PressKey(rating, Key.Up);
            Assert.That(rating.Value, Is.EqualTo(2.5));

            PressKey(rating, Key.Down);
            Assert.That(rating.Value, Is.EqualTo(2));
        }

        [Test]
        public void AFractionSnapsToAHalfFirst()
        {
            var left = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 2.3 });
            PressKey(left, Key.Left);
            Assert.That(left.Value, Is.EqualTo(2));

            var right = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 2.3 });
            PressKey(right, Key.Right);
            Assert.That(right.Value, Is.EqualTo(2.5));
        }

        [Test]
        public void TheHalfKeysAreMirroredRightToLeft()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, FlowDirection = FlowDirection.RightToLeft, Value = 2 });

            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(1.5));

            PressKey(rating, Key.Left);
            Assert.That(rating.Value, Is.EqualTo(2));

            PressKey(rating, Key.Up);
            Assert.That(rating.Value, Is.EqualTo(2.5));
        }

        [Test]
        public void RightFromUnsetTakesTheInitialSetValueWithHalfSteps()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, InitialSetValue = 3, Value = -1 });
            PressKey(rating, Key.Right);
            Assert.That(rating.Value, Is.EqualTo(3));
        }

        [TestCase(true, 0.2, 0.5)]
        [TestCase(true, 0, 0.5)]
        [TestCase(true, 3.3, 3.3)]
        [TestCase(false, 0.2, 1)]
        public void ValueCoercesToAHalfAtTheLeast(bool isHalfStepEnabled, double set, double expected)
            => Assert.That(new RatingControl { IsHalfStepEnabled = isHalfStepEnabled, Value = set }.Value, Is.EqualTo(expected));

        [Test]
        public void ChangingIsHalfStepEnabledCoercesTheValueAgain()
        {
            var rating = new RatingControl { IsHalfStepEnabled = true, Value = 0.5 };

            rating.IsHalfStepEnabled = false;
            Assert.That(rating.Value, Is.EqualTo(1));

            rating.IsHalfStepEnabled = true;
            Assert.That(rating.Value, Is.EqualTo(0.5));
        }

        [Test]
        public void ClearDisabledStopsAtAHalf()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, IsClearEnabled = false, Value = 0.5 });

            PressKey(rating, Key.Left);

            Assert.That(rating.Value, Is.EqualTo(0.5));
        }

        [Test]
        public void ARightToLeftClickOnTheHalfNearerTheStartGivesAHalfStar()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, FlowDirection = FlowDirection.RightToLeft, Margin = new Thickness(100, 0, 0, 0) });

            // right to left the row starts at the right edge, so the half nearer the start is the right half on screen
            this.ClickInWindow(rating, this.BoundsInWindow(rating).Right - ThirdStarLeftHalf);

            Assert.That(rating.Value, Is.EqualTo(2.5));
        }

        private RatingControl ShowWin10WithHalfSteps()
        {
            var rating = this.ShowWithStyle("MahApps.Styles.RatingControl.Win10");
            rating.IsHalfStepEnabled = true;
            return rating;
        }

        // the Win10 look grows the stars near the pointer about their centres, which moves the drawn left edge of the
        // 1st star, at 7 by 4.8 and at 31 by 1.4, so measured from that edge both would land in the next half
        [TestCase(7, 0.5, new[] { 16.0, 0.0, 0.0, 0.0, 0.0 })]
        [TestCase(28, 1.5, new[] { 32.0, 16.0, 0.0, 0.0, 0.0 })]
        [TestCase(31, 1.5, new[] { 32.0, 16.0, 0.0, 0.0, 0.0 })]
        [TestCase(36, 2, new[] { 32.0, 32.0, 0.0, 0.0, 0.0 })]
        public void AGrownFirstStarDoesNotShiftTheHalves(double x, double expected, double[] widths)
        {
            var rating = this.ShowWin10WithHalfSteps();

            Move(rating, x);
            var preview = ClipWidths(rating);
            Click(rating, x);

            Assert.Multiple(() =>
                {
                    Assert.That(preview, Is.EqualTo(widths));
                    Assert.That(rating.Value, Is.EqualTo(expected));
                });
        }

        [Test]
        public void ReleasingAtTheLeftEdgeClearsWithHalfSteps()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = 3 });

            Press(rating, 32);
            Release(rating, 0);

            Assert.That(rating.Value, Is.EqualTo(-1));
        }

        [Test]
        public void NoHalfPreviewBeforeTheFirstMove()
        {
            var rating = this.Show(new RatingControl { IsHalfStepEnabled = true, Value = -1 });

            RaiseMouse(rating, ThirdStarLeftHalf, Mouse.MouseEnterEvent);
            rating.PlaceholderValue = 2;

            Assert.That(ClipWidths(rating), Is.All.EqualTo(0.0));
        }
    }
}
