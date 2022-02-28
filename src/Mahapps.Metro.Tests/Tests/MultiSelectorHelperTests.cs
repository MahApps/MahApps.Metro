// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class MultiSelectorHelperTests : AutomationTestFixtureBase<MultiSelectorHelperTestsFixture>
    {
        public MultiSelectorHelperTests(MultiSelectorHelperTestsFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        [DisplayTestMethodName]
        public async void SelectedItemsShouldBeSyncedByMultiSelectionHelper()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.FirstListBox.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.SelectedItems);
            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void PreSelectedItemsShouldBeSyncedByMultiSelectionHelper()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void SelectedItemsShouldBeSyncedAndContainsOnlyOnce()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.FirstListBox.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.SelectedItems);
            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.FirstListBox.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.SelectedItems);
            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void AddedItemShouldBeSynced()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.FirstListBox?.SelectedItems.Add(this.fixture.Window?.Items[1]);

            Assert.Equal(2, this.fixture.Window?.MultiSelectionComboBox.SelectedItems.Count);
            Assert.Equal(2, this.fixture.Window?.FirstListBox.SelectedItems.Count);
            Assert.Equal(2, this.fixture.Window?.SecondListBox.SelectedItems.Count);

            this.fixture.Window?.SecondListBox?.SelectedItems.Add(this.fixture.Window?.Items[2]);

            Assert.Equal(3, this.fixture.Window?.MultiSelectionComboBox.SelectedItems.Count);
            Assert.Equal(3, this.fixture.Window?.FirstListBox.SelectedItems.Count);
            Assert.Equal(3, this.fixture.Window?.SecondListBox.SelectedItems.Count);

            this.fixture.Window?.MultiSelectionComboBox?.SelectedItems.Add(this.fixture.Window?.Items[3]);

            Assert.Equal(4, this.fixture.Window?.MultiSelectionComboBox.SelectedItems.Count);
            Assert.Equal(4, this.fixture.Window?.FirstListBox.SelectedItems.Count);
            Assert.Equal(4, this.fixture.Window?.SecondListBox.SelectedItems.Count);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void RemovedItemShouldBeSynced()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);

            Assert.Single(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Single(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Single(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Remove(this.fixture.Window?.Items[0]);

            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void MovedItemShouldBeSynced()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[1]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.FirstListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.FirstListBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.SecondListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.SecondListBox.SelectedItems[2]);

            this.fixture.Window?.SelectedItems.Move(0, 2);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.FirstListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.FirstListBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.SecondListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.SecondListBox.SelectedItems[2]);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void ReplacedItemShouldBeSynced()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[1]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.FirstListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.FirstListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.FirstListBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.SecondListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.SecondListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.SecondListBox.SelectedItems[2]);

            this.fixture.Window.SelectedItems[0] = this.fixture.Window?.Items[3];
            this.fixture.Window.SelectedItems[2] = this.fixture.Window?.Items[4];

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.FirstListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.FirstListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.FirstListBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.SecondListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.SecondListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.SecondListBox.SelectedItems[2]);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void ClearedItemShouldBeSynced()
        {
            await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            Assert.NotNull(this.fixture.Window?.SelectedItems);
            Assert.Empty(this.fixture.Window?.SelectedItems);
            Assert.NotNull(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);

            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[0]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[1]);
            this.fixture.Window?.SelectedItems.Add(this.fixture.Window?.Items[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.MultiSelectionComboBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.FirstListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.FirstListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.FirstListBox.SelectedItems[2]);

            Assert.Equal(this.fixture.Window?.SelectedItems[0], this.fixture.Window?.SecondListBox.SelectedItems[0]);
            Assert.Equal(this.fixture.Window?.SelectedItems[1], this.fixture.Window?.SecondListBox.SelectedItems[1]);
            Assert.Equal(this.fixture.Window?.SelectedItems[2], this.fixture.Window?.SecondListBox.SelectedItems[2]);

            this.fixture.Window?.SelectedItems.Clear();

            Assert.Empty(this.fixture.Window?.MultiSelectionComboBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.FirstListBox.SelectedItems);
            Assert.Empty(this.fixture.Window?.SecondListBox.SelectedItems);
        }
    }
}