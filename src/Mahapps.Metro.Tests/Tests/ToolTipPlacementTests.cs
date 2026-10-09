// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Windows puts a tool tip above what it explains rather than below the pointer, as WPF does:
    /// ToolTip_Partial.cpp of microsoft-ui-xaml has PlacementMode_Top as the default, centres the tip
    /// on the pointer with 20 between the two, and on the control with 12 when the keyboard opened
    /// it. ToolTipHelper.PlaceLikeWindows does the same in WPF, and the two Windows looks turn it on.
    /// The pointer cannot be moved by a test, so the cases here are the ones the keyboard opens; the
    /// pointer goes through the same arithmetic with a point in place of the control.
    /// </summary>
    [TestFixture]
    public class ToolTipPlacementTests : WindowTestFixture<TestWindow>
    {
        private ToolTip? opened;

        [TearDown]
        public void CloseTheToolTip()
        {
            if (this.opened is not null)
            {
                this.opened.IsOpen = false;
                this.opened = null;
            }
        }

        [TestCase("MahApps.Styles.ToolTip", false)]
        [TestCase("MahApps.Styles.ToolTip.Win10", true)]
        [TestCase("MahApps.Styles.ToolTip.WinUI", true)]
        [Description("The Windows looks place their tool tip the way Windows does, the Metro one leaves it where WPF puts it.")]
        public void TheWindowsLooksPlaceLikeWindows(string key, bool expected)
        {
            var tip = new ToolTip { Style = (Style)Application.Current.FindResource(key) };

            Assert.Multiple(() =>
                {
                    Assert.That(ToolTipHelper.GetPlaceLikeWindows(tip), Is.EqualTo(expected));
                    Assert.That(tip.Placement, Is.EqualTo(expected ? PlacementMode.Custom : PlacementMode.Mouse));
                    Assert.That(tip.CustomPopupPlacementCallback, expected ? Is.Not.Null : Is.Null);
                });
        }

        [Test]
        [Description("A tool tip that is not placed like Windows any more goes back to where WPF puts it.")]
        public void TurningItOffGivesThePlacementBack()
        {
            var tip = new ToolTip();
            ToolTipHelper.SetPlaceLikeWindows(tip, true);
            ToolTipHelper.SetPlaceLikeWindows(tip, false);

            Assert.Multiple(() =>
                {
                    Assert.That(tip.Placement, Is.EqualTo(PlacementMode.Mouse));
                    Assert.That(tip.CustomPopupPlacementCallback, Is.Null);
                });
        }

        [Test]
        [Description("Centred on the control, 12 above it, and 12 below it where there is no room above. WPF hands the callback device pixels, so the 12 is scaled by the DPI.")]
        public void TheKeyboardPutsItCentredAboveTheControl()
        {
            var (tip, scale) = this.Open(null);
            var popup = new Size(100, 30);
            var target = new Size(200, 40);

            var placements = tip.CustomPopupPlacementCallback(popup, target, new Point());

            Assert.That(placements, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
                {
                    Assert.That(placements[0].Point.X, Is.EqualTo(50).Within(0.01), "centred");
                    Assert.That(placements[0].Point.Y, Is.EqualTo(-30 - 12 * scale).Within(0.01), "above");
                    Assert.That(placements[1].Point.X, Is.EqualTo(50).Within(0.01), "centred below as well");
                    Assert.That(placements[1].Point.Y, Is.EqualTo(40 + 12 * scale).Within(0.01), "below where there is no room above");
                });
        }

        [Test]
        [Description("The offsets of the tool tip move it on from there, the way they move any popup.")]
        public void TheOffsetsMoveItOn()
        {
            var (tip, scale) = this.Open(null);

            var placements = tip.CustomPopupPlacementCallback(new Size(100, 30), new Size(200, 40), new Point(10, 5));

            Assert.Multiple(() =>
                {
                    Assert.That(placements[0].Point.X, Is.EqualTo(50 + 10 * scale).Within(0.01));
                    Assert.That(placements[0].Point.Y, Is.EqualTo(-30 - 12 * scale + 5 * scale).Within(0.01));
                });
        }

        [Test]
        [Description("The room the shadow of the WinUI tool tip takes is no part of the tip, so it is the box that stands 12 above the control, not the shadow.")]
        public void TheShadowIsLeftOutOfTheGap()
        {
            Assume.That(SystemParameters.DropShadow, Is.True, "this desktop draws no shadows");

            var (tip, scale) = this.Open("MahApps.Styles.ToolTip.WinUI");
            // the margin the WinUI template gives the box for its shadow: 8 4 8 12
            var popup = new Size((100 + 16) * scale, (30 + 16) * scale);
            var target = new Size(200, 40);

            var placements = tip.CustomPopupPlacementCallback(popup, target, new Point());

            Assert.Multiple(() =>
                {
                    Assert.That(placements[0].Point.X, Is.EqualTo((200 - popup.Width) / 2).Within(0.01), "the shadow is the same on both sides");
                    Assert.That(placements[0].Point.Y + popup.Height - 12 * scale, Is.EqualTo(-12 * scale).Within(0.01), "the bottom of the box 12 above the control");
                    Assert.That(placements[1].Point.Y + 4 * scale, Is.EqualTo(40 + 12 * scale).Within(0.01), "the top of the box 12 below it");
                });
        }

        private (ToolTip Tip, double Scale) Open(string? key)
        {
            var target = new Button { Content = "42" };
            var tip = new ToolTip { Content = "42", PlacementTarget = target };
            if (key is not null)
            {
                tip.Style = (Style)Application.Current.FindResource(key);
            }
            else
            {
                // the Metro tool tip the application wears keeps room for a shadow of its own, which these cases leave out
                tip.HasDropShadow = false;
                ToolTipHelper.SetPlaceLikeWindows(tip, true);
            }

            target.ToolTip = tip;
            this.window!.Content = target;
            this.window.UpdateLayout();

            // a tool tip casts its shadow only from inside its popup, which is where the callback is asked
            tip.IsOpen = true;
            this.opened = tip;
            this.window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

            return (tip, VisualTreeHelper.GetDpi(target).DpiScaleY);
        }
    }
}
