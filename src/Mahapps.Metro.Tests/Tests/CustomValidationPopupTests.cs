// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The message beside a box is a popup, and a popup is a window of its own that knows nothing
    /// about the view the box sits in. So a box the view has scrolled away used to leave its
    /// message behind, floating next to nothing, which is what GH-4404 is about.
    /// </summary>
    [TestFixture]
    public class CustomValidationPopupTests
    {
        /// <summary>
        /// The window of this fixture is what the tests work on, boxes and all, so it is opened
        /// here rather than filled per test the way <see cref="WindowTestFixture{TWindow}"/> does.
        /// </summary>
        private ValidationPopupWindow? window;

        [OneTimeSetUp]
        public async Task OpenTheWindow()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<ValidationPopupWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void CloseTheWindow()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void Settle()
        {
            for (var round = 0; round < 3; round++)
            {
                this.window!.UpdateLayout();
                this.window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
            }
        }

        [TearDown]
        public void PutTheWindowBack()
        {
            if (this.window is not null)
            {
                this.window.Scroller.ScrollToVerticalOffset(0);
                this.window.Height = 600;
                this.Settle();
            }
        }

        private CustomValidationPopup PopupOf(TextBox box)
        {
            Assume.That(Validation.GetHasError(box), Is.True, "the box should be in error for there to be a message");

            var layer = AdornerLayer.GetAdornerLayer(box);
            Assert.That(layer, Is.Not.Null, "the box should carry an adorner layer to draw the error on");

            var popup = layer!.FindChildren<CustomValidationPopup>(true)
                              .FirstOrDefault(candidate => ReferenceEquals(candidate.AdornedElement, box));
            Assert.That(popup, Is.Not.Null, "the error template should have put a popup on the layer");

            return popup!;
        }

        [Test]
        [Description("A box the view still shows gets its message, which is what everything else rests on.")]
        public void TheMessageShowsForABoxThatIsThere()
        {
            var popup = this.PopupOf(this.window!.Near);
            Assert.Multiple(() =>
                {
                    Assert.That(popup.CanShow, Is.True);
                    Assert.That(popup.IsOpen, Is.True);
                });
        }

        [Test]
        [Description("A box the view has scrolled past keeps its message to itself, rather than leaving it next to nothing.")]
        public void NoMessageForABoxTheViewHasScrolledAway()
        {
            var popup = this.PopupOf(this.window!.Far);

            Assert.Multiple(() =>
                {
                    Assert.That(popup.CanShow, Is.False);
                    Assert.That(popup.IsOpen, Is.False);
                });
        }

        [Test]
        [Description("And it gets its message back once the view brings it up again.")]
        public void TheMessageComesBackWithTheBox()
        {
            var popup = this.PopupOf(this.window!.Far);
            Assume.That(popup.IsOpen, Is.False, "it should start out without one");

            this.window.Scroller.ScrollToVerticalOffset(700);
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(popup.CanShow, Is.True);
                    Assert.That(popup.IsOpen, Is.True);
                });
        }


        [Test]
        [Description("A window that is made smaller takes the box out of the view just as scrolling does, and the message goes with it.")]
        public void TheMessageGoesWhenTheWindowIsMadeSmaller()
        {
            var popup = this.PopupOf(this.window!.Middle);
            Assume.That(popup.IsOpen, Is.True, "it should start out with one");

            this.window.Height = 200;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(popup.CanShow, Is.False);
                    Assert.That(popup.IsOpen, Is.False);
                });
        }

        [Test]
        [Description("And it is back once the window has room for the box again.")]
        public void TheMessageComesBackWithTheWindow()
        {
            var popup = this.PopupOf(this.window!.Middle);

            this.window.Height = 200;
            this.Settle();
            Assume.That(popup.IsOpen, Is.False, "the box should be out of the view by now");

            this.window.Height = 600;
            this.Settle();

            Assert.Multiple(() =>
                {
                    Assert.That(popup.CanShow, Is.True);
                    Assert.That(popup.IsOpen, Is.True);
                });
        }
    }
}
