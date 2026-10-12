// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class InfoBarsControlTests : WindowTestFixture<TestWindow>
    {
        private InfoBarsControl Show(params object[] items)
        {
            var host = new InfoBarsControl();
            foreach (var item in items)
            {
                host.Items.Add(item);
            }

            this.window!.Content = host;
            this.window.UpdateLayout();
            return host;
        }

        private static Color BackgroundOf(InfoBar infoBar)
        {
            return ((SolidColorBrush)infoBar.FindChild<Border>("ContentRoot")!.Background).Color;
        }

        [Test]
        public void ABarInTheHostIsHostedAndOneOutsideIsNot()
        {
            var inside = new InfoBar { IsOpen = true, Title = "Update" };
            this.Show(inside);

            Assert.Multiple(() =>
                {
                    Assert.That(InfoBarsControl.GetIsHosted(inside), Is.True);
                    Assert.That(InfoBarsControl.GetIsHosted(new InfoBar()), Is.False);
                });
        }

        [Test]
        public void AnythingElseGetsABarAsItsContainer()
        {
            var host = this.Show("42 files are waiting.");

            var container = host.ItemContainerGenerator.ContainerFromIndex(0);

            Assert.Multiple(() =>
                {
                    Assert.That(container, Is.InstanceOf<InfoBar>());
                    Assert.That(InfoBarsControl.GetIsHosted((InfoBar)container), Is.True);
                });
        }

        [Test]
        public void AnInformationalBarInTheHostIsOpaque()
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update" };
            this.Show(infoBar);

            Assert.That(BackgroundOf(infoBar), Is.EqualTo((Color)ColorConverter.ConvertFromString("#FFFAFAFA")));
        }

        [Test]
        public void TheOpaqueGroundFollowsATheme()
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update" };
            this.Show(infoBar);

            ThemeManager.Current.ChangeTheme(this.window!, "Dark.Blue");
            try
            {
                this.window!.UpdateLayout();

                Assert.That(BackgroundOf(infoBar), Is.EqualTo((Color)ColorConverter.ConvertFromString("#FF1B1B1B")));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }

        [Test]
        public void TheOtherSeveritiesKeepTheirGround()
        {
            var infoBar = new InfoBar { IsOpen = true, Title = "Update", Severity = InfoBarSeverity.Error };
            this.Show(infoBar);

            Assert.That(infoBar.FindChild<Border>("ContentRoot")!.Background, Is.SameAs(infoBar.FindResource("MahApps.Brushes.InfoBar.Error.Background")));
        }

        [Test]
        public void OpenBarsStandApartAndAClosedOneTakesNoRoom()
        {
            var open = new InfoBar { IsOpen = true, Title = "Update" };
            var closed = new InfoBar { Title = "Offline" };
            this.Show(open, closed);

            Assert.Multiple(() =>
                {
                    Assert.That(open.Margin, Is.EqualTo(new Thickness(0, 0, 0, 8)));
                    Assert.That(closed.Margin, Is.EqualTo(new Thickness(0)));
                    Assert.That(closed.ActualHeight, Is.EqualTo(0));
                });
        }

        [Test]
        public void TheHostStandsOffTheEdgeAndTakesNoFocus()
        {
            var host = this.Show();

            Assert.Multiple(() =>
                {
                    Assert.That(host.Margin, Is.EqualTo(new Thickness(12, 12, 12, 0)));
                    Assert.That(host.Focusable, Is.False);
                    Assert.That(host.Background, Is.Null, "the content around the bars should stay clickable");
                });
        }
    }
}
