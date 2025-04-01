// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class AnimatedTabControlTest
    {
        [Test]
        public async Task MetroAnimatedSingleRowTabControlShouldSelectTheCorrectTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<AnimatedTabControlWindow>();
            var templateSelector = window.TryFindResource("TabControlContentTemplateSelector") as TabControlContentTemplateSelector;
            Assert.That(templateSelector, Is.Not.Null);

            var tabControl = window.MetroTabControl;
            Assert.That(tabControl.Items.Count, Is.EqualTo(2));

            var tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(0));
            var contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            var textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((FirstViewModel)tabItem.Content).Name));

            tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(1));
            window.Invoke(() => tabItem.IsSelected = true);
            contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((SecondViewModel)tabItem.Content).Name));

            window.Close();
        }

        [Test]
        public async Task TabControlWithAnimatedSingleRowTabControlStyleShouldSelectTheCorrectTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<AnimatedTabControlWindow>();
            var templateSelector = window.TryFindResource("TabControlContentTemplateSelector") as TabControlContentTemplateSelector;
            Assert.That(templateSelector, Is.Not.Null);

            var tabControl = window.NormalTabControl;
            Assert.That(tabControl.Items.Count, Is.EqualTo(2));

            var tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(0));
            var contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            var textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((FirstViewModel)tabItem.Content).Name));

            tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(1));
            window.Invoke(() => tabItem.IsSelected = true);
            contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((SecondViewModel)tabItem.Content).Name));

            window.Close();
        }

        [Test]
        public async Task MetroAnimatedTabControlShouldSelectTheCorrectTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<AnimatedTabControlWindow>();
            var templateSelector = window.TryFindResource("TabControlContentTemplateSelector") as TabControlContentTemplateSelector;
            Assert.That(templateSelector, Is.Not.Null);

            var tabControl = window.MetroTabControl2;
            Assert.That(tabControl.Items.Count, Is.EqualTo(2));

            var tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(0));
            var contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            var textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((FirstViewModel)tabItem.Content).Name));

            tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(1));
            window.Invoke(() => tabItem.IsSelected = true);
            contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((SecondViewModel)tabItem.Content).Name));

            window.Close();
        }

        [Test]
        public async Task TabControlWithAnimatedTabControlStyleShouldSelectTheCorrectTemplate()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<AnimatedTabControlWindow>();
            var templateSelector = window.TryFindResource("TabControlContentTemplateSelector") as TabControlContentTemplateSelector;
            Assert.That(templateSelector, Is.Not.Null);

            var tabControl = window.NormalTabControl2;
            Assert.That(tabControl.Items.Count, Is.EqualTo(2));

            var tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(0));
            var contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            var textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((FirstViewModel)tabItem.Content).Name));

            tabItem = ((TabItem)tabControl.ItemContainerGenerator.ContainerFromIndex(1));
            window.Invoke(() => tabItem.IsSelected = true);
            contentPresenter = tabControl.FindChild<ContentPresenter>("PART_SelectedContentHost");
            Assert.That(contentPresenter, Is.Not.Null);
            contentPresenter.ApplyTemplate();
            textBlock = contentPresenter.FindChild<TextBlock>(null);
            Assert.That(textBlock, Is.Not.Null);
            Assert.That(textBlock.Text, Is.EqualTo(((SecondViewModel)tabItem.Content).Name));

            window.Close();
        }
    }
}