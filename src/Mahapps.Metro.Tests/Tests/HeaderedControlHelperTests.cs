// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0014:SetValue must use registered type", Justification = "<Pending>")]
    [TestFixture]
    public class HeaderedControlHelperTests
    {
        private HeaderedControlHelperTestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<HeaderedControlHelperTestWindow>().ConfigureAwait(false);
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
            this.PreparePropertiesForTest([
                HeaderedControlHelper.HeaderBackgroundProperty.Name,
                HeaderedControlHelper.HeaderForegroundProperty.Name,
                HeaderedControlHelper.HeaderMarginProperty.Name,
                HeaderedControlHelper.HeaderFontFamilyProperty.Name,
                HeaderedControlHelper.HeaderFontSizeProperty.Name,
                HeaderedControlHelper.HeaderFontStretchProperty.Name,
                HeaderedControlHelper.HeaderFontWeightProperty.Name,
                HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name,
                HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name,
                TabItem.ForegroundProperty.Name,
                Flyout.ForegroundProperty.Name,
                TabItem.PaddingProperty.Name,
            ]);
        }

        private void PreparePropertiesForTest(IList<string>? properties = null)
        {
            this.window?.TestGroupBox.ClearDependencyProperties(properties);
            this.window?.TestGroupBoxClean.ClearDependencyProperties(properties);
            this.window?.TestGroupBoxVS.ClearDependencyProperties(properties);
            this.window?.TestMetroHeader.ClearDependencyProperties(properties);
            this.window?.TestColorPalette.ClearDependencyProperties(properties);
            this.window?.TestToggleSwitch.ClearDependencyProperties(properties);
            this.window?.TestExpander.ClearDependencyProperties(properties);
            this.window?.TestExpanderVS.ClearDependencyProperties(properties);
            this.window?.TestTabControl.ClearDependencyProperties(properties);
            this.window?.TestTabItem.ClearDependencyProperties(properties);
            this.window?.TestTabItemUnselected.ClearDependencyProperties(properties);
            this.window?.TestTabControlVS.ClearDependencyProperties(properties);
            this.window?.TestTabItemVS.ClearDependencyProperties(properties);
            this.window?.TestTabItemVSUnselected.ClearDependencyProperties(properties);
            this.window?.TestMetroTabControl.ClearDependencyProperties(properties);
            this.window?.TestMetroTabItem.ClearDependencyProperties(properties);
            this.window?.TestMetroTabItemUnselected.ClearDependencyProperties(properties);
            this.window?.TestFlyout.ClearDependencyProperties(properties);
        }

        [Test]
        public void GroupBoxShouldUseHeaderBackgroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerBackground = Brushes.BlueViolet;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestGroupBox.FindChild<Border>("HeaderSite")?.Background, Is.EqualTo(headerBackground));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestGroupBoxClean.FindChild<Grid>("HeaderSite")?.Background, Is.EqualTo(headerBackground));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestGroupBoxVS.FindChild<Grid>("HeaderSite")?.Background, Is.EqualTo(headerBackground));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestMetroHeader.FindChild<Grid>("PART_Header")?.Background, Is.EqualTo(headerBackground));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestColorPalette.FindChild<Border>("HeaderSite")?.Background, Is.EqualTo(headerBackground));
        }

        [Test]
        public void ExpanderShouldUseHeaderBackgroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerBackground = Brushes.BlueViolet;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestExpander.FindChild<Border>("HeaderSite")?.Background, Is.EqualTo(headerBackground));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Background, Is.EqualTo(headerBackground));
        }

        [Test]
        public void TabItemShouldUseHeaderBackgroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerBackground = Brushes.BlueViolet;

            var selectedHeaderBackground = window.TestStackPanelVS.TryFindResource("MahApps.Brushes.BackgroundSelected") as Brush;
            Assert.That(selectedHeaderBackground, Is.Not.Null);

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestTabItem.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestTabItem.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));

            window.TestTabItemVSUnselected.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestTabItemVSUnselected.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestTabItemVSUnselected.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestTabItemVS.Background, Is.EqualTo(selectedHeaderBackground));
            Assert.That(window.TestTabItemVS.FindChild<Border>("Border")?.Background, Is.EqualTo(selectedHeaderBackground));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestMetroTabItem.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestMetroTabItem.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));
        }

        [Test]
        public void TabControlShouldUseHeaderBackgroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerBackground = Brushes.BlueViolet;

            var selectedHeaderBackground = window.TestStackPanelVS.TryFindResource("MahApps.Brushes.BackgroundSelected") as Brush;
            Assert.That(selectedHeaderBackground, Is.Not.Null);

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestTabItem.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestTabItem.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestTabItemVS.Background, Is.EqualTo(selectedHeaderBackground));
            Assert.That(window.TestTabItemVS.FindChild<Border>("Border")?.Background, Is.EqualTo(selectedHeaderBackground));
            Assert.That(window.TestTabItemVSUnselected.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestTabItemVSUnselected.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestMetroTabItem.Background, Is.EqualTo(headerBackground));
            Assert.That(window.TestMetroTabItem.FindChild<Border>("Border")?.Background, Is.EqualTo(headerBackground));
        }

        [Test]
        public void FlyoutShouldUseHeaderBackgroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerBackground = Brushes.BlueViolet;

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Background, Is.EqualTo(headerBackground));
        }

        [Test]
        public void GroupBoxShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Foreground, Is.EqualTo(headerForeground));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Foreground, Is.EqualTo(headerForeground));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Foreground, Is.EqualTo(headerForeground));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Foreground, Is.EqualTo(headerForeground));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.Foreground, Is.EqualTo(headerForeground));
        }

        [Test]
        public void ToggleSwitchShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            var contentPresenter = window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter");
            Assert.That(contentPresenter, Is.Not.Null);
            Assert.That(TextElement.GetForeground(contentPresenter), Is.EqualTo(headerForeground));
        }

        [Test]
        public void ExpanderShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Foreground, Is.EqualTo(headerForeground));
        }

        [Test]
        public void TabItemShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            var selectedHeaderForeground = window.TryFindResource("MahApps.Brushes.Accent") as Brush;
            Assert.That(selectedHeaderForeground, Is.Not.Null);

            // TabItem

            var tabForeground = Brushes.Aqua;

            window.TestTabItemUnselected.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            window.TestTabItemUnselected.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestTabItemUnselected.Foreground, Is.EqualTo(tabForeground));
            Assert.That(window.TestTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestTabItem.Foreground, Is.EqualTo(tabForeground));
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(selectedHeaderForeground));

            window.TestTabItemVS.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestTabItemVS.Foreground, Is.EqualTo(tabForeground));
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestMetroTabItemUnselected.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            window.TestMetroTabItemUnselected.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestMetroTabItemUnselected.Foreground, Is.EqualTo(tabForeground));
            Assert.That(window.TestMetroTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestMetroTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestMetroTabItem.Foreground, Is.EqualTo(tabForeground));
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(selectedHeaderForeground));
        }

        [Test]
        public void TabControlShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            var selectedHeaderForeground = window.TryFindResource("MahApps.Brushes.Accent") as Brush;
            Assert.That(selectedHeaderForeground, Is.Not.Null);

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(selectedHeaderForeground));
            Assert.That(window.TestTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));
            Assert.That(window.TestTabItemVSUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(selectedHeaderForeground));
            Assert.That(window.TestMetroTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground, Is.EqualTo(headerForeground));
        }

        [Test]
        public void FlyoutShouldUseForegroundProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerForeground = Brushes.Crimson;

            // Flyout

            var flyoutForeground = Brushes.Aqua;

            window.TestFlyout.SetCurrentValue(Flyout.ForegroundProperty, flyoutForeground);
            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.That(window.TestFlyout.Foreground, Is.EqualTo(flyoutForeground));
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Foreground, Is.EqualTo(headerForeground));
        }

        [Test]
        public void GroupBoxShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Margin, Is.EqualTo(headerMargin));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Margin, Is.EqualTo(headerMargin));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Margin, Is.EqualTo(headerMargin));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Margin, Is.EqualTo(headerMargin));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.Margin, Is.EqualTo(headerMargin));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.Margin, Is.EqualTo(headerMargin));
        }

        [Test]
        public void ExpanderShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Padding, Is.EqualTo(headerMargin));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Padding, Is.EqualTo(headerMargin));
        }

        [Test]
        public void TabItemShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(headerMargin));
            window.TestTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(new Thickness(8)));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(headerMargin));
            window.TestTabItemVS.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(new Thickness(8)));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin, Is.EqualTo(headerMargin));
            window.TestMetroTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.That(window.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin, Is.EqualTo(new Thickness(8)));
        }

        [Test]
        public void TabControlShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(headerMargin));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin, Is.EqualTo(headerMargin));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin, Is.EqualTo(headerMargin));
        }

        [Test]
        public void FlyoutShouldUseHeaderMarginProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var headerMargin = new Thickness(4);

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Padding, Is.EqualTo(headerMargin));
        }

        [Test]
        public void GroupBoxShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.VerticalAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void ExpanderShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment, Is.EqualTo(horizontalAlignment));
            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment, Is.EqualTo(verticalAlignment));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment, Is.EqualTo(horizontalAlignment));
            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void TabItemShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void TabControlShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment, Is.EqualTo(horizontalAlignment));
            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void FlyoutShouldUseHeaderContentAlignmentProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.HorizontalContentAlignment, Is.EqualTo(horizontalAlignment));
            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.VerticalContentAlignment, Is.EqualTo(verticalAlignment));
        }

        [Test]
        public void GroupBoxShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontFamily, Is.EqualTo(fontFamily));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            var contentPresenter = window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter");
            Assert.That(contentPresenter, Is.Not.Null);
            Assert.That(TextElement.GetFontFamily(contentPresenter), Is.EqualTo(fontFamily));
        }

        [Test]
        public void ExpanderShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontFamily, Is.EqualTo(fontFamily));
        }

        [Test]
        public void TabItemShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));
        }

        [Test]
        public void TabControlShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily, Is.EqualTo(fontFamily));
        }

        [Test]
        public void FlyoutShouldUseHeaderFontFamilyProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontFamily = new FontFamily("Arial");

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontFamily, Is.EqualTo(fontFamily));
        }

        [Test]
        public void GroupBoxShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontSize, Is.EqualTo(fontSize));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontSize, Is.EqualTo(fontSize));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontSize, Is.EqualTo(fontSize));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontSize, Is.EqualTo(fontSize));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontSize, Is.EqualTo(fontSize));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            var contentPresenter = window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter");
            Assert.That(contentPresenter, Is.Not.Null);
            Assert.That(TextElement.GetFontSize(contentPresenter), Is.EqualTo(fontSize));
        }

        [Test]
        public void ExpanderShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontSize, Is.EqualTo(fontSize));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontSize, Is.EqualTo(fontSize));
        }

        [Test]
        public void TabItemShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));
        }

        [Test]
        public void TabControlShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize, Is.EqualTo(fontSize));
        }

        [Test]
        public void FlyoutShouldUseHeaderFontSizeProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            const double fontSize = 48d;

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontSize, Is.EqualTo(fontSize));
        }

        [Test]
        public void GroupBoxShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontStretch, Is.EqualTo(fontStretch));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            var contentPresenter = window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter");
            Assert.That(contentPresenter, Is.Not.Null);
            Assert.That(TextElement.GetFontStretch(contentPresenter), Is.EqualTo(fontStretch));
        }

        [Test]
        public void ExpanderShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontStretch, Is.EqualTo(fontStretch));
        }

        [Test]
        public void TabItemShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));
        }

        [Test]
        public void TabControlShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch, Is.EqualTo(fontStretch));
        }

        [Test]
        public void FlyoutShouldUseHeaderFontStretchProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontStretch = FontStretches.Condensed;

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontStretch, Is.EqualTo(fontStretch));
        }

        [Test]
        public void GroupBoxShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // GroupBox

            window.TestGroupBox.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestGroupBoxClean.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestGroupBoxVS.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestMetroHeader.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestColorPalette.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontWeight, Is.EqualTo(fontWeight));
        }

        [Test]
        public void ToggleSwitchShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // ToggleSwitch

            window.TestToggleSwitch.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            var contentPresenter = window.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter");
            Assert.That(contentPresenter, Is.Not.Null);
            Assert.That(TextElement.GetFontWeight(contentPresenter), Is.EqualTo(fontWeight));
        }

        [Test]
        public void ExpanderShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // Expander

            window.TestExpander.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestExpanderVS.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontWeight, Is.EqualTo(fontWeight));
        }

        [Test]
        public void TabItemShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // TabItem

            window.TestTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestTabItemVS.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestMetroTabItem.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));
        }

        [Test]
        public void TabControlShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // TabControl

            window.TestTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestTabControlVS.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));

            window.TestMetroTabControl.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight, Is.EqualTo(fontWeight));
        }

        [Test]
        public void FlyoutShouldUseHeaderFontWeightProperty()
        {
            Assert.That(this.window, Is.Not.Null);

            var fontWeight = FontWeights.ExtraBold;

            // Flyout

            window.TestFlyout.SetCurrentValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.That(window.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontWeight, Is.EqualTo(fontWeight));
        }
    }
}