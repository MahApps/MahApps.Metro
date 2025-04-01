// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MultiSelectorHelperTests
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
            this.PreparePropertiesForTest();
        }

        private void PreparePropertiesForTest()
        {
            this.window?.SelectedItems?.Clear();
            this.window?.MultiSelectionComboBox?.SelectedItems?.Clear();
            this.window?.FirstListBox?.SelectedItems?.Clear();
            this.window?.SecondListBox?.SelectedItems?.Clear();
        }

        [Test]
        public void SelectedItemsShouldBeSyncedByMultiSelectionHelper()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.FirstListBox.SelectedItems.Add(window.Items[0]);

            Assert.That(window.SelectedItems, Has.One.Items);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);
        }

        [Test]
        public void PreSelectedItemsShouldBeSyncedByMultiSelectionHelper()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);
        }

        [Test]
        public void SelectedItemsShouldBeSyncedAndContainsOnlyOnce()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.FirstListBox.SelectedItems.Add(window.Items[0]);

            Assert.That(window.SelectedItems, Has.One.Items);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);

            window.FirstListBox.SelectedItems.Add(window.Items[0]);

            Assert.That(window.SelectedItems, Has.One.Items);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);
        }

        [Test]
        public void AddedItemShouldBeSynced()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);

            window.FirstListBox.SelectedItems.Add(window.Items[1]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.Count.EqualTo(2));
            Assert.That(window.FirstListBox.SelectedItems, Has.Count.EqualTo(2));
            Assert.That(window.SecondListBox.SelectedItems, Has.Count.EqualTo(2));

            window.SecondListBox.SelectedItems.Add(window.Items[2]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.Count.EqualTo(3));
            Assert.That(window.FirstListBox.SelectedItems, Has.Count.EqualTo(3));
            Assert.That(window.SecondListBox.SelectedItems, Has.Count.EqualTo(3));

            window.MultiSelectionComboBox.SelectedItems.Add(window.Items[3]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.Count.EqualTo(4));
            Assert.That(window.FirstListBox.SelectedItems, Has.Count.EqualTo(4));
            Assert.That(window.SecondListBox.SelectedItems, Has.Count.EqualTo(4));
        }

        [Test]
        public void RemovedItemShouldBeSynced()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);

            window.SelectedItems.Remove(window.Items[0]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Has.One.Items);
            Assert.That(window.FirstListBox.SelectedItems, Has.One.Items);
            Assert.That(window.SecondListBox.SelectedItems, Has.One.Items);

            window.MultiSelectionComboBox.SelectedItems.RemoveAt(0);

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);
        }

        [Test]
        public void MovedItemShouldBeSynced()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.FirstListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.FirstListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.SecondListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.SecondListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            window.SelectedItems.Move(0, 2);

            Assert.That(window.MultiSelectionComboBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.FirstListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.FirstListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.SecondListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.SecondListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));
        }

        [Test]
        public void ReplacedItemShouldBeSynced()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.FirstListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.FirstListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.FirstListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.SecondListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.SecondListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.SecondListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            window.SelectedItems[0] = window.Items[3];
            window.SelectedItems[2] = window.Items[4];

            Assert.That(window.MultiSelectionComboBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.FirstListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.FirstListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.FirstListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.SecondListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.SecondListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.SecondListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));
        }

        [Test]
        public void ClearedItemShouldBeSynced()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(window.SelectedItems, Is.Not.Null);
            Assert.That(window.SelectedItems, Is.Empty);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Not.Null);
            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.That(window.MultiSelectionComboBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.MultiSelectionComboBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.FirstListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.FirstListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.FirstListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            Assert.That(window.SecondListBox.SelectedItems[0], Is.EqualTo(window.SelectedItems[0]));
            Assert.That(window.SecondListBox.SelectedItems[1], Is.EqualTo(window.SelectedItems[1]));
            Assert.That(window.SecondListBox.SelectedItems[2], Is.EqualTo(window.SelectedItems[2]));

            window.SelectedItems.Clear();

            Assert.That(window.MultiSelectionComboBox.SelectedItems, Is.Empty);
            Assert.That(window.FirstListBox.SelectedItems, Is.Empty);
            Assert.That(window.SecondListBox.SelectedItems, Is.Empty);
        }
    }
}