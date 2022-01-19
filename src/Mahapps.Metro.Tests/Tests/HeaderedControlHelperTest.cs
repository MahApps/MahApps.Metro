// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests;

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

        this.fixture.Window?.TestGroupBox.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBox.FindChild<Border>("HeaderSite")?.Background);

        this.fixture.Window?.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBoxClean.FindChild<Grid>("HeaderSite")?.Background);

        this.fixture.Window?.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestGroupBoxVS.FindChild<Grid>("HeaderSite")?.Background);

        this.fixture.Window?.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderBackgroundProperty, headerBackground);
        Assert.Equal(headerBackground, this.fixture.Window?.TestMetroHeader.FindChild<Grid>("PART_Header")?.Background);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderForegroundProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        var headerForeground = Brushes.Crimson;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Foreground);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderForegroundProperty, headerForeground);
        Assert.Equal(headerForeground, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Foreground);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderMarginProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        var headerMargin = new Thickness(4);

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.Margin);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.Margin);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.Margin);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderMarginProperty, headerMargin);
        Assert.Equal(headerMargin, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.Margin);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderContentAlignmentProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty, horizontalAlignment);
        Assert.Equal(horizontalAlignment, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.HorizontalAlignment);

        const VerticalAlignment verticalAlignment = VerticalAlignment.Top;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderVerticalContentAlignmentProperty, verticalAlignment);
        Assert.Equal(verticalAlignment, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.VerticalAlignment);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontFamilyProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        var fontFamily = new FontFamily("Arial");

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontFamilyProperty, fontFamily);
        Assert.Equal(fontFamily, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontFamily);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontSizeProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        const double fontSize = 48d;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontSize);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontSizeProperty, fontSize);
        Assert.Equal(fontSize, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontSize);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontStretchProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        var fontStretch = FontStretches.Condensed;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontStretchProperty, fontStretch);
        Assert.Equal(fontStretch, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontStretch);
    }

    [Fact]
    [DisplayTestMethodName]
    public async Task TestHeaderFontWeightProperty()
    {
        await this.fixture.PrepareForTestAsync().ConfigureAwait(false);
        await TestHost.SwitchToAppThread();

        var window = this.fixture.Window;

        var fontWeight = FontWeights.ExtraBold;

        window.TestGroupBox.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, window.TestGroupBox.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

        window.TestGroupBoxClean.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, window.TestGroupBoxClean.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

        window.TestGroupBoxVS.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, window.TestGroupBoxVS.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);

        window.TestMetroHeader.SetValue(HeaderedControlHelper.HeaderFontWeightProperty, fontWeight);
        Assert.Equal(fontWeight, window.TestMetroHeader.FindChild<ContentControlEx>("HeaderContent")?.FontWeight);
    }
}