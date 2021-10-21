// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class MultiSelectorHelperTests : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public async void SelectedItemsShouldBeSyncedByMultiSelectionHelper()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.FirstListBox.SelectedItems.Add(window.Items[0]);

            Assert.Single(window.SelectedItems);
            Assert.Single(window.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void AddedItemShouldBeSyncedToListBox()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.SelectedItems.Add(window.Items[0]);

            Assert.Single(window.FirstListBox.SelectedItems);
            Assert.Single(window.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void RemovedItemShouldBeSyncedToListBox()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.SelectedItems.Add(window.Items[0]);

            Assert.Single(window.FirstListBox.SelectedItems);
            Assert.Single(window.SecondListBox.SelectedItems);

            window.SelectedItems.Remove(window.Items[0]);

            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void MovedItemShouldBeSyncedToListBox()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.Equal(window.SelectedItems[0], window.FirstListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[2], window.FirstListBox.SelectedItems[2]);

            Assert.Equal(window.SelectedItems[0], window.SecondListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[2], window.SecondListBox.SelectedItems[2]);

            window.SelectedItems.Move(0, 2);

            Assert.Equal(window.SelectedItems[0], window.FirstListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[2], window.FirstListBox.SelectedItems[2]);

            Assert.Equal(window.SelectedItems[0], window.SecondListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[2], window.SecondListBox.SelectedItems[2]);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void ReplacedItemShouldBeSyncedToListBox()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.Equal(window.SelectedItems[0], window.FirstListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.FirstListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.FirstListBox.SelectedItems[2]);

            Assert.Equal(window.SelectedItems[0], window.SecondListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.SecondListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.SecondListBox.SelectedItems[2]);

            window.SelectedItems[0] = window.Items[3];
            window.SelectedItems[2] = window.Items[4];

            Assert.Equal(window.SelectedItems[0], window.FirstListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.FirstListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.FirstListBox.SelectedItems[2]);

            Assert.Equal(window.SelectedItems[0], window.SecondListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.SecondListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.SecondListBox.SelectedItems[2]);
        }

        [Fact]
        [DisplayTestMethodName]
        public async void ClearedItemShouldBeSyncedToListBox()
        {
            await TestHost.SwitchToAppThread();

            var window = await WindowHelpers.CreateInvisibleWindowAsync<MultiSelectorHelperTestWindow>();

            Assert.Empty(window.SelectedItems);
            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);

            window.SelectedItems.Add(window.Items[0]);
            window.SelectedItems.Add(window.Items[1]);
            window.SelectedItems.Add(window.Items[2]);

            Assert.Equal(window.SelectedItems[0], window.FirstListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.FirstListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.FirstListBox.SelectedItems[2]);

            Assert.Equal(window.SelectedItems[0], window.SecondListBox.SelectedItems[0]);
            Assert.Equal(window.SelectedItems[1], window.SecondListBox.SelectedItems[1]);
            Assert.Equal(window.SelectedItems[2], window.SecondListBox.SelectedItems[2]);

            window.SelectedItems.Clear();

            Assert.Empty(window.FirstListBox.SelectedItems);
            Assert.Empty(window.SecondListBox.SelectedItems);
        }
    }
}