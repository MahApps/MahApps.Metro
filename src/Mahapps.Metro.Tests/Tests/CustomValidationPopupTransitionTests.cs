// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
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
    /// A page that is left stays in a <see cref="TransitioningContentControl"/> until it has faded
    /// out, and its message used to stay open over it and over the page coming in, until the old
    /// page was gone for good.
    /// </summary>
    [TestFixture]
    public class CustomValidationPopupTransitionTests
    {
        [Test]
        [Description("The message of a page that is left goes as soon as the page starts to fade, not once it is gone.")]
        public async Task TheMessageGoesWithThePageItIsOn()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<ValidationPopupTransitionWindow>().ConfigureAwait(true);
            try
            {
                for (var round = 0; round < 3; round++)
                {
                    window.UpdateLayout();
                    window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
                }

                var popup = AdornerLayer.GetAdornerLayer(window.Box)!
                                        .FindChildren<CustomValidationPopup>(true)
                                        .First(candidate => ReferenceEquals(candidate.AdornedElement, window.Box));
                Assume.That(popup.IsOpen, Is.True, "the box should show its message before the page is left");

                window.Pages.Content = new TextBlock { Text = "Another page" };
                window.Dispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

                Assume.That(window.Pages.IsTransitioning, Is.True, "the old page should still be fading out");
                Assert.Multiple(() =>
                    {
                        Assert.That(popup.CanShow, Is.False);
                        Assert.That(popup.IsOpen, Is.False);
                    });
            }
            finally
            {
                window.Close();
            }
        }
    }
}
