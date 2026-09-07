// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A clip lives in the coordinates of the element it sits on, so the geometry of the button has to
    /// describe the content of the border rather than the border itself.
    /// </summary>
    [TestFixture]
    public class VisualStudioButtonClipTests
    {
        private static Button CreateButton(Thickness padding)
        {
            var dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/VS/Button.xaml", UriKind.Absolute) };

            var button = new Button
                         {
                             Width = 200,
                             Height = 45,
                             Padding = padding,
                             BorderThickness = new Thickness(2),
                             Style = (Style)dictionary["MahApps.Styles.Button.VisualStudio"],
                             Content = "Beam me up..."
                         };
            ControlsHelper.SetCornerRadius(button, new CornerRadius(22));

            return button;
        }

        private static FrameworkElement GetClippedElement(Button button)
        {
            var border = button.FindChild<Border>("Border");
            Assert.That(border, Is.Not.Null, "the template should carry the border");

            var content = VisualTreeHelper.GetChild(border!, 0) as FrameworkElement;
            Assert.That(content, Is.Not.Null, "the border should carry the clipped content");

            return content!;
        }

        private static async Task<Button> ShowButtonAsync(TestWindow window, Thickness padding)
        {
            var button = CreateButton(padding);

            window.Content = button;
            window.UpdateLayout();
            button.UpdateLayout();

            await Task.Yield();

            return button;
        }

        [TestCase(0)]
        [TestCase(4)]
        public async Task TheClipShouldCoverTheClippedElement(double padding)
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var button = await ShowButtonAsync(window, new Thickness(padding));
                var content = GetClippedElement(button);

                ClipAssert.CoversElement(content, "button");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task TheClipShouldRoundEveryCornerOfTheContent()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>();

            try
            {
                var button = await ShowButtonAsync(window, new Thickness(0));
                var content = GetClippedElement(button);

                // The bottom right corner is what the report was about.
                ClipAssert.CutsEveryCorner(content, "button");
            }
            finally
            {
                window.Close();
            }
        }
    }
}
