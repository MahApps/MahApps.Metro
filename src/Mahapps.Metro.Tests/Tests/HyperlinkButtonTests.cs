// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class HyperlinkButtonTests : WindowTestFixture<TestWindow>
    {
        /// <summary>Records what the button would have opened instead of opening it.</summary>
        private class RecordingHyperlinkButton : HyperlinkButton
        {
            public List<Uri> Launched { get; } = new();

            public void Press(bool pressed) => this.IsPressed = pressed;

            protected override void LaunchUri(Uri uri) => this.Launched.Add(uri);
        }

        private T Show<T>(T button)
            where T : HyperlinkButton
        {
            this.window!.Content = button;
            this.window.UpdateLayout();
            return button;
        }

        private static void Click(ButtonBase button)
        {
            var peer = (IInvokeProvider)UIElementAutomationPeer.CreatePeerForElement(button).GetPattern(PatternInterface.Invoke)!;
            peer.Invoke();
            ClipAssert.Pump();
        }

        [Test]
        public void AClickHandsTheUriOnAfterTheClickEvent()
        {
            var button = this.Show(new RecordingHyperlinkButton { Content = "MahApps", NavigateUri = new Uri("https://github.com/MahApps/") });
            var events = new List<string>();
            button.Click += (_, _) => events.Add("Click");
            button.RequestNavigate += (_, e) => events.Add($"RequestNavigate {e.Uri}");

            Click(button);

            Assert.Multiple(() =>
                {
                    Assert.That(events, Is.EqualTo(new[] { "Click", "RequestNavigate https://github.com/MahApps/" }));
                    Assert.That(button.Launched, Is.EqualTo(new[] { new Uri("https://github.com/MahApps/") }), "nobody took it, so it opens");
                });
        }

        [Test]
        public void AHandledRequestOpensNothing()
        {
            var button = this.Show(new RecordingHyperlinkButton { Content = "MahApps", NavigateUri = new Uri("https://github.com/MahApps/") });
            button.RequestNavigate += (_, e) => e.Handled = true;

            Click(button);

            Assert.That(button.Launched, Is.Empty);
        }

        [Test]
        public void TheRequestBubblesLikeTheOneOfTheWpfHyperlink()
        {
            var button = new RecordingHyperlinkButton { Content = "MahApps", NavigateUri = new Uri("https://github.com/MahApps/") };
            var host = new Border { Child = button };
            Uri? heard = null;
            host.AddHandler(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler((_, e) => heard = e.Uri));
            this.window!.Content = host;
            this.window.UpdateLayout();

            Click(button);

            Assert.That(heard, Is.EqualTo(new Uri("https://github.com/MahApps/")), "a Frame listens for this very event");
        }

        [Test]
        public void WithoutAUriThereIsNoRequest()
        {
            var button = this.Show(new RecordingHyperlinkButton { Content = "MahApps" });
            var requests = 0;
            button.RequestNavigate += (_, _) => requests++;

            Click(button);

            Assert.Multiple(() =>
                {
                    Assert.That(requests, Is.Zero);
                    Assert.That(button.Launched, Is.Empty);
                });
        }

        [TestCase("https://github.com/MahApps/", true)]
        [TestCase("http://github.com/MahApps/", true)]
        [TestCase("mailto:someone@example.com", true)]
        [TestCase("gallery:42", false)]
        [TestCase("file:///C:/answer.txt", false)]
        [TestCase("help.html", false)]
        [Description("Only the web and mail open by themselves. Any other scheme can start whatever program registered it, so it goes to the handlers of RequestNavigate and no further.")]
        public void OnlyTheWebAndMailOpenByThemselves(string uri, bool opens)
        {
            var button = this.Show(new RecordingHyperlinkButton { Content = "MahApps", NavigateUri = new Uri(uri, UriKind.RelativeOrAbsolute) });
            Uri? requested = null;
            button.RequestNavigate += (_, e) => requested = e.Uri;

            Click(button);

            Assert.Multiple(() =>
                {
                    Assert.That(requested, Is.EqualTo(button.NavigateUri), "the handlers hear of every URI");
                    Assert.That(button.Launched, opens ? Has.Count.EqualTo(1) : Is.Empty);
                });
        }

        [Test]
        public void TheUnderlineGoesOnTextOnly()
        {
            var text = this.Show(new HyperlinkButton { Content = "MahApps" });
            var textAccess = text.FindChild<AccessText>();
            var block = this.Show(new HyperlinkButton { Content = new TextBlock { Text = "MahApps" } });
            var ownBlock = (TextBlock)block.Content;

            Assert.Multiple(() =>
                {
                    Assert.That(textAccess, Is.Not.Null, "string content is drawn by the button's own template");
                    Assert.That(textAccess!.TextDecorations, Is.EqualTo(TextDecorations.Underline));
                    Assert.That(ownBlock.TextDecorations, Is.Null.Or.Empty, "a text block of the app keeps its own decorations");
                });
        }

        [Test]
        public void IsUnderlineVisibleTakesTheUnderlineAway()
        {
            var button = this.Show(new HyperlinkButton { Content = "MahApps", IsUnderlineVisible = false });

            Assert.That(button.FindChild<AccessText>()!.TextDecorations, Is.Null.Or.Empty);
        }

        [Test]
        [Description("The button underlines its own text and nothing a template of the app puts inside it, such as a label of a view model.")]
        public void TextInsideATemplatedContentKeepsItsOwnLook()
        {
            var template = (DataTemplate)System.Windows.Markup.XamlReader.Parse(
                "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Label Content=\"x_y\" /></DataTemplate>");
            var button = this.Show(new HyperlinkButton { Content = new object(), ContentTemplate = template });

            var label = button.FindChild<Label>();
            Assert.That(label, Is.Not.Null, "the template of the app should be in the button");

            var texts = label!.FindChildren<AccessText>(true);
            Assert.Multiple(() =>
                {
                    Assert.That(texts, Is.Not.Empty, "a label draws its string with an access text");
                    Assert.That(texts, Has.All.Matches<AccessText>(t => t.TextDecorations is null || t.TextDecorations.Count == 0), "the label is not the button's text");
                });
        }

        [Test]
        public void ContentStringFormatStillFormatsTheText()
        {
            var button = this.Show(new HyperlinkButton { Content = "GitHub", ContentStringFormat = "Visit {0}" });

            Assert.That(button.FindChildren<TextBlock>(true), Has.Some.Matches<TextBlock>(t => t.Text == "Visit GitHub"));
        }

        [Test]
        public void AnAccessKeyStillWorks()
        {
            var button = this.Show(new HyperlinkButton { Content = "_Help" });

            Assert.That(button.FindChild<AccessText>()!.AccessKey, Is.EqualTo('H'));
        }

        [Test]
        public void ItShowsTheHand()
        {
            var button = this.Show(new HyperlinkButton { Content = "MahApps" });

            Assert.That(button.Cursor, Is.EqualTo(Cursors.Hand));
        }

        [Test]
        public void AScreenReaderSeesALink()
        {
            var button = this.Show(new HyperlinkButton { Content = "MahApps" });
            var peer = UIElementAutomationPeer.CreatePeerForElement(button);

            Assert.Multiple(() =>
                {
                    Assert.That(peer.GetAutomationControlType(), Is.EqualTo(AutomationControlType.Hyperlink));
                    Assert.That(peer.GetClassName(), Is.EqualTo("HyperlinkButton"));
                    Assert.That(peer.GetName(), Is.EqualTo("MahApps"));
                    Assert.That(peer.GetPattern(PatternInterface.Invoke), Is.Not.Null);
                });
        }

        private static readonly DependencyPropertyKey MouseOverKey
            = (DependencyPropertyKey)typeof(UIElement).GetField("IsMouseOverPropertyKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!;

        private RecordingHyperlinkButton ShowIn(string styleKey)
        {
            var button = new RecordingHyperlinkButton { Content = "MahApps", Style = (Style)this.window!.FindResource(styleKey) };
            return this.Show(button);
        }

        [TestCase("MahApps.Styles.HyperlinkButton.Win10", "MahApps.Brushes.Hyperlink.Win10")]
        [TestCase("MahApps.Styles.HyperlinkButton.WinUI", "MahApps.Brushes.Hyperlink.WinUI")]
        public void TheTextTakesTheColoursOfTheLink(string styleKey, string brushes)
        {
            var button = this.ShowIn(styleKey);
            Assert.That(button.Foreground, Is.SameAs(this.window!.FindResource($"{brushes}.Foreground")), "at rest");

            button.SetValue(MouseOverKey, true);
            Assert.That(button.Foreground, Is.SameAs(this.window.FindResource($"{brushes}.ForegroundPointerOver")), "under the mouse");

            button.Press(true);
            Assert.That(button.Foreground, Is.SameAs(this.window.FindResource($"{brushes}.ForegroundPressed")), "pressed");

            button.Press(false);
            button.ClearValue(MouseOverKey);
            button.IsEnabled = false;
            Assert.That(button.Foreground, Is.SameAs(this.window.FindResource($"{brushes}.ForegroundDisabled")), "disabled");
        }

        [Test]
        public void TheWinUIButtonLightsUpUnderTheMouse()
        {
            var button = this.ShowIn("MahApps.Styles.HyperlinkButton.WinUI");
            Assert.That(((System.Windows.Media.SolidColorBrush)button.Background).Color.A, Is.Zero, "transparent at rest");

            button.SetValue(MouseOverKey, true);
            Assert.That(button.Background, Is.SameAs(this.window!.FindResource("MahApps.Brushes.WinUI.SubtleFillSecondary")));

            button.Press(true);
            Assert.That(button.Background, Is.SameAs(this.window.FindResource("MahApps.Brushes.WinUI.SubtleFillTertiary")));
        }

        [TestCase("MahApps.Styles.HyperlinkButton", true, 0, 0, 0, 0)]
        [TestCase("MahApps.Styles.HyperlinkButton.Win10", true, 0, 6, 0, 7)]
        [TestCase("MahApps.Styles.HyperlinkButton.WinUI", false, 11, 5, 11, 6)]
        public void EachSetHasTheUnderlineAndThePaddingOfItsOriginal(string styleKey, bool underlined, double left, double top, double right, double bottom)
        {
            var button = this.ShowIn(styleKey);

            Assert.Multiple(() =>
                {
                    Assert.That(button.IsUnderlineVisible, Is.EqualTo(underlined));
                    Assert.That(button.Padding, Is.EqualTo(new Thickness(left, top, right, bottom)));
                    Assert.That(button.Cursor, Is.EqualTo(Cursors.Hand));
                });
        }
    }
}
