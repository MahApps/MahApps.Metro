// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0014:SetValue must use registered type", Justification = "<Pending>")]
    public class HeaderedControlHelperTests : AutomationTestFixtureBase<HeaderedControlHelperTestsFixture>
    {
        public HeaderedControlHelperTests(HeaderedControlHelperTestsFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderBackgroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderBackgroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerBackground = Brushes.BlueViolet;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBox.FindChild<Border>("HeaderSite")?.Background);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBoxClean.FindChild<Grid>("HeaderSite")?.Background);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBoxVS.FindChild<Grid>("HeaderSite")?.Background);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestMetroHeader.FindChild<Grid>("PART_Header")?.Background);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestColorPalette.FindChild<Border>("HeaderSite")?.Background);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderBackgroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderBackgroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerBackground = Brushes.BlueViolet;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestExpander.FindChild<Border>("HeaderSite")?.Background);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Background);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderBackgroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderBackgroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerBackground = Brushes.BlueViolet;

            var selectedHeaderBackground = this.fixture.Window?.TestStackPanelVS.TryFindResource("MahApps.Brushes.BackgroundSelected") as Brush;
            Assert.NotNull(selectedHeaderBackground);

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.FindChild<Border>("Border")?.Background);

            this.fixture.Window?.TestTabItemVSUnselected.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItemVSUnselected.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItemVSUnselected.FindChild<Border>("Border")?.Background);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(selectedHeaderBackground, this.fixture.Window?.TestTabItemVS.Background);
            Assert.Equal(selectedHeaderBackground, this.fixture.Window?.TestTabItemVS.FindChild<Border>("Border")?.Background);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.FindChild<Border>("Border")?.Background);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderBackgroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderBackgroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerBackground = Brushes.BlueViolet;

            var selectedHeaderBackground = this.fixture.Window?.TestStackPanelVS.TryFindResource("MahApps.Brushes.BackgroundSelected") as Brush;
            Assert.NotNull(selectedHeaderBackground);

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.FindChild<Border>("Border")?.Background);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(selectedHeaderBackground, this.fixture.Window?.TestTabItemVS.Background);
            Assert.Equal(selectedHeaderBackground, this.fixture.Window?.TestTabItemVS.FindChild<Border>("Border")?.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItemVSUnselected.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestTabItemVSUnselected.FindChild<Border>("Border")?.Background);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.Background);
            Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.FindChild<Border>("Border")?.Background);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderBackgroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderBackgroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerBackground = Brushes.BlueViolet;

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
            Assert.Equal(headerBackground, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Background);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, TextElement.GetForeground(this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Foreground);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { TabItem.ForegroundProperty.Name, HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            var selectedHeaderForeground = this.fixture.Window?.TryFindResource("MahApps.Brushes.Accent") as Brush;
            Assert.NotNull(selectedHeaderForeground);

            // TabItem

            var tabForeground = Brushes.Aqua;

            this.fixture.Window?.TestTabItemUnselected.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            this.fixture.Window?.TestTabItemUnselected.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(tabForeground, this.fixture.Window?.TestTabItemUnselected.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(tabForeground, this.fixture.Window?.TestTabItem.Foreground);
            Assert.Equal(selectedHeaderForeground, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestTabItemVS.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(tabForeground, this.fixture.Window?.TestTabItemVS.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestMetroTabItemUnselected.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            this.fixture.Window?.TestMetroTabItemUnselected.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(tabForeground, this.fixture.Window?.TestMetroTabItemUnselected.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestMetroTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestMetroTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(tabForeground, this.fixture.Window?.TestMetroTabItem.Foreground);
            Assert.Equal(selectedHeaderForeground, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            var selectedHeaderForeground = this.fixture.Window?.TryFindResource("MahApps.Brushes.Accent") as Brush;
            Assert.NotNull(selectedHeaderForeground);

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(selectedHeaderForeground, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestTabItemVSUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(selectedHeaderForeground, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestMetroTabItemUnselected.FindChild<ContentControlEx>("ContentSite")?.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseForegroundProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { Flyout.ForegroundProperty.Name, HeaderedControlHelper.HeaderForegroundProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerForeground = Brushes.Crimson;

            // Flyout

            var flyoutForeground = Brushes.Aqua;

            this.fixture.Window?.TestFlyout.SetCurrentValue(Flyout.ForegroundProperty, flyoutForeground);
            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
            Assert.Equal(flyoutForeground, this.fixture.Window?.TestFlyout.Foreground);
            Assert.Equal(headerForeground, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Foreground);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Margin);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Margin);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Margin);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Margin);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.Margin);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.Margin);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Padding);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Padding);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { TabItem.PaddingProperty.Name, HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin);
            this.fixture.Window?.TestTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.Equal(new Thickness(8), this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin);
            this.fixture.Window?.TestTabItemVS.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.Equal(new Thickness(8), this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin);
            this.fixture.Window?.TestMetroTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
            Assert.Equal(new Thickness(8), this.fixture.Window?.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.Margin);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderMarginProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderMarginProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var headerMargin = new Thickness(4);

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
            Assert.Equal(headerMargin, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.Padding);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);
            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);
            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);
            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);
            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);
            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.HorizontalAlignment);
            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")?.VerticalAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment);
            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment);
            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderContentAlignmentProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty.Name, HeaderedControlHelper.HeaderVerticalContentAlignmentProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
            const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
            Assert.Equal(horizontalAlignment, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.HorizontalContentAlignment);
            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
            Assert.Equal(verticalAlignment, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.VerticalContentAlignment);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, TextElement.GetFontFamily(this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontFamily);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontFamily);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontFamily);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderFontFamilyProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontFamilyProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontFamily = new FontFamily("Arial");

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
            Assert.Equal(fontFamily, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontFamily);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, TextElement.GetFontSize(this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontSize);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontSize);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontSize);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderFontSizeProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontSizeProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            const double fontSize = 48d;

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
            Assert.Equal(fontSize, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontSize);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, TextElement.GetFontStretch(this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontStretch);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontStretch);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontStretch);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontStretch);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderFontStretchProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontStretchProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontStretch = FontStretches.Condensed;

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
            Assert.Equal(fontStretch, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontStretch);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task GroupBoxShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // GroupBox

            this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

            this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

            this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

            this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

            this.fixture.Window?.TestColorPalette.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestColorPalette.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ToggleSwitchShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // ToggleSwitch

            this.fixture.Window?.TestToggleSwitch.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, TextElement.GetFontWeight(this.fixture.Window?.TestToggleSwitch.FindChild<ContentPresenter>("HeaderContentPresenter")));
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task ExpanderShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // Expander

            this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontWeight);

            this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontWeight);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabItemShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // TabItem

            this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);

            this.fixture.Window?.TestTabItemVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontWeight);

            this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task TabControlShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // TabControl

            this.fixture.Window?.TestTabControl.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);

            this.fixture.Window?.TestTabControlVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestTabItemVS.FindChild<ContentControlEx>("ContentSite")?.FontWeight);

            this.fixture.Window?.TestMetroTabControl.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);
        }

        [Fact]
        [DisplayTestMethodName]
        public async Task FlyoutShouldUseHeaderFontWeightProperty()
        {
            await this.fixture.PrepareForTestAsync(new[] { HeaderedControlHelper.HeaderFontWeightProperty.Name }).ConfigureAwait(false);
            await TestHost.SwitchToAppThread();

            var fontWeight = FontWeights.ExtraBold;

            // Flyout

            this.fixture.Window?.TestFlyout.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
            Assert.Equal(fontWeight, this.fixture.Window?.TestFlyout.FindChild<MetroThumbContentControl>("PART_Header")?.FontWeight);
        }
    }
}