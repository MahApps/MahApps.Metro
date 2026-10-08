// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class ContentDialogTests : WindowTestFixture<ContentDialogWindow>
    {
        private const string Metro = "MahApps.Styles.ContentDialog";
        private const string Win10 = "MahApps.Styles.ContentDialog.Win10";
        private const string WinUI = "MahApps.Styles.ContentDialog.WinUI";

        private Grid ActiveContainer => this.window!.FindChild<Grid>("PART_MetroActiveDialogContainer")!;

        private static ContentDialog NewDialog(string styleKey, string? primary = "42", string? secondary = "43", string? close = "Cancel")
        {
            return new ContentDialog
                   {
                       Title = "A question",
                       Content = new TextBlock { Text = "What is the answer?" },
                       PrimaryButtonText = primary,
                       SecondaryButtonText = secondary,
                       CloseButtonText = close,
                       Style = (Style)Application.Current.FindResource(styleKey)
                   };
        }

        private Task<ContentDialogResult> ShowDialog(ContentDialog dialog)
        {
            var showing = this.window!.ShowContentDialogAsync(dialog);
            ClipAssert.PumpUntil(() => dialog.FindChild<Grid>("PART_Overlay")?.Visibility == Visibility.Visible);
            return showing;
        }

        private static async Task<ContentDialogResult> ClosedWith(Task<ContentDialogResult> showing)
        {
            await showing.Within("the content dialog to close");
            return showing.Result;
        }

        private static Button Part(ContentDialog dialog, string name) => dialog.FindChild<Button>(name)!;

        private static void Click(ContentDialog dialog, string part)
        {
            var button = Part(dialog, part);
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
            ClipAssert.Pump();
        }

        private static void PressKey(UIElement target, Key key)
        {
            var source = PresentationSource.FromVisual(target)!;
            target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, key) { RoutedEvent = Keyboard.KeyDownEvent });
            ClipAssert.Pump();
        }

        [TearDown]
        public void CloseAllDialogs()
        {
            foreach (var dialog in this.ActiveContainer.Children.OfType<ContentDialog>().ToList())
            {
                dialog.Hide();
            }

            ClipAssert.PumpUntil(() => !this.ActiveContainer.Children.OfType<ContentDialog>().Any(), 2000);
        }

        [TestCase(Metro, "PART_PrimaryButton", ContentDialogResult.Primary)]
        [TestCase(Win10, "PART_PrimaryButton", ContentDialogResult.Primary)]
        [TestCase(WinUI, "PART_PrimaryButton", ContentDialogResult.Primary)]
        [TestCase(Metro, "PART_SecondaryButton", ContentDialogResult.Secondary)]
        [TestCase(Win10, "PART_SecondaryButton", ContentDialogResult.Secondary)]
        [TestCase(WinUI, "PART_SecondaryButton", ContentDialogResult.Secondary)]
        [TestCase(Metro, "PART_CloseButton", ContentDialogResult.None)]
        [TestCase(Win10, "PART_CloseButton", ContentDialogResult.None)]
        [TestCase(WinUI, "PART_CloseButton", ContentDialogResult.None)]
        public async Task AButtonClosesWithItsResult(string style, string part, ContentDialogResult expected)
        {
            var dialog = NewDialog(style);
            var showing = this.ShowDialog(dialog);

            Click(dialog, part);

            Assert.That(await ClosedWith(showing), Is.EqualTo(expected));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task EscapeIsTheCloseButtonEvenWithoutText(string style)
        {
            var command = new CountingCommand();
            var clicked = 0;
            var dialog = NewDialog(style, close: null);
            dialog.CloseButtonCommand = command;
            dialog.CloseButtonClick += (_, _) => clicked++;
            var showing = this.ShowDialog(dialog);

            PressKey(dialog, Key.Escape);

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
            Assert.That(clicked, Is.EqualTo(1));
            Assert.That(command.Executed, Is.EqualTo(1));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task HideClosesWithNone(string style)
        {
            var dialog = NewDialog(style);
            var showing = this.ShowDialog(dialog);

            dialog.Hide();

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void ACanceledClickKeepsTheDialogOpen(string style)
        {
            var dialog = NewDialog(style);
            dialog.PrimaryButtonClick += (_, e) => e.Cancel = true;
            _ = this.ShowDialog(dialog);

            Click(dialog, "PART_PrimaryButton");

            Assert.That(dialog.IsOpen, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void ACommandThatCannotRunKeepsTheDialogOpen(string style)
        {
            var command = new CountingCommand { Allowed = false };
            var dialog = NewDialog(style);
            dialog.PrimaryButtonCommand = command;
            _ = this.ShowDialog(dialog);

            Click(dialog, "PART_PrimaryButton");

            Assert.That(dialog.IsOpen, Is.True);
            Assert.That(command.Executed, Is.EqualTo(0));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task PrimaryDoesNotRunTheCloseCommands(string style)
        {
            var close = new CountingCommand();
            var titleBar = new CountingCommand { Allowed = false };
            var dialog = NewDialog(style);
            dialog.CloseButtonCommand = close;
            dialog.TitleBarCloseButtonCommand = titleBar;
            var showing = this.ShowDialog(dialog);

            Click(dialog, "PART_PrimaryButton");

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Primary));
            Assert.That(close.Executed, Is.EqualTo(0));
            Assert.That(titleBar.Executed, Is.EqualTo(0));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task TheXIsTheCloseButton(string style)
        {
            var close = new CountingCommand();
            var titleBar = new CountingCommand { Allowed = false };
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = true;
            dialog.ShowTitleBarCloseButton = true;
            dialog.CloseButtonCommand = close;
            dialog.TitleBarCloseButtonCommand = titleBar;
            var showing = this.ShowDialog(dialog);

            Click(dialog, "PART_TitleBarCloseButton");

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
            Assert.That(close.Executed, Is.EqualTo(1));
            Assert.That(titleBar.Executed, Is.EqualTo(0));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task ClosingKnowsTheResultAndCanCancel(string style)
        {
            var dialog = NewDialog(style);
            ContentDialogResult? seen = null;
            var cancel = true;
            dialog.Closing += (_, e) =>
                {
                    seen = e.Result;
                    e.Cancel = cancel;
                };
            CancelEventArgs? inherited = null;
            ((ChildWindow)dialog).Closing += (_, e) => inherited = e;
            var showing = this.ShowDialog(dialog);

            Click(dialog, "PART_SecondaryButton");

            Assert.That(seen, Is.EqualTo(ContentDialogResult.Secondary));
            Assert.That(inherited, Is.InstanceOf<ContentDialogClosingEventArgs>());
            Assert.That(dialog.IsOpen, Is.True);

            cancel = false;
            Click(dialog, "PART_SecondaryButton");

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Secondary));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void AButtonWithoutTextIsNotThere(string style)
        {
            var dialog = NewDialog(style, secondary: null);
            _ = this.ShowDialog(dialog);

            Assert.That(Part(dialog, "PART_SecondaryButton").Visibility, Is.EqualTo(Visibility.Collapsed));
            Assert.That(Part(dialog, "PART_PrimaryButton").Visibility, Is.EqualTo(Visibility.Visible));
            Assert.That(Part(dialog, "PART_CloseButton").Visibility, Is.EqualTo(Visibility.Visible));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void TheEnabledSwitchesReachTheButtons(string style)
        {
            var dialog = NewDialog(style);
            dialog.IsPrimaryButtonEnabled = false;
            dialog.IsSecondaryButtonEnabled = false;
            _ = this.ShowDialog(dialog);

            Assert.That(Part(dialog, "PART_PrimaryButton").IsEnabled, Is.False);
            Assert.That(Part(dialog, "PART_SecondaryButton").IsEnabled, Is.False);
        }

        [TestCase(Metro, "MahApps.Styles.Button.Dialogs.Accent")]
        [TestCase(Win10, "MahApps.Styles.Button.Accent.Win10")]
        [TestCase(WinUI, "MahApps.Styles.Button.Accent.WinUI")]
        public void TheDefaultButtonIsTheAccentOne(string style, string accentKey)
        {
            var dialog = NewDialog(style);
            dialog.DefaultButton = ContentDialogButton.Secondary;
            _ = this.ShowDialog(dialog);

            Assert.That(Part(dialog, "PART_SecondaryButton").Style, Is.SameAs(Application.Current.FindResource(accentKey)));
            Assert.That(Part(dialog, "PART_PrimaryButton").Style, Is.Not.SameAs(Application.Current.FindResource(accentKey)));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task EnterClicksTheDefaultButton(string style)
        {
            var dialog = NewDialog(style);
            dialog.DefaultButton = ContentDialogButton.Primary;
            var showing = this.ShowDialog(dialog);

            PressKey(dialog, Key.Enter);

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Primary));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void EnterDoesNothingForADisabledDefaultButton(string style)
        {
            var dialog = NewDialog(style);
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.IsPrimaryButtonEnabled = false;
            _ = this.ShowDialog(dialog);

            PressKey(dialog, Key.Enter);

            Assert.That(dialog.IsOpen, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void EnterInAMultiLineBoxStaysThere(string style)
        {
            var box = new TextBox { AcceptsReturn = true };
            var dialog = NewDialog(style);
            dialog.Content = box;
            dialog.DefaultButton = ContentDialogButton.Primary;
            _ = this.ShowDialog(dialog);

            PressKey(box, Key.Enter);

            Assert.That(dialog.IsOpen, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void TheDefaultButtonHasTheFocusWhenTheContentHasNone(string style)
        {
            var dialog = NewDialog(style);
            dialog.DefaultButton = ContentDialogButton.Secondary;
            _ = this.ShowDialog(dialog);

            var secondary = Part(dialog, "PART_SecondaryButton");
            ClipAssert.PumpUntil(() => secondary.IsFocused);

            Assert.That(secondary.IsFocused, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void FocusableContentKeepsTheFocus(string style)
        {
            var box = new TextBox();
            var dialog = NewDialog(style);
            dialog.Content = box;
            dialog.DefaultButton = ContentDialogButton.Primary;
            _ = this.ShowDialog(dialog);

            ClipAssert.PumpUntil(() => box.IsFocused);

            Assert.That(box.IsFocused, Is.True);
        }

        [Test]
        public async Task ShowAsyncTakesTheGivenWindow()
        {
            var dialog = NewDialog(Metro);
            var showing = dialog.ShowAsync(this.window!);
            ClipAssert.PumpUntil(() => dialog.IsOpen);

            Assert.That(this.ActiveContainer.Children.Contains(dialog), Is.True);

            dialog.Hide();
            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
        }

        [Test]
        public async Task ShowAsyncFallsBackToTheMainWindow()
        {
            var application = Application.Current;
            var mainWindow = application.MainWindow;
            application.MainWindow = this.window;

            try
            {
                var dialog = NewDialog(Metro);
                var showing = dialog.ShowAsync();
                ClipAssert.PumpUntil(() => dialog.IsOpen);

                Assert.That(this.ActiveContainer.Children.Contains(dialog), Is.True);

                dialog.Hide();
                await ClosedWith(showing);
            }
            finally
            {
                application.MainWindow = mainWindow;
            }
        }

        [Test]
        public void TwoDialogsStack()
        {
            var first = NewDialog(Metro);
            var second = NewDialog(Metro);
            _ = this.ShowDialog(first);
            _ = this.ShowDialog(second);

            Assert.That(Panel.GetZIndex(second), Is.GreaterThan(Panel.GetZIndex(first)));
        }

        private static Rect Bounds(FrameworkElement element, FrameworkElement relativeTo)
        {
            return element.TransformToAncestor(relativeTo).TransformBounds(new Rect(element.RenderSize));
        }

        [TestCase(Win10, "CommandSpace")]
        [TestCase(WinUI, "CommandGrid")]
        public void ThreeButtonsShareTheRowInThirds(string style, string rowName)
        {
            var dialog = NewDialog(style);
            _ = this.ShowDialog(dialog);
            var row = dialog.FindChild<FrameworkElement>(rowName)!;

            var widths = new[] { "PART_PrimaryButton", "PART_SecondaryButton", "PART_CloseButton" }.Select(p => Part(dialog, p).ActualWidth).ToArray();

            // three equal columns; in the Windows 10 original the middle button gives up two more units to its margins,
            // and the layout rounding to whole device pixels may add one pixel on either side
            var pixel = 1 / System.Windows.Media.VisualTreeHelper.GetDpi(dialog).PixelsPerDip;
            Assert.That(widths.Max() - widths.Min(), Is.LessThanOrEqualTo(2 + 2 * pixel + 0.01), "three thirds");
            Assert.That(Bounds(Part(dialog, "PART_PrimaryButton"), row).Left, Is.LessThan(Bounds(Part(dialog, "PART_SecondaryButton"), row).Left));
            Assert.That(Bounds(Part(dialog, "PART_SecondaryButton"), row).Left, Is.LessThan(Bounds(Part(dialog, "PART_CloseButton"), row).Left));
        }

        [TestCase(Win10, "CommandSpace", null, null, "Cancel")]
        [TestCase(WinUI, "CommandGrid", null, null, "Cancel")]
        [TestCase(Win10, "CommandSpace", "42", null, null)]
        [TestCase(WinUI, "CommandGrid", "42", null, null)]
        [TestCase(Win10, "CommandSpace", null, "43", null)]
        [TestCase(WinUI, "CommandGrid", null, "43", null)]
        public void ASingleButtonTakesTheRightHalf(string style, string rowName, string? primary, string? secondary, string? close)
        {
            var dialog = NewDialog(style, primary, secondary, close);
            _ = this.ShowDialog(dialog);
            var row = dialog.FindChild<FrameworkElement>(rowName)!;
            var part = primary is not null ? "PART_PrimaryButton" : secondary is not null ? "PART_SecondaryButton" : "PART_CloseButton";

            var bounds = Bounds(Part(dialog, part), row);

            Assert.That(bounds.Left, Is.GreaterThanOrEqualTo(row.ActualWidth / 2 - 1));
            Assert.That(bounds.Right, Is.EqualTo(row.ActualWidth).Within(1.0));
        }

        [TestCase(Win10, "CommandSpace", "42", "43", null)]
        [TestCase(WinUI, "CommandGrid", "42", "43", null)]
        [TestCase(Win10, "CommandSpace", "42", null, "Cancel")]
        [TestCase(WinUI, "CommandGrid", "42", null, "Cancel")]
        [TestCase(Win10, "CommandSpace", null, "43", "Cancel")]
        [TestCase(WinUI, "CommandGrid", null, "43", "Cancel")]
        public void TwoButtonsShareTheRowInHalves(string style, string rowName, string? primary, string? secondary, string? close)
        {
            var dialog = NewDialog(style, primary, secondary, close);
            _ = this.ShowDialog(dialog);
            var row = dialog.FindChild<FrameworkElement>(rowName)!;
            var shown = new[] { ("PART_PrimaryButton", primary), ("PART_SecondaryButton", secondary), ("PART_CloseButton", close) }
                        .Where(b => b.Item2 is not null)
                        .Select(b => Bounds(Part(dialog, b.Item1), row))
                        .OrderBy(b => b.Left)
                        .ToArray();

            Assert.That(shown[0].Width, Is.EqualTo(shown[1].Width).Within(1.0));
            Assert.That(shown[0].Left, Is.EqualTo(0).Within(1.0));
            Assert.That(shown[1].Right, Is.EqualTo(row.ActualWidth).Within(1.0));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void WithoutTheTitleBarTheTitleIsInTheContent(string style)
        {
            var dialog = NewDialog(style);
            _ = this.ShowDialog(dialog);

            Assert.That(dialog.ShowTitleBar, Is.False, "the style leaves the bar out");
            Assert.That(dialog.FindChild<FrameworkElement>("PART_TitleBar")!.IsVisible, Is.False);
            Assert.That(dialog.FindChild<FrameworkElement>("ContentTitle")!.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void WithTheTitleBarTheTitleIsInTheBar(string style)
        {
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = true;
            _ = this.ShowDialog(dialog);

            Assert.That(dialog.FindChild<FrameworkElement>("PART_TitleBar")!.IsVisible, Is.True);
            Assert.That(dialog.FindChild<FrameworkElement>("ContentTitle")!.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void WithoutATitleThereIsNoTitleSpace(string style)
        {
            var dialog = NewDialog(style);
            dialog.Title = null;
            _ = this.ShowDialog(dialog);

            Assert.That(dialog.FindChild<FrameworkElement>("ContentTitle")!.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase(Win10, "Segoe UI", "Segoe UI")]
        [TestCase(WinUI, "Segoe UI Variable Text, Segoe UI", "Segoe UI Variable Display, Segoe UI")]
        public void TheTitleAndTheContentAreStepsOfTheRamp(string style, string text, string title)
        {
            var dialog = NewDialog(style);
            _ = this.ShowDialog(dialog);

            var heading = dialog.FindChild<MetroThumbContentControl>("PART_ContentTitleThumb")!.FindChild<TextBlock>()!;
            var content = (TextBlock)dialog.Content;

            Assert.Multiple(() =>
                {
                    Assert.That(heading.FontSize, Is.EqualTo(20), "the size of the title");
                    Assert.That(heading.FontFamily.Source, Is.EqualTo(title), "the family of the title");
                    Assert.That(content.FontSize, Is.EqualTo(14), "the size of the content");
                    Assert.That(content.FontFamily.Source, Is.EqualTo(text), "the family of the content");
                });
        }

        [TestCase(Win10, "Segoe UI")]
        [TestCase(WinUI, "Segoe UI Variable Small, Segoe UI")]
        public void TheTitleInTheBarIsWrittenLikeAWindowTitle(string style, string family)
        {
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = true;
            _ = this.ShowDialog(dialog);

            var title = dialog.FindChild<MetroThumbContentControl>("PART_TitleBarThumb")!.FindChild<TextBlock>()!;

            Assert.Multiple(() =>
                {
                    Assert.That(title.FontSize, Is.EqualTo(12));
                    Assert.That(title.FontFamily.Source, Is.EqualTo(family));
                });
        }

        [TestCase(Win10, false)]
        [TestCase(WinUI, false)]
        [TestCase(Win10, true)]
        [TestCase(WinUI, true)]
        public void TheXShowsOnlyWhenAskedFor(string style, bool titleBar)
        {
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = titleBar;
            _ = this.ShowDialog(dialog);
            var x = titleBar ? "PART_TitleBarCloseButton" : "PART_ContentTitleCloseButton";

            Assert.That(Part(dialog, x).IsVisible, Is.False);

            dialog.ShowTitleBarCloseButton = true;
            ClipAssert.Pump();

            Assert.That(Part(dialog, x).IsVisible, Is.True);
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        public void TheXInTheTitleBarWearsTheColourOfTheTitleBar(string style)
        {
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = true;
            dialog.ShowTitleBarCloseButton = true;
            dialog.TitleBarForeground = System.Windows.Media.Brushes.Orange;
            _ = this.ShowDialog(dialog);

            var x = Part(dialog, "PART_TitleBarCloseButton");

            Assert.That(((System.Windows.Media.SolidColorBrush)x.Foreground).Color, Is.EqualTo(System.Windows.Media.Colors.Orange));
            Assert.That(x.Width, Is.EqualTo(46));
        }

        [TestCase(Win10, false)]
        [TestCase(WinUI, false)]
        [TestCase(Win10, true)]
        [TestCase(WinUI, true)]
        public async Task TheXInEitherPlaceIsTheCloseButton(string style, bool titleBar)
        {
            var command = new CountingCommand();
            var dialog = NewDialog(style);
            dialog.ShowTitleBar = titleBar;
            dialog.ShowTitleBarCloseButton = true;
            dialog.CloseButtonCommand = command;
            var showing = this.ShowDialog(dialog);

            Click(dialog, titleBar ? "PART_TitleBarCloseButton" : "PART_ContentTitleCloseButton");

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
            Assert.That(command.Executed, Is.EqualTo(1));
        }

        [TestCase(Win10, false, false)]
        [TestCase(WinUI, false, false)]
        [TestCase(Win10, false, true)]
        [TestCase(WinUI, false, true)]
        [TestCase(Win10, true, true)]
        [TestCase(WinUI, true, true)]
        [TestCase(Win10, true, false)]
        [TestCase(WinUI, true, false)]
        public void TheTitleMovesTheDialogOnlyWithAllowMove(string style, bool allowMove, bool titleBar)
        {
            var dialog = NewDialog(style);
            dialog.AllowMove = allowMove;
            dialog.ShowTitleBar = titleBar;
            dialog.HorizontalContentAlignment = HorizontalAlignment.Center;
            _ = this.ShowDialog(dialog);
            var thumb = dialog.FindChild<MetroThumbContentControl>(titleBar ? "PART_TitleBarThumb" : "PART_ContentTitleThumb")!;

            thumb.RaiseEvent(new DragDeltaEventArgs(40, 30) { RoutedEvent = MetroThumbContentControl.DragDeltaEvent });
            ClipAssert.Pump();

            Assert.That(dialog.OffsetX, allowMove ? Is.EqualTo(40) : Is.EqualTo(0));
        }

        [Test]
        public void TheWinUIDialogIsRoundedLikeAnOverlay()
        {
            var dialog = NewDialog(WinUI);
            _ = this.ShowDialog(dialog);

            Assert.That(dialog.CornerRadius, Is.EqualTo(Application.Current.FindResource("MahApps.CornerRadius.WinUI.Overlay")));
            Assert.That(dialog.FindChild<Border>("PART_Border")!.CornerRadius, Is.EqualTo(dialog.CornerRadius));
        }

        [Test]
        public void WithoutButtonTextsTheWinUIRowIsGone()
        {
            var dialog = NewDialog(WinUI, null, null, null);
            _ = this.ShowDialog(dialog);

            Assert.That(dialog.FindChild<FrameworkElement>("CommandSpace")!.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void TheWinUIContentSitsOnItsOwnLayerOverTheButtons()
        {
            var dialog = NewDialog(WinUI);
            _ = this.ShowDialog(dialog);

            var layer = dialog.FindChild<Border>("ContentLayer")!;

            Assert.That(((System.Windows.Media.SolidColorBrush)layer.Background).Color, Is.EqualTo(((System.Windows.Media.SolidColorBrush)dialog.FindResource("MahApps.Brushes.ContentDialog.WinUI.TopOverlay")).Color));
            Assert.That(layer.BorderThickness, Is.EqualTo(new Thickness(0, 0, 0, 1)));
            Assert.That(dialog.FindChild<Border>("CommandSpace")!.Background, Is.SameAs(dialog.Background));
        }

        [TestCase(Win10, "MahApps.Brushes.ContentDialog.Win10.Background")]
        [TestCase(WinUI, "MahApps.Brushes.ContentDialog.WinUI.Background")]
        [NUnit.Framework.Description("An open dialog takes the colours of a theme it did not open in.")]
        public void AnOpenDialogFollowsATheme(string style, string backgroundKey)
        {
            var dialog = NewDialog(style);
            _ = this.ShowDialog(dialog);

            try
            {
                ControlzEx.Theming.ThemeManager.Current.ChangeTheme(this.window!, "Dark.Blue");
                ClipAssert.Pump();

                Assert.That(((System.Windows.Media.SolidColorBrush)dialog.Background).Color,
                            Is.EqualTo(((System.Windows.Media.SolidColorBrush)this.window!.FindResource(backgroundKey)).Color));
            }
            finally
            {
                ControlzEx.Theming.ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }

        private sealed class TheContext
        {
            public override string ToString() => "the context";
        }

        [Test]
        public async Task TheCoordinatorShowsAContentDialog()
        {
            var context = new TheContext();
            MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(this.window!, context);

            try
            {
                var dialog = NewDialog(Metro);
                var showing = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance.ShowContentDialogAsync(context, dialog);
                ClipAssert.PumpUntil(() => dialog.IsOpen);

                Click(dialog, "PART_PrimaryButton");

                Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Primary));
            }
            finally
            {
                MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(this.window!, null);
            }
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        [NUnit.Framework.Description("Shown again and closed without a button, a dialog says None, not what closed it the time before.")]
        public async Task AReshownDialogForgetsTheLastResult(string style)
        {
            var dialog = NewDialog(style);
            var showing = this.ShowDialog(dialog);
            Click(dialog, "PART_PrimaryButton");
            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Primary));
            ClipAssert.PumpUntil(() => dialog.Parent is null);

            showing = this.ShowDialog(dialog);
            dialog.SetCurrentValue(ChildWindow.IsOpenProperty, false);

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
            Assert.That(dialog.ClosedBy, Is.EqualTo(CloseReason.None));
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        public async Task TheOverlayClosesWithNone(string style)
        {
            var dialog = NewDialog(style);
            dialog.CloseOnOverlay = true;
            var showing = this.ShowDialog(dialog);

            var overlay = dialog.FindChild<Grid>("PART_Overlay")!;
            overlay.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseLeftButtonDownEvent, Source = overlay });

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.None));
        }

        [Test]
        [NUnit.Framework.Description("Showing a dialog that is already open says so at once, as WinUI does, rather than waiting for ever.")]
        public void ShowingAnOpenDialogAgainThrows()
        {
            var dialog = NewDialog(Metro);
            _ = this.ShowDialog(dialog);

            Assert.That(async () => await dialog.ShowAsync(this.window!), Throws.InvalidOperationException);
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [NUnit.Framework.Description("The close button in the corner is there when asked for, title or no title.")]
        public void TheCornerXShowsWithoutATitle(string style)
        {
            var dialog = NewDialog(style);
            dialog.Title = null;
            dialog.ShowTitleBarCloseButton = true;
            _ = this.ShowDialog(dialog);

            Assert.That(Part(dialog, "PART_ContentTitleCloseButton").IsVisible, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        [NUnit.Framework.Description("A box that is switched off or hidden is not where the focus goes; the next one that can take it is.")]
        public void TheFocusSkipsWhatCannotTakeIt(string style)
        {
            var off = new TextBox { IsEnabled = false };
            var hidden = new TextBox { Visibility = Visibility.Collapsed };
            var inHiddenPanel = new TextBox();
            var reachable = new TextBox();
            var dialog = NewDialog(style);
            dialog.Content = new StackPanel { Children = { off, hidden, new StackPanel { Visibility = Visibility.Collapsed, Children = { inHiddenPanel } }, reachable } };
            dialog.DefaultButton = ContentDialogButton.Primary;
            _ = this.ShowDialog(dialog);

            ClipAssert.PumpUntil(() => reachable.IsFocused);

            Assert.That(reachable.IsFocused, Is.True);
        }

        [TestCase(Metro)]
        [TestCase(Win10)]
        [TestCase(WinUI)]
        [NUnit.Framework.Description("Content with nothing that can take the focus leaves it to the default button.")]
        public void OnlyUnreachableContentLeavesTheFocusToTheDefaultButton(string style)
        {
            var dialog = NewDialog(style);
            dialog.Content = new StackPanel { Children = { new TextBox { IsEnabled = false }, new ScrollViewer { Content = new TextBlock { Text = "42" } } } };
            dialog.DefaultButton = ContentDialogButton.Secondary;
            _ = this.ShowDialog(dialog);

            var secondary = Part(dialog, "PART_SecondaryButton");
            ClipAssert.PumpUntil(() => secondary.IsFocused);

            Assert.That(secondary.IsFocused, Is.True);
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [NUnit.Framework.Description("The stroke is see-through, as in Microsoft's originals, so the dialog lies under it and not the window behind.")]
        public void TheStrokeLiesOnTheDialogItself(string style)
        {
            var dialog = NewDialog(style);
            _ = this.ShowDialog(dialog);

            var underlay = dialog.FindChild<Border>("BackgroundUnderlay")!;
            var border = dialog.FindChild<Border>("PART_Border")!;

            Assert.That(underlay, Is.Not.Null);
            Assert.That(((System.Windows.Media.SolidColorBrush)underlay.Background).Color, Is.EqualTo(((System.Windows.Media.SolidColorBrush)dialog.Background).Color));
            Assert.That(underlay.ActualWidth, Is.EqualTo(border.ActualWidth).Within(0.5));
            Assert.That(underlay.ActualHeight, Is.EqualTo(border.ActualHeight).Within(0.5));
            Assert.That(underlay.CornerRadius, Is.EqualTo(border.CornerRadius));
        }

        [TestCase(Metro)]
        [NUnit.Framework.Description("The command of a button gets the parameter that is set for it.")]
        public async Task TheCommandGetsItsParameter(string style)
        {
            var command = new CountingCommand();
            var dialog = NewDialog(style);
            dialog.PrimaryButtonCommand = command;
            dialog.PrimaryButtonCommandParameter = "42";
            var showing = this.ShowDialog(dialog);

            Click(dialog, "PART_PrimaryButton");

            Assert.That(await ClosedWith(showing), Is.EqualTo(ContentDialogResult.Primary));
            Assert.That(command.Executed, Is.EqualTo(1));
            Assert.That(command.LastParameter, Is.EqualTo("42"));
        }
    }
}
