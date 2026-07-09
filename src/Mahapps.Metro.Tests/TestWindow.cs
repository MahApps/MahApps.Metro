using System;
#if NET6_0_OR_GREATER
using System.Threading.Tasks;
#endif
using MahApps.Metro.Controls;

namespace MahApps.Metro.Tests;

#if NET6_0_OR_GREATER
public class TestWindow : MetroWindow, IDisposable, IAsyncDisposable
#else
public class TestWindow : MetroWindow, IDisposable
#endif
{
    public void Dispose()
    {
        this.Close();
    }

#if NET6_0_OR_GREATER
    public ValueTask DisposeAsync()
    {
        this.Close();
        return ValueTask.CompletedTask;
    }
#endif
}