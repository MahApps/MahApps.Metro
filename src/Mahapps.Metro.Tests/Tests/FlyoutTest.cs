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
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class FlyoutTest
    {
        [Test]
        public async Task AdaptsWindowCommandsToDarkFlyout()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = new Flyout { Theme = FlyoutTheme.Dark };
            window.Flyouts?.Items.Add(flyout);

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            window.AssertWindowButtonCommandsColor(((SolidColorBrush)flyout.Foreground).Color);

            window.Close();
        }

        [Test]
        public async Task DefaultFlyoutPositionIsLeft()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.DefaultFlyout.Position, Is.EqualTo(Position.Left));

            window.Close();
        }

        [Test]
        public async Task FlyoutIsClosedByDefault()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.DefaultFlyout.IsOpen, Is.False);

            window.Close();
        }

        [Test]
        public async Task FlyoutIsHiddenByDefault()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            // root grid should be hidden
            Assert.That(window.DefaultFlyout.Visibility, Is.EqualTo(Visibility.Hidden));

            window.Close();
        }

        [Test]
        public async Task HiddenIconIsBelowFlyout()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            window.IconOverlayBehavior = OverlayBehavior.Never;

            Assert.That(window.Icon, Is.Not.Null);
            var icon = window.FindChild<FrameworkElement>("PART_Icon");
            Assert.That(icon, Is.Not.Null);

            var flyout = window.LeftFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            var iconZIndex = Panel.GetZIndex(icon);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.That(flyoutZIndex, Is.EqualTo(0));
            Assert.That(iconZIndex, Is.EqualTo(1));
            Assert.That(flyoutZIndex < iconZIndex, Is.True);

            window.Close();
        }

        [Test]
        public async Task HiddenLeftWindowCommandsAreBelowFlyout()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            window.LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            var windowCommands = window.FindChild<ContentPresenter>("PART_LeftWindowCommands");
            Assert.That(windowCommands, Is.Not.Null);

            var flyout = window.LeftFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.That(flyoutZIndex, Is.EqualTo(0));
            Assert.That(windowCommandsZIndex, Is.EqualTo(1));
            Assert.That(flyoutZIndex < windowCommandsZIndex, Is.True);

            window.Close();
        }

        [Test]
        public async Task HiddenRightWindowCommandsAreBelowFlyout()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            window.RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            var windowCommands = window.FindChild<ContentPresenter>("PART_RightWindowCommands");
            Assert.That(windowCommands, Is.Not.Null);

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.That(flyoutZIndex, Is.EqualTo(0));
            Assert.That(windowCommandsZIndex, Is.EqualTo(1));
            Assert.That(flyoutZIndex < windowCommandsZIndex, Is.True);

            window.Close();
        }

        [Test]
        public async Task WindowButtonCommandsAreOverFlyout()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var windowCommands = window.FindChild<ContentPresenter>("PART_WindowButtonCommands");
            Assert.That(windowCommands, Is.Not.Null);

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            var windowCommandsZIndex = Panel.GetZIndex(windowCommands);
            var flyoutZIndex = Panel.GetZIndex(flyout);

            Assert.That(windowCommandsZIndex > flyoutZIndex, Is.True);

            window.Close();
        }

        [Test]
        public async Task RaisesIsOpenChangedEvent()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = window.RightFlyout;

            var completionSource = new TaskCompletionSource<bool>();

            void FlyoutOnIsOpenChanged(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.IsOpenChanged -= FlyoutOnIsOpenChanged;
                completionSource.SetResult(flyout.IsOpen);
            }

            flyout.IsOpenChanged += FlyoutOnIsOpenChanged;

            flyout.IsOpen = true;

            var opened = await completionSource.Task;
            Assert.That(opened, Is.True);

            window.Close();
        }

        [Test]
        public async Task RaisesClosingFinishedEvent()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.That(opened, Is.True);

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.That(closed, Is.True);

            window.Close();
        }

        [Test]
        public async Task FindFlyoutWithFindChildren()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.DoesNotThrow(() =>
                {
                    var flyouts = (window.Content as DependencyObject).FindChildren<Flyout>(true);
                    var flyoutOnGrid = flyouts.FirstOrDefault(f => f.Name == nameof(window.FlyoutOnGrid));
                    Assert.That(flyoutOnGrid, Is.Not.Null);
                });

            window.Close();
        }

        [Test]
        public async Task DefaultFlyoutThemeIsDark()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.DefaultFlyout.Theme, Is.EqualTo(FlyoutTheme.Dark));

            window.Close();
        }

        [Test]
        public async Task WindowButtonCommandsForegroundBrushShouldBeAlwaysOverrideDefaultWindowCommandsBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.WindowButtonCommands, Is.Not.Null);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.That(opened, Is.True);

            Assert.That(window.WindowButtonCommands.Foreground, Is.EqualTo(brush));

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.That(closed, Is.True);

            Assert.That(window.WindowButtonCommands.Foreground, Is.EqualTo(brush));

            window.Close();
        }

        [Test]
        public async Task LeftWindowCommandsForegroundBrushShouldAlwaysOverrideDefaultWindowCommandsBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.LeftWindowCommands, Is.Not.Null);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.LeftFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.That(opened, Is.True);

            Assert.That(window.LeftWindowCommands.Foreground, Is.EqualTo(brush));

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.That(closed, Is.True);

            Assert.That(window.LeftWindowCommands.Foreground, Is.EqualTo(brush));

            window.Close();
        }

        [Test]
        public async Task RightWindowCommandsForegroundBrushShouldAlwaysOverrideDefaultWindowCommandsBrush()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<FlyoutWindow>();

            Assert.That(window.RightWindowCommands, Is.Not.Null);

            var brush = Brushes.Red;

            window.OverrideDefaultWindowCommandsBrush = brush;

            var flyout = window.RightFlyout;

            var openingCompletionSource = new TaskCompletionSource<bool>();
            var closingCompletionSource = new TaskCompletionSource<bool>();

            void FlyoutOnOpeningFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.OpeningFinished -= FlyoutOnOpeningFinished;
                openingCompletionSource.SetResult(flyout.IsOpen);
            }

            void FlyoutOnClosingFinished(object o, RoutedEventArgs routedEventArgs)
            {
                flyout.ClosingFinished -= FlyoutOnClosingFinished;
                closingCompletionSource.SetResult(!flyout.IsOpen);
            }

            flyout.OpeningFinished += FlyoutOnOpeningFinished;
            flyout.ClosingFinished += FlyoutOnClosingFinished;

            flyout.IsOpen = true;

            var opened = await openingCompletionSource.Task;
            Assert.That(opened, Is.True);

            Assert.That(window.RightWindowCommands.Foreground, Is.EqualTo(brush));

            flyout.IsOpen = false;

            var closed = await closingCompletionSource.Task;
            Assert.That(closed, Is.True);

            Assert.That(window.RightWindowCommands.Foreground, Is.EqualTo(brush));

            window.Close();
        }
    }
}