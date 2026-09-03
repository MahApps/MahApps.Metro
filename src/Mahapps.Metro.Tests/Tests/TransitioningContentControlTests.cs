// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class TransitioningContentControlTests
    {
        private TransitioningContentControlWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TransitioningContentControlWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.window?.TheTransitioningContentControl.ClearDependencyProperties(new[] { nameof(TransitioningContentControl.Transition), nameof(TransitioningContentControl.CustomVisualStatesName) });
        }

        [Test]
        public void ShouldNameTheTransitionThatCouldNotBeFound()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheTransitioningContentControl.CustomVisualStatesName = "ThisStateDoesNotExist";

            var exception = Assert.Throws<MahAppsException>(() => this.window.TheTransitioningContentControl.Transition = TransitionType.Custom);

            Assert.That(exception?.Message, Is.EqualTo("'Custom' transition could not be found!"));
        }

        [Test]
        public void ShouldKeepThePreviousTransitionWhenTheNewOneCouldNotBeFound()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheTransitioningContentControl.Transition = TransitionType.Left;
            this.window.TheTransitioningContentControl.CustomVisualStatesName = "ThisStateDoesNotExist";

            Assert.Throws<MahAppsException>(() => this.window.TheTransitioningContentControl.Transition = TransitionType.Custom);

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Left));
        }

        [Test]
        public void ShouldAcceptACustomTransitionThatExists()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(this.window.TheTransitioningContentControl.CustomVisualStatesName, Is.EqualTo("CustomTransition"));

            Assert.DoesNotThrow(() => this.window.TheTransitioningContentControl.Transition = TransitionType.Custom);
        }
    }
}