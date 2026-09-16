// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4458 and GH-4461: the helper keeps a collection in step with what a list has selected. It
    /// does that from the SelectionChanged event, which is a routed one, so it hears about lists it
    /// has nothing to do with, and it hears about its own after whoever else was listening.
    /// </summary>
    [TestFixture]
    public class MultiSelectorHelperSyncTests
    {
        private MultiSelectorHelperTestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>().ConfigureAwait(false);
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
            this.window?.FirstListBox.UnselectAll();
            this.window?.SecondListBox.UnselectAll();
            this.window?.OuterListView.UnselectAll();
            this.window?.SelectedItems.Clear();

            ClipAssert.Pump();
        }

        [Test]
        [Description("GH-4458: somebody listening for the change sees the collection as it is by then, not as it was.")]
        public void TheCollectionIsInStepByTheTimeAnybodyElseHears()
        {
            var items = new ObservableCollection<string> { "one", "two", "three" };
            var selected = new ObservableCollection<string>();

            var listBox = new ListBox { ItemsSource = items, SelectionMode = SelectionMode.Extended };

            // whoever is listening registered first, which is what an event in XAML does
            var countWhenHeard = -1;
            listBox.SelectionChanged += (_, _) => countWhenHeard = selected.Count;

            MultiSelectorHelper.SetSelectedItems(listBox, selected);

            listBox.SelectedItems.Add("two");
            ClipAssert.Pump();

            Assert.That(selected, Has.Count.EqualTo(1), "the helper should have kept up");
            Assert.That(countWhenHeard, Is.EqualTo(1), "and it should have done so before anybody else was told");
        }

        [Test]
        [Description("GH-4461: a list inside a list keeps its selection to itself.")]
        public void AListInsideAListIsNoneOfTheOuterOnesBusiness()
        {
            Assert.That(this.window, Is.Not.Null);

            var row = this.window!.OuterListView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            Assert.That(row, Is.Not.Null, "the outer list should have built its rows");

            var inner = row!.FindChild<ListBox>("InnerListBox");
            Assert.That(inner, Is.Not.Null, "and each row should hold a list of its own");

            Assert.That(() =>
                        {
                            inner!.SetCurrentValue(Selector.SelectedIndexProperty, 0);
                            ClipAssert.Pump();
                        },
                        Throws.Nothing);

            Assert.That(this.window.SelectedItems, Is.Empty, "nothing the inner list does belongs in the collection of the outer one");
        }

        [Test]
        [Description("What the list it belongs to does still lands in the collection.")]
        public void WhatTheListItBelongsToDoesStillCounts()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.FirstListBox.SelectedItems.Add(this.window.Items[1]);
            ClipAssert.Pump();

            Assert.That(this.window.SelectedItems, Does.Contain(this.window.Items[1]));
        }
    }
}
