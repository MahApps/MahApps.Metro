// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The type ramps of the three looks. The Windows 10 one is the one of the UWP generic.xaml, the
    /// WinUI one the one of TextBlock_themeresources.xaml in microsoft-ui-xaml, and the Metro one is
    /// made of the sizes Fonts.xaml has always had.
    /// </summary>
    [TestFixture]
    public class TypographyTests : WindowTestFixture<TestWindow>
    {
        private const string Text = "Segoe UI Variable Text, Segoe UI";
        private const string Small = "Segoe UI Variable Small, Segoe UI";
        private const string Display = "Segoe UI Variable Display, Segoe UI";

        [TestCase("MahApps.Styles.TextBlock.Caption.WinUI", 12, "Normal", Small)]
        [TestCase("MahApps.Styles.TextBlock.Body.WinUI", 14, "Normal", Text)]
        [TestCase("MahApps.Styles.TextBlock.BodyStrong.WinUI", 14, "SemiBold", Text)]
        [TestCase("MahApps.Styles.TextBlock.BodyLarge.WinUI", 18, "Normal", Text)]
        [TestCase("MahApps.Styles.TextBlock.BodyLargeStrong.WinUI", 18, "SemiBold", Text)]
        [TestCase("MahApps.Styles.TextBlock.Subtitle.WinUI", 20, "SemiBold", Display)]
        [TestCase("MahApps.Styles.TextBlock.Title.WinUI", 28, "SemiBold", Display)]
        [TestCase("MahApps.Styles.TextBlock.TitleLarge.WinUI", 40, "SemiBold", Display)]
        [TestCase("MahApps.Styles.TextBlock.Display.WinUI", 68, "SemiBold", Display)]
        [TestCase("MahApps.Styles.TextBlock.Caption.Win10", 12, "Normal", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Body.Win10", 14, "Normal", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Base.Win10", 14, "SemiBold", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Subtitle.Win10", 20, "Normal", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Title.Win10", 24, "350", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Subheader.Win10", 34, "Light", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Header.Win10", 46, "Light", "Segoe UI")]
        [TestCase("MahApps.Styles.TextBlock.Caption", 12, "Normal", "Segoe UI, Lucida Sans Unicode, Verdana")]
        [TestCase("MahApps.Styles.TextBlock.Body", 14, "Normal", "Segoe UI, Lucida Sans Unicode, Verdana")]
        [TestCase("MahApps.Styles.TextBlock.Subtitle", 20, "Normal", "Segoe UI, Lucida Sans Unicode, Verdana")]
        [TestCase("MahApps.Styles.TextBlock.Subheader", 29.333, "Normal", "Segoe UI Light, Lucida Sans Unicode, Verdana")]
        [TestCase("MahApps.Styles.TextBlock.Header", 40, "Normal", "Segoe UI Light, Lucida Sans Unicode, Verdana")]
        [Description("Every step of a ramp has the size, the weight and the family its look gives it.")]
        public void EveryStepOfARampIsWhatItsLookSays(string key, double size, string weight, string family)
        {
            var text = new TextBlock { Text = "42", Style = (Style)Application.Current.FindResource(key) };
            this.window!.Content = text;
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(text.FontSize, Is.EqualTo(size).Within(0.001));
                    Assert.That(text.FontWeight.ToString(), Is.EqualTo(weight));
                    Assert.That(text.FontFamily.Source, Is.EqualTo(family));
                });
        }

        [Test]
        [Description("A step takes its size through a key, so an application that wants a larger title overrides that key and nothing else.")]
        public void AStepCanBeMadeLarger()
        {
            var text = new TextBlock { Text = "42", Style = (Style)Application.Current.FindResource("MahApps.Styles.TextBlock.Title.WinUI") };
            var host = new Border { Child = text };
            host.Resources["MahApps.Font.Size.Title.WinUI"] = 32d;
            this.window!.Content = host;
            this.window.UpdateLayout();

            Assert.That(text.FontSize, Is.EqualTo(32));
        }

        [TestCase("MahApps.Styles.MetroWindow.Win10", "MahApps.Font.Size.Window.Title.Win10")]
        [TestCase("MahApps.Styles.MetroWindow.WinUI", "MahApps.Font.Size.Window.Title.WinUI")]
        [Description("The title of a window of a set takes its size through a key of its look, so an application can make it larger.")]
        public void TheTitleOfAWindowOfASetCanBeMadeLarger(string style, string key)
        {
            var window = new MetroWindow
                         {
                             Style = (Style)Application.Current.FindResource(style),
                             Title = "Deep Thought",
                             Width = 400,
                             Height = 300,
                             Left = -10000,
                             ShowInTaskbar = false
                         };
            window.Resources[key] = 16d;
            window.Show();
            try
            {
                window.UpdateLayout();
                var titleText = window.FindChild<MetroThumbContentControl>("PART_TitleBar")?.FindChild<TextBlock>();

                Assert.That(titleText, Is.Not.Null, "the title bar should show the title");
                Assert.That(titleText!.FontSize, Is.EqualTo(16));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase("MahApps.Styles.MetroWindow.Win10", "Segoe UI", "Segoe UI")]
        [TestCase("MahApps.Styles.MetroWindow.WinUI", "Segoe UI Variable Text, Segoe UI", "Segoe UI Variable Small, Segoe UI")]
        [Description("A window of a Windows set writes its content in the font of the set at 14, and its title the way Windows writes a title bar, at 12.")]
        public void AWindowOfASetWritesLikeWindows(string key, string content, string title)
        {
            var text = new TextBlock { Text = "42" };
            var window = new MetroWindow
                         {
                             Style = (Style)Application.Current.FindResource(key),
                             Title = "Deep Thought",
                             Content = text,
                             Width = 400,
                             Height = 300,
                             Left = -10000,
                             ShowInTaskbar = false
                         };
            window.Show();
            try
            {
                window.UpdateLayout();
                var titleText = window.FindChild<MetroThumbContentControl>("PART_TitleBar")?.FindChild<TextBlock>();

                Assert.That(titleText, Is.Not.Null, "the title bar should show the title");
                Assert.Multiple(() =>
                    {
                        Assert.That(text.FontSize, Is.EqualTo(14), "the size of the content");
                        Assert.That(text.FontFamily.Source, Is.EqualTo(content), "the family of the content");
                        Assert.That(titleText!.FontSize, Is.EqualTo(12), "the size of the title");
                        Assert.That(titleText.FontFamily.Source, Is.EqualTo(title), "the family of the title");
                    });
            }
            finally
            {
                window.Close();
            }
        }
    }
}
