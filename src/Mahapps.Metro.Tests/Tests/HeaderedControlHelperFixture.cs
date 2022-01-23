// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using Xunit;

namespace MahApps.Metro.Tests.Tests;

public class HeaderedControlHelperFixture : IAsyncLifetime
{
    /// <summary>
    /// Called immediately after the class has been created, before it is used.
    /// </summary>
    public async Task InitializeAsync()
    {
        await TestHost.SwitchToAppThread();

        this.Window = await WindowHelpers.CreateInvisibleWindowAsync<HeaderedControlHelperTestWindow>().ConfigureAwait(false);
    }

    public HeaderedControlHelperTestWindow? Window { get; private set; }

    public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(DependencyObject? obj)
    {
        if (obj != null)
        {
            LocalValueEnumerator lve = obj.GetLocalValueEnumerator();
            while (lve.MoveNext())
            {
                yield return lve.Current.Property;
            }
        }
    }

    public static void ClearDependencyProperties(DependencyObject? obj)
    {
        if (obj != null)
        {
            foreach (var property in EnumerateDependencyProperties(obj))
            {
                if (property.ReadOnly == false)
                {
                    obj.ClearValue(property);
                }
            }
        }
    }

    public async Task PrepareForTestAsync()
    {
        await TestHost.SwitchToAppThread();

        ClearDependencyProperties(this.Window?.TestGroupBox);
        ClearDependencyProperties(this.Window?.TestGroupBoxClean);
        ClearDependencyProperties(this.Window?.TestGroupBoxVS);
        ClearDependencyProperties(this.Window?.TestMetroHeader);
        ClearDependencyProperties(this.Window?.TestColorPalette);
        ClearDependencyProperties(this.Window?.TestExpander);
        ClearDependencyProperties(this.Window?.TestExpanderVS);
        ClearDependencyProperties(this.Window?.TestTabControl);
        ClearDependencyProperties(this.Window?.TestTabItem);
        ClearDependencyProperties(this.Window?.TestTabControlVS);
        ClearDependencyProperties(this.Window?.TestTabItemVS);
        ClearDependencyProperties(this.Window?.TestMetroTabControl);
        ClearDependencyProperties(this.Window?.TestMetroTabItem);
        ClearDependencyProperties(this.Window?.TestFlyout);
    }

    /// <summary>
    /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
    /// if the class also implements that.
    /// </summary>
    public Task DisposeAsync()
    {
        this.Window = null;

        return Task.CompletedTask;
    }
}