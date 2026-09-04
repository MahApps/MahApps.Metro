// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
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

        private sealed class TransitionViewModel : INotifyPropertyChanged
        {
            private TransitionType transition;

            public TransitionType Transition
            {
                get => this.transition;
                set
                {
                    if (value == this.transition)
                    {
                        return;
                    }

                    this.transition = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Transition)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

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
            this.window?.TheTransitioningContentControlWithoutAnyTransition.ClearDependencyProperties(new[] { nameof(TransitioningContentControl.Transition), nameof(TransitioningContentControl.CustomVisualStatesName) });
            this.window?.TheInheritedTransitionPanel.ClearValue(TransitioningContentControl.TransitionProperty);
        }

        [Test]
        public void ShouldKeepThePreviousTransitionWhenTheNewOneCouldNotBeFound()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheTransitioningContentControl.Transition = TransitionType.Left;
            this.window.TheTransitioningContentControl.CustomVisualStatesName = "ThisStateDoesNotExist";

            Assert.DoesNotThrow(() => this.window.TheTransitioningContentControl.Transition = TransitionType.Custom);

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Left));
        }

        [Test]
        public void ShouldKeepABindingOnTransitionWhenTheTransitionCouldNotBeFound()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheTransitioningContentControl.CustomVisualStatesName = "ThisStateDoesNotExist";

            var viewModel = new TransitionViewModel { Transition = TransitionType.Left };
            BindingOperations.SetBinding(this.window.TheTransitioningContentControl,
                                         TransitioningContentControl.TransitionProperty,
                                         new Binding(nameof(TransitionViewModel.Transition)) { Source = viewModel });

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Left));

            Assert.DoesNotThrow(() => viewModel.Transition = TransitionType.Custom);

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Left));

            viewModel.Transition = TransitionType.Up;

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Up));
        }

        [Test]
        public void ShouldNotRecurseWhenTheFallbackTransitionIsMissingAsWell()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.DoesNotThrow(() => this.window.TheTransitioningContentControlWithoutAnyTransition.Transition = TransitionType.Up);

            Assert.That(this.window.TheTransitioningContentControlWithoutAnyTransition.Transition, Is.EqualTo(TransitioningContentControl.DefaultTransitionState));
        }

        [Test]
        public void ShouldKeepATransitionThatWasSetBeforeTheTemplateWasApplied()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(this.window.TheTransitioningContentControlWithTransitionFromXaml.Transition, Is.EqualTo(TransitionType.Custom));
        }

        [Test]
        public void ShouldInheritTheTransitionFromAParentPanel()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheInheritedTransitionPanel.SetValue(TransitioningContentControl.TransitionProperty, TransitionType.Up);

            Assert.That(this.window.TheInheritedTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Up));
        }

        [Test]
        public void ShouldInheritTheTransitionSetOnAParentPanelInXaml()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(this.window.TheTransitioningContentControlWithInheritedTransitionFromXaml.Transition, Is.EqualTo(TransitionType.LeftReplace));
        }

        [Test]
        public void ShouldAcceptACustomTransitionThatExists()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(this.window.TheTransitioningContentControl.CustomVisualStatesName, Is.EqualTo("CustomTransition"));

            Assert.DoesNotThrow(() => this.window.TheTransitioningContentControl.Transition = TransitionType.Custom);

            Assert.That(this.window.TheTransitioningContentControl.Transition, Is.EqualTo(TransitionType.Custom));
        }
    }
}
