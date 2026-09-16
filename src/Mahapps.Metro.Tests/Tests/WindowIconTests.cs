// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4488: the icon on the title bar opens the system menu on a left click, the way a window
    /// does. A right click is not the same thing: an icon template that brings a context menu of
    /// its own gets to show it.
    /// </summary>
    [TestFixture]
    public class WindowIconTests
    {
        private const int WM_ENTERMENULOOP = 0x0211;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EndMenu();

        private IconTemplateWindow window = null!;
        private Grid iconContent = null!;
        private ContextMenu iconMenu = null!;
        private int systemMenuOpenings;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<IconTemplateWindow>();

            this.iconContent = this.window.FindChild<Grid>("IconWithMenu")!;
            Assert.That(this.iconContent, Is.Not.Null, "the icon template should be up");

            this.iconMenu = this.iconContent.ContextMenu!;
            Assert.That(this.iconMenu, Is.Not.Null, "and it should carry a context menu");

            // The system menu holds the message loop until somebody picks something, which nobody is
            // here to do, so it is sent away as soon as it shows up, and that it showed up is the
            // thing these tests read.
            ((HwndSource)PresentationSource.FromVisual(this.window)!).AddHook(this.OnMessage);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        [SetUp]
        public void SetUp()
        {
            this.systemMenuOpenings = 0;
            this.iconContent.SetCurrentValue(FrameworkElement.ContextMenuProperty, this.iconMenu);
        }

        private IntPtr OnMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_ENTERMENULOOP)
            {
                this.systemMenuOpenings++;
                EndMenu();
            }

            return IntPtr.Zero;
        }

        private void ClickTheIcon(MouseButton button, params RoutedEvent[] routedEvents)
        {
            foreach (var routedEvent in routedEvents)
            {
                this.iconContent.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, button)
                                            {
                                                RoutedEvent = routedEvent,
                                                Source = this.iconContent
                                            });
            }

            ClipAssert.Pump();
        }

        [Test]
        [Description("A press of the left button on the icon opens the system menu, the way it does on any window.")]
        public void TheLeftButtonOpensTheSystemMenu()
        {
            this.ClickTheIcon(MouseButton.Left, UIElement.MouseDownEvent, UIElement.MouseLeftButtonDownEvent);

            Assert.That(this.systemMenuOpenings, Is.EqualTo(1));
        }

        [Test]
        [Description("A title bar that goes away and comes back leaves the icon with one handler, not two.")]
        public void TheIconOpensTheSystemMenuOnceAfterTheTitleBarCameBack()
        {
            this.window.SetCurrentValue(MetroWindow.ShowTitleBarProperty, false);
            this.window.SetCurrentValue(MetroWindow.ShowTitleBarProperty, true);
            ClipAssert.Pump();

            this.ClickTheIcon(MouseButton.Left, UIElement.MouseDownEvent, UIElement.MouseLeftButtonDownEvent);

            Assert.That(this.systemMenuOpenings, Is.EqualTo(1));
        }

        [Test]
        [Description("A press of the right button does not, because the menu belongs at the end of the click, not the start of it.")]
        public void TheRightButtonLeavesTheSystemMenuAloneWhenItGoesDown()
        {
            this.ClickTheIcon(MouseButton.Right, UIElement.MouseDownEvent, UIElement.MouseRightButtonDownEvent);

            Assert.That(this.systemMenuOpenings, Is.Zero);
        }

        [Test]
        [Description("A right click on a template that has a menu of its own leaves that menu to it.")]
        public void ARightClickLeavesTheMenuOfTheTemplateAlone()
        {
            this.ClickTheIcon(MouseButton.Right, UIElement.MouseUpEvent, UIElement.MouseRightButtonUpEvent);

            Assert.That(this.systemMenuOpenings, Is.Zero);
        }

        [Test]
        [Description("Where the template has none, a right click falls back to the system menu.")]
        public void ARightClickOpensTheSystemMenuWithoutAMenuOfItsOwn()
        {
            this.iconContent.SetCurrentValue(FrameworkElement.ContextMenuProperty, null);

            this.ClickTheIcon(MouseButton.Right, UIElement.MouseUpEvent, UIElement.MouseRightButtonUpEvent);

            Assert.That(this.systemMenuOpenings, Is.EqualTo(1));
        }

        [Test]
        [Description("And a window told not to show the system menu on a right click shows none.")]
        public void ARightClickShowsNothingWhereTheWindowSaysSo()
        {
            this.iconContent.SetCurrentValue(FrameworkElement.ContextMenuProperty, null);
            this.window.SetCurrentValue(MetroWindow.ShowSystemMenuOnRightClickProperty, false);

            try
            {
                this.ClickTheIcon(MouseButton.Right, UIElement.MouseUpEvent, UIElement.MouseRightButtonUpEvent);

                Assert.That(this.systemMenuOpenings, Is.Zero);
            }
            finally
            {
                this.window.SetCurrentValue(MetroWindow.ShowSystemMenuOnRightClickProperty, true);
            }
        }
    }
}
