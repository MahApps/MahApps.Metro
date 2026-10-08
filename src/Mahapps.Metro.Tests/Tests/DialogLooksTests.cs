// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The old dialogs in the Windows 10 and the WinUI look: a card in the middle, drawn like the
    /// content dialog of that look, picked for a single call through the custom resource dictionary.
    /// </summary>
    [TestFixture]
    public class DialogLooksTests
    {
        private const string Win10 = "Win10";
        private const string WinUI = "WinUI";

        internal static ResourceDictionary Look(string name)
        {
            return new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/{name}/Dialogs.xaml", UriKind.Absolute) };
        }

        internal static async Task<TDialog> WaitFor<TDialog>(MetroWindow window)
            where TDialog : BaseMetroDialog
        {
            TDialog? dialog = null;

            for (var i = 0; i < 100 && dialog is null; i++)
            {
                dialog = await window.GetCurrentDialogAsync<TDialog>();

                if (dialog is null)
                {
                    await Task.Delay(20);
                }
            }

            Assert.That(dialog, Is.Not.Null, $"the {typeof(TDialog).Name} should be up");
            ClipAssert.Pump();

            return dialog!;
        }

        internal static FrameworkElement Card(FrameworkElement dialog)
        {
            var card = dialog.FindChild<FrameworkElement>("Card");
            Assert.That(card, Is.Not.Null, "the template should carry the card");
            return card!;
        }

        private static Color ColorOf(FrameworkElement dialog, string key)
        {
            return ((SolidColorBrush)dialog.FindResource(key)).Color;
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Handed the dictionary of a look, a custom dialog comes up as a card in the middle.")]
        public async Task ACustomDialogTakesTheLookItIsHanded(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var dialog = new CustomDialog(window, new MetroDialogSettings { CustomResourceDictionary = Look(look) }) { Title = "42", Content = new TextBlock { Text = "The answer" } };
                _ = window.ShowMetroDialogAsync(dialog);
                await WaitFor<CustomDialog>(window);

                Assert.That(dialog.Template, Is.SameAs(dialog.FindResource($"MahApps.Templates.BaseMetroDialog.{look}")));

                var card = Card(dialog);
                Assert.That(card.MinWidth, Is.EqualTo(320));
                Assert.That(card.MaxWidth, Is.EqualTo(548));
                var left = card.TranslatePoint(new Point(), dialog).X;
                Assert.That(left, Is.EqualTo(dialog.ActualWidth - left - card.ActualWidth).Within(1.0), "the card should sit in the middle");

                var border = dialog.FindChild<Border>("CardBorder")!;
                Assert.That(((SolidColorBrush)border.Background).Color, Is.EqualTo(ColorOf(dialog, $"MahApps.Brushes.ContentDialog.{look}.Background")));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A dialog that needs more room than the card of the content dialog says so through a resource.")]
        public async Task TheCardCanBeMadeWider(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var dialog = new CustomDialog(window, new MetroDialogSettings { CustomResourceDictionary = Look(look) }) { Title = "42", Content = new Border { Width = 700, Height = 20 } };
                dialog.Resources["MahApps.Sizes.Dialogs.Card.MaxWidth"] = 760d;
                _ = window.ShowMetroDialogAsync(dialog);
                await WaitFor<CustomDialog>(window);

                Assert.That(Card(dialog).MaxWidth, Is.EqualTo(760));
                Assert.That(Card(dialog).ActualWidth, Is.GreaterThan(700));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("Handed nothing, a custom dialog keeps the Metro band.")]
        public async Task ACustomDialogStaysMetroByItself()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var dialog = new CustomDialog(window) { Title = "42" };
                _ = window.ShowMetroDialogAsync(dialog);
                await WaitFor<CustomDialog>(window);

                // the template of the theme and the one the application merged are two instances, so
                // the band is told by what it lacks
                Assert.That(dialog.FindChild<FrameworkElement>("PART_Content"), Is.Not.Null);
                Assert.That(dialog.FindChild<FrameworkElement>("Card"), Is.Null);
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10, MessageDialogStyle.AffirmativeAndNegative, 2)]
        [TestCase(WinUI, MessageDialogStyle.AffirmativeAndNegative, 2)]
        [TestCase(Win10, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, 3)]
        [TestCase(WinUI, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, 3)]
        [TestCase(Win10, MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, 4)]
        [TestCase(WinUI, MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, 4)]
        [Description("The buttons that show share the row in equal parts.")]
        public async Task TheButtonsShareTheRow(string look, MessageDialogStyle style, int count)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var settings = new MetroDialogSettings { CustomResourceDictionary = Look(look), FirstAuxiliaryButtonText = "Later", SecondAuxiliaryButtonText = "Never" };
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", style, settings);
                var dialog = await WaitFor<MessageDialog>(window);

                Assert.That(dialog.Template, Is.SameAs(dialog.FindResource($"MahApps.Templates.MessageDialog.{look}")));

                var shown = new[] { "PART_AffirmativeButton", "PART_NegativeButton", "PART_FirstAuxiliaryButton", "PART_SecondAuxiliaryButton" }
                            .Select(p => dialog.FindChild<Button>(p)!)
                            .Where(b => b.IsVisible)
                            .ToList();
                Assert.That(shown, Has.Count.EqualTo(count));

                var pixel = 1 / VisualTreeHelper.GetDpi(dialog).PixelsPerDip;
                var widths = shown.Select(b => b.ActualWidth).ToList();
                Assert.That(widths.Max() - widths.Min(), Is.LessThanOrEqualTo(pixel + 0.01));
                Assert.That(shown.Sum(b => b.ActualWidth + 8), Is.EqualTo(dialog.FindChild<FrameworkElement>("ButtonRow")!.ActualWidth).Within(count * pixel + 0.01));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Without an icon the message starts where the title does.")]
        public async Task WithoutAnIconTheMessageIsNotIndented(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", MessageDialogStyle.Affirmative, new MetroDialogSettings { CustomResourceDictionary = Look(look) });
                var dialog = await WaitFor<MessageDialog>(window);

                var title = dialog.FindChild<TextBlock>("PART_Title")!.TranslatePoint(new Point(), dialog).X;
                var message = dialog.FindChild<TextBlock>("PART_MessageTextBlock")!.TranslatePoint(new Point(), dialog).X;

                Assert.That(message, Is.EqualTo(title).Within(1.0));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10, "Segoe UI", "Segoe UI", "Normal")]
        [TestCase(WinUI, "Segoe UI Variable Text, Segoe UI", "Segoe UI Variable Display, Segoe UI", "SemiBold")]
        [Description("The title of a dialog in a Windows look is the Subtitle step of its ramp, the message the Body step, and the buttons write like the other controls of that look.")]
        public async Task TheTextOfADialogIsOnTheRampOfItsLook(string look, string text, string titleFamily, string titleWeight)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", MessageDialogStyle.Affirmative, new MetroDialogSettings { CustomResourceDictionary = Look(look) });
                var dialog = await WaitFor<MessageDialog>(window);

                var title = dialog.FindChild<TextBlock>("PART_Title")!;
                var message = dialog.FindChild<TextBlock>("PART_MessageTextBlock")!;
                var button = dialog.FindChild<Button>("PART_AffirmativeButton")!;

                Assert.Multiple(() =>
                    {
                        Assert.That(title.FontSize, Is.EqualTo(20), "the size of the title");
                        Assert.That(title.FontFamily.Source, Is.EqualTo(titleFamily), "the family of the title");
                        Assert.That(title.FontWeight.ToString(), Is.EqualTo(titleWeight), "the weight of the title");
                        Assert.That(message.FontSize, Is.EqualTo(14), "the size of the message");
                        Assert.That(message.FontFamily.Source, Is.EqualTo(text), "the family of the message");
                        Assert.That(button.FontSize, Is.EqualTo(14), "the size of the button");
                        Assert.That(button.FontFamily.Source, Is.EqualTo(text), "the family of the button");
                    });
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A single button stands in the right half.")]
        public async Task ASingleButtonStandsRight(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", MessageDialogStyle.Affirmative, new MetroDialogSettings { CustomResourceDictionary = Look(look) });
                var dialog = await WaitFor<MessageDialog>(window);

                var row = dialog.FindChild<FrameworkElement>("ButtonRow")!;
                var left = dialog.FindChild<Button>("PART_AffirmativeButton")!.TranslatePoint(new Point(), row).X;

                Assert.That(left, Is.GreaterThanOrEqualTo(row.ActualWidth / 2 - 1));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10, MessageDialogResult.Affirmative, "PART_AffirmativeButton")]
        [TestCase(WinUI, MessageDialogResult.Affirmative, "PART_AffirmativeButton")]
        [TestCase(Win10, MessageDialogResult.Negative, "PART_NegativeButton")]
        [TestCase(WinUI, MessageDialogResult.Negative, "PART_NegativeButton")]
        [Description("The button the settings name for return wears the accent of the look.")]
        public async Task TheDefaultButtonWearsTheAccent(string look, MessageDialogResult focus, string part)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { CustomResourceDictionary = Look(look), DefaultButtonFocus = focus });
                var dialog = await WaitFor<MessageDialog>(window);

                Assert.That(dialog.FindChild<Button>(part)!.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.Button.Accent.{look}")));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The input dialog asks with the text box of its look and hands the line back.")]
        public async Task TheInputDialogTakesTheLook(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var answer = window.ShowInputAsync("Deep Thought", "What is the answer to everything?", new MetroDialogSettings { CustomResourceDictionary = Look(look) });
                var dialog = await WaitFor<InputDialog>(window);

                Assert.That(dialog.Template, Is.SameAs(dialog.FindResource($"MahApps.Templates.InputDialog.{look}")));
                var box = dialog.FindChild<TextBox>("PART_TextBox")!;
                Assert.That(box.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.TextBox.{look}")));
                var affirmative = dialog.FindChild<Button>("PART_AffirmativeButton")!;
                Assert.That(affirmative.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.Button.Accent.{look}")));

                box.SetCurrentValue(TextBox.TextProperty, "42");
                affirmative.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

                Assert.That(await answer, Is.EqualTo("42"));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The login dialog asks with the boxes of its look.")]
        public async Task TheLoginDialogTakesTheLook(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { CustomResourceDictionary = Look(look), InitialUsername = "MahApps", RememberCheckBoxVisibility = Visibility.Visible });
                var dialog = await WaitFor<LoginDialog>(window);

                Assert.That(dialog.Template, Is.SameAs(dialog.FindResource($"MahApps.Templates.LoginDialog.{look}")));
                Assert.That(dialog.FindChild<TextBox>("PART_TextBox")!.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.TextBox.{look}")));
                Assert.That(dialog.FindChild<PasswordBox>("PART_PasswordBox")!.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.PasswordBox.{look}")));
                Assert.That(dialog.FindChild<CheckBox>("PART_RememberCheckBox")!.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.CheckBox.{look}")));
                Assert.That(dialog.FindChild<TextBox>("PART_TextBox")!.Text, Is.EqualTo("MahApps"));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10, false)]
        [TestCase(WinUI, false)]
        [TestCase(Win10, true)]
        [TestCase(WinUI, true)]
        [Description("The password box shows its eye only when the settings ask for a preview.")]
        public async Task TheLoginDialogShowsTheEyeOnlyWhenAsked(string look, bool preview)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { CustomResourceDictionary = Look(look), EnablePasswordPreview = preview });
                var dialog = await WaitFor<LoginDialog>(window);

                Assert.That(PasswordBoxHelper.GetShowRevealButton(dialog.FindChild<PasswordBox>("PART_PasswordBox")!), Is.EqualTo(preview));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Without its negative button, the login dialog puts the one that is left on the right.")]
        public async Task TheLoginDialogWithOneButtonPutsItRight(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                _ = window.ShowLoginAsync("Authentication", "Enter your password", new LoginDialogSettings { CustomResourceDictionary = Look(look), ShouldHideUsername = true, NegativeButtonVisibility = Visibility.Collapsed });
                var dialog = await WaitFor<LoginDialog>(window);

                var row = dialog.FindChild<FrameworkElement>("ButtonRow")!;
                var left = dialog.FindChild<Button>("PART_AffirmativeButton")!.TranslatePoint(new Point(), row).X;

                Assert.That(left, Is.GreaterThanOrEqualTo(row.ActualWidth / 2 - 1));
                Assert.That(dialog.FindChild<TextBox>("PART_TextBox")!.Visibility, Is.EqualTo(Visibility.Collapsed));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("The progress dialog shows the bar of its look, and no row of buttons while it cannot be cancelled.")]
        public async Task TheProgressDialogTakesTheLook(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Please wait...", "We are baking now some cupcakes!", settings: new MetroDialogSettings { CustomResourceDictionary = Look(look), AnimateShow = false, AnimateHide = false });
                var dialog = await WaitFor<ProgressDialog>(window);

                Assert.That(dialog.Template, Is.SameAs(dialog.FindResource($"MahApps.Templates.ProgressDialog.{look}")));
                Assert.That(dialog.FindChild<MetroProgressBar>("PART_ProgressBar")!.Style, Is.SameAs(dialog.FindResource($"MahApps.Styles.MetroProgressBar.{look}")));
                Assert.That(dialog.FindChild<FrameworkElement>("ButtonRow")!.IsVisible, Is.False);

                controller.SetCancelable(true);
                ClipAssert.Pump();
                Assert.That(dialog.FindChild<Button>("PART_NegativeButton")!.IsVisible, Is.True);

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("Turned the other way round, the card is drawn in the colours of the other theme.")]
        public async Task AnInvertedDialogTakesTheOtherTheme(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                ThemeManager.Current.ChangeTheme(window, "Dark.Blue");
                _ = window.ShowMessageAsync("Deep Thought", "Come back in seven and a half million years.", settings: new MetroDialogSettings { CustomResourceDictionary = Look(look), ColorScheme = MetroDialogColorScheme.Inverted });
                var dialog = await WaitFor<MessageDialog>(window);

                var light = ThemeManager.Current.GetTheme("Light.Blue")!;
                var expected = ((SolidColorBrush)light.Resources[$"MahApps.Brushes.ContentDialog.{look}.Background"]).Color;

                Assert.That(((SolidColorBrush)dialog.FindChild<Border>("CardBorder")!.Background).Color, Is.EqualTo(expected));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A message taller than MaximumBodyHeight scrolls inside the card, and the buttons stay in it.")]
        public async Task ALongMessageScrollsAndTheButtonsStay(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var message = string.Join(Environment.NewLine, Enumerable.Range(1, 40).Select(i => $"Line {i} of the answer"));
                _ = window.ShowMessageAsync("Deep Thought", message, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { CustomResourceDictionary = Look(look), MaximumBodyHeight = 100 });
                var dialog = await WaitFor<MessageDialog>(window);

                var viewer = dialog.FindChild<ScrollViewer>("PART_MessageScrollViewer")!;
                Assert.That(viewer.ActualHeight, Is.LessThanOrEqualTo(101));
                Assert.That(viewer.ExtentHeight, Is.GreaterThan(viewer.ViewportHeight));

                var card = Card(dialog);
                var button = dialog.FindChild<Button>("PART_AffirmativeButton")!;
                var bottom = button.TranslatePoint(new Point(0, button.ActualHeight), card).Y;
                Assert.That(button.IsVisible, Is.True);
                Assert.That(bottom, Is.LessThanOrEqualTo(card.ActualHeight));
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        [Description("The WinUI progress dialog keeps its button layer away until there is something to cancel.")]
        public async Task TheWinUIProgressDialogHasNoEmptyButtonLayer()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var controller = await window.ShowProgressAsync("Please wait...", "We are baking now some cupcakes!", settings: new MetroDialogSettings { CustomResourceDictionary = Look(WinUI), AnimateShow = false, AnimateHide = false });
                var dialog = await WaitFor<ProgressDialog>(window);

                Assert.That(dialog.FindChild<FrameworkElement>("CommandSpace")!.IsVisible, Is.False);

                controller.SetCancelable(true);
                ClipAssert.Pump();
                Assert.That(dialog.FindChild<FrameworkElement>("CommandSpace")!.IsVisible, Is.True);

                await controller.CloseAsync();
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10, "Message")]
        [TestCase(WinUI, "Message")]
        [TestCase(Win10, "Input")]
        [TestCase(WinUI, "Input")]
        [TestCase(Win10, "Login")]
        [TestCase(WinUI, "Login")]
        [TestCase(Win10, "Progress")]
        [TestCase(WinUI, "Progress")]
        [Description("Each dialog without a title leaves no room for one.")]
        public async Task NoTitleLeavesNoRoomInAnyDialog(string look, string kind)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var settings = new LoginDialogSettings { CustomResourceDictionary = Look(look), AnimateShow = false, AnimateHide = false };
                BaseMetroDialog dialog;
                switch (kind)
                {
                    case "Message":
                        _ = window.ShowMessageAsync(null!, "Come back in seven and a half million years.", settings: settings);
                        dialog = await WaitFor<MessageDialog>(window);
                        break;
                    case "Input":
                        _ = window.ShowInputAsync(null!, "What is the answer to everything?", settings);
                        dialog = await WaitFor<InputDialog>(window);
                        break;
                    case "Login":
                        _ = window.ShowLoginAsync(null!, "Enter your credentials", settings);
                        dialog = await WaitFor<LoginDialog>(window);
                        break;
                    default:
                        _ = window.ShowProgressAsync(null!, "We are baking now some cupcakes!", settings: settings);
                        dialog = await WaitFor<ProgressDialog>(window);
                        break;
                }

                Assert.That(dialog.FindChild<TextBlock>("PART_Title")!.Visibility, Is.EqualTo(Visibility.Collapsed));
            }
            finally
            {
                window.Close();
            }
        }

        [TestCase(Win10)]
        [TestCase(WinUI)]
        [Description("A dialog without a title leaves no room for one.")]
        public async Task NoTitleLeavesNoRoom(string look)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();

            try
            {
                var dialog = new CustomDialog(window, new MetroDialogSettings { CustomResourceDictionary = Look(look) }) { Content = new TextBlock { Text = "The answer" } };
                _ = window.ShowMetroDialogAsync(dialog);
                await WaitFor<CustomDialog>(window);

                Assert.That(dialog.FindChild<TextBlock>("PART_Title")!.Visibility, Is.EqualTo(Visibility.Collapsed));
            }
            finally
            {
                window.Close();
            }
        }
    }
}
