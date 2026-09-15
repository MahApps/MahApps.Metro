// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4108: every change of content built two views, the one arriving and the one on its way out,
    /// because the outgoing content was handed to the second presenter, which had to raise it from its
    /// template all over again only to fade it away. A plain ContentControl builds the one arriving.
    /// </summary>
    [TestFixture]
    public class TransitioningContentControlViewTests
    {
        /// <summary>A view that says so every time one of it is built.</summary>
        public sealed class CountedView : ContentControl
        {
            private static readonly List<string> BuiltSoFar = new List<string>();

            public CountedView()
            {
                BuiltSoFar.Add(this.GetType().Name);
            }

            /// <summary>What has been built, in the order it was built in.</summary>
            public static IReadOnlyList<string> Built => BuiltSoFar;

            public static void Note(string name)
            {
                BuiltSoFar.Add(name);
            }

            public static void Forget()
            {
                BuiltSoFar.Clear();
            }
        }

        public sealed class OtherCountedView : ContentControl
        {
            public OtherCountedView()
            {
                CountedView.Note(this.GetType().Name);
            }
        }

        private sealed class First
        {
        }

        private sealed class Second
        {
        }

        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
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
            CountedView.Forget();
        }

        [Test]
        [Description("Moving to content with a template of its own builds the view arriving and nothing else. The one on its way out is already built and stays where it is until it has faded.")]
        public void ChangingContentBuildsTheViewArrivingAndNoOther()
        {
            var control = this.Show(out var first, out var second);

            control.SetCurrentValue(ContentControl.ContentProperty, first);
            this.Settle();

            Assume.That(CountedView.Built, Is.EqualTo(new[] { nameof(CountedView) }), "the first view should have been built to start with");
            CountedView.Forget();

            control.SetCurrentValue(ContentControl.ContentProperty, second);
            this.Settle();

            Assert.That(CountedView.Built, Is.EqualTo(new[] { nameof(OtherCountedView) }));
        }

        [Test]
        [Description("Content sharing a template with what is showing builds one view and no more. It cannot build none: the two are faded over one another, so both have to be standing there at once, which is the one thing a plain ContentControl gets away with that this cannot.")]
        public void ChangingToContentOfTheSameTemplateBuildsOneViewAndNoMore()
        {
            var control = this.Show(out var first, out _);

            control.SetCurrentValue(ContentControl.ContentProperty, first);
            this.Settle();
            CountedView.Forget();

            control.SetCurrentValue(ContentControl.ContentProperty, new First());
            this.Settle();

            Assert.That(CountedView.Built, Is.EqualTo(new[] { nameof(CountedView) }));
        }

        [Test]
        [Description("Once the fade is over, the content that arrived is the one left on show and the one that went is let go of, so the control holds nothing back.")]
        public void WhatIsLeftOnShowIsTheContentThatArrived()
        {
            var control = this.Show(out var first, out var second);

            control.SetCurrentValue(ContentControl.ContentProperty, first);
            this.Settle();
            control.SetCurrentValue(ContentControl.ContentProperty, second);
            this.Settle();

            control.AbortTransition();
            this.Settle();

            var holding = new List<ContentPresenter>();
            foreach (var presenter in Presenters(control))
            {
                if (presenter.Content is not null)
                {
                    holding.Add(presenter);
                }
            }

            Assert.That(holding, Has.Count.EqualTo(1), "one presenter should be holding something and the other nothing");
            Assert.That(holding[0].Content, Is.SameAs(second), "and it should be the content that arrived last");
        }

        private static IEnumerable<ContentPresenter> Presenters(TransitioningContentControl control)
        {
            foreach (var name in new[] { "PreviousContentPresentationSite", "CurrentContentPresentationSite" })
            {
                if (control.Template?.FindName(name, control) is ContentPresenter presenter)
                {
                    yield return presenter;
                }
            }
        }

        private TransitioningContentControl Show(out object first, out object second)
        {
            Assert.That(this.window, Is.Not.Null);

            first = new First();
            second = new Second();

            var control = new TransitioningContentControl { Width = 200, Height = 100 };
            control.Resources.Add(new DataTemplateKey(typeof(First)), TemplateFor(typeof(CountedView)));
            control.Resources.Add(new DataTemplateKey(typeof(Second)), TemplateFor(typeof(OtherCountedView)));

            this.window!.Content = control;
            this.Settle();

            Assert.That(control.IsLoaded, Is.True, "the control should be up before a test looks at it");

            return control;
        }

        private static DataTemplate TemplateFor(System.Type view)
        {
            return new DataTemplate { VisualTree = new FrameworkElementFactory(view) };
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
