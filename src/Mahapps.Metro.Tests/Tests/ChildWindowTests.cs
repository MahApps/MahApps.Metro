// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

// The template still frames the window with the obsolete ClipBorder, as SimpleChildWindow did.
// That border goes when the rounding of the child window is looked at.
#pragma warning disable CS0618

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class ChildWindowTests : WindowTestFixture<ChildWindowWindow>
    {
        private Grid ActiveContainer => this.window!.FindChild<Grid>("PART_MetroActiveDialogContainer")!;

        private ChildWindow NewChildWindow() => (ChildWindow)this.window!.FindResource("ChildWindow");

        /// <summary>
        /// Puts a child window into the active container by hand, the way the manager does it, for
        /// the tests that look at the control alone.
        /// </summary>
        private ChildWindow OpenByHand()
        {
            var childWindow = this.NewChildWindow();
            this.ActiveContainer.Children.Add(childWindow);
            childWindow.SetCurrentValue(ChildWindow.IsOpenProperty, true);
            WaitUntilShown(childWindow);
            return childWindow;
        }

        private static void WaitUntilShown(ChildWindow childWindow)
        {
            ClipAssert.PumpUntil(() => childWindow.FindChild<Grid>("PART_Overlay")?.Visibility == Visibility.Visible);
        }

        [TearDown]
        public void CloseAllChildWindows()
        {
            foreach (var childWindow in this.ActiveContainer.Children.OfType<ChildWindow>().ToList())
            {
                childWindow.Close();
            }

            // the hide storyboard has to run out before the manager takes its windows out, and a slow
            // build agent needs longer for that than a fixed wait gave it
            ClipAssert.PumpUntil(() => this.ActiveContainer.Children.OfType<ChildWindow>().All(c => c.FindChild<Grid>("PART_Overlay")?.Visibility != Visibility.Visible));
            ClipAssert.Pump(100);

            // what a test opened by hand is still there, no manager was around to take it out
            foreach (var childWindow in this.ActiveContainer.Children.OfType<ChildWindow>().ToList())
            {
                this.ActiveContainer.Children.Remove(childWindow);
            }

            // and the window may only say that no dialog is open once the last one has gone
            ClipAssert.PumpUntil(() => !this.window!.IsAnyDialogOpen);
        }

        [Test]
        public void TheImplicitStyleGivesTheTemplate()
        {
            var childWindow = this.OpenByHand();

            Assert.That(childWindow.FindChild<Grid>("PART_Window"), Is.Not.Null);
            Assert.That(childWindow.FindChild<ClipBorder>("PART_Border"), Is.Not.Null);
            Assert.That(childWindow.FindChild<MetroThumbContentControl>("PART_TitleBarThumb"), Is.Not.Null);
        }

        [Test]
        public void TheTitleBarTakesAFractionalHeight()
        {
            var childWindow = this.OpenByHand();

            childWindow.TitleBarHeight = 32.5;
            ClipAssert.Pump();

            // the height it is given, since layout rounding moves the one it ends up with to whole device pixels
            Assert.That(childWindow.FindChild<Grid>("PART_TitleBar")!.Height, Is.EqualTo(32.5));
        }

        [Test]
        public void TheCornerRadiusReachesTheBorder()
        {
            var childWindow = this.OpenByHand();

            childWindow.CornerRadius = new CornerRadius(8);
            ClipAssert.Pump();

            Assert.That(childWindow.GetValue(ChildWindow.CornerRadiusProperty), Is.EqualTo(new CornerRadius(8)));
            Assert.That(childWindow.GetValue(Border.CornerRadiusProperty), Is.EqualTo(default(CornerRadius)), "the wrapper must not write the attached property of Border");
            Assert.That(childWindow.FindChild<ClipBorder>("PART_Border")!.CornerRadius, Is.EqualTo(new CornerRadius(8)));
        }

        [Test]
        public void TheOverlayFollowsAThemeChange()
        {
            var childWindow = this.OpenByHand();

            try
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Dark.Blue");
                ClipAssert.Pump();

                // a dark veil in the dark theme as well, where the theme foreground would be a white one
                var expected = (Color)this.window!.FindResource("MahApps.Colors.ChildWindow.Overlay");
                var overlay = (SolidColorBrush)childWindow.OverlayBrush;
                Assert.That(overlay.Color, Is.EqualTo(expected));
                Assert.That(overlay.Color, Is.EqualTo(Colors.Black));
                Assert.That(overlay.Opacity, Is.EqualTo(0.7).Within(0.001));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }

        private Task<TResult?> Show<TResult>(ChildWindow childWindow, ChildWindowManager.OverlayFillBehavior fill = ChildWindowManager.OverlayFillBehavior.FullWindow)
        {
            var showing = this.window!.ShowChildWindowAsync<TResult>(childWindow, fill);
            WaitUntilShown(childWindow);
            return showing;
        }

        [Test]
        public async Task TheResultOfCloseComesBack()
        {
            var childWindow = this.NewChildWindow();
            var showing = this.Show<string>(childWindow);

            Assert.That(this.ActiveContainer.Children.Contains(childWindow), Is.True);

            childWindow.Close("42");
            await showing.Within("the child window to close");

            Assert.That(showing.Result, Is.EqualTo("42"));
            Assert.That(this.ActiveContainer.Children.Contains(childWindow), Is.False);
        }

        [Test]
        public async Task TheCloseReasonComesBackWithoutAResult()
        {
            var childWindow = this.NewChildWindow();
            var showing = this.Show<CloseReason>(childWindow);

            childWindow.Close(CloseReason.Cancel);
            await showing.Within("the child window to close");

            Assert.That(showing.Result, Is.EqualTo(CloseReason.Cancel));
        }

        [Test]
        public async Task IsAnyDialogOpenFollowsTheChildWindow()
        {
            Assert.That(this.window!.IsAnyDialogOpen, Is.False);

            var childWindow = this.NewChildWindow();
            var showing = this.Show<object>(childWindow);

            Assert.That(this.window.IsAnyDialogOpen, Is.True);

            childWindow.Close();
            await showing.Within("the child window to close");

            Assert.That(this.window.IsAnyDialogOpen, Is.False);
        }

        [Test]
        public async Task WindowContentGivesTheRowBack()
        {
            Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(0));

            for (var round = 0; round < 2; round++)
            {
                var childWindow = this.NewChildWindow();
                var showing = this.Show<object>(childWindow, ChildWindowManager.OverlayFillBehavior.WindowContent);

                Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(1), $"round {round}");
                Assert.That(Grid.GetRowSpan(this.ActiveContainer), Is.EqualTo(1), $"round {round}");

                childWindow.Close();
                await showing.Within("the child window to close");

                Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(0), $"round {round}");
                Assert.That(Grid.GetRowSpan(this.ActiveContainer), Is.EqualTo(2), $"round {round}");
            }
        }

        [Test]
        public async Task TwoChildWindowsShareTheContentRow()
        {
            var first = this.NewChildWindow();
            var second = this.NewChildWindow();
            var showingFirst = this.Show<object>(first, ChildWindowManager.OverlayFillBehavior.WindowContent);
            var showingSecond = this.Show<object>(second, ChildWindowManager.OverlayFillBehavior.WindowContent);

            Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(1));

            first.Close();
            await showingFirst.Within("the first child window to close");

            Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(1), "the second one still needs the row");

            second.Close();
            await showingSecond.Within("the second child window to close");

            Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(0));
        }

        [Test]
        public async Task WindowContentBelowTheTitleBarStaysInRowOne()
        {
            this.window!.SetCurrentValue(MetroWindow.ShowDialogsOverTitleBarProperty, false);

            try
            {
                ClipAssert.Pump();
                Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(1));

                var childWindow = this.NewChildWindow();
                var showing = this.Show<object>(childWindow, ChildWindowManager.OverlayFillBehavior.WindowContent);

                Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(1));

                childWindow.Close();
                await showing.Within("the child window to close");
            }
            finally
            {
                this.window.SetCurrentValue(MetroWindow.ShowDialogsOverTitleBarProperty, true);
                ClipAssert.Pump();
            }

            Assert.That(Grid.GetRow(this.ActiveContainer), Is.EqualTo(0), "after the close the trigger decides again");
        }

        [Test]
        public async Task AMetroDialogOverAChildWindowIsOnTop()
        {
            var childWindow = this.NewChildWindow();
            var showing = this.Show<object>(childWindow);

            var dialog = new CustomDialog { Title = "42" };
            await this.window!.ShowMetroDialogAsync(dialog).Within("the dialog to show");

            Assert.That(Panel.GetZIndex(dialog), Is.GreaterThan(Panel.GetZIndex(childWindow)));

            await this.window.HideMetroDialogAsync(dialog).Within("the dialog to hide");
            childWindow.Close();
            await showing.Within("the child window to close");
        }

        [Test]
        public async Task AChildWindowOverAMetroDialogIsOnTop()
        {
            var dialog = new CustomDialog { Title = "42" };
            await this.window!.ShowMetroDialogAsync(dialog).Within("the dialog to show");

            var childWindow = this.NewChildWindow();
            var showing = this.Show<object>(childWindow);

            Assert.That(Panel.GetZIndex(childWindow), Is.GreaterThan(Panel.GetZIndex(dialog)));

            // and one more dialog on top of both goes over the child window again
            var second = new CustomDialog { Title = "43" };
            await this.window.ShowMetroDialogAsync(second).Within("the second dialog to show");

            Assert.That(Panel.GetZIndex(second), Is.GreaterThan(Panel.GetZIndex(childWindow)));

            await this.window.HideMetroDialogAsync(second).Within("the second dialog to hide");
            childWindow.Close();
            await showing.Within("the child window to close");
            await this.window.HideMetroDialogAsync(dialog).Within("the dialog to hide");

            Assert.That(this.window.IsAnyDialogOpen, Is.False);
        }

        private static void PressEscape(ChildWindow childWindow)
        {
            var source = PresentationSource.FromVisual(childWindow)!;
            childWindow.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, Key.Escape) { RoutedEvent = Keyboard.KeyDownEvent });
        }

        private static void ClickTheOverlay(ChildWindow childWindow)
        {
            var overlay = childWindow.FindChild<Grid>("PART_Overlay")!;
            overlay.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.MouseLeftButtonDownEvent, Source = overlay });
        }

        [Test]
        public async Task EscapeCloses()
        {
            var childWindow = this.NewChildWindow();
            var showing = this.Show<CloseReason>(childWindow);

            PressEscape(childWindow);
            await showing.Within("the child window to close");

            Assert.That(showing.Result, Is.EqualTo(CloseReason.Escape));
        }

        [Test]
        public void EscapeDoesNothingWithoutCloseByEscape()
        {
            var childWindow = this.NewChildWindow();
            childWindow.CloseByEscape = false;
            _ = this.Show<object>(childWindow);

            PressEscape(childWindow);
            ClipAssert.Pump();

            Assert.That(childWindow.IsOpen, Is.True);
        }

        [Test]
        public async Task TheOverlayClosesWithCloseOnOverlay()
        {
            var childWindow = this.NewChildWindow();
            childWindow.CloseOnOverlay = true;
            var showing = this.Show<CloseReason>(childWindow);

            ClickTheOverlay(childWindow);
            await showing.Within("the child window to close");

            Assert.That(showing.Result, Is.EqualTo(CloseReason.Overlay));
        }

        [Test]
        public void TheOverlayDoesNothingByDefault()
        {
            var childWindow = this.NewChildWindow();
            _ = this.Show<object>(childWindow);

            ClickTheOverlay(childWindow);
            ClipAssert.Pump();

            Assert.That(childWindow.IsOpen, Is.True);
        }

        [Test]
        public void TheFocusedElementGetsTheFocus()
        {
            var childWindow = this.NewChildWindow();
            var textBox = (TextBox)((StackPanel)childWindow.Content).Children[0];
            childWindow.FocusedElement = textBox;

            _ = this.Show<object>(childWindow);
            ClipAssert.PumpUntil(() => textBox.IsFocused);

            Assert.That(textBox.IsFocused, Is.True);
        }

        // the handler is taken off again, or the teardown could not close the window either
        [Test]
        public void ACanceledClosingKeepsItOpen()
        {
            var childWindow = this.NewChildWindow();
            void Cancel(object? sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = true;
            childWindow.Closing += Cancel;
            var showing = this.Show<object>(childWindow);

            try
            {
                Assert.That(childWindow.Close(CloseReason.Ok), Is.False);
                ClipAssert.Pump();

                Assert.That(childWindow.IsOpen, Is.True);
                Assert.That(childWindow.ClosedBy, Is.EqualTo(CloseReason.None));
                Assert.That(showing.IsCompleted, Is.False);
            }
            finally
            {
                childWindow.Closing -= Cancel;
            }
        }

        private sealed class TheContext
        {
            public override string ToString() => "the context";
        }

        [Test]
        public async Task TheCoordinatorShowsAChildWindow()
        {
            var context = new TheContext();
            DialogParticipation.SetRegister(this.window!, context);

            try
            {
                var childWindow = this.NewChildWindow();
                var showing = DialogCoordinator.Instance.ShowChildWindowAsync<string>(context, childWindow, ChildWindowManager.OverlayFillBehavior.FullWindow);
                WaitUntilShown(childWindow);

                Assert.That(this.ActiveContainer.Children.Contains(childWindow), Is.True);

                childWindow.Close("42");
                await showing.Within("the child window to close");

                Assert.That(showing.Result, Is.EqualTo("42"));
            }
            finally
            {
                DialogParticipation.SetRegister(this.window!, null);
            }
        }

        [Test]
        [Description("A click that reaches the child window under an open dialog does not lift it over that dialog.")]
        public async Task AClickOnTheChildWindowLeavesTheDialogOnTop()
        {
            var childWindow = this.NewChildWindow();
            var showing = this.Show<object>(childWindow);

            var dialog = new CustomDialog { Title = "42" };
            await this.window!.ShowMetroDialogAsync(dialog).Within("the dialog to show");

            childWindow.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.PreviewMouseDownEvent });

            Assert.That(Panel.GetZIndex(dialog), Is.GreaterThan(Panel.GetZIndex(childWindow)));

            await this.window.HideMetroDialogAsync(dialog).Within("the dialog to hide");
            childWindow.Close();
            await showing.Within("the child window to close");
        }

        [Test]
        [Description("Of two child windows, the one clicked comes to the front.")]
        public void AClickBringsAChildWindowOverAnother()
        {
            var first = this.NewChildWindow();
            var second = this.NewChildWindow();
            _ = this.Show<object>(first);
            _ = this.Show<object>(second);

            Assert.That(Panel.GetZIndex(second), Is.GreaterThan(Panel.GetZIndex(first)));

            first.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) { RoutedEvent = UIElement.PreviewMouseDownEvent });

            Assert.That(Panel.GetZIndex(first), Is.GreaterThan(Panel.GetZIndex(second)));
        }

        [Test]
        [Description("A child window shown in a panel inside another one closes on its own.")]
        public async Task AnInnerChildWindowLeavesTheOuterOneOpen()
        {
            var outer = this.NewChildWindow();
            var showingOuter = this.Show<object>(outer);

            var inner = this.NewChildWindow();
            var showingInner = this.window!.ShowChildWindowAsync<object>(inner, (StackPanel)outer.Content);
            WaitUntilShown(inner);

            inner.Close();
            await showingInner.Within("the inner child window to close");

            Assert.That(outer.IsOpen, Is.True);
            Assert.That(this.ActiveContainer.Children.Contains(outer), Is.True);
            Assert.That(showingOuter.IsCompleted, Is.False);
        }

        private Button AccessKeyButton(string content, System.Action clicked)
        {
            var button = new Button { Content = content };
            button.Click += (_, _) => clicked();
            return button;
        }

        /// <summary>
        /// Presses an access key the way the keyboard does it with the focus inside the given scope.
        /// </summary>
        private static void PressAccessKey(object scope, string key)
        {
            AccessKeyManager.ProcessKey(scope, key, false);
            ClipAssert.Pump();
        }

        [Test]
        [Description("A child window is a scope for access keys, as a dialog is, so _Go on a button inside it works.")]
        public void AnAccessKeyInsideTheChildWindowWorks()
        {
            var clicked = false;
            var childWindow = this.NewChildWindow();
            ((StackPanel)childWindow.Content).Children.Add(this.AccessKeyButton("_Go", () => clicked = true));
            _ = this.Show<object>(childWindow);

            PressAccessKey(childWindow, "G");

            Assert.That(clicked, Is.True);
        }

        [Test]
        [Description("What lies under the child window stays out of reach of its access keys.")]
        public void AnAccessKeyBehindTheChildWindowIsBlocked()
        {
            var clicked = false;
            this.window!.Content = this.AccessKeyButton("_Back", () => clicked = true);
            ClipAssert.Pump();

            var childWindow = this.NewChildWindow();
            _ = this.Show<object>(childWindow);

            PressAccessKey(childWindow, "B");

            Assert.That(clicked, Is.False);
        }

        private static void ClickTheTitleBarCloseButton(ChildWindow childWindow)
        {
            var button = childWindow.FindChild<Button>("PART_TitleBarCloseButton")!;
            button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, button));
        }

        [Test]
        [Description("The command of the X belongs to the X: closing in code, by Escape or by the overlay does not run it.")]
        public async Task TheTitleBarCloseButtonCommandRunsOnlyForTheX()
        {
            var command = new CountingCommand();

            var closedInCode = this.NewChildWindow();
            closedInCode.TitleBarCloseButtonCommand = command;
            var showing = this.Show<object>(closedInCode);
            closedInCode.Close();
            await showing.Within("the child window to close in code");

            Assert.That(command.Executed, Is.EqualTo(0));

            var closedByTheX = this.NewChildWindow();
            closedByTheX.ShowTitleBarCloseButton = true;
            closedByTheX.TitleBarCloseButtonCommand = command;
            showing = this.Show<object>(closedByTheX);
            ClickTheTitleBarCloseButton(closedByTheX);
            await showing.Within("the child window to close by the X");

            Assert.That(command.Executed, Is.EqualTo(1));
        }

        [Test]
        [Description("A command that cannot run keeps the X from closing, and nothing else.")]
        public async Task ACommandThatCannotRunHoldsOnlyTheX()
        {
            var command = new CountingCommand { Allowed = false };
            var childWindow = this.NewChildWindow();
            childWindow.ShowTitleBarCloseButton = true;
            childWindow.TitleBarCloseButtonCommand = command;
            var showing = this.Show<object>(childWindow);

            ClickTheTitleBarCloseButton(childWindow);
            ClipAssert.Pump();

            Assert.That(childWindow.IsOpen, Is.True);

            childWindow.Close();
            await showing.Within("the child window to close in code");

            Assert.That(command.Executed, Is.EqualTo(0));
        }

        [TestCase("MahApps.Styles.TextBox.Win10")]
        [TestCase("MahApps.Styles.TextBox.WinUI")]
        [Description("A box of the Windows 10 or the WinUI set takes the focus as the window opens; WPF threw when the focus arrived before the box had its template.")]
        public void ABoxOfAnotherSetTakesTheFocus(string style)
        {
            var box = new TextBox { Style = (Style)Application.Current.FindResource(style) };
            var childWindow = this.NewChildWindow();
            childWindow.Content = new StackPanel { Children = { box } };

            _ = this.Show<object>(childWindow);
            ClipAssert.PumpUntil(() => box.IsFocused);

            Assert.That(box.IsFocused, Is.True);
        }
    }
}
