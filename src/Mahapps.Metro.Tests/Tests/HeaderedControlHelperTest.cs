// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("WpfAnalyzers.DependencyProperty", "WPF0014:SetValue must use registered type", Justification = "<Pending>")]
public class HeaderedControlHelperTest : AutomationTestBase, IClassFixture<HeaderedControlHelperFixture>
{
    private readonly HeaderedControlHelperFixture fixture;

    public HeaderedControlHelperTest(HeaderedControlHelperFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderBackgroundProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestExpander.FindChild<Border>("HeaderSite")?.Background);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Background);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.Background);
        Assert.Equal(headerBackground, this.fixture.Window?.TestTabItem.FindChild<Border>("Border")?.Background);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.Background);
        Assert.Equal(headerBackground, this.fixture.Window?.TestMetroTabItem.FindChild<Border>("Border")?.Background);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderForegroundProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Foreground);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Foreground);

        // TabItem

        var tabForeground = Brushes.Aqua;
        this.fixture.Window?.TestTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(tabForeground, this.fixture.Window?.TestTabItem.Foreground);
        Assert.Equal(headerForeground, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);

        this.fixture.Window?.TestMetroTabItem.SetCurrentValue(TabItem.ForegroundProperty, tabForeground);
        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(tabForeground, this.fixture.Window?.TestMetroTabItem.Foreground);
        Assert.Equal(headerForeground, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.Foreground);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderMarginProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.Padding);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.Padding);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin);
        this.fixture.Window?.TestTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
        Assert.Equal(new Thickness(8), this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.Margin);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, this.fixture.Window?.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin);
        this.fixture.Window?.TestMetroTabItem.SetCurrentValue(TabItem.PaddingProperty, new Thickness(8));
        Assert.Equal(new Thickness(8), this.fixture.Window?.TestMetroTabItem.FindChild<Grid>("PART_ContentSite")?.Margin);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderContentAlignmentProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment);
        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.HorizontalContentAlignment);
        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.VerticalContentAlignment);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.HorizontalAlignment);
        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.VerticalAlignment);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontFamilyProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontFamily);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontFamily);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontFamily);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontSizeProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontSize);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontSize);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontSize);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontStretchProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontStretch);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontStretch);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontStretch);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontWeightProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
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

        // Expander

        this.fixture.Window?.TestExpander.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, this.fixture.Window?.TestExpander.FindChild<ToggleButton>("ToggleSite")?.FontWeight);

        this.fixture.Window?.TestExpanderVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, this.fixture.Window?.TestExpanderVS.FindChild<ToggleButton>("ToggleSite")?.FontWeight);

        // TabItem

        this.fixture.Window?.TestTabItem.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, this.fixture.Window?.TestTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);

        this.fixture.Window?.TestMetroTabItem.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, this.fixture.Window?.TestMetroTabItem.FindChild<ContentControlEx>("ContentSite")?.FontWeight);
    }
}