// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class InfoBarTests : WindowTestFixture<TestWindow>
    {
        private InfoBar Show(InfoBar infoBar)
        {
            this.window!.Content = infoBar;
            this.window.UpdateLayout();
            return infoBar;
        }

        private static List<string> Record(InfoBar infoBar)
        {
            var events = new List<string>();
            infoBar.CloseButtonClick += (_, _) => events.Add("CloseButtonClick");
            infoBar.Closing += (_, e) => events.Add($"Closing {e.Reason}");
            infoBar.Closed += (_, e) => events.Add($"Closed {e.Reason}");
            return events;
        }

        private static void ClickClose(InfoBar infoBar)
        {
            var closeButton = infoBar.FindChild<ButtonBase>("PART_CloseButton");
            Assert.That(closeButton, Is.Not.Null, "the template should carry the close button");
            closeButton!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        [Test]
        public void TheCloseButtonClosesTheBarWithItsReason()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Message = "42 files are ready." });
            var events = Record(infoBar);

            ClickClose(infoBar);

            Assert.Multiple(() =>
                {
                    Assert.That(events, Is.EqualTo(new[] { "CloseButtonClick", "Closing CloseButton", "Closed CloseButton" }));
                    Assert.That(infoBar.IsOpen, Is.False);
                });
        }

        [Test]
        public void ClosingFromCodeIsProgrammatic()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });
            var events = Record(infoBar);

            infoBar.IsOpen = false;

            Assert.That(events, Is.EqualTo(new[] { "Closing Programmatic", "Closed Programmatic" }));
        }

        [Test]
        public void AReasonOfTheCloseButtonDoesNotOutliveItsClose()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });
            ClickClose(infoBar);
            infoBar.IsOpen = true;
            var events = Record(infoBar);

            infoBar.IsOpen = false;

            Assert.That(events, Is.EqualTo(new[] { "Closing Programmatic", "Closed Programmatic" }));
        }

        [Test]
        public void ACancelledCloseKeepsTheBarOpen()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });
            var events = Record(infoBar);
            infoBar.Closing += (_, e) => e.Cancel = true;

            ClickClose(infoBar);
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(events, Is.EqualTo(new[] { "CloseButtonClick", "Closing CloseButton" }));
                    Assert.That(infoBar.IsOpen, Is.True);
                    Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [Test]
        public void AClosedBarFoldsAwayButStaysVisible()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });

            infoBar.IsOpen = false;
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(infoBar.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [Test]
        public void ABarDeclaredClosedOpensLater()
        {
            var infoBar = new InfoBar { Title = "Update" };
            var events = Record(infoBar);
            this.Show(infoBar);

            infoBar.IsOpen = true;
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(events, Is.Empty);
                    Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [Test]
        public void WithoutAnIconTheBarShowsTheGlyphsOfItsSeverity()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Severity = InfoBarSeverity.Error });

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<Grid>("StandardIconArea")!.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(infoBar.FindChild<Viewbox>("UserIconBox")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(infoBar.FindChild<TextBlock>("IconBackground")!.Text, Is.EqualTo(""));
                    Assert.That(infoBar.FindChild<TextBlock>("StandardIcon")!.Text, Is.EqualTo(""));
                    Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Background, Is.SameAs(infoBar.FindResource("MahApps.Brushes.InfoBar.Error.Background")));
                });
        }

        [TestCase(InfoBarSeverity.Informational, "")]
        [TestCase(InfoBarSeverity.Success, "")]
        [TestCase(InfoBarSeverity.Warning, "")]
        [TestCase(InfoBarSeverity.Error, "")]
        public void EverySeverityHasItsGlyph(InfoBarSeverity severity, string glyph)
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Severity = severity });

            Assert.That(infoBar.FindChild<TextBlock>("StandardIcon")!.Text, Is.EqualTo(glyph));
        }

        [Test]
        public void AnIconTakesThePlaceOfTheGlyphs()
        {
            var icon = new Border { Width = 16, Height = 16 };
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Icon = icon });

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<Grid>("StandardIconArea")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(infoBar.FindChild<Viewbox>("UserIconBox")!.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(infoBar.FindChild<ContentPresenter>("UserIcon")!.Content, Is.SameAs(icon));
                });
        }

        [Test]
        public void AnIconTemplateAloneIsGivenTheSeverity()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Severity = InfoBarSeverity.Warning, IconTemplate = new DataTemplate() });

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<Viewbox>("UserIconBox")!.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(infoBar.FindChild<ContentPresenter>("UserIcon")!.Content, Is.EqualTo(InfoBarSeverity.Warning));
                });
        }

        [Test]
        public void AHiddenIconHidesBoth()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", IsIconVisible = false, Icon = new Border() });

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<Grid>("StandardIconArea")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                    Assert.That(infoBar.FindChild<Viewbox>("UserIconBox")!.Visibility, Is.EqualTo(Visibility.Collapsed));
                });
        }

        [Test]
        public void ABarThatCannotBeClosedHasNoCloseButton()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", IsClosable = false });

            Assert.That(infoBar.FindChild<ButtonBase>("PART_CloseButton")!.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void ContentMovesUpWithoutTitleMessageOrAction()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Content = new Border { Height = 20 } });
            var contentArea = infoBar.FindChild<ContentPresenter>("PART_ContentArea")!;

            Assert.That(Grid.GetRow(contentArea), Is.EqualTo(0));

            infoBar.Message = "42 files are ready.";

            Assert.That(Grid.GetRow(contentArea), Is.EqualTo(1));
        }

        [Test]
        public void ALocalForegroundColoursTitleAndMessage()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update", Message = "42 files are ready.", Foreground = Brushes.Red });

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<TextBlock>("Title")!.Foreground, Is.SameAs(Brushes.Red));
                    Assert.That(infoBar.FindChild<TextBlock>("Message")!.Foreground, Is.SameAs(Brushes.Red));
                });
        }

        [Test]
        public void TheBarIsAStatusBarToAutomation()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });

            var peer = System.Windows.Automation.Peers.UIElementAutomationPeer.CreatePeerForElement(infoBar);

            Assert.Multiple(() =>
                {
                    Assert.That(peer, Is.InstanceOf<MahApps.Metro.Automation.Peers.InfoBarAutomationPeer>());
                    Assert.That(peer.GetAutomationControlType(), Is.EqualTo(System.Windows.Automation.Peers.AutomationControlType.StatusBar));
                    Assert.That(peer.GetClassName(), Is.EqualTo("InfoBar"));
                    Assert.That(peer.GetName(), Is.EqualTo("Update"));
                    Assert.That(peer.IsControlElement(), Is.True);
                });
        }

        [Test]
        public void AClosedBarLeavesTheControlView()
        {
            var infoBar = this.Show(new InfoBar { Title = "Update" });

            var peer = System.Windows.Automation.Peers.UIElementAutomationPeer.CreatePeerForElement(infoBar);

            Assert.That(peer.IsControlElement(), Is.False);
        }

        [Test]
        public void TheOpenedNotificationNamesIconTitleAndMessage()
        {
            var infoBar = new NotifyingInfoBar { Severity = InfoBarSeverity.Warning, Title = "Update", Message = "42 files are ready." };

            Assert.That(infoBar.OpenedNotification, Is.EqualTo($"{MahApps.Metro.Lang.Automation.InfoBarIconWarning} Update 42 files are ready."));
        }

        [TestCase("MahApps.Styles.InfoBar", 0d)]
        [TestCase("MahApps.Styles.InfoBar.Win10", 1d)]
        [TestCase("MahApps.Styles.InfoBar.WinUI", 1d)]
        public void EveryLookDrawsTheBar(string key, double border)
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update", Message = "42 files are ready.", Severity = InfoBarSeverity.Success };
            infoBar.SetResourceReference(FrameworkElement.StyleProperty, key);
            this.Show(infoBar);

            var root = infoBar.FindChild<Border>("ContentRoot")!;

            Assert.Multiple(() =>
                {
                    Assert.That(root.BorderThickness, Is.EqualTo(new Thickness(border)));
                    Assert.That(root.Background, Is.SameAs(infoBar.FindResource("MahApps.Brushes.InfoBar.Success.Background")));
                    Assert.That(infoBar.ActualHeight, Is.GreaterThanOrEqualTo(48));
                    Assert.That(infoBar.FindChild<ButtonBase>("PART_CloseButton")!.Style, Is.Not.Null);
                });
        }

        [Test]
        public void ARefusedCloseDoesNotOpenTheBarAgain()
        {
            var infoBar = (NotifyingInfoBar)this.Show(new NotifyingInfoBar { IsOpen = true, Title = "Unsaved" });
            infoBar.Closing += (_, e) => e.Cancel = true;
            var opened = infoBar.OpenedCount;

            ClickClose(infoBar);

            Assert.That(infoBar.OpenedCount, Is.EqualTo(opened), "a reader should not hear the bar a second time when it never went");
        }

        [Test]
        public void ARefusedCloseKeepsATwoWayBindingAndItsSource()
        {
            var source = new OpenSource { IsOpen = true };
            var infoBar = new InfoBar { Title = "Unsaved", DataContext = source };
            infoBar.SetBinding(InfoBar.IsOpenProperty, new System.Windows.Data.Binding(nameof(OpenSource.IsOpen)) { Mode = System.Windows.Data.BindingMode.TwoWay });
            infoBar.Closing += (_, e) => e.Cancel = true;
            this.Show(infoBar);

            ClickClose(infoBar);
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(source.IsOpen, Is.True);
                    Assert.That(System.Windows.Data.BindingOperations.GetBindingExpression(infoBar, InfoBar.IsOpenProperty), Is.Not.Null);
                    Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Visibility, Is.EqualTo(Visibility.Visible));
                });
        }

        [Test]
        public void AMessageThatWrapsGoesUnderTheTitle()
        {
            this.window!.Width = 400;
            try
            {
                var infoBar = this.Show(new InfoBar
                                        {
                                            IsOpen = true,
                                            Title = "Update",
                                            Message = "The update to version 3 has been downloaded and will be installed the next time the app starts, unless you install it now."
                                        });

                var title = infoBar.FindChild<TextBlock>("Title")!;
                var message = infoBar.FindChild<TextBlock>("Message")!;

                Assert.Multiple(() =>
                    {
                        Assert.That(message.TranslatePoint(new Point(), title).Y, Is.GreaterThan(0), "the message should stand under the title");
                        Assert.That(message.TranslatePoint(new Point(), title).X, Is.EqualTo(0).Within(0.5), "and start where the title starts");
                    });
            }
            finally
            {
                this.window.ClearValue(FrameworkElement.WidthProperty);
            }
        }

        [Test]
        public void ContentActionAndIconBelongToTheBarWhileItIsClosed()
        {
            var content = new Border();
            var action = new Button();
            var icon = new Border();
            var infoBar = new InfoBar { DataContext = 42, Content = content, ActionButton = action, Icon = icon };

            Assert.Multiple(() =>
                {
                    Assert.That(LogicalTreeHelper.GetParent(content), Is.SameAs(infoBar));
                    Assert.That(LogicalTreeHelper.GetParent(action), Is.SameAs(infoBar));
                    Assert.That(LogicalTreeHelper.GetParent(icon), Is.SameAs(infoBar));
                    Assert.That(action.DataContext, Is.EqualTo(42), "a binding in the action should work before the bar has ever been open");
                });

            var other = new Button();
            infoBar.ActionButton = other;

            Assert.Multiple(() =>
                {
                    Assert.That(LogicalTreeHelper.GetParent(action), Is.Null, "the action that was replaced is let go");
                    Assert.That(LogicalTreeHelper.GetParent(other), Is.SameAs(infoBar));
                });
        }

        [Test]
        public void AnExceptionInAClosingHandlerComesOutAsItself()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });
            infoBar.Closing += (_, _) => throw new InvalidOperationException("42");

            Assert.That(() => infoBar.IsOpen = false, Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void AnExceptionInAClosedHandlerComesOutAsItself()
        {
            var infoBar = this.Show(new InfoBar { IsOpen = true, Title = "Update" });
            infoBar.Closed += (_, _) => throw new InvalidOperationException("42");

            Assert.That(() => infoBar.IsOpen = false, Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void ClosingByKeyboardHandsTheFocusOn()
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update" };
            var next = new TextBox();
            var page = new StackPanel();
            page.Children.Add(infoBar);
            page.Children.Add(next);
            this.window!.Content = page;
            this.window.UpdateLayout();

            var closeButton = infoBar.FindChild<ButtonBase>("PART_CloseButton")!;
            Assert.That(closeButton.Focus(), Is.True, "the close button should take the focus");

            ClickClose(infoBar);

            Assert.That(next.IsKeyboardFocused, Is.True, "the focus should go on to what follows the bar, not back to the window");
        }

        [TestCase("MahApps.Styles.InfoBar")]
        [TestCase("MahApps.Styles.InfoBar.Win10")]
        [TestCase("MahApps.Styles.InfoBar.WinUI")]
        public void TitleAndMessageNeverBreakInsideAWord(string key)
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update", Message = "42 files are ready." };
            infoBar.SetResourceReference(FrameworkElement.StyleProperty, key);
            this.Show(infoBar);

            Assert.Multiple(() =>
                {
                    Assert.That(infoBar.FindChild<TextBlock>("Title")!.TextWrapping, Is.EqualTo(TextWrapping.WrapWithOverflow));
                    Assert.That(infoBar.FindChild<TextBlock>("Message")!.TextWrapping, Is.EqualTo(TextWrapping.WrapWithOverflow));
                });
        }

        [TestCase("MahApps.Styles.InfoBar.Win10", "MahApps.Styles.FocusVisualStyle.Win10")]
        [TestCase("MahApps.Styles.InfoBar.WinUI", "MahApps.Styles.FocusVisualStyle.WinUI")]
        public void TheCloseButtonShowsTheFocusOfItsLook(string key, string focusVisual)
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update" };
            infoBar.SetResourceReference(FrameworkElement.StyleProperty, key);
            this.Show(infoBar);

            var closeButton = infoBar.FindChild<ButtonBase>("PART_CloseButton")!;

            Assert.Multiple(() =>
                {
                    Assert.That(closeButton.FocusVisualStyle, Is.Null);
                    Assert.That(FocusVisualHelper.GetFocusVisualStyle(closeButton), Is.SameAs(infoBar.FindResource(focusVisual)));
                });
        }

        [Test]
        public void TheMetroCloseButtonShowsTheFlatFocus()
        {
            var closeButton = this.Show(new InfoBar { IsOpen = true, Title = "Update" }).FindChild<ButtonBase>("PART_CloseButton")!;

            Assert.That(closeButton.FocusVisualStyle, Is.SameAs(closeButton.FindResource("MahApps.Styles.Button.FocusVisualStyle.Flat")));
        }

        private class OpenSource
        {
            public bool IsOpen { get; set; }
        }

        private class NotifyingInfoBar : InfoBar
        {
            public string OpenedNotification => this.GetOpenedNotification();

            public int OpenedCount { get; private set; }

            protected override void OnOpened()
            {
                this.OpenedCount++;
                base.OnOpened();
            }
        }
    }
}
